using ROSE.Core.Configuration;
using ROSE.Core.Models;
using ROSE.Core.Services;
using System.Text.Json;

namespace ROSE.Infrastructure.Services
{
    public class SettingsService : ISettingsService
    {
        private readonly string _settingsPath;
        private Settings _settings;

        public SettingsService()
        {
            AppPaths.EnsureDirectoriesExist();
            _settingsPath = AppPaths.SettingsFilePath;
            _settings = LoadSettings();
        }

        public Settings GetSettings()
        {
            return _settings;
        }

        public void SaveSettings(Settings settings)
        {
            _settings = settings;
            var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_settingsPath, json);
        }

        public void UpdateTheme(string theme)
        {
            _settings.Theme = theme;
            SaveSettings(_settings);
        }

        private Settings LoadSettings()
        {
            if (File.Exists(_settingsPath))
            {
                try
                {
                    var json = File.ReadAllText(_settingsPath);
                    return JsonSerializer.Deserialize<Settings>(json) ?? new Settings();
                }
                catch
                {
                    return new Settings();
                }
            }
            return new Settings();
        }
    }
}