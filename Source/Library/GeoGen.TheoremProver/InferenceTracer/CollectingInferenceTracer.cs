#nullable enable
using GeoGen.Core;

namespace GeoGen.TheoremProver
{
    /// <summary>
    /// An <see cref="IInferenceTracer"/> that retains every event of every session it observes.
    /// Suitable for test harnesses or report generators — call <see cref="DrainSession"/> after a
    /// session ends to collect the events for one specific configuration, or <see cref="Sessions"/>
    /// to inspect everything observed since construction.
    /// <para>
    /// Thread-safe: events from different threads (or interleaved sessions) are linked back to their
    /// owning configuration. The default <see cref="TheoremProver"/> is single-threaded per call so
    /// in practice events arrive sequentially.
    /// </para>
    /// </summary>
    public sealed class CollectingInferenceTracer : IInferenceTracer
    {
        private readonly object _gate = new();
        private readonly List<TracedSession> _sessions = new();
        private TracedSession? _activeSession;

        /// <summary>All sessions observed so far, in start-order.</summary>
        public IReadOnlyList<TracedSession> Sessions
        {
            get
            {
                lock (_gate)
                    return _sessions.ToArray();
            }
        }

        public void OnSessionStart(Configuration configuration)
        {
            lock (_gate)
            {
                _activeSession = new TracedSession(configuration);
                _sessions.Add(_activeSession);
            }
        }

        public void OnInference(Configuration configuration, InferenceEvent @event)
        {
            lock (_gate)
            {
                // Defensive: in normal use OnSessionStart is always called first.
                if (_activeSession is null || !ReferenceEquals(_activeSession.Configuration, configuration))
                    return;

                _activeSession.Events.Add(@event);
            }
        }

        public void OnSessionEnd(Configuration configuration)
        {
            lock (_gate)
            {
                if (_activeSession is not null && ReferenceEquals(_activeSession.Configuration, configuration))
                    _activeSession = null;
            }
        }

        /// <summary>
        /// Pop and return the most recent completed session for <paramref name="configuration"/>.
        /// Returns null if no such session exists.
        /// </summary>
        public TracedSession? DrainSession(Configuration configuration)
        {
            lock (_gate)
            {
                for (var i = _sessions.Count - 1; i >= 0; i--)
                {
                    if (ReferenceEquals(_sessions[i].Configuration, configuration))
                    {
                        var session = _sessions[i];
                        _sessions.RemoveAt(i);
                        return session;
                    }
                }
                return null;
            }
        }
    }

    /// <summary>
    /// All inference events recorded for a single proving session.
    /// </summary>
    public sealed class TracedSession
    {
        public Configuration Configuration { get; }
        public List<InferenceEvent> Events { get; } = new();

        public TracedSession(Configuration configuration)
        {
            Configuration = configuration;
        }
    }
}
