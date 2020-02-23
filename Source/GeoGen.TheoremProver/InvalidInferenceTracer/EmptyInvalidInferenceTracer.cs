using GeoGen.Core;

namespace GeoGen.TheoremProver
{
    /// <summary>
    /// Represents an empty <see cref="IInvalidInferenceTracer"/> that does nothing.
    /// </summary>
    public class EmptyInvalidInferenceTracer : IInvalidInferenceTracer
    {
        /// <inheritdoc/>
        public void MarkInvalidInferrence(Configuration configuration, Theorem theorem, InferenceRule inferenceRule, Theorem[] negativeAssumptions, Theorem[] possitiveAssumptions)
        {
        }
    }
}