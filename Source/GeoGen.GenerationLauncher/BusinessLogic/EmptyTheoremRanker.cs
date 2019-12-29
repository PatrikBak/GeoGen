using GeoGen.Core;
using GeoGen.TheoremProver;
using GeoGen.TheoremRanker;
using System.Collections.Generic;

namespace GeoGen.GenerationLauncher
{
    /// <summary>
    /// The implementation of <see cref="ITheoremRanker"/> that returns an empty ranking.
    /// </summary>
    public class EmptyTheoremRanker : ITheoremRanker
    {
        /// <summary>
        /// Ranks a given theorem, potentially using all given provided context.
        /// </summary>
        /// <param name="theorem">The theorem to be ranked.</param>
        /// <param name="configuration">The configuration where the theorem holds.</param>
        /// <param name="allTheorems">The map of all theorems of the configuration.</param>
        /// <param name="proverOutput">The output from the theorem prover for all the theorems of the configuration.</param>
        /// <returns>The theorem ranking of the theorem containing ranking and coefficient for particular aspects of ranking.</returns>
        public TheoremRanking Rank(Theorem theorem, Configuration configuration, TheoremMap allTheorems, TheoremProverOutput proverOutput)
            // Return an empty ranking
            => new TheoremRanking(new Dictionary<RankedAspect, RankingData>());
    }
}
