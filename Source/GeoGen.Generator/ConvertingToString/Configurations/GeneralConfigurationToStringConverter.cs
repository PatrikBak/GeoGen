using GeoGen.Core;
using GeoGen.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.Generator
{
    /// <summary>
    /// The default implementation of <see cref="IGeneralConfigurationToStringConverter"/>. 
    /// From the outside the conversion appears to work like this: We convert all the 
    /// constructed objects using the passed <see cref="IToStringConverter{T}"/>, where 'T'
    /// is <see cref="ConfigurationObject"/>, then sort them lexicographically, and join them 
    /// using the '|' separator. However, the class uses a sophisticated caching method.
    /// <para>
    /// This class assumes that if a <see cref="GeneratedConfiguration"/> has a previous 
    /// configuration, then it had to be already converted, and is cached. The cached
    /// configuration strings are not stored directly. The class only stores particular
    /// objects of each configuration, in a <see cref="SortedSet{T}"/> of strings.
    /// The sorted sets remembers the lexicographical order of these strings. When 
    /// we need to convert a configuration that has a previous one, then we won't convert
    /// all the objects, and then sort them, but we will copy the sorted set representing
    /// the objects of the previous configuration, and insert the new object to it.
    /// This algorithm is practically faster for long-running generations than the naive one.
    /// </para>
    /// </summary>
    public class GeneralConfigurationToStringConverter : IGeneralConfigurationToStringConverter
    {
        #region Private constants

        /// <summary>
        /// The separator of individual converted objects in the final string.
        /// </summary>
        private const string ObjectsSeparator = "|";

        #endregion

        #region Private fields

        /// <summary>
        /// The cache that maps each to string converter to a cache for this converter, represented
        /// as a dictionary mapping configurations to sorted sets of their individual converted objects.
        /// For the motivation behind using sorted sets see the documentation of the class.
        /// </summary>
        private readonly Dictionary<IToStringConverter<ConfigurationObject>, Dictionary<Configuration, SortedSet<string>>> _cache = new Dictionary<IToStringConverter<ConfigurationObject>, Dictionary<Configuration, SortedSet<string>>>();

        #endregion

        #region IGeneralConfigurationToStringConverter implementation

        /// <summary>
        /// Converts a given configuration to a string, using a given configuration object to string converter.
        /// </summary>
        /// <param name="configuration">The configuration to be converted.</param>
        /// <param name="objectToString">The configuration object to string converter.</param>
        /// <returns>A string representation of the configuration.</returns>
        public string ConvertToString(GeneratedConfiguration configuration, IToStringConverter<ConfigurationObject> objectToString)
        {
            // Get the converted configurations cache (or add and return a new one) for the passed converter
            var configurationsCache = _cache.GetOrAdd(objectToString, () => new Dictionary<Configuration, SortedSet<string>>());

            // Declare the result
            SortedSet<string> result;

            // If there is no previous configuration, we can't use the cache
            if (configuration.PreviousConfiguration is null)
            {
                // We need to convert all the constructed objects manually and wrap them in a sorted set
                result = configuration.ConstructedObjects.Select(objectToString.ConvertToString).ToSortedSet();
            }
            // Otherwise there is a previous configuration. This class
            // assumes it has been converted, i.e. we can assume it is already cached
            else
            {
                // Get the set representing the previous configuration. This is O(n)
                result = new SortedSet<string>(configurationsCache[configuration.PreviousConfiguration]);

                // Convert the new object
                var newObjectsString = objectToString.ConvertToString(configuration.LastConstructedObject);

                // Add it to the set. Adding an object is O(log n) 
                result.Add(newObjectsString);
            }

            // Cache the result
            configurationsCache.Add(configuration, result);

            // Join the converted objects using the default separator to get the final string
            return result.ToJoinedString(ObjectsSeparator);
        }

        #endregion
    }
}