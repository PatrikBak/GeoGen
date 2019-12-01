using System;

namespace GeoGen.TheoremRanker
{
    /// <summary>
    /// Represents a ranking data combining <see cref="Ranking"/> and <see cref="Message"/> 
    /// provided by a <see cref="IAspectTheoremRanker"/>, with a custom <see cref="Coefficient"/>.
    /// </summary>
    public class RankingData
    {
        #region Public properties

        /// <summary>
        /// The actual ranking calculated by a ranker.
        /// </summary>
        public double Ranking { get; }

        /// <summary>
        /// The coefficient that is associated with this ranking. The total contribution of this ranking
        /// is then the product of this coefficient and <see cref="Ranking"/>.
        /// </summary>
        public double Coefficient { get; }

        /// <summary>
        /// The message provided by the ranker explaining the value of <see cref="Ranking"/>.
        /// </summary>
        public string Message { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="RankingData"/> class.
        /// </summary>
        /// <param name="ranking">The actual ranking calculated by a ranker.</param>
        /// <param name="coefficient">The coefficient that is associated with this ranking. The total 
        /// contribution of this ranking is then the product of this coefficient and <see cref="Ranking"/>.</param>
        /// <param name="message">The message provided by the ranker explaining the value of <see cref="Ranking"/>.</param>
        public RankingData(double ranking, double coefficient, string message)
        {
            Ranking = ranking;
            Coefficient = coefficient;
            Message = message ?? throw new ArgumentNullException(nameof(message));
        }

        #endregion
    }
}