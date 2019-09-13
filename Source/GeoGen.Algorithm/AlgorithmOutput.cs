using GeoGen.Core;
using GeoGen.Generator;
using GeoGen.TheoremProver;

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
        public TheoremsMap Theorems { get; set; }

        /// <summary>
        /// The output of the theorem prover.
        /// </summary>
        public TheoremProverOutput ProverOutput { get; set; }
    }
}