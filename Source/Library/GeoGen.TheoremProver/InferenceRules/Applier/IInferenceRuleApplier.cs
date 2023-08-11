using GeoGen.Core;

namespace GeoGen.TheoremProver
{
    /// <summary>
    /// Represents a service that is able to take an <see cref="InferenceRuleApplierInput"/>
    /// and infer new <see cref="Theorem"/>s. For more information see the documentation of
    /// <see cref="InferenceRuleApplierInput"/>.
    /// </summary>
    public interface IInferenceRuleApplier
    {
        /// <summary>
        /// Infers theorems from a given <see cref="InferenceRuleApplierInput"/>.
        /// </summary>
        /// <param name="input">The input containing all needed data for the inference.</param>
        /// <returns>The inferred theorems with the used assumptions. It might contain one inference multiple times.</returns>
        IEnumerable<(Theorem inferredTheorem, Theorem[] negativeAssumptions, Theorem[] positiveAssumptions)> InferTheorems(InferenceRuleApplierInput input);
    }
}