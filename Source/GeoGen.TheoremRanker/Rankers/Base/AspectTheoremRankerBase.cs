using GeoGen.Core;
using GeoGen.TheoremProver;
using GeoGen.Utilities;

namespace GeoGen.TheoremRanker
{
    /// <summary>
    /// The base class for <see cref="IAspectTheoremRanker"/>.
    /// </summary>
    public abstract class AspectTheoremRankerBase : IAspectTheoremRanker
    {
        #region IAspectTheoremRanker properties

        /// <summary>
        /// The aspect of theorems that this ranker ranks.
        /// </summary>
        public RankedAspect RankedAspect { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="AspectTheoremRankerBase"/> class.
        /// </summary>
        protected AspectTheoremRankerBase()
        {
            // Find the aspect
            RankedAspect = FindRankedAspectFromClassName();
        }

        #endregion

        #region Finding ranked aspect from the class name

        /// <summary>
        /// Infers the ranked aspect from the class name. 
        /// The class name should be in the form {type}Ranker.
        /// </summary>
        /// <returns>The inferred ranked aspect.</returns>
        private RankedAspect FindRankedAspectFromClassName()
        {
            // Call the utility helper that does the job
            return EnumUtilities.ParseEnumValueFromClassName<RankedAspect>(GetType(), classNamePrefix: "Ranker");
        }

        #endregion

        #region IAspectTheoremRanker methods

        /// <summary>
        /// Ranks a given theorem, potentially using all given provided context.
        /// </summary>
        /// <param name="theorem">The theorem to be ranked.</param>
        /// <param name="configuration">The configuration where the theorem holds.</param>
        /// <param name="allTheorems">The map of all theorems of the configuration.</param>
        /// <param name="proverOutput">The output from the theorem prover for all the theorems of the configuration.</param>
        /// <returns>A number representing the ranking of the theorem together with the explanation of how it was calculated.</returns>
        public abstract (double ranking, string message) Rank(Theorem theorem, Configuration configuration, TheoremMap allTheorems, TheoremProverOutput proverOutput);

        #endregion
    }
}