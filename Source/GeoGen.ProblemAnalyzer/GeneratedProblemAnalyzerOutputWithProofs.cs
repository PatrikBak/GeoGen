using GeoGen.Core;
using GeoGen.TheoremProver;
using GeoGen.TheoremRanker;
using System;
using System.Collections.Generic;

namespace GeoGen.ProblemAnalyzer
{
    /// <summary>
    /// Represents an output of <see cref="IGeneratedProblemAnalyzer"/> that includes <see cref="TheoremProofs"/>.
    /// </summary>
    public class GeneratedProblemAnalyzerOutputWithProofs : GeneratedProblemAnalyzerOutputBase
    {
        #region Public properties

        /// <summary>
        /// The dictionary mapping proved theorems to their proofs.
        /// </summary>
        public IReadOnlyDictionary<Theorem, TheoremProof> TheoremProofs { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="GeneratedProblemAnalyzerOutputWithProofs"/> class.
        /// </summary>
        /// <param name="simplifiedTheorems">The results of theorem simplification.</param>
        /// <param name="theoremRankings">The rankings of interesting theorems.</param>
        /// <param name="interestingTheorems">The interesting theorems sorted ascending by their total ranking.</param>
        /// <param name="theoremProofs">The dictionary mapping proved theorems to their proofs.</param>
        public GeneratedProblemAnalyzerOutputWithProofs(IReadOnlyDictionary<Theorem, (Theorem newTheorem, Configuration newConfiguration)> simplifiedTheorems,
                                                        IReadOnlyDictionary<Theorem, TheoremRanking> theoremRankings,
                                                        IReadOnlyList<Theorem> interestingTheorems,
                                                        IReadOnlyDictionary<Theorem, TheoremProof> theoremProofs)

            : base(simplifiedTheorems, theoremRankings, interestingTheorems)
        {
            TheoremProofs = theoremProofs ?? throw new ArgumentNullException(nameof(theoremProofs));
        }

        #endregion
    }
}
