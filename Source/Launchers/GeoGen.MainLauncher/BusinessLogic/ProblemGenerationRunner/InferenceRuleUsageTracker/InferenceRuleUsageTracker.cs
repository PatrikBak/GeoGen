using GeoGen.Core;
using GeoGen.TheoremProver;
using GeoGen.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.MainLauncher
{
    /// <summary>
    /// The default implementation of <see cref="IInferenceRuleUsageTracker"/> This service requires to know about all the 
    /// inference rules so it can easily point out to those that haven't been used at all (and might potentially be unnecessary).
    /// </summary>
    public class InferenceRuleUsageTracker : IInferenceRuleUsageTracker
    {
        #region Private fields

        /// <summary>
        /// The dictionary mapping inference rules to the number of their usages.
        /// </summary>
        private readonly Dictionary<InferenceRule, int> _inferenceRuleUsages;

        #endregion

        #region IInferenceRuleUsageTracker properties

        /// <inheritdoc/>
        public IReadOnlyDictionary<InferenceRule, int> RuleUsages => _inferenceRuleUsages;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="InferenceRuleUsageTracker"/> class.
        /// </summary>
        /// <param name="managerData">The data for the rules manager containing all inference rules.</param>
        public InferenceRuleUsageTracker(InferenceRuleManagerData managerData)
        {
            // Set the inference rules with 0 usages initially
            _inferenceRuleUsages = managerData?.Rules.ToDictionary(rule => rule, rule => 0) ?? throw new ArgumentNullException(nameof(managerData));
        }

        #endregion

        #region IInferenceRuleUsageTracker methods

        /// <inheritdoc/>
        public void MarkProofs(IEnumerable<TheoremProof> proofs)
        {
            // Prepare a set of theorems whose proof has been processed
            var processedTheorems = new HashSet<Theorem>();

            // Local function that processes a single proof
            void Process(TheoremProof proof)
            {
                // If the theorem has been processed, do nothing
                if (processedTheorems.Contains(proof.Theorem))
                    return;

                // If the proof is done via an inference rule, mark it
                // Since we initially have all the rules available, it should not crash
                if (proof.Data is CustomInferenceData customData)
                    _inferenceRuleUsages[customData.Rule]++;

                // Process the assumptions
                proof.ProvedAssumptions.ForEach(Process);
            }

            // Process the given proofs
            proofs.ForEach(Process);
        }

        #endregion
    }
}