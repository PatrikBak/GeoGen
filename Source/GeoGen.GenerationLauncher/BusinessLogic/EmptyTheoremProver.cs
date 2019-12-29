using GeoGen.Core;
using GeoGen.TheoremProver;
using System.Collections.Generic;

namespace GeoGen.GenerationLauncher
{
    /// <summary>
    /// The implementation of <see cref="ITheoremProver"/> that returns an empty output.
    /// </summary>
    public class EmptyTheoremProver : ITheoremProver
    {
        /// <summary>
        /// Performs the analysis for a given input.
        /// </summary>
        /// <param name="input">The input for the analyzer.</param>
        /// <returns>The output of the analysis.</returns>
        public TheoremProverOutput Analyze(TheoremProverInput input) => new TheoremProverOutput
        (
            provenTheorems: new Dictionary<Theorem, TheoremProofAttempt>(),
            unprovenTheorems: new Dictionary<Theorem, IReadOnlyList<TheoremProofAttempt>>(),
            unprovenDiscoveredTheorems: new Dictionary<Theorem, IReadOnlyList<TheoremProofAttempt>>()
        );
    }
}
