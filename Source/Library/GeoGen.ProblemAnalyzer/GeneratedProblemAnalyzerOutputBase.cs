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
        /// The interesting theorems sorted ascending by their total ranking. 
        /// </summary>
        public IReadOnlyList<RankedTheorem> InterestingTheorems { get; }

        /// <summary>
        /// The theorems that are not among <see cref="InterestingTheorems"/> because they have been ruled out
        /// because they are asymmetric. This list will be empty if an <see cref="IGeneratedProblemAnalyzer"/> is
        /// told not to exclude asymmetric problems.
        /// </summary>
        public IReadOnlyCollection<Theorem> NotInterestringAsymmetricTheorems { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="GeneratedProblemAnalyzerOutputBase"/> class.
        /// </summary>
        /// <param name="simplifiedTheorems"><inheritdoc cref="SimplifiedTheorems" path="/summary"/></param>
        /// <param name="interestingTheorems"><inheritdoc cref="InterestingTheorems" path="/summary"/></param>
        /// <param name="notInterestringAsymmetricTheorems"><inheritdoc cref="NotInterestringAsymmetricTheorems" path="/summary"/></param>
        protected GeneratedProblemAnalyzerOutputBase(IReadOnlyDictionary<Theorem, (Theorem newTheorem, Configuration newConfiguration)> simplifiedTheorems,
                                                     IReadOnlyList<RankedTheorem> interestingTheorems,
                                                     IReadOnlyCollection<Theorem> notInterestringAsymmetricTheorems)
        {
            SimplifiedTheorems = simplifiedTheorems ?? throw new ArgumentNullException(nameof(simplifiedTheorems));
            InterestingTheorems = interestingTheorems ?? throw new ArgumentNullException(nameof(interestingTheorems));
            NotInterestringAsymmetricTheorems = notInterestringAsymmetricTheorems ?? throw new ArgumentNullException(nameof(notInterestringAsymmetricTheorems));
        }

        #endregion
    }
}
