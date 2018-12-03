using GeoGen.Core;

namespace GeoGen.Generator
{
    /// <summary>
    /// Represents a resolver of the id of a configuration object. Except for an
    /// obvious implementation <see cref="IDefaultObjectIdResolver"/>, in some cases
    /// it's useful to have a <see cref="IDictionaryObjectIdResolver"/>. For the usage
    /// of them, <see cref="IMinimalFormResolver"/>.
    /// </summary>
    internal interface IObjectIdResolver
    {
        /// <summary>
        /// Gets the id of the resolver.
        /// </summary>
        int Id { get; }

        /// <summary>
        /// Resolves the id of a given configuration object.
        /// </summary>
        /// <param name="configurationObject">The configuration object.</param>
        /// <returns>The id.</returns>
        int ResolveId(ConfigurationObject configurationObject);
    }
}