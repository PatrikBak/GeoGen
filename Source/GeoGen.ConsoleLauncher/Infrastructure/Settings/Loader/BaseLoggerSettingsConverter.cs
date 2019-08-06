using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Text.RegularExpressions;

namespace GeoGen.ConsoleLauncher
{
    /// <summary>
    /// The <see cref="JsonConverter"/> for the type of <see cref="BaseLoggerSettings"/>.
    /// </summary>
    public class BaseLoggerSettingsConverter : JsonConverter
    {
        #region JsonConverter implementation

        public override bool CanConvert(Type objectType)
        {
            // We can convert only derived types of the BaseLoggerSettings
            return typeof(BaseLoggerSettings).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            // Get the JSON object representing the current settings
            var item = JObject.Load(reader);

            // Find the name of the class that we should serialize
            var name = item["Name"].Value<string>();

            // Distinguish between loggers
            return name switch
            {
                // If it's console logger...
                "ConsoleLogger" => item.ToObject<ConsoleLoggerSettings>() as BaseLoggerSettings,

                // If it's file logger...
                "FileLogger" => item.ToObject<FileLoggerSettings>(),

                // Otherwise we failed...
                _ => throw new Exception("Unable to parse the settings file"),
            };
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            // Find the name of the logger
            var loggerName = ExtractLoggerName(value);

            // Get the JSON object from the passed value
            var jsonObject = (JObject)JToken.FromObject(value);

            // Add the name property
            jsonObject.AddFirst(new JProperty("Name", new JValue(loggerName)));

            // Write the object to the writer
            jsonObject.WriteTo(writer);
        }

        #endregion

        #region Private helpers

        /// <summary>
        /// Finds the name of the logger based on the passed object, which we know that will be
        /// an instance of <see cref="BaseLoggerSettings"/>.
        /// </summary>
        /// <param name="value">The settings.</param>
        /// <returns>The extracted name of the logger.</returns>
        private string ExtractLoggerName(object value)
        {
            // Find the name of the class
            var className = value.GetType().Name;

            // Try to match the regex 
            var match = Regex.Match(className, "^(.*)LoggerSettings");

            // If we succeeded...
            return match.Success
                // Return the value of the first group (the name of the logger) with the prefix 'Logger'
                ? $"{match.Groups[1].Value}Logger"
                // Otherwise throw an exception
                : throw new Exception($"Invalid name for the logger class: {match}. It should end with the string 'LoggerSettings'.");
        }

        #endregion
    }
}
