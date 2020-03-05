using GeoGen.Core;

namespace GeoGen.TheoremRanker
{
    /// <summary>
    /// Represents a ranker of <see cref="Theorem"/> within their context represented by <see cref="Configuration"/> 
    /// and <see cref="TheoremMap"/> of all theorems of the configuration.
    /// </summary>
    public interface ITheoremRanker
    {
        /// <summary>
        /// Ranks a given theorem within a given context.
        /// </summary>
        /// <param name="theorem">The theorem to be ranked.</param>
        /// <param name="configuration">The configuration where the theorem holds.</param>
        /// <param name="allTheorems">The map of all theorems of the configuration.</param>
        /// <returns>The ranked theorem.</returns>
        RankedTheorem Rank(Theorem theorem, Configuration configuration, TheoremMap allTheorems);
    }
}
