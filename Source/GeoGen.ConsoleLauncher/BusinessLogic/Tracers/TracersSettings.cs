namespace GeoGen.ConsoleLauncher
{
    /// <summary>
    /// Contains all settings used in tracers writing information per batch.
    /// </summary>
    public class TracersSettings
    {
        #region Public properties

        /// <summary>
        /// Indicates whether tracing of constructor failures is on.
        /// </summary>
        public bool TraceConstructorFailures { get; }

        /// <summary>
        /// The settings for <see cref="ConstructorFailureTracer"/>. 
        /// </summary>
        public ConstructorFailureTracerSettings ConstructorFailureTracerSettings { get; }

        /// <summary>
        /// Indicates whether tracing of geometry failures is on.
        /// </summary>
        public bool TraceGeometryFailures { get; }

        /// <summary>
        /// The settings for <see cref="GeometryFailureTracer"/>. 
        /// </summary>
        public GeometryFailureTracerSettings GeometryFailureTracerSettings { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="TracersSettings"/> class.
        /// </summary>
        /// <param name="traceConstructorFailures">Indicates whether tracing of constructor failures is on.</param>
        /// <param name="constructorFailureTracerSettings">The settings for <see cref="ConstructorFailureTracer"/>.</param>
        /// <param name="traceGeometryFailures">Indicates whether tracing of geometry failures is on.</param>
        /// <param name="geometryFailureTracerSettings">The settings for <see cref="GeometryFailureTracer"/>.</param>
        public TracersSettings(bool traceConstructorFailures,
                               ConstructorFailureTracerSettings constructorFailureTracerSettings,
                               bool traceGeometryFailures,
                               GeometryFailureTracerSettings geometryFailureTracerSettings)
        {
            TraceConstructorFailures = traceConstructorFailures;
            ConstructorFailureTracerSettings = constructorFailureTracerSettings;
            TraceGeometryFailures = traceGeometryFailures;
            GeometryFailureTracerSettings = geometryFailureTracerSettings;
        }

        #endregion
    }
}
