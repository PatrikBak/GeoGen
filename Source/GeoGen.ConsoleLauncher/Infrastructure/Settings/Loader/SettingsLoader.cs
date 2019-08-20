using Newtonsoft.Json;
using System;
using System.IO;

namespace GeoGen.ConsoleLauncher
{
    /// <summary>
    /// Represents a class that loads the settings for this application from the settings file.
    /// </summary>
    public static class SettingsLoader
    {
        #region Private fields

        /// <summary>
        /// The path to the settings file.
        /// </summary>
        private const string _settingsFilePath = "settings.json";

        #endregion

        #region Load method

        /// <summary>
        /// Loads the settings for the class. If not successful, uses default values.
        /// </summary>
        /// <returns>The loaded settings.</returns>
        public static Settings Load()
        {
            try
            {
                // Read the settings file
                var settingsJson = File.ReadAllText(_settingsFilePath);

                // Try to deserialize the settings.
                return JsonConvert.DeserializeObject<Settings>(settingsJson, new BaseLoggerSettingsConverter());
            }
            // If anything bad happens
            catch (Exception)
            {
                // Create the default settings
                var settings = new DefaultSettings();

                try
                {
                    // Serialize it
                    var settingsJson = JsonConvert.SerializeObject(settings, Formatting.Indented, new BaseLoggerSettingsConverter());

                    // Try to save it
                    File.WriteAllText(_settingsFilePath, settingsJson);
                }
                // If it doesn't work, the file is locked, or whatever. Never mind...
                catch { }

                // Return the default settings
                return settings;
            }
        }

        #endregion
    }
}
