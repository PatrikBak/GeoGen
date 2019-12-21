namespace GeoGen.ConsoleLauncher
{
    /// <summary>
    /// Contains all settings used in tracers writing information per batch.
    /// </summary>
    public class TracersSettings
    {
        #region Public properties

        /// <summary>
        /// The settings for <see cref="ConstructorFailureTracer"/>. The value should be null if the tracing shouldn't happen.
        /// </summary>
        public ConstructorFailureTracerSettings ConstructorFailureTracerSettings { get; }

        /// <summary>
        /// The settings for <see cref="GeometryFailureTracer"/>. The value should be null if the tracing shouldn't happen.
        /// </summary>
        public GeometryFailureTracerSettings GeometryFailureTracerSettings { get; }

        /// <summary>
        /// The settings for <see cref="SubtheoremDeriverGeometryFailureTracer"/>. The value should be null if the tracing shouldn't happen.
        /// </summary>
        public SubtheoremDeriverGeometryFailureTracerSettings SubtheoremDeriverGeometryFailureTracerSettings { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="TracersSettings"/> class.
        /// </summary>
        /// <param name="constructorFailureTracerSettings">The settings for <see cref="ConstructorFailureTracer"/>. The value should be null if the tracing shouldn't happen.</param>
        /// <param name="geometryFailureTracerSettings">The settings for <see cref="GeometryFailureTracer"/>. The value should be null if the tracing shouldn't happen.</param>
        /// <param name="subtheoremDeriverGeometryFailureTracerSettings">The settings for <see cref="SubtheoremDeriverGeometryFailureTracer"/>. The value should be null if the tracing shouldn't happen.</param>
        public TracersSettings(ConstructorFailureTracerSettings constructorFailureTracerSettings,
                               GeometryFailureTracerSettings geometryFailureTracerSettings,
                               SubtheoremDeriverGeometryFailureTracerSettings subtheoremDeriverGeometryFailureTracerSettings)
        {
            ConstructorFailureTracerSettings = constructorFailureTracerSettings;
            GeometryFailureTracerSettings = geometryFailureTracerSettings;
            SubtheoremDeriverGeometryFailureTracerSettings = subtheoremDeriverGeometryFailureTracerSettings;
        }

        #endregion
    }
}
