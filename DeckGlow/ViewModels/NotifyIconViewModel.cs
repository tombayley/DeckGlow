using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows;
using DeckGlow.Services;
using DeckGlow.Data;
using System.Windows.Input;
using System;
using Microsoft.Extensions.DependencyInjection;
using DeckGlow.Views;

namespace DeckGlow.ViewModels
{
    public partial class NotifyIconViewModel : ObservableObject
    {

        private IServiceProvider _serviceProvider;
        private SettingsService _settingsService;
        private StreamDeckService _streamDeckService;
        private WindowFocusService _windowFocusService;

        public ICommand ShowWindowCommand { get { return new RelayCommand(ShowWindow); } }

        public bool StartOnBoot
        {
            get => _settingsService.StartOnBoot;
            set => _settingsService.StartOnBoot = value;
        }

        public NotifyIconViewModel(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _settingsService = _serviceProvider.GetRequiredService<SettingsService>();
            _streamDeckService = _serviceProvider.GetRequiredService<StreamDeckService>();
            _windowFocusService = _serviceProvider.GetRequiredService<WindowFocusService>();
            _windowFocusService.FocusChangeEvent += FocusService_FocusChangeEvent;
        }

        /// <summary>
        /// Called when global window focus changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FocusService_FocusChangeEvent(object? sender, WindowFocusChangeEventArgs e)
        {
            // Use default brightness if not matching app found in app config
            int brightness = _settingsService.DefaultBrightness;

            AppConfigItem? appConfigItem = _settingsService.AppConfig.GetApp(e.ProcessPath);
            if (appConfigItem != null)
            {
                // Matching app found in app config, use it's brightness value
                brightness = appConfigItem.Brightness;
            }
            _streamDeckService.SetBrightness(brightness);
        }

        public void ShowWindow()
        {
            Application.Current.MainWindow ??= _serviceProvider.GetRequiredService<MainWindow>();
            Application.Current.MainWindow.Show();
            Application.Current.MainWindow.Activate();
        }

        [RelayCommand]
        public void ExitApplication()
        {
            _windowFocusService.Dispose();
            Application.Current.Shutdown();
        }

    }
}
