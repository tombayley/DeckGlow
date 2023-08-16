using Newtonsoft.Json;
using DeckGlow.Data;
using DeckGlow.Properties;
using DeckGlow.Managers;
using Serilog;

namespace DeckGlow.Services
{
    public class SettingsService
    {
        public AppConfig AppConfig;

        public int DefaultBrightness
        {
            get => Settings.Default.DefaultBrightness;
            set
            {
                if (value == Settings.Default.DefaultBrightness) return;
                Settings.Default.DefaultBrightness = value;
                Settings.Default.Save();
            }
        }

        public bool StartOnBoot
        {
            get => StartupManager.CheckLaunchOnUserLogin();
            set
            {
                if (value) StartupManager.EnableLaunchOnUserLogin();
                else StartupManager.DisableLaunchOnUserLogin();
            }
        }

        public bool FirstLaunch
        {
            get => Settings.Default.FirstLaunch;
            set
            {
                Settings.Default.FirstLaunch = value;
                Settings.Default.Save();
            }
        }

        public SettingsService()
        {
            LoadAppConfig();
        }

        public void LoadAppConfig()
        {
            string appConfigJson = Settings.Default.AppConfigJson;
            try
            {
                AppConfig = JsonConvert.DeserializeObject<AppConfig>(appConfigJson) ?? new AppConfig();
            }
            catch (JsonSerializationException ex)
            {
                Log.Fatal(ex, "An error occurred during app config deserialization: {msg}", ex.Message);
            }
        }

        public void SaveAppConfig()
        {
            string appConfigJson = JsonConvert.SerializeObject(AppConfig);
            Settings.Default.AppConfigJson = appConfigJson;
            Settings.Default.Save();
        }

    }
}
