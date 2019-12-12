using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.IO;
using System.Threading.Tasks;
using static GeoGen.Infrastructure.Log;

namespace GeoGen.Infrastructure
{
    /// <summary>
    /// Represents a class that loads the settings for this application from the settings file.
    /// </summary>
    public static class SettingsLoader
    {
        /// <summary>
        /// Loads the settings for the class. If not successful, uses default values.
        /// </summary>
        /// <param name="settingsFilePath">The path to the settings file.</param>
        /// <returns>The loaded settings.</returns>
        public static async Task<TSettings> LoadAsync<TSettings>(string settingsFilePath, TSettings defaultSettings)
        {
            // Prepare the settings content
            string settingsJson;

            try
            {
                // Read the settings file
                settingsJson = await File.ReadAllTextAsync(settingsFilePath);
            }
            catch (Exception e)
            {
                // Re-thrown the problem
                throw new SettingsException($"Unable to read the settings file: {settingsFilePath}", e);
            }

            try
            {
                // Try to deserialize the settings.
                var settings = JsonConvert.DeserializeObject<TSettings>(settingsJson, new BaseLoggerSettingsConverter());

                // Log that we're done
                LoggingManager.LogInfo($"Settings loaded from: {settingsFilePath}");

                // Return the settings
                return settings;
            }
            // Catch any problem
            catch (Exception e)
            {
                // Serialize the default settings
                var defaultSettingsJson = JsonConvert.SerializeObject(defaultSettings, Formatting.Indented, new BaseLoggerSettingsConverter(), new StringEnumConverter());

                // Throw a problem with the most informative message
                throw new SettingsException($"Couldn't parse the settings from '{settingsFilePath}', the message: {e.Message}\n\n" +
                                            $"For inspiration, these are the default settings: \n\n{defaultSettingsJson}\n\n");
            }
        }
    }
}
