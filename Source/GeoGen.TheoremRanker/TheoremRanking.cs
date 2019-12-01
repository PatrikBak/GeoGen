using GeoGen.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.TheoremRanker
{
    /// <summary>
    /// Represents a ranking of <see cref="Theorem"/> awarded by <see cref="ITheoremRanker"/>.
    /// </summary>
    public class TheoremRanking : IComparable<TheoremRanking>
    {
        #region Public properties

        /// <summary>
        /// The dictionary mapping ranked aspects to the particular rankings.
        /// </summary>
        public IReadOnlyDictionary<RankedAspect, RankingData> Ranking { get; }

        /// <summary>
        /// The total ranking calculated as the linear combination of the individual entries of <see cref="Ranking"/>.
        /// </summary>
        public double TotalRanking { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="TheoremRanking"/> class.
        /// </summary>
        /// <param name="ranking">The dictionary mapping ranked aspects to the particular rankings..</param>
        public TheoremRanking(IReadOnlyDictionary<RankedAspect, RankingData> ranking)
        {
            Ranking = ranking ?? throw new ArgumentNullException(nameof(ranking));

            // Calculate the total ranking
            TotalRanking = Ranking.Values.Select(data => data.Coefficient * data.Ranking).Sum();
        }

        #endregion

        #region IComparable implementation

        /// <summary>
        /// Compares the <see cref="TotalRanking"/>s of this and given ranking object.
        /// </summary>
        /// <param name="otherRanking">The other ranking object..</param>
        /// <returns>-1, if this ranking is smaller; 0, if they are the same; 1 if this ranking is larger.</returns>
        public int CompareTo(TheoremRanking otherRanking) => TotalRanking.CompareTo(otherRanking.TotalRanking);

        #endregion
    }
}