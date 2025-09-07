using System;
using System.Collections.Generic;
using System.IO;

namespace DeckGlow.Data
{

    [Serializable]
    public class AppConfig
    {

        public IDictionary<string, AppConfigItem> AppConfigDict { get; set; }

        public AppConfig()
        {
            AppConfigDict = new Dictionary<string, AppConfigItem>(StringComparer.OrdinalIgnoreCase);
        }

        public void SetAppBrightness(string key, int brightness)
        {
            AppConfigDict.TryGetValue(key, out AppConfigItem? item);
            item.Brightness = brightness;
        }

        public AppConfigItem? GetApp(string key)
        {
            // Check for exact match first
            if (AppConfigDict.TryGetValue(key, out AppConfigItem? appConfigItem))
            {
                return appConfigItem;
            }
            // Check for subdirectory match
            return GetAppForDir(key);
        }

        public AppConfigItem? GetAppForDir(string dirPath)
        {
            foreach (var entry in AppConfigDict)
            {
                string dictPath = Path.GetFullPath(entry.Key).ToLowerInvariant();
                // Check if the dirPath is a subdirectory of the dictionary path
                if (dirPath.StartsWith(dictPath, StringComparison.OrdinalIgnoreCase))
                {
                    return entry.Value;
                }
            }
            // No match found
            return null;
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
