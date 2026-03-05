using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json;
using SnippetHotkeys.Models;

namespace SnippetHotkeys.Storage
{
    /// ***************************************************************** ///
    /// Class:      ConfigStore
    /// Summary:    Handles loading and saving application configuration
    ///             to a JSON file stored in the user's AppData folder
    /// ***************************************************************** /// 
    public sealed class ConfigStore 
    {
        // Full path to the config.json file
        private readonly string _configPath;

        // Constructor - Determines the AppData folder location and ensures the
        //      SnippetHotkeys directory exists
        public ConfigStore()
        {
            // Example: C:\Users\<User>\AppData\Roaming
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            // Create app-specific subfolder
            string folder = Path.Combine(appData, "SnippetHotkeys");

            // Ensure directory exists
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            // Final config file path
            _configPath = Path.Combine(folder, "config.json");
        }

        // Loads configuration from disk
        public AppConfig Load()
        {
            // First run: no config file yet
            if (!File.Exists(_configPath))
            {
                var defaultConfig = CreateDefaultConfig();
                Save(defaultConfig);
                return defaultConfig;
            }

            try
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                string json = File.ReadAllText(_configPath);

                // If config exists but is empty/null/can't deserialize into AppConfig,
                // return an empty config (do NOT re-seed defaults).
                return JsonSerializer.Deserialize<AppConfig>(json, options) ?? new AppConfig();
            }
            catch
            {
                // Backup the corrupted file so user doesn't lose it completely
                string backupPath = Path.Combine(
                    Path.GetDirectoryName(_configPath)!,
                    $"config.bad.{DateTime.Now:yyyyMMdd_HHmmss}.json");

                try { File.Move(_configPath, backupPath); } catch { /* ignore */ }

                // Start fresh (empty config)
                var fresh = new AppConfig();
                Save(fresh);
                return fresh;
            }
        }

        // Saves the provided configuration to disk & Overwrites existing file
        public void Save(AppConfig config)
        {
            // Pretty-print JSON for readability
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            // Convert object to JSON string
            string json = JsonSerializer.Serialize(config, options);

            // Write to file (overwrite if exists)
            File.WriteAllText(_configPath, json);
        }

        // Creates the initial default configuration used on first run
        private static AppConfig CreateDefaultConfig()
        {
            return new AppConfig
            {
                Version = 1,
                Hotkeys =
                {
                    new HotkeyBinding
                    {
                        Enabled = true,
                        Hotkey = "Ctrl+Alt+N",
                        Name = "Scheduled",
                        Snippet = "This has been scheduled for {NEXT_BUSINESS_DATE}."
                    },
                    new HotkeyBinding
                    {
                        Enabled = true,
                        Hotkey = "Ctrl+Alt+A",
                        Name = "Attached",
                        Snippet = "Please see attached."
                    },
                    new HotkeyBinding
                    {
                        Enabled = true,
                        Hotkey = "Ctrl+Alt+P",
                        Name = "PO Needed",
                        Snippet = "We will need a PO of $143 to reschedule."
                    },
                }
            };
        }
    }
}
