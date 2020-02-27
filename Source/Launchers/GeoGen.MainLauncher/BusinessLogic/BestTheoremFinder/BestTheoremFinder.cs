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
        private readonly RankingLadder<TheoremWithRanking, TheoremRanking> _ladder;

        #endregion

        #region IBestTheoremTracker properties

        /// <summary>
        /// The best theorems that currently have been found.
        /// </summary>
        public IEnumerable<TheoremWithRanking> BestTheorems => _ladder.Select(pair => pair.item);

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="BestTheoremFinder"/> class.
        /// </summary>
        /// <param name="settings">The settings for the finder.</param>
        public BestTheoremFinder(BestTheoremFinderSettings settings)
        {
            // Initialize the ladder with the requested capacity
            _ladder = new RankingLadder<TheoremWithRanking, TheoremRanking>(capacity: settings.NumberOfTheorems);
        }

        #endregion

        #region IBestTheoremsTracker methods

        /// <summary>
        /// Gives given theorems for the finder to judge them.
        /// </summary>
        /// <param name="theorems">The theorems to be examined.</param>
        /// <param name="bestTheoremsChanged">Indicates whether <see cref="BestTheorems"/> has changed after adding all theorems.</param>
        public void AddTheorems(IEnumerable<TheoremWithRanking> theorems, out bool bestTheoremsChanged)
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