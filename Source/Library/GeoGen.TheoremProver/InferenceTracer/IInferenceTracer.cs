#nullable enable
using GeoGen.Core;

namespace GeoGen.TheoremProver
{
    /// <summary>
    /// One trace event recorded by an <see cref="IInferenceTracer"/>: the prover accepted
    /// <see cref="Theorem"/> as proven, justified by <see cref="Assumptions"/> via the listed rule type.
    /// </summary>
    /// <param name="Sequence">Monotonic per-session counter, starting at 1.</param>
    /// <param name="Rule">The kind of inference that produced the theorem (custom rule, trivial, transitivity, …).</param>
    /// <param name="CustomRule">For <see cref="InferenceRuleType.CustomRule"/> events, the underlying <see cref="InferenceRule"/>; null otherwise.</param>
    /// <param name="Theorem">The theorem that was just proven.</param>
    /// <param name="Assumptions">The other theorems that justified <paramref name="Theorem"/>.</param>
    public sealed record InferenceEvent(
        int Sequence,
        InferenceRuleType Rule,
        InferenceRule? CustomRule,
        Theorem Theorem,
        IReadOnlyList<Theorem> Assumptions);

    /// <summary>
    /// Observer that fires once per accepted inference inside <see cref="TheoremProver"/>.
    /// Mirrors the <see cref="IInvalidInferenceTracer"/> pattern but on the *successful* path,
    /// so callers can build a complete event log of what the prover noticed and in what order.
    /// <para>
    /// The default binding is <see cref="EmptyInferenceTracer"/>, a no-op. Callers that want
    /// telemetry rebind in IoC (typically to a collecting implementation that the harness drains).
    /// </para>
    /// </summary>
    public interface IInferenceTracer
    {
        /// <summary>
        /// Notify that a new proving session has started for <paramref name="configuration"/>.
        /// Implementations that aggregate per-session state should reset on this call.
        /// </summary>
        void OnSessionStart(Configuration configuration);

        /// <summary>
        /// Notify that the prover accepted <paramref name="event"/>'s theorem as proven.
        /// </summary>
        void OnInference(Configuration configuration, InferenceEvent @event);

        /// <summary>
        /// Notify that the proving session has ended. Implementations may flush aggregated state here.
        /// </summary>
        void OnSessionEnd(Configuration configuration);
    }
}
