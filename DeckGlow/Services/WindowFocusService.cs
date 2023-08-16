using System;
using System.Runtime.InteropServices;

namespace DeckGlow.Services
{
    public class WindowFocusService : IDisposable
    {
        public event EventHandler<WindowFocusChangeEventArgs> FocusChangeEvent;

        // Crashes if not global
        private WinEventDelegate? _winEventDelegate = null;
        private static IntPtr _winEventHook;

        delegate void WinEventDelegate(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime);

        [DllImport("user32.dll")]
        static extern IntPtr SetWinEventHook(uint eventMin, uint eventMax, IntPtr hmodWinEventProc, WinEventDelegate lpfnWinEventProc, uint idProcess, uint idThread, uint dwFlags);

        [DllImport("user32.dll")]
        static extern bool UnhookWinEvent(IntPtr hWinEventHook);

        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("kernel32.dll")]
        static extern IntPtr OpenProcess(uint dwDesiredAccess, bool bInheritHandle, uint dwProcessId);

        [DllImport("kernel32.dll")]
        static extern bool QueryFullProcessImageName(IntPtr hProcess, uint dwFlags, [Out] char[] lpExeName, ref uint lpdwSize);

        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool CloseHandle(IntPtr hObject);

        public WindowFocusService()
        {
            const uint WINEVENT_OUTOFCONTEXT = 0;
            const uint EVENT_SYSTEM_FOREGROUND = 3;

            // Register callback
            _winEventDelegate = new WinEventDelegate(WinEventCallback);
            _winEventHook = SetWinEventHook(EVENT_SYSTEM_FOREGROUND, EVENT_SYSTEM_FOREGROUND, IntPtr.Zero, _winEventDelegate, 0, 0, WINEVENT_OUTOFCONTEXT);
        }

        /// <summary>
        /// Get the full file path of a process
        /// </summary>
        /// <param name="hwnd"></param>
        /// <returns></returns>
        private static string? GetWindowApplicationPath(IntPtr hwnd)
        {
            GetWindowThreadProcessId(hwnd, out uint processId);

            const uint PROCESS_QUERY_LIMITED_INFORMATION = 0x00001000;
            IntPtr processHandle = OpenProcess(PROCESS_QUERY_LIMITED_INFORMATION, false, processId);

            if (processHandle == IntPtr.Zero) return null;

            // 256 max window path length
            char[] buffer = new char[256];
            uint bufferSize = (uint)buffer.Length;

            bool result = QueryFullProcessImageName(processHandle, 0, buffer, ref bufferSize);
            CloseHandle(processHandle);

            if (!result) return null;

            // Return the process file path, trimming empty space from the char array buffer assignment
            return new string(buffer).Trim('\0');
        }

        /// <summary>
        /// Called when window focus changes
        /// </summary>
        /// <param name="hWinEventHook"></param>
        /// <param name="eventType"></param>
        /// <param name="hwnd"></param>
        /// <param name="idObject"></param>
        /// <param name="idChild"></param>
        /// <param name="dwEventThread"></param>
        /// <param name="dwmsEventTime"></param>
        public void WinEventCallback(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            string? path = GetWindowApplicationPath(hwnd);
            if (path == null) return;

            // Dispatch the event to registered listeners
            FocusChangeEvent?.Invoke(null, new WindowFocusChangeEventArgs
            {
                ProcessPath = path
            });
        }

        public void Dispose()
        {
            if (_winEventHook != IntPtr.Zero)
            {
                UnhookWinEvent(_winEventHook);
            }
        }

    }

    public class WindowFocusChangeEventArgs : EventArgs
    {
        public string ProcessPath { get; set; }
    }

}
