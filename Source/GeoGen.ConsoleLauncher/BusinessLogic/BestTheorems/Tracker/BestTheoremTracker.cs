using GeoGen.TheoremRanker;
using GeoGen.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.ConsoleLauncher
{
    /// <summary>
    /// The default implementation of <see cref="IBestTheoremTracker"/> can track a specified number of theorems.
    /// </summary>
    public class BestTheoremTracker : IBestTheoremTracker
    {
        #region Private fields

        /// <summary>
        /// The ladder of best theorems.
        /// </summary>
        private readonly RankingLadder<TheoremData, TheoremRanking> _ladder;

        #endregion

        #region IBestTheoremTracker properties

        /// <summary>
        /// The best theorems tracked by this tracker, sorted from the best one.
        /// </summary>
        public IEnumerable<TheoremData> BestTheorems => _ladder.Select(pair => pair.item);

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="BestTheoremTracker"/> class.
        /// </summary>
        /// <param name="settings">The settings for the tracker.</param>
        public BestTheoremTracker(BestTheoremTrackerSettings settings)
        {
            // Initialize the ladder with the requested capacity
            _ladder = new RankingLadder<TheoremData, TheoremRanking>(capacity: settings.NumberOfTheorems);
        }

        #endregion

        #region IBestTheoremsTracker methods

        /// <summary>
        /// Adds given theorems to the tracker.
        /// </summary>
        /// <param name="theorems">The theorems to be added.</param>
        /// <param name="bestTheoremsChanged">Indicates whether the content of best theorems has changed after adding all theorems.</param>
        public void AddTheorems(IEnumerable<TheoremData> theorems, out bool bestTheoremsChanged)
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