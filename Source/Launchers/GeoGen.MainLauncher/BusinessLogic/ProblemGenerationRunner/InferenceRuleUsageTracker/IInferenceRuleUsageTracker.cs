using GeoGen.TheoremProver;
using System.Collections.Generic;

namespace GeoGen.MainLauncher
{
    /// <summary>
    /// Represents a tracker of used <see cref="InferenceRule"/> within <see cref="TheoremProof"/>s.
    /// This is just a diagnostic tool to provide a better idea of who the prover uses the database of these rules.
    /// </summary>
    public interface IInferenceRuleUsageTracker
    {
        /// <summary>
        /// The dictionary containing the counts of the used rules.
        /// </summary>
        public IReadOnlyDictionary<InferenceRule, int> RuleUsages { get; }

        /// <summary>
        /// Finds and tracks the inner used rules of given proofs.
        /// </summary>
        /// <param name="proofs">The proofs.</param>
        void MarkProofs(IEnumerable<TheoremProof> proofs);
    }
}