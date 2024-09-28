using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using static dContact.dContact;

namespace dContact
{
    public static class Config
    {
        private static readonly string configPath = Path.Combine(Instance.ModuleDirectory, "Config.json");
        public static ConfigModel config;
        private static FileSystemWatcher fileWatcher;

        public static void Initialize()
        {
            if (!File.Exists(configPath))
            {
                Instance.Logger.LogInformation("Configuration file does not exist. Creating a new file with default settings.");
                CreateDefaultConfig();
            }

            config = LoadConfig();

            SetupFileWatcher();
        }

        private static void CreateDefaultConfig()
        {
            var defaultConfig = new ConfigModel
            {
                AdminContacts = new List<ContactData>
                {
                    new ContactData
                    {
                        Name = "D3X",
                        Rank = "Właściciel Serwera",
                        Contacts = new Dictionary<string, string>
                        {
                            { "Steam", "https://steamcommunity.com/id/dd3xx" },
                            { "Forum", "https://cs-zjarani.pl" },
                            { "Discord", "dd3xx" }
                        }
                    },
                }
            };

            SaveConfig(defaultConfig);
        }

        private static ConfigModel LoadConfig()
        {
            try
            {
                string json = File.ReadAllText(configPath);
                var loadedConfig = JsonConvert.DeserializeObject<ConfigModel>(json);

                if (loadedConfig == null || loadedConfig.AdminContacts == null || !loadedConfig.AdminContacts.Any())
                {
                    Instance.Logger.LogError("The configuration file is empty or has an incorrect structure.");
                    return null;
                }

                return loadedConfig;
            }
            catch (Exception ex)
            {
                Instance.Logger.LogError($"Error loading the configuration file: {ex.Message}");
                return null;
            }
        }

        public static void SaveConfig(ConfigModel config)
        {
            try
            {
                string json = JsonConvert.SerializeObject(config, Formatting.Indented);
                File.WriteAllText(configPath, json);
            }
            catch (Exception ex)
            {
                Instance.Logger.LogError($"Error saving the configuration file: {ex.Message}");
            }
        }

        private static void SetupFileWatcher()
        {
            fileWatcher = new FileSystemWatcher(Path.GetDirectoryName(configPath))
            {
                Filter = Path.GetFileName(configPath),
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size
            };

            fileWatcher.Changed += (sender, e) => {
                Thread.Sleep(500);
                var newConfig = LoadConfig();
                if (newConfig != null)
                {
                    config = newConfig;
                    adminsList = config.AdminContacts;
                }
            };

            fileWatcher.EnableRaisingEvents = true;
        }

        public class ConfigModel
        {
            public Settings Settings { get; set; } = new Settings();
            public List<ContactData> AdminContacts { get; set; }
        }

        public class Settings
        {
            public string Contact_Commands { get; set; } = "contact, kontakt";
            public string Menu_Title { get; set; } = "[ ★ CS-Zjarani | Kontakty ★ ]";
            public string Menu_Title_Color { get; set; } = "#29cc94";
        }

        public class ContactData
        {
            public string Name { get; set; }
            public string Rank { get; set; }
            public Dictionary<string, string> Contacts { get; set; } = new Dictionary<string, string>();
        }
    }
}