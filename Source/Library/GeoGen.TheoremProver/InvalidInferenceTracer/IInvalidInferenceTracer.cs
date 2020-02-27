using GeoGen.Core;

namespace GeoGen.TheoremProver
{
    /// <summary>
    /// Represents a service that tracer inference results that turned out to be geometrically invalid or degenerated.
    /// </summary>
    public interface IInvalidInferenceTracer
    {
        /// <summary>
        /// Mark a given inferred theorem as invalid.
        /// </summary>
        /// <param name="configuration">The configuration in which the theorem should hold.</param>
        /// <param name="invalidConclusion">The inferred theorem that is either geometrically invalid or degenerated.</param>
        /// <param name="inferenceRule">The inference rule that has been used to conclude the theorem.</param>
        /// <param name="negativeAssumptions">The negative pre-conditions of the inference.</param>
        /// <param name="possitiveAssumptions">The assumptions of the inference.</param>
        void MarkInvalidInferrence(Configuration configuration, Theorem invalidConclusion, InferenceRule inferenceRule, Theorem[] negativeAssumptions, Theorem[] possitiveAssumptions);
    }
}