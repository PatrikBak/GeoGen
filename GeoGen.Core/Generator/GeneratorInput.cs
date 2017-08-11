using System.Collections.Generic;
using GeoGen.Core.Configurations;
using GeoGen.Core.Constructions;

namespace GeoGen.Core.Generator
{
    /// <summary>
    /// Represents an input for generator.
    /// </summary>
    public class GeneratorInput
    {
        #region Public proprties

        /// <summary>
        /// Gets or sets the initial configuration from which the generation proccess starts.
        /// </summary>
        public Configuration InitialConfiguration { get; set; }

        /// <summary>
        /// Gets or sets the constructions collection that are supposed to be used to extend the initial configuration.
        /// </summary>
        public ICollection<Construction> Constructions { get; set; }

        /// <summary>
        /// Gets or sets the maximal number of iterations that are to be performed by the generator.
        /// </summary>
        public int MaximalNumberOfIterations { get; set; }

        #endregion
    }
}