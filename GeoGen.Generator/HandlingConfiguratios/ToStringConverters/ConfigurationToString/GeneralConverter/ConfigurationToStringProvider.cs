using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Core.Configurations;
using GeoGen.Utilities;

namespace GeoGen.Generator
{
    /// <summary>
    /// A default implementation of <see cref="IConfigurationToStringProvider"/>.
    /// It simply converts each object to string and join these strings using the default
    /// separator. This sealed class is thread-safe.
    /// </summary>
    internal sealed class ConfigurationToStringProvider : IConfigurationToStringProvider
    {
        #region Private constants

        /// <summary>
        /// The objects separator.
        /// </summary>
        private const string ObjectsSeparator = "|";

        #endregion

        #region Private fields

        /// <summary>
        /// The cache dictionary mapping ids of object converters to the dictionary 
        /// mapping converted configurations ids to the sorted set of their
        /// constructed objects.
        /// </summary>
        private readonly Dictionary<int, Dictionary<int, SortedSet<string>>> _cache;

        /// <summary>
        /// The initial configuration.
        /// </summary>
        private readonly Configuration _initialConfiguration;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructs a configuration to string provider that has the reference
        /// to the initial configuration which it uses as the base for recursive
        /// to string conversion.
        /// </summary>
        /// <param name="initialConfiguration">The initial configuration.</param>
        public ConfigurationToStringProvider(Configuration initialConfiguration)
        {
            _initialConfiguration = initialConfiguration ?? throw new ArgumentNullException(nameof(initialConfiguration));
            _cache = new Dictionary<int, Dictionary<int, SortedSet<string>>>();
        }

        #endregion

        #region IConfigurationToStringProvider implementation

        /// <summary>
        /// Converts a given configuration to string, using a given 
        /// configuration object to string provider.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="objectToString">The configuration object to string provider.</param>
        /// <returns>The string representation of the configuration.</returns>
        public string ConvertToString(ConfigurationWrapper configuration, IObjectToStringConverter objectToString)
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            if (objectToString == null)
                throw new ArgumentNullException(nameof(objectToString));

            // Get the dictionary for the converter
            var dictionary = GetDictionaryForConverter(objectToString);

            // Pull initial configuration
            var previousConfiguration = configuration.PreviousConfiguration;

            // Unwrap configuration.
            var unwraped = previousConfiguration?.Configuration ?? throw new GeneratorException("Attempt to convert initial configuration directly");

            // Find out if it's initial configuration (which is not supposed be converted)
            var isInitial = _initialConfiguration == unwraped;

            // We should convert the initial configuration only as the first converted one
            var shouldWeConvertInitialFirst = isInitial && dictionary.Empty();

            // If it's initial, we need to convert this one first
            if (shouldWeConvertInitialFirst)
            {
                // Initialize the new result
                var initialResult = new SortedSet<string>();

                // Add all constructed objects to the set
                foreach (var newObject in unwraped.ConstructedObjects)
                {
                    // Convert object to string
                    var stringValue = objectToString.ConvertToString(newObject);

                    // Add it to the set
                    initialResult.Add(stringValue);
                }

                // Pull id of the configuration that should be the initial one
                var id = previousConfiguration.Id ?? throw GeneratorException.ConfigurationIdNotSet();

                // Cache the result
                dictionary.Add(id, initialResult);
            }

            // Now we can convert the current one. Pull id of previous configuration
            var previousId = previousConfiguration.Id ?? throw GeneratorException.ConfigurationIdNotSet();

            // We'll pull the initial sorted set
            var initialConfigurationSet = dictionary[previousId];

            // Copy the set. This should be O(n)
            var result = new SortedSet<string>(initialConfigurationSet);

            // Add new objects to the new set
            foreach (var newObject in configuration.LastAddedObjects)
            {
                // Convert object to string
                var stringValue = objectToString.ConvertToString(newObject);

                // Add it to the set
                result.Add(stringValue);
            }

            // Pull id of the current configuration
            var currentId = configuration.Id ?? throw GeneratorException.ConfigurationIdNotSet();

            // Cache the result
            dictionary.Add(currentId, result);

            // Return joined result
            return string.Join(ObjectsSeparator, result);
        }

        private Dictionary<int, SortedSet<string>> GetDictionaryForConverter(IObjectToStringConverter converter)
        {
            // Pull converter id
            var converterId = converter.Id;

            // If the cache doesn't contain this id
            if (!_cache.ContainsKey(converterId))
            {
                // Add new dictionary to the cache
                _cache.Add(converterId, new Dictionary<int, SortedSet<string>>());
            }

            // Return the cache dictionary
            return _cache[converterId];
        }

        #endregion
    }
}