using GeoGen.Core;
using GeoGen.Generator;
using System.Collections.Generic;

namespace GeoGen.ConsoleLauncher
{
    /// <summary>
    /// Represents an input for the <see cref="IAlgorithm"/>.
    /// </summary>
    public class AlgorithmInput
    {
        /// <summary>
        /// The input for the generator module.
        /// </summary>
        public GeneratorInput GeneratorInput { get; set; }

        /// <summary>
        /// The list of easy theorems whose consequences should be mark as not interesting.
        /// </summary>
        public List<Theorem> TemplateTheorems { get; set; }
    }
}
