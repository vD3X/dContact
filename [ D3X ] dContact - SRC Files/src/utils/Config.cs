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
                Instance.Logger.LogInformation("Plik konfiguracyjny nie istnieje. Tworzenie nowego pliku z domyślną konfiguracją.");
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
                        name = "D3X",
                        rank = "Właściciel Serwera",
                        steam = "https://steamcommunity.com/id/dd3xx",
                        forum = "https://cs-zjarani.pl",
                        discord = "dd3xx"
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
                    Instance.Logger.LogError("Plik konfiguracyjny jest pusty lub ma błędną strukturę.");
                    return null;
                }

                Instance.Logger.LogInformation("Konfiguracja została załadowana poprawnie.");
                return loadedConfig;
            }
            catch (Exception ex)
            {
                Instance.Logger.LogError($"Błąd podczas wczytywania pliku konfiguracyjnego: {ex.Message}");
                return null;
            }
        }

        public static void SaveConfig(ConfigModel config)
        {
            try
            {
                string json = JsonConvert.SerializeObject(config, Formatting.Indented);
                File.WriteAllText(configPath, json);
                Instance.Logger.LogInformation("Plik konfiguracyjny został zapisany.");
            }
            catch (Exception ex)
            {
                Instance.Logger.LogError($"Błąd podczas zapisywania pliku konfiguracyjnego: {ex.Message}");
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
                    Instance.Logger.LogInformation("Konfiguracja została zaktualizowana po zmianie pliku.");
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
            public string Contact_Command { get; set; } = "contact, kontakt";
            public string Title { get; set; } = "[ ★ CS-Zjarani | Kontakty ★ ]";
            public string TitleColor { get; set; } = "#29cc94";
        }

        public class ContactData
        {
            public string name { get; set; }
            public string rank { get; set; }
            public string steam { get; set; }
            public string forum { get; set; }
            public string discord { get; set; }
        }
    }
}