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
        /// Gets or sets the initial configuration from which the generation process starts. The configuration
        /// shouldn't be identified, nor its internal objects should or constructions should.
        /// </summary>
        public Configuration InitialConfiguration { get; set; }

        /// <summary>
        /// Gets or sets the constructions that are supposed to be used to extend the configurations, including the initial one.
        /// The constructions must have distinct names and cannot be identified.
        /// </summary>
        public ICollection<Construction> Constructions { get; set; }

        /// <summary>
        /// Gets or sets the maximal number of iterations that are to be performed by the generator.
        /// </summary>
        public int MaximalNumberOfIterations { get; set; }

        /// <summary>
        /// Gets or sets the number of pictures to which a configuration is drawn and tested for theorems.
        /// </summary>
        public int NumberOfContainers { get; set; }

        /// <summary>
        /// Gets or sets the maximal number of attempts to resolve inconsistencies between containers by reconstructing all of them.
        /// </summary>
        public int MaximalAttemptsToReconstructAllContainers { get; set; }

        /// <summary>
        /// The maximal number of attempts to reconstruct a single objects container.
        /// </summary>
        public int MaximalAttemptsToReconstructOneContainer { get; set; }        
    }
}