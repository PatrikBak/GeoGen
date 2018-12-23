using GeoGen.Core;
using System.Collections.Generic;

namespace GeoGen.Generator
{
    /// <summary>
    /// The default implementation of <see cref="IConfigurationToStringProvider"/>. 
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
    public class ConfigurationToStringProvider : IConfigurationToStringProvider
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
        private readonly Dictionary<IToStringConverter<ConfigurationObject>, Dictionary<int, SortedSet<string>>> _cachedSets = new Dictionary<IToStringConverter<ConfigurationObject>, Dictionary<int, SortedSet<string>>>();

        #endregion

        #region IConfigurationToStringProvider implementation

        /// <summary>
        /// Converts a given configuration to string, using a given 
        /// configuration object to string converter. 
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="objectToString">The configuration object to string converter.</param>
        /// <returns>The string representation of the configuration.</returns>
        public string ConvertToString(GeneratedConfiguration configuration, IToStringConverter<ConfigurationObject> objectToString)
        {
            // First ensure the converter is cached
            if (!_cachedSets.ContainsKey(objectToString))
                _cachedSets.Add(objectToString, new Dictionary<int, SortedSet<string>>());

            // Now take the dictionary (mapping configuration ids to the sorted object strings)
            var configurationsCache = _cachedSets[objectToString];

            // Pull previous configuration
            var previousConfiguration = configuration.PreviousConfiguration;

            // Find out if it's the initial configuration.
            var isInitial = previousConfiguration is null;

            // If yes, we need to convert it object-by-object
            if (isInitial)
            {
                // Initialize the new result
                var initialResult = new SortedSet<string>();

                // Add all constructed objects to the set
                foreach (var newObject in configuration.ConstructedObjects)
                {
                    // Convert object to string
                    var stringValue = objectToString.ConvertToString(newObject);

                    // Add it to the set
                    initialResult.Add(stringValue);
                }

                // Cache the result
                configurationsCache.Add(configuration.Id, initialResult);

                // And terminate
                return string.Join(ObjectsSeparator, initialResult);
            }

            // Find the set of strings corresponding to the original configuration (with regards to current converter)
            var originalConfigurationSet = configurationsCache[previousConfiguration.Id];

            // Copy the set. This should be O(n) - which is the reason for caching
            var result = new SortedSet<string>(originalConfigurationSet);

            // Add the new objects to the new set
            foreach (var newObject in configuration.LastAddedObjects)
            {
                // Convert object to string
                var stringValue = objectToString.ConvertToString(newObject);

                // Add it to the set. This is log(n) operation and preserves the lexicographical order
                result.Add(stringValue);
            }

            // Cache the result
            configurationsCache.Add(configuration.Id, result);

            // Construct joined result. The objects will be sorted, because the sorted set 
            // keep its elements in the sorted order. That way we get the representation
            // independent on the order of constructed objects.
            return string.Join(ObjectsSeparator, result);
        }

        #endregion
    }
}