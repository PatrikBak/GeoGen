using GeoGen.Core;
using GeoGen.Generator;
using GeoGen.TheoremProver;
using GeoGen.TheoremRanker;
using System;
using System.Collections.Generic;

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
        /// The found theorems for the configurations that don't use last object of the configuration.
        /// </summary>
        public TheoremMap OldTheorems { get; }

        /// <summary>
        /// The found theorems for the configurations that use the last object of the configuration.
        /// </summary>
        public TheoremMap NewTheorems { get; }

        /// <summary>
        /// The output of the theorem prover.
        /// </summary>
        public TheoremProverOutput ProverOutput { get; }

        /// <summary>
        /// The rankings of unproven theorems.
        /// </summary>
        public IReadOnlyDictionary<Theorem, TheoremRanking> Rankings { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="AlgorithmOutput"/> class.
        /// </summary>
        /// <param name="configuration">The generated configuration.</param>
        /// <param name="oldTheorems">The found theorems for the configurations that don't use last object of the configuration.</param>
        /// <param name="newTheorems">The found theorems for the configurations that use the last object of the configuration.</param>
        /// <param name="proverOutput">The output of the theorem prover.</param>
        /// <param name="rankings">The rankings of unproven theorems.</param>
        public AlgorithmOutput(GeneratedConfiguration configuration,
                               TheoremMap oldTheorems,
                               TheoremMap newTheorems,
                               TheoremProverOutput proverOutput,
                               IReadOnlyDictionary<Theorem, TheoremRanking> rankings)
        {
            Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            OldTheorems = oldTheorems ?? throw new ArgumentNullException(nameof(oldTheorems));
            NewTheorems = newTheorems ?? throw new ArgumentNullException(nameof(newTheorems));
            ProverOutput = proverOutput ?? throw new ArgumentNullException(nameof(proverOutput));
            Rankings = rankings ?? throw new ArgumentNullException(nameof(rankings));
        }

        #endregion
    }
}