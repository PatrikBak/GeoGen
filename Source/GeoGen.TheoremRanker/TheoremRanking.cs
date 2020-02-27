using GeoGen.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.TheoremRanker
{
    /// <summary>
    /// Represents a ranking of a <see cref="Theorem"/> awarded by a <see cref="ITheoremRanker"/>.
    /// </summary>
    public class TheoremRanking : IComparable<TheoremRanking>
    {
        #region Public properties

        /// <summary>
        /// The dictionary mapping ranked aspects to the particular rankings.
        /// </summary>
        public IReadOnlyDictionary<RankedAspect, RankingData> Rankings { get; }

        /// <summary>
        /// The total ranking calculated as a sum of particular <see cref="RankingData.Contribution"/>s.
        /// </summary>
        public double TotalRanking { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="TheoremRanking"/> class.
        /// </summary>
        /// <param name="rankings">The dictionary mapping ranked aspects to the particular rankings.</param>
        public TheoremRanking(IReadOnlyDictionary<RankedAspect, RankingData> rankings)
        {
            Rankings = rankings ?? throw new ArgumentNullException(nameof(rankings));

            // Calculate the total ranking by summing the contributions
            TotalRanking = Rankings.Values.Select(data => data.Contribution).Sum();
        }

        #endregion

        #region IComparable implementation

        /// <summary>
        /// Compares the <see cref="TotalRanking"/>s of this and a given ranking object.
        /// </summary>
        /// <param name="otherRanking">The other ranking object.</param>
        /// <returns>-1, if this ranking is smaller; 0, if they are the same; 1 if this ranking is larger.</returns>
        public int CompareTo(TheoremRanking otherRanking) => TotalRanking.CompareTo(otherRanking.TotalRanking);

        #endregion
    }
}