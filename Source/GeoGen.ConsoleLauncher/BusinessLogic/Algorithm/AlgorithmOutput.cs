using GeoGen.Core;
using GeoGen.Generator;
using GeoGen.TheoremsAnalyzer;
using System.Collections.Generic;

namespace GeoGen.ConsoleLauncher
{
    /// <summary>
    /// Represents an output of the <see cref="IAlgorithm"/>.
    /// </summary>
    public class AlgorithmOutput
    {
        /// <summary>
        /// The output of the generator module.
        /// </summary>
        public GeneratorOutput GeneratorOutput { get; set; }

        /// <summary>
        /// The output of the theorems finder module.
        /// </summary>
        public List<Theorem> Theorems { get; set; }

        /// <summary>
        /// The dictionary mapping non-olympiad theorems to their feedback. The ones that are not presents are hopefully olympiad.
        /// </summary>
        public Dictionary<Theorem, TheoremFeedback> AnalyzerOutput { get; set; }
    }
}