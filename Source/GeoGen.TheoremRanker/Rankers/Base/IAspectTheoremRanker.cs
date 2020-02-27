using GeoGen.Core;

namespace GeoGen.TheoremRanker
{
    /// <summary>
    /// Represents a ranker of a particular <see cref="RankedAspect"/>.
    /// </summary>
    public interface IAspectTheoremRanker
    {
        /// <summary>
        /// The aspect that this ranker ranks.
        /// </summary>
        RankedAspect RankedAspect { get; }

        /// <summary>
        /// Ranks a given theorem within a given context with regards to the <see cref="RankedAspect"/>.
        /// </summary>
        /// <param name="theorem">The theorem to be ranked.</param>
        /// <param name="configuration">The configuration where the theorem holds.</param>
        /// <param name="allTheorems">The map of all theorems of the configuration.</param>
        /// <returns>The theorem ranking of the theorem for the <see cref="RankedAspect"/>.</returns>
        double Rank(Theorem theorem, Configuration configuration, TheoremMap allTheorems);
    }
}
