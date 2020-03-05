using GeoGen.ConfigurationGenerator;
using GeoGen.Infrastructure;
using GeoGen.MainLauncher;
using GeoGen.ProblemGenerator;
using System;

namespace GeoGen.ConfigurationGenerationLauncher
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
        /// The settings for <see cref="ProblemGeneratorInputProvider"/>.
        /// </summary>
        public ProblemGeneratorInputProviderSettings ProblemGeneratorInputProviderSettings { get; }

        /// <summary>
        /// The settings for <see cref="GenerationOnlyProblemGenerationRunner"/>.
        /// </summary>
        public GenerationOnlyProblemGenerationRunnerSettings GenerationOnlyProblemGenerationRunnerSettings { get; }

        /// <summary>
        /// The settings for <see cref="ProblemGenerator.ProblemGenerator"/>.
        /// </summary>
        public ProblemGeneratorSettings ProblemGeneratorSettings { get; }

        /// <summary>
        /// The settings related to the generator module.
        /// </summary>
        public GenerationSettings GenerationSettings { get; }

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
        /// Initializes a new instance of the <see cref="Settings"/> class.
        /// </summary>
        /// <param name="loggingSettings">The settings for the logging system.</param>
        /// <param name="problemGeneratorInputProviderSettings">The settings for <see cref="ProblemGeneratorInputProvider"/>.</param>
        /// <param name="generationOnlyProblemGenerationRunnerSettings">The settings for <see cref="GenerationOnlyProblemGenerationRunner"/>.</param>
        /// <param name="problemGeneratorSettings">The settings for <see cref="ProblemGenerator.ProblemGenerator"/>.</param>
        /// <param name="generationSettings">The settings related to the generator module.</param>
        /// <param name="traceConstructorFailures">Indicates whether tracing of constructor failures is on.</param>
        /// <param name="constructorFailureTracerSettings">The settings for <see cref="ConstructorFailureTracer"/>. This value can be null if we don't want to trace them.</param>
        /// <param name="traceGeometryFailures">Indicates whether tracing of geometry failures is on.</param>
        /// <param name="geometryFailureTracerSettings">The settings for <see cref="GeometryFailureTracer"/>. This value can be null if we don't want to trace them.</param>
        public Settings(LoggingSettings loggingSettings,
                        ProblemGeneratorInputProviderSettings problemGeneratorInputProviderSettings,
                        GenerationOnlyProblemGenerationRunnerSettings generationOnlyProblemGenerationRunnerSettings,
                        ProblemGeneratorSettings problemGeneratorSettings,
                        GenerationSettings generationSettings,
                        bool traceConstructorFailures,
                        ConstructorFailureTracerSettings constructorFailureTracerSettings,
                        bool traceGeometryFailures,
                        GeometryFailureTracerSettings geometryFailureTracerSettings)
        {
            LoggingSettings = loggingSettings ?? throw new ArgumentNullException(nameof(loggingSettings));
            ProblemGeneratorInputProviderSettings = problemGeneratorInputProviderSettings ?? throw new ArgumentNullException(nameof(problemGeneratorInputProviderSettings));
            GenerationOnlyProblemGenerationRunnerSettings = generationOnlyProblemGenerationRunnerSettings ?? throw new ArgumentNullException(nameof(generationOnlyProblemGenerationRunnerSettings));
            ProblemGeneratorSettings = problemGeneratorSettings ?? throw new ArgumentNullException(nameof(problemGeneratorSettings));
            GenerationSettings = generationSettings ?? throw new ArgumentNullException(nameof(generationSettings));
            TraceConstructorFailures = traceConstructorFailures;
            ConstructorFailureTracerSettings = constructorFailureTracerSettings;
            TraceGeometryFailures = traceGeometryFailures;
            GeometryFailureTracerSettings = geometryFailureTracerSettings;

            // Ensure that construction failure settings are set if they are supposed to be traced
            if (TraceConstructorFailures && constructorFailureTracerSettings == null)
                throw new MainLauncherException("The construction failure tracer settings must be set as we are supposed to be tracing them.");

            // Ensure that geometry failure settings are set if they are supposed to be traced
            if (TraceGeometryFailures && geometryFailureTracerSettings == null)
                throw new MainLauncherException("The geometry failure tracer settings must be set as we are supposed to be tracing them.");
        }

        #endregion
    }
}
