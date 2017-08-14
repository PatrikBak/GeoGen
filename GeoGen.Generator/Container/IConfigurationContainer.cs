using System.Collections.Generic;
using GeoGen.Core.Configurations;
using GeoGen.Generator.Constructor;
using GeoGen.Generator.Wrappers;

namespace GeoGen.Generator.Container
{
    /// <summary>
    /// Represents a container for all generated configurations. It's meant to be the processor
    /// of symetric configurations. It's supposed to be a part of a single <see cref="IGeneratorContext"/>.
    /// It implements the <see cref="IEnumerable{T}"/> interface whose generic type is <see cref="ConfigurationWrapper"/>.
    /// </summary>
    internal interface IConfigurationContainer : IEnumerable<ConfigurationWrapper>
    {
        void Initialize(Configuration initialConfiguration);

        /// <summary>
        /// Adds a new layer of configurations, i.e. the configurations generated in one iteration.
        /// </summary>
        /// <param name="newLayerConfigurations">The enumerable of configurations.</param>
        void AddLayer(List<ConstructorOutput> newLayerConfigurations);
    }
}