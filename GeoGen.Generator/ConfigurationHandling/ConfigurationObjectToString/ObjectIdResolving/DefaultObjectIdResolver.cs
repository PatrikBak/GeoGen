using System;
using GeoGen.Core.Configurations;

namespace GeoGen.Generator.ConfigurationHandling.ConfigurationObjectToString.ObjectIdResolving
{
    /// <summary>
    /// A default implementation of <see cref="IObjectIdResolver"/>
    /// that just simply returns the id of a given object (it must be set).
    /// </summary>
    internal class DefaultObjectIdResolver : IObjectIdResolver
    {
        #region IObjectIdResolver implementation

        /// <summary>
        /// Resolve the id of a given configuration object.
        /// </summary>
        /// <param name="configurationObject">The configuration object.</param>
        /// <returns>The id.</returns>
        public int ResolveId(ConfigurationObject configurationObject)
        {
            if (configurationObject == null)
                throw new ArgumentNullException(nameof(configurationObject));

            return configurationObject.Id ?? throw new GeneratorException("Configuration object without id");
        }

        #endregion
    }
}