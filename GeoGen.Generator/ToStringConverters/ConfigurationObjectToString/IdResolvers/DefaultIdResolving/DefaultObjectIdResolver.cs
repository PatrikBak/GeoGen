using GeoGen.Core;

namespace GeoGen.Generator
{
    /// <summary>
    /// A default implementation of <see cref="IObjectIdResolver"/>.
    /// </summary>
    internal class DefaultObjectIdResolver : IDefaultObjectIdResolver
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
            return configurationObject.Id ?? throw new GeneratorException("Configurations id must be set.");
        }

        #endregion
    }
}