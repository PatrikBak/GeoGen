using GeoGen.Core;
using GeoGen.ProblemGenerator;
using GeoGen.TheoremRanker;

namespace GeoGen.ProblemAnalyzer
{
    /// <summary>
    /// The base class for the possible types of output of a <see cref="IGeneratedProblemAnalyzer"/>.
    /// </summary>
    public abstract class GeneratedProblemAnalyzerOutputBase
    {
        #region Public properties

        /// <summary>
        /// The interesting theorems sorted ascending by their total ranking. 
        /// </summary>
        public IReadOnlyList<RankedTheorem> InterestingTheorems { get; }

        /// <summary>
        /// The theorems that are not among <see cref="InterestingTheorems"/> because they have been ruled out
        /// because of symmetry, based on a <see cref="SymmetryGenerationMode"/>.
        /// </summary>
        public IReadOnlyCollection<Theorem> NotInterestringTheorems { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="GeneratedProblemAnalyzerOutputBase"/> class.
        /// </summary>
        /// <param name="interestingTheorems"><inheritdoc cref="InterestingTheorems" path="/summary"/></param>
        /// <param name="notInterestringTheorems"><inheritdoc cref="NotInterestringTheorems" path="/summary"/></param>
        protected GeneratedProblemAnalyzerOutputBase(IReadOnlyList<RankedTheorem> interestingTheorems,
                                                     IReadOnlyCollection<Theorem> notInterestringTheorems)
        {
            InterestingTheorems = interestingTheorems ?? throw new ArgumentNullException(nameof(interestingTheorems));
            NotInterestringTheorems = notInterestringTheorems ?? throw new ArgumentNullException(nameof(notInterestringTheorems));
        }

        #endregion
    }
}
