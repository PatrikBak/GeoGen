using System;

namespace GeoGen.ConsoleLauncher
{
    /// <summary>
    /// Contains all settings used in tracers writing information per batch.
    /// </summary>
    public class TracingSettings
    {
        #region Public properties

        /// <summary>
        /// Indicates whether tracing of constructor failures is on.
        /// </summary>
        public bool TraceConstructorFailures { get; }

        /// <summary>
        /// The settings for <see cref="ConstructorFailureTracer"/>. This value can be null if we don't want to trace them.
        /// </summary>
        public ConstructorFailureTracerSettings ConstructorFailureTracerSettings { get; }

        /// <summary>
        /// Indicates whether tracing of geometry failures is on.
        /// </summary>
        public bool TraceGeometryFailures { get; }

        /// <summary>
        /// The settings for <see cref="GeometryFailureTracer"/>. This value can be null if we don't want to trace them.
        /// </summary>
        public GeometryFailureTracerSettings GeometryFailureTracerSettings { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="TracingSettings"/> class.
        /// </summary>
        /// <param name="traceConstructorFailures">Indicates whether tracing of constructor failures is on.</param>
        /// <param name="constructorFailureTracerSettings">The settings for <see cref="ConstructorFailureTracer"/>. This value can be null if we don't want to trace them.</param>
        /// <param name="traceGeometryFailures">Indicates whether tracing of geometry failures is on.</param>
        /// <param name="geometryFailureTracerSettings">The settings for <see cref="GeometryFailureTracer"/>. This value can be null if we don't want to trace them.</param>
        public TracingSettings(bool traceConstructorFailures,
                               ConstructorFailureTracerSettings constructorFailureTracerSettings,
                               bool traceGeometryFailures,
                               GeometryFailureTracerSettings geometryFailureTracerSettings)
        {
            TraceConstructorFailures = traceConstructorFailures;
            ConstructorFailureTracerSettings = constructorFailureTracerSettings;
            TraceGeometryFailures = traceGeometryFailures;
            GeometryFailureTracerSettings = geometryFailureTracerSettings;

            // Ensure that construction failure settings are set if they are supposed to be traced
            if (TraceConstructorFailures && constructorFailureTracerSettings == null)
                throw new ArgumentException("The construction failure tracer settings must be set as we are supposed to be tracing them.");

            // Ensure that geometry failure settings are set if they are supposed to be traced
            if (TraceGeometryFailures && geometryFailureTracerSettings == null)
                throw new ArgumentException("The geometry failure tracer settings must be set as we are supposed to be tracing them.");
        }

        #endregion
    }
}
