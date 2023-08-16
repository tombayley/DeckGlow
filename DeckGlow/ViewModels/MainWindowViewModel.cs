using CommunityToolkit.Mvvm.Input;
using DeckGlow.Data;
using DeckGlow.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace DeckGlow.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {

        private SettingsService _settingsService;
        private StreamDeckService _streamDeckService;

        public int DefaultBrightness
        {
            get => _settingsService.DefaultBrightness;
            set
            {
                _settingsService.DefaultBrightness = value;
                OnPropertyChanged(nameof(DefaultBrightness));
                _streamDeckService.SetBrightness(value);
            }
        }

        public string AppVersion
        {
            get => App.VersionString;
        }

        public delegate void DeleteAppEventAction(AppItemViewModel appItemViewModel);
        public event DeleteAppEventAction DeleteAppEvent;

        public ObservableCollection<AppItemViewModel> AppItems { get; set; }

        public ICommand DeleteApp_ClickCommand { get; private set; }

        public MainWindowViewModel(
            SettingsService settingsService,
            StreamDeckService streamDeckService
        )
        {
            _settingsService = settingsService;
            _streamDeckService = streamDeckService;

            DefaultBrightness = _settingsService.DefaultBrightness;

            DeleteApp_ClickCommand = new RelayCommand<AppItemViewModel>(DeleteApp_Click);
            AppItems = new ObservableCollection<AppItemViewModel>();

            foreach (KeyValuePair<string, AppConfigItem> entry in _settingsService.AppConfig.AppConfigDict)
            {
                AddAppItem(entry.Key, entry.Value.Brightness);
            }
        }

        private void AddAppItem(string key, int brightness)
        {
            AppItemViewModel viewModel = new AppItemViewModel
            {
                AppPath = key,
                Brightness = brightness
            };
            viewModel.BrightnessChangeEvent += AppItemBrightnessChangeEvent;
            AppItems.Add(viewModel);
        }

        private void RemoveAppItem(AppItemViewModel appItemViewModel)
        {
            appItemViewModel.BrightnessChangeEvent -= AppItemBrightnessChangeEvent;
            AppItems.Remove(appItemViewModel);
        }

        private void AppItemBrightnessChangeEvent(object? sender, BrightnessChangeEventArgs e)
        {
            _settingsService.AppConfig.SetAppBrightness(
                e.AppItemViewModel.AppPath,
                e.AppItemViewModel.Brightness
            );
            _settingsService.SaveAppConfig();
            // Preview the brightness
            _streamDeckService.SetBrightness(e.AppItemViewModel.Brightness);
        }

        public void SetDefaultBrightness(int defaultBrightness, bool apply)
        {
            DefaultBrightness = defaultBrightness;
            if (apply)
            {
                _streamDeckService.SetBrightness(defaultBrightness);
            }
        }

        public void AddApp(string appPath, int brightness)
        {
            if (_settingsService.AppConfig.Containskey(appPath)) return;

            _settingsService.AppConfig.AddApp(appPath, brightness);
            _settingsService.SaveAppConfig();

            AddAppItem(appPath, brightness);
        }

        public void RemoveApp(AppItemViewModel appItemViewModel)
        {
            _settingsService.AppConfig.RemoveApp(appItemViewModel.AppPath);
            _settingsService.SaveAppConfig();

            RemoveAppItem(appItemViewModel);
        }

        private async void DeleteApp_Click(AppItemViewModel appItemViewModel)
        {
            RemoveApp(appItemViewModel);
            DeleteAppEvent?.Invoke(appItemViewModel);
        }

    }
}
