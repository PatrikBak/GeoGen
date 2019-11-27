using GeoGen.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.TheoremRanker
{
    /// <summary>
    /// Represents a ranking of <see cref="Theorem"/> awarded by <see cref="ITheoremRanker"/>.
    /// </summary>
    public class TheoremRanking
    {
        #region Public properties

        /// <summary>
        /// The dictionary mapping ranked aspects to the particular rankings with their coefficients.
        /// </summary>
        public IReadOnlyDictionary<RankedAspect, (double coefficient, double ranking)> Ranking { get; }

        /// <summary>
        /// The total ranking calculated as the linear combination of the individual entries of <see cref="Ranking"/>.
        /// </summary>
        public double TotalRanking => Ranking.Values.Select(pair => pair.coefficient * pair.ranking).Sum();

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="TheoremRanking"/> class.
        /// </summary>
        /// <param name="ranking">The dictionary mapping ranked aspects to the particular rankings with their coefficients.</param>
        public TheoremRanking(IReadOnlyDictionary<RankedAspect, (double coefficient, double ranking)> ranking)
        {
            Ranking = ranking ?? throw new ArgumentNullException(nameof(ranking));
        }

        #endregion
    }
}