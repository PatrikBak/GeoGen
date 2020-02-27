using GeoGen.TheoremProver;
using System.Collections.Generic;

namespace GeoGen.ConsoleLauncher
{
    /// <summary>
    /// Represents a tracker of used <see cref="InferenceRule"/> within <see cref="TheoremProof"/>s.
    /// This is just a diagnostic tool to provide a better idea of who the prover uses the database of these rules.
    /// </summary>
    public interface IInferenceRuleUsageTracker
    {
        /// <summary>
        /// The dictionary containing the counts of the used rules. It includes even rules that have not been used.
        /// </summary>
        public IReadOnlyDictionary<InferenceRule, int> UsedRulesCounts { get; }

        /// <summary>
        /// Finds and tracks the inner used rules of given proofs.
        /// </summary>
        /// <param name="proofs">The proofs.</param>
        void MarkProofs(IEnumerable<TheoremProof> proofs);
    }
}