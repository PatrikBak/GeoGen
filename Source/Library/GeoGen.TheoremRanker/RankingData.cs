namespace GeoGen.TheoremRanker
{
    /// <summary>
    /// Represents a ranking data a holding <see cref="Ranking"/> calculated by a <see cref="IAspectTheoremRanker"/>
    /// and a <see cref="Weight"/> of this ranking in the total ranking.
    /// </summary>
    public class RankingData
    {
        #region Public properties

        /// <summary>
        /// The ranking value calculated by a ranker.
        /// </summary>
        public double Ranking { get; }

        /// <summary>
        /// The weight that is associated with this ranking. 
        /// </summary>
        public double Weight { get; }

        /// <summary>
        /// The contribution of the ranking to the total ranking, calculates as a product of 
        /// <see cref="Ranking"/>  and <see cref="Weight"/>. 
        /// </summary>
        public double Contribution { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="RankingData"/> class.
        /// </summary>
        /// <param name="ranking">The ranking value calculated by a ranker.</param>
        /// <param name="weight">The weight that is associated with this ranking. </param>
        public RankingData(double ranking, double weight)
        {
            Ranking = ranking;
            Weight = weight;

            // Calculate the contribution
            Contribution = ranking * weight;
        }

        #endregion
    }
}