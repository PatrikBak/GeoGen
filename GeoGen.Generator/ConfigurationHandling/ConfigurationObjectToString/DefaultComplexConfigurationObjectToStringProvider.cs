using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using GeoGen.Core.Configurations;
using GeoGen.Generator.ConfigurationHandling.ConfigurationObjectToString.ObjectIdResolving;
using GeoGen.Generator.Constructing.Arguments.ArgumentsToString;

namespace GeoGen.Generator.ConfigurationHandling.ConfigurationObjectToString
{
    /// <summary>
    /// An implementation of <see cref="ComplexConfigurationObjectToStringProviderBase"/> that
    /// uses <see cref="DefaultObjectIdResolver"/>. This class is meant to be used 
    /// in a class that cares care of creating unique <see cref="ConfigurationObject"/>s.
    /// These object don't have their id set at first, but we assume that all their underlying
    /// objects have and that they also have their string versions cached. This caching must be
    /// done manually using the method CacheObject, since the id is not known during the 
    /// to string conversion process.
    /// </summary>
    internal class DefaultComplexConfigurationObjectToStringProvider : ComplexConfigurationObjectToStringProviderBase
    {
        #region Constructor

        /// <summary>
        /// Constructs a default complex configuration object to string provider 
        /// with a given arguments to string provider and a given default
        /// configuration object id resolver.
        /// </summary>
        /// <param name="argumentsToStringProvider">The arguments to string provider.</param>
        /// <param name="resolver">The resolver.</param>
        public DefaultComplexConfigurationObjectToStringProvider(IArgumentsToStringProvider argumentsToStringProvider,
                                                                 DefaultObjectIdResolver resolver)
            : base(argumentsToStringProvider, resolver)
        {
        }

        #endregion

        #region Abstract methods from base implementation

        /// <summary>
        /// Resolve if a given object has it's to string value already cached.
        /// </summary>
        /// <param name="configurationObject">The configuration object.</param>
        /// <returns>The cached value, if exists, otherwise an empty string.</returns>
        protected override string ResolveCachedValue(ConfigurationObject configurationObject)
        {
            // If the objects doesn't have an idea, we can't have it cached
            if (!configurationObject.Id.HasValue)
            {
                return string.Empty;
            }

            try
            {
                // We assume that we must have cached all objects (if not, this will cause 
                // KeyNotFound exception, so we'll hit the catch block)
                return Cache[configurationObject.Id.Value];
            }
            catch (KeyNotFoundException)
            {
                throw new GeneratorException("The object with this id hasn't been cached.");
            }
        }

        /// <summary>
        /// Handles the resulting string value after constructing it, before returning it.
        /// </summary>
        /// <param name="configurationObject">The configuration object.</param>
        /// <param name="result">The object's string value.</param>
        protected override void HandleResult(ConfigurationObject configurationObject, string result)
        {
            // We can't do anyting useful here (we can't cache cause we don't have an id -
            // if we had we would have returned the cached version).
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Caches a string version associated with the object of a given id. We call this manually
        /// because the id is not supposed to be set when we first call the convert method.
        /// </summary>
        /// <param name="configurationObjectId">The configuration object id.</param>
        /// <param name="stringVersion">The string version.</param>
        public void CacheObject(int configurationObjectId, string stringVersion)
        {
            try
            {
                Cache.TryAdd(configurationObjectId, stringVersion);
            }
            catch (ArgumentException)
            {
                throw new GeneratorException("The object with this id has been already cached.");
            }
        }

        /// <summary>
        /// Clears the cache of the provider.
        /// </summary>
        public void ClearCache()
        {
            Cache.Clear();
        }

        #endregion
    }
}