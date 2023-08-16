using Microsoft.Win32;
using Serilog;
using System;

namespace DeckGlow.Managers
{
    internal class StartupManager
    {

        private const string RunKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
        private static readonly RegistryView RegView = Environment.Is64BitOperatingSystem ? RegistryView.Registry32 : RegistryView.Default;

        internal static bool CheckLaunchOnUserLogin()
        {
            try
            {
                using var localKey = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegView);
                using var key = localKey.OpenSubKey(RunKey, false);
                var val = (string)key?.GetValue(App.Name, string.Empty);
                if (string.IsNullOrWhiteSpace(val)) return false;
                return val == $"{App.ExecutablePath}";
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "[REG] Unable to get status of run-on-login: {msg}", ex.Message);
                return false;
            }
        }

        internal static bool EnableLaunchOnUserLogin()
        {
            try
            {
                using var localKey = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegView);
                using var key = localKey.CreateSubKey(RunKey, true);
                key.OpenSubKey("Run", true);
                key.SetValue(App.Name, $"{App.ExecutablePath}", RegistryValueKind.String);
                key.Flush();
                return true;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "[REG] Unable to set executable as run-on-login: {msg}", ex.Message);
                return false;
            }
        }

        internal static bool DisableLaunchOnUserLogin()
        {
            try
            {
                using var localKey = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegView);
                using var key = localKey.OpenSubKey(RunKey, true);
                key?.DeleteValue(App.Name, false);
                key?.Flush();
                return true;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "[REG] Unable to remove executable from run-on-login: {msg}", ex.Message);
                return false;
            }
        }
    }
}
