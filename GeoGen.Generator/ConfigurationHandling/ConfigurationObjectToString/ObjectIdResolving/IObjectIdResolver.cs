using GeoGen.Core.Configurations;

namespace GeoGen.Generator.ConfigurationHandling.ConfigurationObjectToString.ObjectIdResolving
{
    /// <summary>
    /// Represents a resolver of ids of configuration objects.
    /// </summary>
    internal interface IObjectIdResolver
    {
        /// <summary>
        /// Resolve the id of a given configuration object.
        /// </summary>
        /// <param name="configurationObject">The configuration object.</param>
        /// <returns>The id.</returns>
        int ResolveId(ConfigurationObject configurationObject);
    }
}