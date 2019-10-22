using GeoGen.Core;
using GeoGen.Generator;
using GeoGen.TheoremProver;
using System;

namespace GeoGen.Algorithm
{
    /// <summary>
    /// Represents an output of the <see cref="IAlgorithm"/>.
    /// </summary>
    public class AlgorithmOutput
    {
        #region Public properties

        /// <summary>
        /// The generated configuration.
        /// </summary>
        public GeneratedConfiguration Configuration { get; }

        /// <summary>
        /// The found theorems for the configurations.
        /// </summary>
        public TheoremMap Theorems { get; }

        /// <summary>
        /// The output of the theorem prover.
        /// </summary>
        public TheoremProverOutput ProverOutput { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="AlgorithmOutput"/> class.
        /// </summary>
        /// <param name="configuration">The generated configuration.</param>
        /// <param name="theorems">The found theorems for the configurations.</param>
        /// <param name="proverOutput">The output of the theorem prover.</param>
        public AlgorithmOutput(GeneratedConfiguration configuration, TheoremMap theorems, TheoremProverOutput proverOutput)
        {
            Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            Theorems = theorems ?? throw new ArgumentNullException(nameof(theorems));
            ProverOutput = proverOutput ?? throw new ArgumentNullException(nameof(proverOutput));
        }

        #endregion
    }
}