using System.Collections.Generic;
using GeoGen.Core.Configurations;
using GeoGen.Generator.ConstructingObjects;

namespace GeoGen.Generator.ConstructingConfigurations
{
    /// <summary>
    /// Represents a container for all generated configurations. The configurations
    /// can be submitted to the container from the <see cref="ConstructorOutput"/>
    /// and it is supposed to take care of processing the output and updating the
    /// <see cref="CurrentLayer"/> property.
    /// </summary>
    internal interface IConfigurationsContainer
    {
        /// <summary>
        /// Gets the current layer of unprocessed configurations
        /// </summary>
        List<ConfigurationWrapper> CurrentLayer { get; }

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
    }
}