using System.Collections.Generic;
using GeoGen.Core.Configurations;

namespace GeoGen.Generator.Container
{
    /// <summary>
    /// Represents a container for all generated configurations. It's meant to be the processor
    /// of symetric configurations. It's supposed to be a part of a single <see cref="IGeneratorContext"/>.
    /// </summary>
    internal interface IConfigurationContainer
    {
        /// <summary>
        /// Gets the configurations currently present in the container.
        /// </summary>
        IEnumerable<Configuration> Configurations { get; }

        /// <summary>
        /// Adds a new layer of configurations, i.e. the configurations generated in one iteration.
        /// </summary>
        /// <param name="newLayerConfigurations">The enumerable of configurations.</param>
        void AddLayer(IEnumerable<Configuration> newLayerConfigurations);
    }
}