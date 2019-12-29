using GeoGen.Algorithm;
using GeoGen.ConsoleLauncher;
using GeoGen.Infrastructure;
using System;

namespace GeoGen.GenerationLauncher
{
    /// <summary>
    /// Represents the settings of the application.
    /// </summary>
    public class Settings
    {
        #region Public properties

        /// <summary>
        /// The settings for the logging system.
        /// </summary>
        public LoggingSettings LoggingSettings { get; }

        /// <summary>
        /// The settings of the folder containing inputs.
        /// </summary>
        public InputFolderSettings InputFolderSettings { get; }

        /// <summary>
        /// The settings for the algorithm runner that runs the generation.
        /// </summary>
        public GenerationAlgorithmRunnerSettings GenerationAlgorithmRunnerSettings { get; }

        /// <summary>
        /// The settings for the algorithm facade.
        /// </summary>
        public AlgorithmFacadeSettings AlgorithmFacadeSettings { get; }

        /// <summary>
        /// The settings related to the generator module.
        /// </summary>
        public GenerationSettings GenerationSettings { get; }

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
        /// Initializes a new instance of the <see cref="Settings"/> class.
        /// </summary>
        /// <param name="loggingSettings">The settings for the logging system.</param>
        /// <param name="inputFolderSettings">The settings of the folder containing inputs.</param>
        /// <param name="generationAlgorithmRunnerSettings">The settings for the algorithm runner that runs the generation.</param>
        /// <param name="algorithmFacadeSettings">The settings for the algorithm facade.</param>
        /// <param name="generationSettings">The settings related to the generator module.</param>
        /// <param name="constructorFailureTracerSettings">The settings for <see cref="ConstructorFailureTracer"/>.</param>
        /// <param name="geometryFailureTracerSettings">The settings for <see cref="GeometryFailureTracer"/>. </param>
        public Settings(LoggingSettings loggingSettings,
                        InputFolderSettings inputFolderSettings,
                        GenerationAlgorithmRunnerSettings generationAlgorithmRunnerSettings,
                        AlgorithmFacadeSettings algorithmFacadeSettings,
                        GenerationSettings generationSettings,
                        ConstructorFailureTracerSettings constructorFailureTracerSettings,
                        GeometryFailureTracerSettings geometryFailureTracerSettings)
        {
            LoggingSettings = loggingSettings ?? throw new ArgumentNullException(nameof(loggingSettings));
            InputFolderSettings = inputFolderSettings ?? throw new ArgumentNullException(nameof(inputFolderSettings));
            GenerationAlgorithmRunnerSettings = generationAlgorithmRunnerSettings ?? throw new ArgumentNullException(nameof(generationAlgorithmRunnerSettings));
            AlgorithmFacadeSettings = algorithmFacadeSettings ?? throw new ArgumentNullException(nameof(algorithmFacadeSettings));
            GenerationSettings = generationSettings ?? throw new ArgumentNullException(nameof(generationSettings));
            ConstructorFailureTracerSettings = constructorFailureTracerSettings ?? throw new ArgumentNullException(nameof(constructorFailureTracerSettings));
            GeometryFailureTracerSettings = geometryFailureTracerSettings ?? throw new ArgumentNullException(nameof(geometryFailureTracerSettings));
        }

        #endregion
    }
}
