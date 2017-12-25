using System.Collections.Generic;
using GeoGen.Core.Configurations;

namespace GeoGen.Generator
{
    /// <summary>
    /// An implementation of <see cref="FullObjectToStringConverterBase"/> that
    /// uses <see cref="DefaultObjectIdResolver"/>. This sealed class is meant to be used 
    /// in a sealed class that cares care of creating unique <see cref="ConfigurationObject"/>s.
    /// These objects don't have their id set at first, but we assume that all their underlying
    /// objects have and that they also have their string versions cached. This caching must be
    /// done manually using the method CacheObject, since the id is not known during the 
    /// to string conversion process.
    /// </summary>
    internal sealed class DefaultFullObjectToStringConverter : FullObjectToStringConverterBase, IDefaultFullObjectToStringConverter
    {
        #region Constructor

        /// <summary>
        /// Constructs a new default full object to string provider with a given
        /// arguments list to string provider and a given default object id resolver.
        /// </summary>
        /// <param name="converter>The arguments list to string provider.</param>
        /// <param name="resolver">The configuration object id resolver.</param>
        public DefaultFullObjectToStringConverter(IArgumentsListToStringConverter converter, IDefaultObjectIdResolver resolver)
            : base(converter, resolver)
        {
        }

        #endregion

        #region Abstract methods from base implementation

        /// <summary>
        /// Resolves if a given object has it's string value already cached.
        /// </summary>
        /// <param name="configurationObject">The configuration object.</param>
        /// <returns>The cached value, if exists, otherwise an empty string.</returns>
        protected override string ResolveCachedValue(ConfigurationObject configurationObject)
        {
            //// If the objects doesn't have an id, we can't have it cached
            //if (!configurationObject.Id.HasValue)
            //{
            //    return string.Empty;
            //}

            //try
            //{
            //    // We assume that we must have cached all objects (if not, this will cause 
            //    // KeyNotFound exception, so we'll hit the catch block)
            //    return Cache[configurationObject.Id.Value];
            //}
            //catch (KeyNotFoundException)
            //{
            //    throw new GeneratorException("The object with this id hasn't been cached.");
            //}

            var id = configurationObject.Id;

            if (id == null)
                return string.Empty;

            var idValue = id.Value;

            if (Cache.ContainsKey(idValue))
                return Cache[idValue];

            return string.Empty;

        }

        /// <summary>
        /// Handles the resulting string value after constructing it, before returning it.
        /// </summary>
        /// <param name="configurationObject">The configuration object.</param>
        /// <param name="result">The object's string value.</param>
        protected override void HandleResult(ConfigurationObject configurationObject, string result)
        {
            // We can't do anything useful here (we can't cache cause we don't have an id -
            // if we had we would have returned the cached version).
        }

        #endregion

        #region IDefaultFullObjectToStringProvider methods

        /// <summary>
        /// Caches a string version associated with the object of a given id. We call this manually
        /// because the id is not supposed to be set when we first call the convert method.
        /// </summary>
        /// <param name="configurationObjectId">The configuration object id.</param>
        /// <param name="stringVersion">The string version.</param>
        public void CacheObject(int configurationObjectId, string stringVersion)
        {
            if (Cache.ContainsKey(configurationObjectId))
                throw new GeneratorException("The object with this id has been already cached.");

            Cache.Add(configurationObjectId, stringVersion);
        }

        #endregion
    }
}