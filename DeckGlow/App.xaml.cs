﻿using H.NotifyIcon;
using DeckGlow.Services;
using System.IO;
using System.Reflection;
using System.Windows;
using DeckGlow.Views;
using Microsoft.Extensions.DependencyInjection;
using DeckGlow.ViewModels;
using System;
using DeckGlow.Properties;

namespace DeckGlow
{
    public partial class App : Application
    {
        private readonly IServiceProvider _serviceProvider;

        private TaskbarIcon? notifyIcon;

        private static Assembly Assembly { get; } = Assembly.GetExecutingAssembly();

        public static string Name { get; } = Assembly.GetName().Name!;

        public static Version Version { get; } = Assembly.GetName().Version!;

        public static string VersionString { get; } = Version.ToString(3);

        public static string ExecutablePath { get; } = Path.ChangeExtension(Assembly.Location, "exe");

        public static string UrlReleaseHistory { get; } = "https://github.com/tombayley/DeckGlow/releases";

        public App()
        {
            IServiceCollection services = new ServiceCollection();
            ConfigureServices(services);
            _serviceProvider = services.BuildServiceProvider();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<SettingsService>();
            services.AddSingleton<StreamDeckService>();
            services.AddSingleton<WindowFocusService>();

            services.AddTransient<MainWindow>();

            services.AddSingleton<NotifyIconViewModel>();
            services.AddSingleton(s => new MainWindowViewModel(
                _serviceProvider.GetRequiredService<SettingsService>(),
                _serviceProvider.GetRequiredService<StreamDeckService>()
            ));
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            if (Settings.Default.UpgradeRequired)
            {
                Settings.Default.Upgrade();
                Settings.Default.UpgradeRequired = false;
                Settings.Default.Save();
            }

            notifyIcon = (TaskbarIcon)FindResource("NotifyIcon");
            notifyIcon.ForceCreate();
            notifyIcon.DataContext = _serviceProvider.GetRequiredService<NotifyIconViewModel>();

            SettingsService settingsService = _serviceProvider.GetRequiredService<SettingsService>();

            // Open the main window only on first launch
            if (settingsService.FirstLaunch)
            {
                OpenMainWindow();
                settingsService.FirstLaunch = false;
            }
        }

        private void OpenMainWindow()
        {
            Current.MainWindow ??= _serviceProvider.GetRequiredService<MainWindow>();
            Current.MainWindow.Show();
            Current.MainWindow.Activate();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            notifyIcon?.Dispose();
            base.OnExit(e);
        }

    }

}
