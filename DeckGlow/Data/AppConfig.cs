using System;
using System.Collections.Generic;

namespace DeckGlow.Data
{

    [Serializable]
    public class AppConfig
    {

        public IDictionary<string, AppConfigItem> AppConfigDict { get; set; }

        public AppConfig()
        {
            AppConfigDict = new Dictionary<string, AppConfigItem>();
        }

        public void SetAppBrightness(string key, int brightness)
        {
            AppConfigDict.TryGetValue(key, out AppConfigItem? item);
            item.Brightness = brightness;
        }

        public AppConfigItem? GetApp(string key)
        {
            AppConfigDict.TryGetValue(key, out AppConfigItem? appConfigItem);
            return appConfigItem;
        }

        public void AddApp(string appName, int brightness)
        {
            AppConfigDict.Add(
                appName,
                new AppConfigItem() { Brightness = brightness }
            );
        }

        public void RemoveApp(string appName)
        {
            AppConfigDict.Remove(appName);
        }

        public bool Containskey(string key)
        {
            return AppConfigDict.ContainsKey(key);
        }

    }

    public class AppConfigItem
    {
        public int Brightness { get; set; }
    }

}
