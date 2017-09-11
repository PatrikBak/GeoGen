using System;
using GeoGen.Core.Configurations;

namespace GeoGen.Generator.ConfigurationHandling.ConfigurationObjectToString.LooseObjectIdResolving
{
    /// <summary>
    /// A default implementation of <see cref="ILooseConfigurationObjectIdResolver"/>
    /// that just simply returns the id of a given object (it must be set).
    /// </summary>
    internal class DefaultLooseConfigurationObjectIdResolver : ILooseConfigurationObjectIdResolver
    {
        #region Instance

        /// <summary>
        /// The single instance of a class.
        /// </summary>
        public static readonly DefaultLooseConfigurationObjectIdResolver Instance = new DefaultLooseConfigurationObjectIdResolver();

        #endregion

        #region ILooseConfigurationObjectIdResolver implementation

        /// <summary>
        /// Resolve the id of a given loose configuration object.
        /// </summary>
        /// <param name="looseConfigurationObject">The loose configuration object.</param>
        /// <returns>The id.</returns>
        public int ResolveId(LooseConfigurationObject looseConfigurationObject)
        {
            if (looseConfigurationObject == null)
                throw new ArgumentNullException(nameof(looseConfigurationObject));

            return looseConfigurationObject.Id ?? throw new GeneratorException("Loose configuration object without id");
        }

        #endregion
    }
}