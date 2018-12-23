using GeoGen.Core;
using System;
using System.Collections.Generic;

namespace GeoGen.Generator
{
    public class FullObjectToStringConverter : IFullObjectToStringConverter
    {
        #region Dependencies

        /// <summary>
        /// The converter of arguments to string.
        /// </summary>
        private readonly IArgumentsToStringProvider _argumentsToStringProvider;

        #endregion

        #region Private fields

        /// <summary>
        /// The cache dictionary mapping object's ids to their string versions. 
        /// </summary>
        private readonly Dictionary<ConstructedConfigurationObject, string> _cache = new Dictionary<ConstructedConfigurationObject, string>();

        /// <summary>
        /// The resolver of the ids of loose objects.
        /// </summary>
        private readonly ILooseObjectsIdResolver _resolver;

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="provider">The converter of arguments to string.</param>
        /// <param name="looseObjectsMap">The resolver of the ids of loose objects.</param>
        public FullObjectToStringConverter(IArgumentsToStringProvider provider, ILooseObjectsIdResolver resolver)
        {
            _argumentsToStringProvider = provider ?? throw new ArgumentNullException(nameof(provider));
            _resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));
        }

        #endregion

        #region IFullObjectToStringConverter methods

        /// <summary>
        /// Converts a given configuration object to string. 
        /// </summary>
        /// <param name="configurationObject">The configuration object.</param>
        /// <returns>The string representation of the object.</returns>
        public string ConvertToString(ConfigurationObject configurationObject)
        {
            // If we have a loose object, we will let the resolver handle the id and convert the id to string
            if (configurationObject is LooseConfigurationObject looseObject)
                return $"{_resolver.ResolveId(looseObject)}";

            // The object must be a constructed one
            var contructedObject = (ConstructedConfigurationObject) configurationObject;

            // Otherwise we have a constructed object. Let's first hit the cache
            if (_cache.ContainsKey(contructedObject))
                return _cache[contructedObject];

            // At this point the object is not cached. Let's construct the argument string.
            // This might cause recursive calls of this function (and in most cases it will)
            // because we're passing this converter as the object to string converter for the arguments
            var argumentsString = _argumentsToStringProvider.ConvertToString(contructedObject.PassedArguments, this);

            // Construct the beginning of the result
            var result = $"{contructedObject.Construction.Id}{argumentsString}";

            // If the object doesn't have a default index (which is 0), then we have to include it
            if (contructedObject.Index != 0)
                result += $"[{contructedObject.Index}]";

            // Cache it
            _cache.Add(contructedObject, result);

            // And finally return it
            return result;
        }

        #endregion
    }
}