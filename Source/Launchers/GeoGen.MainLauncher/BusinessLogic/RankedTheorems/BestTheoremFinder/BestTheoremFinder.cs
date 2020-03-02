using GeoGen.TheoremRanker;
using GeoGen.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.MainLauncher
{
    /// <summary>
    /// The default implementation of <see cref="IBestTheoremFinder"/> can track a specified number of theorems.
    /// The current implementation doesn't do any check whether we have duplicated (potentially from multiple sources).
    /// </summary>
    public class BestTheoremFinder : IBestTheoremFinder
    {
        #region Private fields

        /// <summary>
        /// The ladder of best theorems.
        /// </summary>
        private readonly RankingLadder<RankedTheorem, TheoremRanking> _ladder;

        #endregion

        #region IBestTheoremFinder properties

        /// <inheritdoc/>
        public IEnumerable<RankedTheorem> BestTheorems => _ladder.Select(pair => pair.item);

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="BestTheoremFinder"/> class.
        /// </summary>
        /// <param name="settings">The settings for the finder.</param>
        public BestTheoremFinder(BestTheoremFinderSettings settings)
        {
            // Initialize the ladder with the requested capacity
            _ladder = new RankingLadder<RankedTheorem, TheoremRanking>(capacity: settings.NumberOfTheorems);
        }

        #endregion

        #region IBestTheoremFinder methods

        /// <inheritdoc/>
        public void AddTheorems(IEnumerable<RankedTheorem> theorems, out bool bestTheoremsChanged)
        {
            // Set the that the best theorems hasn't initially changed
            bestTheoremsChanged = false;

            // Add all the theorems to the ladder
            foreach (var theoremData in theorems)
            {
                // Add the current data
                _ladder.Add(theoremData, theoremData.Ranking, out var contentChanged);

                // If this adding changed the content, then we mark it
                if (contentChanged)
                    bestTheoremsChanged = true;
            }
        }

        #endregion
    }
}