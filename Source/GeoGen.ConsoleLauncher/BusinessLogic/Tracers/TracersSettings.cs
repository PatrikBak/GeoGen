using System;

namespace GeoGen.ConsoleLauncher
{
    /// <summary>
    /// Contains all settings used in tracers writing information per batch.
    /// </summary>
    public class TracersSettings
    {
        #region Public properties

        /// <summary>
        /// The settings for <see cref="ConstructorFailureTracer"/>.
        /// </summary>
        public ConstructorFailureTracerSettings ConstructorFailureTracerSettings { get; }

        /// <summary>
        /// The settings for <see cref="GeometryFailureTracer"/>.
        /// </summary>
        public GeometryFailureTracerSettings GeometryFailureTracerSettings { get; }

        /// <summary>
        /// The settings for <see cref="SubtheoremDeriverGeometryFailureTracer"/>.
        /// </summary>
        public SubtheoremDeriverGeometryFailureTracerSettings SubtheoremDeriverGeometryFailureTracerSettings { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="TracersSettings"/> class.
        /// </summary>
        /// <param name="constructorFailureTracerSettings">The settings for <see cref="ConstructorFailureTracer"/>.</param>
        /// <param name="geometryFailureTracerSettings">The settings for <see cref="GeometryFailureTracer"/>.</param>
        /// <param name="subtheoremDeriverGeometryFailureTracerSettings">The settings for <see cref="SubtheoremDeriverGeometryFailureTracer"/>.</param>
        public TracersSettings(ConstructorFailureTracerSettings constructorFailureTracerSettings,
                               GeometryFailureTracerSettings geometryFailureTracerSettings,
                               SubtheoremDeriverGeometryFailureTracerSettings subtheoremDeriverGeometryFailureTracerSettings)
        {
            ConstructorFailureTracerSettings = constructorFailureTracerSettings ?? throw new ArgumentNullException(nameof(constructorFailureTracerSettings));
            GeometryFailureTracerSettings = geometryFailureTracerSettings ?? throw new ArgumentNullException(nameof(geometryFailureTracerSettings));
            SubtheoremDeriverGeometryFailureTracerSettings = subtheoremDeriverGeometryFailureTracerSettings ?? throw new ArgumentNullException(nameof(subtheoremDeriverGeometryFailureTracerSettings));
        }

        #endregion
    }
}
