using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Immutable;

namespace GeoGen.Infrastructure
{
    /// <summary>
    /// Provides a type-safe way to retrieve settings stored in JSON configuration files.
    /// </summary>
    public class JsonConfiguration
    {
        #region Public properties

        /// <summary>
        /// The list of loaded files with configuration.
        /// </summary>
        public ImmutableList<string> LoadedConfigurationFilePaths { get; private set; } = ImmutableList<string>.Empty;

        #endregion

        #region Private fields

        /// <summary>
        /// The parsed JSON settings files
        /// </summary>
        private readonly IReadOnlyList<JObject> _loadedConfigurationFiles;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonConfiguration"/> class containing the contests of the passed settings files.
        /// </summary>
        /// <param name="jsonConfigurationFilePaths">The paths to the JSON settings files. Non-existent ones are silently ignored.</param>
        public JsonConfiguration(params string[] jsonConfigurationFilePaths) =>
            // Load all the configuration files by taking the passed files
            _loadedConfigurationFiles = jsonConfigurationFilePaths
                // That exists
                .Where(file => File.Exists(file))
                // Load them
                .Select(file =>
                {
                    // Parse the JSON
                    var jsonConfiguration = (JObject)JsonConvert.DeserializeObject(File.ReadAllText(file));

                    // Mark the loaded file
                    LoadedConfigurationFilePaths = LoadedConfigurationFilePaths.Add(file);

                    // Return the JSON
                    return jsonConfiguration;
                })
                // Enumerate
                .ToList();

        #endregion

        #region Public methods

        /// <summary>
        /// Retrieves a specific settings raw JSON string with a given property name.
        /// </summary>
        /// <param name="propertyName">The name of the property in the JSON configuration with the settings.</param>
        /// <returns>The JSON settings string.</returns>
        public string GetSettingsAsJson(string propertyName)
            // The value from the last loaded file overwrites
            => _loadedConfigurationFiles.LastOrDefault(json => json.Children<JProperty>().Any(property => property.Name == propertyName))
                // Get the property
                ?[propertyName]
                // Converting it to a string results in a JSON string
                .ToString()
                // Ensure there is this key
                ?? throw new ConfigurationException($"The configuration does not contain settings for property {propertyName}.");

        /// <summary>
        /// Retrieves a specific settings object from the configuration. The name of the corresponding
        /// section should correspond to the name of the settings class. If it doesn't, it can be set
        /// via <paramref name="propertyName"/>.
        /// </summary>
        /// <typeparam name="T">The type of the settings object.</typeparam>
        /// <param name="propertyName">The name of the property in the JSON configuration with the settings.</param>
        /// <returns>The retrieved settings object.</returns>
        public T GetSettings<T>(string propertyName = null)
        {
            // Ensure the property name is equal to the name of the class, if it is not specified
            propertyName ??= typeof(T).Name;

            // The value from the last loaded file overwrites
            var settingsJson = _loadedConfigurationFiles.LastOrDefault(json => json.Children<JProperty>().Any(property => property.Name == propertyName))
                // If the value is not there, make aware
                ?? throw new ConfigurationException($"The configuration does not contain settings of type {typeof(T)} under property name {propertyName}.");

            try
            {
                // Otherwise try to serialize it
                return settingsJson[propertyName].ToObject<T>();
            }
            catch (Exception e)
            {
                // Make aware if it doesn't work out
                throw new ConfigurationException($"Cannot serialize {settingsJson} into an object of type {typeof(T)}", e);
            }
        }

        #endregion
    }
}