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
        /// Loads the JSON settings of a given type from a file.
        /// </summary>
        /// <typeparam name="TSettings">The type of settings to be loaded.</typeparam>
        /// <param name="settingsFilePath">The path to the settings file.</param>
        /// <param name="defaultSettings">The default settings, used as an example for how the JSON should look like.</param>
        /// <returns>The loaded settings.</returns>
        public static async Task<TSettings> LoadFromFileAsync<TSettings>(string settingsFilePath, TSettings defaultSettings)
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

            // Delegate the call to the other method
            return LoadFromString(settingsJson, defaultSettings);
        }

        /// <summary>
        /// Loads the JSON settings of a given type from a string.
        /// </summary>
        /// <typeparam name="TSettings">The type of settings to be loaded.</typeparam>
        /// <param name="settingsJson">The JSON string with the settings to be loaded.</param>
        /// <param name="defaultSettings">The default settings, used as an example for how the JSON should look like.</param>
        /// <returns>The loaded settings.</returns>
        public static TSettings LoadFromString<TSettings>(string settingsJson, TSettings defaultSettings)
        {
            try
            {
                // Try to deserialize the settings.
                var settings = JsonConvert.DeserializeObject<TSettings>(settingsJson, new BaseLoggerSettingsConverter());

                // If it's null, make aware
                if (settings == null)
                    throw new SettingsException("The deserializer of settings returned null.");

                // Log that we're done
                LoggingManager.LogInfo($"Settings successfully loaded.");

                // Return the settings
                return settings;
            }
            // Catch any problem
            catch (Exception e)
            {
                // Serialize the default settings
                var defaultSettingsJson = JsonConvert.SerializeObject(defaultSettings, Formatting.Indented, new BaseLoggerSettingsConverter(), new StringEnumConverter());

                // Throw a problem with the most informative message
                throw new SettingsException($"Couldn't parse the settings, the message: {e.Message}\n\n" +
                                            $"For inspiration, these are the default settings: \n\n{defaultSettingsJson}\n\n");
            }
        }
    }
}