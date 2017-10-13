using System;
using GeoGen.Core.Configurations;

namespace GeoGen.Generator.ConstructingConfigurations.ObjectToString.ObjectIdResolving
{
    /// <summary>
    /// A default implementation of <see cref="IObjectIdResolver"/>
    /// that just simply returns the id of a given object (it must be set).
    /// </summary>
    internal sealed class DefaultObjectIdResolver : IObjectIdResolver
    {
        #region Public static fields

        /// <summary>
        /// The default id of the resolver. 
        /// </summary>
        public static readonly int DefaultId = 0;

        #endregion

        #region IObjectResolver properties

        /// <summary>
        /// Gets the id of the resolver.
        /// </summary>
        public int Id => DefaultId;

        #endregion

        #region IObjectIdResolver methods

        /// <summary>
        /// Resolves the id of a given configuration object.
        /// </summary>
        /// <param name="configurationObject">The configuration object.</param>
        /// <returns>The id.</returns>
        public int ResolveId(ConfigurationObject configurationObject)
        {
            if (configurationObject == null)
                throw new ArgumentNullException(nameof(configurationObject));

            return configurationObject.Id ?? throw new GeneratorException("Id must be set.");
        }

        #endregion
    }
}