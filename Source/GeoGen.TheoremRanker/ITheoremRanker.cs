using GeoGen.Core;
using GeoGen.TheoremProver;

namespace GeoGen.TheoremRanker
{
    /// <summary>
    /// Represents the ranker of <see cref="Theorem"/> within their context represented by 
    /// <see cref="Configuration"/>, all of its theorems, and <see cref="TheoremProverOutput"/> for them.
    /// The ranking is returned within a <see cref="TheoremRanking"/> object.
    /// </summary>
    public interface ITheoremRanker
    {
        /// <summary>
        /// Ranks a given theorem, potentially using all given provided context.
        /// </summary>
        /// <param name="theorem">The theorem to be ranked.</param>
        /// <param name="configuration">The configuration where the theorem holds.</param>
        /// <param name="allTheorems">The map of all theorems of the configuration.</param>
        /// <param name="proverOutput">The output from the theorem prover for all the theorems of the configuration.</param>
        /// <returns>The theorem ranking of the theorem containing ranking and coefficient for particular aspects of ranking.</returns>
        TheoremRanking Rank(Theorem theorem, Configuration configuration, TheoremMap allTheorems, TheoremProverOutput proverOutput);
    }
}
