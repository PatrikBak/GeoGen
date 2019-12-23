using GeoGen.Core;
using GeoGen.Generator;
using GeoGen.TheoremProver;
using GeoGen.TheoremRanker;
using GeoGen.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.Algorithm
{
    /// <summary>
    /// Represents an output of the <see cref="IAlgorithmFacade"/>.
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
        /// The rankings of unproven theorems. Each should be ranked.
        /// </summary>
        public IReadOnlyDictionary<Theorem, TheoremRanking> Rankings { get; }

        /// <summary>
        /// The simplifications of unproven theorems. Not each must be simplified.
        /// </summary>
        public IReadOnlyDictionary<Theorem, (Configuration newConfiguration, Theorem newTheorem)> SimplifiedTheorems { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="AlgorithmOutput"/> class.
        /// </summary>
        /// <param name="configuration">The generated configuration.</param>
        /// <param name="oldTheorems">The found theorems for the configurations that don't use last object of the configuration.</param>
        /// <param name="newTheorems">The found theorems for the configurations that use the last object of the configuration.</param>
        /// <param name="proverOutput">The output of the theorem prover.</param>
        /// <param name="rankings">The rankings of unproven theorems. Each should be ranked.</param>
        /// <param name="simplifiedTheorems">The simplifications of unproven theorems. Not each must be simplified.</param>
        public AlgorithmOutput(GeneratedConfiguration configuration,
                               TheoremMap oldTheorems,
                               TheoremMap newTheorems,
                               TheoremProverOutput proverOutput,
                               IReadOnlyDictionary<Theorem, TheoremRanking> rankings,
                               IReadOnlyDictionary<Theorem, (Configuration newConfiguration, Theorem newTheorem)> simplifiedTheorems)
        {
            Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            OldTheorems = oldTheorems ?? throw new ArgumentNullException(nameof(oldTheorems));
            NewTheorems = newTheorems ?? throw new ArgumentNullException(nameof(newTheorems));
            ProverOutput = proverOutput ?? throw new ArgumentNullException(nameof(proverOutput));
            Rankings = rankings ?? throw new ArgumentNullException(nameof(rankings));
            SimplifiedTheorems = simplifiedTheorems ?? throw new ArgumentNullException(nameof(simplifiedTheorems));
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Finds the most interesting theorems of this output. It is done in the following way:
        /// 
        /// 1. Look at each group of <see cref="TheoremProverOutput.UnprovenTheoremGroups"/>.
        /// 2. From each group select the theorem with the highest ranking.
        /// 3. If a theorem can be simplified, then return its simplified version.
        /// 4. These are the final theorems.
        /// 
        /// </summary>
        /// <returns>The interesting theorems of the output.</returns>
        public IEnumerable<TheoremWithRanking> FindInterestingTheorems()
        {
            // Take the groups
            return ProverOutput.UnprovenTheoremGroups
                // From each group take the theorem with the highest ranking
                .Select(group => group.MaxItem(theorem => Rankings[theorem].TotalRanking))
                // Now construct the final result according to the fact whether the theorem can be simplified
                .Select(theorem => SimplifiedTheorems.ContainsKey(theorem)
                    // If yes, take its simplified version in the simplified configuration (and sadly just the original ranking)
                    ? new TheoremWithRanking(SimplifiedTheorems[theorem].newTheorem, Rankings[theorem], SimplifiedTheorems[theorem].newConfiguration, isSimplified: true)
                    // If no, just take the theorem itself...
                    : new TheoremWithRanking(theorem, Rankings[theorem], Configuration, isSimplified: false));
        }

        #endregion
    }
}