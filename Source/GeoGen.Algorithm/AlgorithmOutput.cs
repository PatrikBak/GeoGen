using GeoGen.Core;
using GeoGen.Generator;
using GeoGen.TheoremsAnalyzer;
using System.Collections.Generic;

namespace GeoGen.Algorithm
{
    /// <summary>
    /// Represents an output of the <see cref="IAlgorithm"/>.
    /// </summary>
    public class AlgorithmOutput
    {
        /// <summary>
        /// The generated configuration.
        /// </summary>
        public GeneratedConfiguration Configuration { get; set; }

        /// <summary>
        /// The found theorems for the configurations.
        /// </summary>
        public IReadOnlyList<Theorem> Theorems { get; set; }

        /// <summary>
        /// The dictionary mapping non-olympiad theorems to their feedback. The ones that are not presents are hopefully olympiad.
        /// </summary>
        public Dictionary<Theorem, TheoremFeedback> AnalyzerOutput { get; set; }
    }
}