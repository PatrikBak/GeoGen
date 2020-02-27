using GeoGen.Core;
using GeoGen.Utilities;

namespace GeoGen.TheoremRanker
{
    /// <summary>
    /// The base class for <see cref="IAspectTheoremRanker"/>s.
    /// </summary>
    public abstract class AspectTheoremRankerBase : IAspectTheoremRanker
    {
        #region IAspectTheoremRanker properties

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public abstract double Rank(Theorem theorem, Configuration configuration, TheoremMap allTheorems);

        #endregion
    }
}