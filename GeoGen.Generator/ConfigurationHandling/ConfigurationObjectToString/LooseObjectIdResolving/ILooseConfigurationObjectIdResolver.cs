using GeoGen.Core.Configurations;

namespace GeoGen.Generator.ConfigurationHandling.ConfigurationObjectToString.LooseObjectIdResolving
{
    /// <summary>
    /// Represents a resolver of ids of loose configuration objects.
    /// </summary>
    internal interface ILooseConfigurationObjectIdResolver
    {
        /// <summary>
        /// Resolve the id of a given loose configuration object.
        /// </summary>
        /// <param name="looseConfigurationObject">The loose configuration object.</param>
        /// <returns>The id.</returns>
        int ResolveId(LooseConfigurationObject looseConfigurationObject);
    }
}