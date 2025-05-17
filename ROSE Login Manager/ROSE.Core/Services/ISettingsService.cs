using ROSE.Core.Models;

namespace ROSE.Core.Services
{
    public interface ISettingsService
    {
        Settings GetSettings();
        void SaveSettings(Settings settings);
        void UpdateTheme(string theme);
    }
}