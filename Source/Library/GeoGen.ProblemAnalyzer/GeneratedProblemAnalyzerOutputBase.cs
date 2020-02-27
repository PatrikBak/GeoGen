using GeoGen.Core;
using GeoGen.TheoremRanker;
using System;
using System.Collections.Generic;

namespace GeoGen.ProblemAnalyzer
{
    /// <summary>
    /// The base class for the possible types of output of a <see cref="IGeneratedProblemAnalyzer"/>.
    /// </summary>
    public abstract class GeneratedProblemAnalyzerOutputBase
    {
        #region Public properties

        /// <summary>
        /// The results of theorem simplification.
        /// </summary>
        public IReadOnlyDictionary<Theorem, (Theorem newTheorem, Configuration newConfiguration)> SimplifiedTheorems { get; }

        /// <summary>
        /// The rankings of interesting theorems.
        /// </summary>
        public IReadOnlyDictionary<Theorem, TheoremRanking> TheoremRankings { get; }

        /// <summary>
        /// The interesting theorems sorted ascending by their total ranking. 
        /// </summary>
        public IReadOnlyList<Theorem> InterestingTheorems { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="GeneratedProblemAnalyzerOutputBase"/> class.
        /// </summary>
        /// <param name="simplifiedTheorems">The results of theorem simplification.</param>
        /// <param name="theoremRankings">The rankings of interesting theorems.</param>
        /// <param name="interestingTheorems">The interesting theorems sorted ascending by their total ranking. </param>
        protected GeneratedProblemAnalyzerOutputBase(IReadOnlyDictionary<Theorem, (Theorem newTheorem, Configuration newConfiguration)> simplifiedTheorems,
                                                     IReadOnlyDictionary<Theorem, TheoremRanking> theoremRankings,
                                                     IReadOnlyList<Theorem> interestingTheorems)
        {
            SimplifiedTheorems = simplifiedTheorems ?? throw new ArgumentNullException(nameof(simplifiedTheorems));
            TheoremRankings = theoremRankings ?? throw new ArgumentNullException(nameof(theoremRankings));
            InterestingTheorems = interestingTheorems ?? throw new ArgumentNullException(nameof(interestingTheorems));
        }

        #endregion
    }
}
