using GeoGen.Core;
using GeoGen.TheoremRanker;
using System;
using System.Collections.Generic;

namespace GeoGen.ProblemAnalyzer
{
    /// <summary>
    /// Represents an output of <see cref="IGeneratedProblemAnalyzer"/> that includes just <see cref="ProvedTheorems"/> without their proofs.
    /// </summary>
    public class GeneratedProblemAnalyzerOutputWithoutProofs : GeneratedProblemAnalyzerOutputBase
    {
        #region Public properties

        /// <summary>
        /// The collection of proved theorems.
        /// </summary>
        public IReadOnlyCollection<Theorem> ProvedTheorems { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="GeneratedProblemAnalyzerOutputWithoutProofs"/> class.
        /// </summary>
        /// <param name="simplifiedTheorems">The results of theorem simplification.</param>
        /// <param name="theoremRankings">The rankings of interesting theorems.</param>
        /// <param name="interestingTheorems">The interesting theorems sorted ascending by their total ranking.</param>
        /// <param name="provedTheorems">The collection of proved theorems.</param>
        public GeneratedProblemAnalyzerOutputWithoutProofs(IReadOnlyDictionary<Theorem, (Theorem newTheorem, Configuration newConfiguration)> simplifiedTheorems,
                                                           IReadOnlyDictionary<Theorem, TheoremRanking> theoremRankings,
                                                           IReadOnlyList<Theorem> interestingTheorems,
                                                           IReadOnlyCollection<Theorem> provedTheorems)

            : base(simplifiedTheorems, theoremRankings, interestingTheorems)
        {
            ProvedTheorems = provedTheorems ?? throw new ArgumentNullException(nameof(provedTheorems));
        }

        #endregion
    }
}
