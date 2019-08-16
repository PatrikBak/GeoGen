using GeoGen.Core;
using GeoGen.Utilities;
using System;
using System.Collections.Generic;

namespace GeoGen.Generator
{
    /// <summary>
    /// The default implementation of <see cref="IFullObjectToStringConverter"/>. This converter converts
    /// loose objects to an integer id, or of integer associated with the mapped version of it,
    /// according to the passed <see cref="LooseObjectRemapping"/> (for the motivation see the documentation 
    /// of <see cref="FullConfigurationToStringConverter"/>). The arguments of constructed objects are converted 
    /// using <see cref="IGeneralArgumentsToStringConverter"/>, which gets passed the object to string converter
    /// corresponding to the one that is currently converting the object. This causes gradual expansion (recursion) 
    /// which in the end returns a string that uses only integers representing the loose objects.
    /// <para>
    /// For example, assume we're using <see cref="GeneralArgumentsToStringConverter"/> and an empty loose objects
    /// mapping. Assume we have two loose objects that have associated integers 1 and 2, and 3 is the midpoint 
    /// between 1 and 2, and 4 is the midpoint of 1 and 3. Furthermore, assume the midpoint construction will be 0.
    /// Then the particular objects are converted like this: 1 to "1", 2 to "2", 3 to "0({1;2})", 4 to "0({0({1;2});1})". 
    /// If we used a mapping that maps 1 to 2 and 2 to 1, then we'd get 1 to "2", 2 to "1", 3 to "0({1;2})", 4 to "0({0({1;2}),2})".
    /// </para>
    /// </summary>
    public class FullObjectToStringConverter : IFullObjectToStringConverter
    {
        #region Dependencies

        /// <summary>
        /// The general converter of arguments to a string.
        /// </summary>
        private readonly IGeneralArgumentsToStringConverter _argumentsToString;

        #endregion

        #region Private fields

        /// <summary>
        /// The cache dictionary that maps each loose objects remapping to the actual cache 
        /// represented as a dictionary mapping constructed objects to their string representations.
        /// </summary>
        private readonly Dictionary<LooseObjectsRemapping, Dictionary<ConstructedConfigurationObject, string>> _stringsCache = new Dictionary<LooseObjectsRemapping, Dictionary<ConstructedConfigurationObject, string>>();

        /// <summary>
        /// The cache dictionary mapping each loose object remapping to an object to string converted that
        /// is supposed to call ConvertToString function with the given remapping. 
        /// </summary>
        private readonly Dictionary<LooseObjectsRemapping, IToStringConverter<ConfigurationObject>> _convertersCache = new Dictionary<LooseObjectsRemapping, IToStringConverter<ConfigurationObject>>();

        /// <summary>
        /// The cache dictionary that maps names of constructions to integer ids that are used in the conversion
        /// of objects to get shorter strings.
        /// </summary>
        private readonly Dictionary<string, int> _constructionIds = new Dictionary<string, int>();

        /// <summary>
        /// The cache dictionary that maps loose objects to their ids.
        /// </summary>
        private readonly Dictionary<LooseConfigurationObject, int> _looseObjectsIds = new Dictionary<LooseConfigurationObject, int>();

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="FullObjectToStringConverter"/> class.
        /// </summary>
        /// <param name="argumentsToString">The general converter of arguments to a string.</param>
        public FullObjectToStringConverter(IGeneralArgumentsToStringConverter argumentsToString)
        {
            _argumentsToString = argumentsToString ?? throw new ArgumentNullException(nameof(argumentsToString));
        }

        #endregion

        #region IFullObjectToStringConverter implementation

        /// <summary>
        /// Converts a given configuration object to a string using a given remapping of loose objects during the conversion.
        /// </summary>
        /// <param name="configurationObject">The object to be converted.</param>
        /// <param name="remapping">The remapping of loose objects to be used during the conversion.</param>
        /// <returns>A string representation of the object.</returns>
        public string ConvertToString(ConfigurationObject configurationObject, LooseObjectsRemapping remapping)
        {
            // Switch based on the objects type
            switch (configurationObject)
            {
                // If we have a loose object...
                case LooseConfigurationObject looseObject:

                    // We let the remapping map the current object
                    var resolvedObject = remapping.Map(looseObject);

                    // Makes sure the object has an id in the dictionary and convert it to a string 
                    return _looseObjectsIds.GetOrAdd(resolvedObject, () => _looseObjectsIds.Count).ToString();

                // If we have a constructed object...
                case ConstructedConfigurationObject constructedObject:

                    // First get the cache dictionary corresponding to the current remapping, or add a new one and return it
                    var cache = _stringsCache.GetOrAdd(remapping, () => new Dictionary<ConstructedConfigurationObject, string>());

                    // Let's first try to hit the cache
                    if (cache.ContainsKey(constructedObject))
                        return cache[constructedObject];

                    // At this point we know the object is not cached. 
                    // We need to get the object converter corresponding to the
                    // current remapping so we can pass it to the arguments to
                    // string converter. This converter will do nothing but 
                    // calling this method with our current remapping. 
                    var converter = _convertersCache.GetOrAdd(remapping, () => new FuncToStringConverter<ConfigurationObject>(obj => ConvertToString(obj, remapping)));

                    // Get the construction id, or add it if it doesn't exist
                    var constructionId = _constructionIds.GetOrAdd(constructedObject.Construction.Name, () => _constructionIds.Count);

                    // Now we construct the arguments string with the found converter
                    var argumentsString = _argumentsToString.ConvertToString(constructedObject.PassedArguments, converter);

                    // Construct the result
                    var result = $"{constructionId}{argumentsString}";

                    // Cache the result
                    cache.Add(constructedObject, result);

                    // And finally return it
                    return result;

                // Default case
                default:
                    throw new GeneratorException($"Unhandled type of configuration object: {configurationObject.GetType()}");
            }
        }

        #endregion
    }
}