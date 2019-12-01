using System.Collections.Generic;

namespace GeoGen.ConsoleLauncher
{
    /// <summary>
    /// Represents a tracker of best <see cref="TheoremData"/>s based on their ranking.
    /// </summary>
    public interface IBestTheoremTracker
    {
        /// <summary>
        /// The best theorems tracked by this tracker, sorted from the best one.
        /// </summary>
        IEnumerable<TheoremData> BestTheorems { get; }

        /// <summary>
        /// Adds given theorems to the tracker.
        /// </summary>
        /// <param name="theorems">The theorems to be added.</param>
        /// <param name="bestTheoremsChanged">Indicates whether the content of best theorems has changed after adding all theorems.</param>
        void AddTheorems(IEnumerable<TheoremData> theorems, out bool bestTheoremsChanged);
    }
}