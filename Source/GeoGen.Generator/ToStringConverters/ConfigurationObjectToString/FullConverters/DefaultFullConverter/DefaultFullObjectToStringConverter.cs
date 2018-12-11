using System.Collections.Generic;
using GeoGen.Core;

namespace GeoGen.Generator
{
    /// <summary>
    /// A default implementation of <see cref="IDefaultFullObjectToStringConverter"/>.
    /// This converter uses an <see cref="IDefaultObjectIdResolver"/>.
    /// </summary>
    internal class DefaultFullObjectToStringConverter : FullObjectToStringConverterBase, IDefaultFullObjectToStringConverter
    {
        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="provider">The converter of arguments to string.</param>
        /// <param name="resolver">The default (identical) id resolver.</param>
        public DefaultFullObjectToStringConverter(IArgumentsToStringProvider provider, IDefaultObjectIdResolver resolver)
                : base(provider, resolver)
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
            // If the objects doesn't have an id, we can't have it cached
            if (!configurationObject.HasId)
            {
                return string.Empty;
            }

            try
            {
                // We assume that we must have cached all objects (if not, this will cause 
                // KeyNotFound exception, so we'll hit the catch block)
                return Cache[configurationObject.Id];
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
            // We can't do anything useful here (we can't cache cause we don't have an id -
            // if we had we would have returned the cached version).
        }

        #endregion

        #region IDefaultFullObjectToStringConverter methods

        /// <summary>
        /// Caches a string version associated with the object of a given id. We call this manually
        /// because the id is not supposed to be set before the first call the convert method.
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