using System.Drawing;
using System.IO;
using System.Windows.Interop;
using System.Windows;
using System.Windows.Media.Imaging;
using System;
using Serilog;

namespace DeckGlow.ViewModels
{
    /// <summary>
    /// Used with each AppItem control in the app list on main window
    /// </summary>
    public class AppItemViewModel : ViewModelBase
    {

        public event EventHandler<BrightnessChangeEventArgs> BrightnessChangeEvent;

        private string _appPath;
        private int _brightness;

        public string AppPath
        {
            get => _appPath;
            set
            {
                bool pathChanged = false;
                if (_appPath != value) pathChanged = true;
                _appPath = value;
                if (pathChanged) UpdateIcon();
                OnPropertyChanged(nameof(AppPath));
            }
        }

        public int Brightness
        {
            get => _brightness;
            set
            {
                _brightness = value;
                OnPropertyChanged(nameof(Brightness));
                BrightnessChangeEvent?.Invoke(null, new BrightnessChangeEventArgs
                {
                    AppItemViewModel = this
                });
            }
        }

        public string AppName { get => Path.GetFileNameWithoutExtension(AppPath); }

        public BitmapSource? AppIcon { get; set; }

        private void UpdateIcon()
        {
            if (string.IsNullOrEmpty(AppPath))
            {
                AppIcon = null;
                return;
            }

            try
            {
                using (Icon? icon = Icon.ExtractAssociatedIcon(AppPath))
                {
                    if (icon == null)
                    {
                        AppIcon = null;
                        return;
                    }
                    AppIcon = Imaging.CreateBitmapSourceFromHIcon(icon.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                }
            }
            catch (FileNotFoundException ex)
            {
                Log.Error(ex, $"File not found: {ex.FileName}", ex.Message);
            }
        }

    }

    public class BrightnessChangeEventArgs : EventArgs
    {
        public AppItemViewModel AppItemViewModel { get; set; }
    }

}
