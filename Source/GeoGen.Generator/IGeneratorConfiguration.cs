using GeoGen.Core;
using System.Collections.Generic;

namespace GeoGen.Generator
{
    /// <summary>
    /// Represents the configuration for <see cref="Generator"/>.
    /// </summary>
    public interface IGeneratorConfiguration
    {
        /// <summary>
        /// The initial configuration from which the generation process starts. The configuration
        /// shouldn't be identified, nor its internal objects should or constructions should.
        /// </summary>
        Configuration InitialConfiguration { get; }

        /// <summary>
        /// The constructions that are supposed to be used to extend the configurations, including the initial one.
        /// The constructions must have distinct names and cannot be identified.
        /// </summary>
        IReadOnlyList<Construction> Constructions { get; }

        /// <summary>
        /// The number of iterations that are to be performed by the generator.
        /// </summary>
        int NumberOfIterations { get; }
    }
}