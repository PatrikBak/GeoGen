using Newtonsoft.Json;
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
        /// <returns>The loaded settings.</returns>
        public static async Task<TSettings> LoadFromFileAsync<TSettings>(string settingsFilePath)
        {
            // Prepare the settings content
            string settingsJson;

            try
            {
                // Open the stream
                using var reader = new StreamReader(new FileStream(settingsFilePath, FileMode.Open));

                // Read the settings file
                settingsJson = await reader.ReadToEndAsync();
            }
            catch (Exception e)
            {
                // Re-thrown the problem
                throw new SettingsException($"Unable to read the settings file: {settingsFilePath}", e);
            }

            // Delegate the call to the other method
            return LoadFromString<TSettings>(settingsJson);
        }

        /// <summary>
        /// Loads the JSON settings of a given type from a string.
        /// </summary>
        /// <typeparam name="TSettings">The type of settings to be loaded.</typeparam>
        /// <param name="settingsJson">The JSON string with the settings to be loaded.</param>
        /// <returns>The loaded settings.</returns>
        public static TSettings LoadFromString<TSettings>(string settingsJson)
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
                // Throw the problem further 
                throw new SettingsException("Couldn't parse the settings", e);
            }
        }
    }
}