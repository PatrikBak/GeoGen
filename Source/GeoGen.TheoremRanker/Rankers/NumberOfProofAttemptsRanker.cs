using GeoGen.Core;
using GeoGen.TheoremProver;

namespace GeoGen.TheoremRanker
{
    /// <summary>
    /// The <see cref="IAspectTheoremRanker"/> of <see cref="RankedAspect.NumberOfProofAttempts"/>.
    /// </summary>
    public class NumberOfProofAttemptsRanker : AspectTheoremRankerBase
    {
        /// <summary>
        /// Ranks a given theorem, potentially using all given provided context.
        /// </summary>
        /// <param name="theorem">The theorem to be ranked.</param>
        /// <param name="configuration">The configuration where the theorem holds.</param>
        /// <param name="allTheorems">The map of all theorems of the configuration.</param>
        /// <param name="proverOutput">The output from the theorem prover for all the theorems of the configuration.</param>
        /// <returns>A number representing the ranking of the theorem together with the explanation of how it was calculated.</returns>
        public override (double ranking, string message) Rank(Theorem theorem, Configuration configuration, TheoremMap allTheorems, TheoremProverOutput proverOutput)
        {
            // Get the number of proof attempts
            var numberOfProofAttempts = proverOutput.UnprovenTheorems[theorem].Count;

            // Now Simply apply the formula described in the documentation of RankedAspect.NumberOfProofAttempts
            // And for the message include the whole formula
            return (1D / (1 + numberOfProofAttempts), $"1 / (1 + proof attempts [{numberOfProofAttempts}])");
        }
    }
}
