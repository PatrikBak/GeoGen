using System.Collections.Generic;
using GeoGen.Core.Generator;
using GeoGen.Utilities;

namespace GeoGen.Generator
{
    /// <summary>
    /// A default implementation of <see cref="IConfigurationToStringProvider"/>. 
    /// 
    /// The conversion needs a given <see cref="IObjectToStringConverter"/> and works
    /// like this: It converts all constructed objects using the converter, sorts them
    /// lexicographically and joins them using the '|' separator.
    /// 
    /// This class should be used like this: In order to convert a configuration to 
    /// string, we must first convert the configuration that was extended to obtain our 
    /// configuration  (i.e. the parental configuration of the given one). The only 
    /// exception is the initial configuration. This one will be pulled from the given
    /// configuration and converted automatically (the reason behind this was simple: 
    /// in the algorithm, there was no need to consume the string version of the initial 
    /// configuration - and that's the only case when we use a configuration that we don't
    /// need to convert to string).
    /// 
    /// The lexicographical order of the old objects is cached in a <see cref="SortedSet{T}"/> 
    /// of strings. So if a configuration has 'n' old objects and 't' new objects, the
    /// conversion is O(n + t log(n+t)), instead of usual O((n+t) log (n+t)). 
    /// 
    /// This caching obviously happens for each <see cref="IObjectToStringConverter"/>.
    /// The key in the caching dictionary is the id of the <see cref="IObjectIdResolver"/>
    /// that is used by a given object converter. So the caching wouldn't work correctly for
    /// distinct converters that use the same id resolver. But in the algorithm, it won't happen.
    /// </summary>
    internal class ConfigurationToStringProvider : IConfigurationToStringProvider
    {
        #region Private constants

        /// <summary>
        /// The configuration objects string versions separator.
        /// </summary>
        private const string ObjectsSeparator = "|";

        #endregion

        #region Private fields

        /// <summary>
        /// The cache. The keys are the ids of <see cref="IObjectIdResolver"/>s
        /// that are used by provided <see cref="IObjectToStringConverter"/>s. The values
        /// are dictionaries mapping ids of configurations to sorted set of the string versions
        /// of their constructed configuration objects.
        /// </summary>
        private readonly Dictionary<int, Dictionary<int, SortedSet<string>>> _cachedSets;

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ConfigurationToStringProvider()
        {
            _cachedSets = new Dictionary<int, Dictionary<int, SortedSet<string>>>();
        }

        #endregion

        #region IConfigurationToStringProvider implementation

        /// <summary>
        /// Converts a given configuration to string, using a given 
        /// configuration object to string converter. 
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="objectToString">The configuration object to string converter.</param>
        /// <returns>The string representation of the configuration.</returns>
        public string ConvertToString(ConfigurationWrapper configuration, IObjectToStringConverter objectToString)
        {
            // Resolve the cached value
            var cachedValue = ResolveCache(configuration, objectToString);

            // If there's any, return it
            if (cachedValue != string.Empty)
                return cachedValue;

            // Get the dictionary for the converter
            var dictionary = GetDictionaryForConverter(objectToString);

            // Pull previous configuration
            var previousConfiguration = configuration.PreviousConfiguration;

            // Find out if it's the initial configuration.
            var isInitial = previousConfiguration.PreviousConfiguration is null;

            // We should convert the initial configuration only as the first converted one (for each converter)
            // Unlike every other configuration, it's not supposed that the initial one
            // will be converted directly by calling this method
            var shouldWeConvertInitialFirst = isInitial && dictionary.Empty();

            // Convert the initial configuration if we should do so
            if (shouldWeConvertInitialFirst)
            {
                // Initialize the new result
                var initialResult = new SortedSet<string>();

                // Add all constructed objects to the set
                foreach (var newObject in previousConfiguration.WrappedConfiguration.ConstructedObjects)
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

            // Let the helper method find the set of strings corresponding to the
            // original configuration (with regards to current converter)
            var originalConfigurationSet = FindOriginalConfigurationSet(configuration, objectToString);

            // Copy the set. This should be O(n) - which is the reason for caching
            var result = new SortedSet<string>(originalConfigurationSet);

            // Add the new objects to the new set
            foreach (var newObject in configuration.LastAddedObjects)
            {
                // Convert object to string
                var stringValue = objectToString.ConvertToString(newObject);

                // Add it to the set. This is log(n) operation and preserves the lexicographical
                result.Add(stringValue);
            }

            // Pull id of the current configuration
            var currentId = configuration.Id ?? throw GeneratorException.ConfigurationIdNotSet();

            // Cache the result
            dictionary.Add(currentId, result);

            // Construct joined result. The objects will be sorted, because the sorted set 
            // keep its elements in the sorted order. That way we get the representation
            // independent on the order of constructed objects.
            var resultString = string.Join(ObjectsSeparator, result);

            // Return the result
            return resultString;
        }

        /// <summary>
        /// Finds out whether a given configuration has already been converted
        /// with a given object to string converted.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="objectToString">The object to string converter.</param>
        /// <returns>The cached value, if there's any, or an empty string otherwise.</returns>
        private string ResolveCache(ConfigurationWrapper configuration, IObjectToStringConverter objectToString)
        {
            // Pull id of the current configuration
            var currentId = configuration.Id ?? throw GeneratorException.ConfigurationIdNotSet();

            // Get dictionary for the converter
            var dictionary = GetDictionaryForConverter(objectToString);

            // If there is the current id
            if (dictionary.ContainsKey(currentId))
            {
                // Return joined strings
                return string.Join(ObjectsSeparator, dictionary[currentId]);
            }

            // Otherwise return empty string
            return string.Empty;
        }

        /// <summary>
        /// Finds the sorted strings representing converted objects of the
        /// previous configuration of a given configuration, with a given
        /// object to string converter.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="objectToString">The object to string converter.</param>
        /// <returns>The sorted strings.</returns>
        private SortedSet<string> FindOriginalConfigurationSet(ConfigurationWrapper configuration, IObjectToStringConverter objectToString)
        {
            // Pull resolver id 
            var resolverId = objectToString.Resolver.Id;

            // Pull id of previous configuration
            var previousId = configuration.PreviousConfiguration.Id ?? throw GeneratorException.ConfigurationIdNotSet();

            // Pull now we can return the original configuration
            return _cachedSets[resolverId][previousId];
        }

        /// <summary>
        /// Gets the cache dictionary for a given object to string converter.
        /// If the dictionary doesn't exist, it will create one and update
        /// the cached sets dictionary.
        /// </summary>
        /// <param name="converter">The object to string converter.</param>
        /// <returns>The cache dictionary.</returns>
        private Dictionary<int, SortedSet<string>> GetDictionaryForConverter(IObjectToStringConverter converter)
        {
            // Pull resolver id
            var resolverId = converter.Resolver.Id;

            // If the cache doesn't contain this id
            if (!_cachedSets.ContainsKey(resolverId))
            {
                // Add new dictionary to the cache
                _cachedSets.Add(resolverId, new Dictionary<int, SortedSet<string>>());
            }

            // Return the cache dictionary
            return _cachedSets[resolverId];
        }

        #endregion
    }
}