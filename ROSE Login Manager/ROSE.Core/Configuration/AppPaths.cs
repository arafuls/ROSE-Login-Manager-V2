using System.IO;

namespace ROSE.Core.Configuration
{
    /// <summary>
    /// Provides centralized access to application paths and file names.
    /// </summary>
    public static class AppPaths
    {
        /// <summary>
        /// The name of the application.
        /// </summary>
        public const string AppName = "ROSE Login Manager";

        /// <summary>
        /// Gets the path to the application's AppData directory.
        /// </summary>
        public static string AppDataPath => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            AppName
        );

        /// <summary>
        /// Gets the path to the data directory.
        /// </summary>
        public static string DataDirectory => Path.Combine(AppDataPath, "Data");

        /// <summary>
        /// Gets the path to the database file.
        /// </summary>
        public static string DatabasePath => Path.Combine(DataDirectory, "profiles.db");

        /// <summary>
        /// Gets the path to the settings directory.
        /// </summary>
        public static string SettingsDirectory => Path.Combine(AppDataPath, "Settings");

        /// <summary>
        /// Gets the path to the settings file.
        /// </summary>
        public static string SettingsFilePath => Path.Combine(SettingsDirectory, "settings.json");

        /// <summary>
        /// Gets the path to the configuration file.
        /// </summary>
        public static string ConfigFilePath => Path.Combine(AppDataPath, "config.xml");

        /// <summary>
        /// Gets the path to the ROSE Online game configuration file.
        /// </summary>
        public static string RoseConfigFilePath => Path.Combine(
            Directory.GetParent(AppDataPath)?.FullName ?? string.Empty,
            "Rednim Games", "ROSE Online", "config", "rose.toml"
        );

        /// <summary>
        /// Gets the path to the machine key file.
        /// </summary>
        public static string MachineKeyPath => Path.Combine(DataDirectory, "machine.key");

        /// <summary>
        /// Ensures all required directories exist.
        /// </summary>
        public static void EnsureDirectoriesExist()
        {
            Directory.CreateDirectory(AppDataPath);
            Directory.CreateDirectory(DataDirectory);
            Directory.CreateDirectory(SettingsDirectory);
        }

        static AppPaths()
        {
            if (!Directory.Exists(AppDataPath))
            {
                Directory.CreateDirectory(AppDataPath);
            }

            // Ensure the data directory exists
            if (!Directory.Exists(DataDirectory))
            {
                Directory.CreateDirectory(DataDirectory);
            }
        }
    }
}