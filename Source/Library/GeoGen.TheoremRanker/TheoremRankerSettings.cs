namespace GeoGen.TheoremRanker
{
    /// <summary>
    /// The settings for <see cref="TheoremRanker"/>.
    /// </summary>
    public class TheoremRankerSettings
    {
        #region Public properties

        /// <summary>
        /// The coefficients used in the calculation of the overall ranking for particular ranked aspects.
        /// </summary>
        public IReadOnlyDictionary<RankedAspect, double> RankingCoefficients { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="TheoremRankerSettings"/> class.
        /// </summary>
        /// <param name="rankingCoefficients">The coefficients used in the calculation of the overall ranking for particular ranked aspects.</param>
        public TheoremRankerSettings(IReadOnlyDictionary<RankedAspect, double> rankingCoefficients)
        {
            RankingCoefficients = rankingCoefficients ?? throw new ArgumentNullException(nameof(rankingCoefficients));
        }

        #endregion
    }
}