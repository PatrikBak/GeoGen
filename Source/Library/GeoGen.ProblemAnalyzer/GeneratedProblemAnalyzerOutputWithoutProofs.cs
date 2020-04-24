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
        /// <param name="simplifiedTheorems"><inheritdoc cref="GeneratedProblemAnalyzerOutputBase.SimplifiedTheorems" path="/summary"/></param>
        /// <param name="interestingTheorems"><inheritdoc cref="GeneratedProblemAnalyzerOutputBase.InterestingTheorems" path="/summary"/></param>
        /// <param name="notInterestringAsymmetricTheorems"><inheritdoc cref="GeneratedProblemAnalyzerOutputBase.NotInterestringAsymmetricTheorems" path="/summary"/></param>
        /// <param name="provedTheorems"><inheritdoc cref="ProvedTheorems" path="/summary"/></param>
        public GeneratedProblemAnalyzerOutputWithoutProofs(IReadOnlyDictionary<Theorem, (Theorem newTheorem, Configuration newConfiguration)> simplifiedTheorems,
                                                           IReadOnlyList<RankedTheorem> interestingTheorems,
                                                           IReadOnlyCollection<Theorem> notInterestringAsymmetricTheorems,
                                                           IReadOnlyCollection<Theorem> provedTheorems)

            : base(simplifiedTheorems, interestingTheorems, notInterestringAsymmetricTheorems)
        {
            ProvedTheorems = provedTheorems ?? throw new ArgumentNullException(nameof(provedTheorems));
        }

        #endregion
    }
}
