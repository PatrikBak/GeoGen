using GeoGen.Core.Configurations;

namespace GeoGen.Generator.ConfigurationsHandling.ConfigurationObjectToString.ConfigurationObjectIdResolving
{
    /// <summary>
    /// Represents a resolver of ids of configuration objects.
    /// </summary>
    internal interface IObjectIdResolver
    {
        /// <summary>
        /// Gets the id of the resolver.
        /// </summary>
        int Id { get; }

        /// <summary>
        /// Resolve the id of a given configuration object.
        /// </summary>
        /// <param name="configurationObject">The configuration object.</param>
        /// <returns>The id.</returns>
        int ResolveId(ConfigurationObject configurationObject);
    }
}