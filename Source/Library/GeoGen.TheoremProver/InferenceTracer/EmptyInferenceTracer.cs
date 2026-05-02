using GeoGen.Core;

namespace GeoGen.TheoremProver
{
    /// <summary>
    /// No-op default for <see cref="IInferenceTracer"/>. Bound by <c>AddTheoremProver</c> so consumers
    /// that don't care about per-inference telemetry pay nothing.
    /// </summary>
    public sealed class EmptyInferenceTracer : IInferenceTracer
    {
        public void OnSessionStart(Configuration configuration) { }
        public void OnInference(Configuration configuration, InferenceEvent @event) { }
        public void OnSessionEnd(Configuration configuration) { }
    }
}
