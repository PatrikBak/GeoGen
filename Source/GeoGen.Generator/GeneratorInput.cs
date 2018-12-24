using GeoGen.Core;
using System.Collections.Generic;

namespace GeoGen.Generator
{
    /// <summary>
    /// Represents an input for a generator.
    /// </summary>
    public class GeneratorInput
    {
        /// <summary>
        /// Gets or sets the initial configuration from which the generation process starts.
        /// </summary>
        public Configuration InitialConfiguration { get; set; }

        /// <summary>
        /// Gets or sets the constructions collection that are supposed to be used to extend the initial configuration
        /// and further configurations as well.
        /// </summary>
        public ICollection<Construction> Constructions { get; set; }

        /// <summary>
        /// Gets or sets the maximal number of iterations that are to be performed by the generator.
        /// </summary>
        public int MaximalNumberOfIterations { get; set; }
    }
}