using GeoGen.Core;
using System.Collections.Generic;

namespace GeoGen.Generator
{
    /// <summary>
    /// Represents an input for the <see cref="Generator"/>.
    /// </summary>
    public class GeneratorInput
    {
        /// <summary>
        /// The initial configuration from which the generation process starts. The configuration
        /// shouldn't be identified, nor its internal objects should or constructions should.
        /// </summary>
        public Configuration InitialConfiguration { get; set; }

        /// <summary>
        /// The constructions that are supposed to be used to extend the configurations, including the initial one.
        /// The constructions must have distinct names and cannot be identified.
        /// </summary>
        public IReadOnlyList<Construction> Constructions { get; set; }

        /// <summary>
        /// The number of iterations that are to be performed by the generator.
        /// </summary>
        public int NumberOfIterations { get; set; }
    }
}