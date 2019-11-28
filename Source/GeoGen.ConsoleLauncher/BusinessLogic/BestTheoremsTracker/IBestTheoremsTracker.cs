using GeoGen.Core;
using GeoGen.TheoremRanker;

namespace GeoGen.ConsoleLauncher
{
    /// <summary>
    /// Represents a tracker of best <see cref="Theorem"/>s based on their rank.
    /// </summary>
    public interface IBestTheoremsTracker
    {
        /// <summary>
        /// Adds a given theorem holding in a given configuration with a given rank. 
        /// </summary>
        /// <param name="theorem">The theorem to potentially be tracked.</param>
        /// <param name="configuration">The configuration where the theorem holds.</param>
        /// <param name="rank">The rank of the theorem.</param>
        /// <param name="id">The id of the theorem.</param>
        void AddTheorem(Theorem theorem, Configuration configuration, TheoremRanking rank, string id);
    }
}