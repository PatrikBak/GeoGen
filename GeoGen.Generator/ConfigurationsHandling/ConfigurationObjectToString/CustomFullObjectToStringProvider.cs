using System;
using System.Collections.Generic;
using System.Diagnostics;
using GeoGen.Core.Configurations;
using GeoGen.Generator.ConfigurationsHandling.ConfigurationObjectToString.ConfigurationObjectIdResolving;
using GeoGen.Generator.ConstructingObjects.Arguments.ArgumentsToString;

namespace GeoGen.Generator.ConfigurationsHandling.ConfigurationObjectToString
{
    /// <summary>
    /// An implementation of <see cref="FullObjectToStringProviderBase"/> that
    /// uses custom <see cref="IObjectIdResolver"/>  This class is meant to be 
    /// used during the symetric configurations detection, together with 
    /// <see cref="DictionaryObjectIdResolver"/>. It expects 
    /// that all objects have their ids already set. It automatically caches the evaluated 
    /// results (unlike <see cref="DefaultFullObjectToStringProvider"/>), 
    /// since it already has the ids available.
    /// </summary>
    internal class CustomFullObjectToStringProvider : FullObjectToStringProviderBase
    {
        #region Constructor

        /// <summary>
        /// Constructs a new custom full object to string provider with a given
        /// custom argument to string provider factory, a given arguments 
        /// list to string provider and a given object id resolver.
        /// </summary>
        /// <param name="factory">The custom argument to string provider factory.</param>
        /// <param name="provider">The arguments list to string provider.</param>
        /// <param name="resolver">The configuration object id resolver.</param>
        public CustomFullObjectToStringProvider
        (
            IArgumentsListToStringProvider provider,
            IObjectIdResolver resolver
        )
            : base(provider, resolver)
        {
        }

        #endregion

        #region Abstract methods from base implementation

        /// <summary>
        /// Resolve sif a given object has it's string value already cached.
        /// </summary>
        /// <param name="configurationObject">The configuration object.</param>
        /// <returns>The cached value, if exists, otherwise an empty string.</returns>
        protected override string ResolveCachedValue(ConfigurationObject configurationObject)
        {
            // We must have an id
            var id = configurationObject.Id ?? throw new GeneratorException("Id must be set");

            // Then we might or might have cached this object.
            return Cache.ContainsKey(id) ? Cache[id] : string.Empty;
        }

        /// <summary>
        /// Handles the resulting string value after constructing it, before returning it.
        /// </summary>
        /// <param name="configurationObject">The configuration object.</param>
        /// <param name="result">The object's string value.</param>
        protected override void HandleResult(ConfigurationObject configurationObject, string result)
        {
            // We must have an id
            var id = configurationObject.Id ?? throw new GeneratorException("Id must be set");

            // Then we can cach the object
            Cache.TryAdd(id, result);
        }

        #endregion
    }
}