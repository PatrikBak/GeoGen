using System.Collections.Generic;
using GeoGen.Core.Configurations;
using GeoGen.Generator.ConstructingObjects;

namespace GeoGen.Generator.ConfigurationsHandling.ConfigurationsContainer
{
    /// <summary>
    /// Represents a container for all generated configurations. 
    /// </summary>
    internal interface IConfigurationContainer
    {
        /// <summary>
        /// Initializes the container with a given initial configuration.
        /// </summary>
        /// <param name="initialConfiguration">The initial configuration.</param>
        void Initialize(Configuration initialConfiguration);

        /// <summary>
        /// Processes a new layer of a constructor output.
        /// </summary>
        /// <param name="newLayerOutput">The new layer output.</param>
        void AddLayer(List<ConstructorOutput> newLayerOutput);

        /// <summary>
        /// Gets the current layer of unprocessed configurations
        /// </summary>
        List<ConfigurationWrapper> CurrentLayer { get; }
    }
}