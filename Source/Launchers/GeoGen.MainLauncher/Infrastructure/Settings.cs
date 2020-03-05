using GeoGen.ConfigurationGenerator;
using GeoGen.Infrastructure;
using GeoGen.ProblemGenerator;
using GeoGen.TheoremFinder;
using GeoGen.TheoremRanker;
using GeoGen.TheoremSorter;
using System;

namespace GeoGen.MainLauncher
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
        /// The settings for <see cref="InferenceRuleProvider"/>.
        /// </summary>
        public InferenceRuleProviderSettings InferenceRuleProviderSettings { get; }

        /// <summary>
        /// The settings for <see cref="SimplificationRuleProvider"/>.
        /// </summary>
        public SimplificationRuleProviderSettings SimplificationRuleProviderSettings { get; }

        /// <summary>
        /// The settings for <see cref="ProblemGenerationRunner"/>.
        /// </summary>
        public ProblemGenerationRunnerSettings ProblemGenerationRunnerSettings { get; }

        /// <summary>
        /// The settings for <see cref="ProblemGenerator.ProblemGenerator"/>.
        /// </summary>
        public ProblemGeneratorSettings ProblemGeneratorSettings { get; }

        /// <summary>
        /// The settings for <see cref="TheoremSorter.TheoremSorter"/>.
        /// </summary>
        public TheoremSorterSettings TheoremSorterSettings { get; }

        /// <summary>
        /// The settings for the theorem finder module.
        /// </summary>
        public TheoremFindingSettings TheoremFindingSettings { get; }

        /// <summary>
        /// The settings for the theorem ranker module.
        /// </summary>
        public TheoremRankingSettings TheoremRankingSettings { get; }

        /// <summary>
        /// The settings for the generator module.
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

        /// <summary>
        /// Indicates whether tracing of invalid inferences is on.
        /// </summary>
        public bool TraceInvalidInferences { get; }

        /// <summary>
        /// The settings for <see cref="InvalidInferenceTracer"/>. This value can be null if we don't want to trace them.
        /// </summary>
        public InvalidInferenceTracerSettings InvalidInferenceTracerSettings { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Settings"/> class.
        /// </summary>
        /// <param name="loggingSettings">The settings for the logging system.</param>
        /// <param name="problemGeneratorInputProviderSettings">The settings for <see cref="ProblemGeneratorInputProvider"/>.</param>
        /// <param name="inferenceRuleProviderSettings">The settings for <see cref="InferenceRuleProvider"/>.</param>
        /// <param name="simplificationRuleProviderSettings">The settings for <see cref="SimplificationRuleProvider"/>.</param>
        /// <param name="problemGenerationRunnerSettings">The settings for <see cref="ProblemGenerationRunner"/>.</param>
        /// <param name="problemGeneratorSettings">The settings for <see cref="ProblemGenerator.ProblemGenerator"/>.</param>
        /// <param name="theoremSorterSettings">The settings for <see cref="TheoremSorter.TheoremSorter"/>.</param>
        /// <param name="theoremFindingSettings">The settings for the theorem finder module.</param>
        /// <param name="theoremRankingSettings">The settings for the theorem ranker module.</param>
        /// <param name="generationSettings">The settings for the generator module.</param>
        /// <param name="traceConstructorFailures">Indicates whether tracing of constructor failures is on.</param>
        /// <param name="constructorFailureTracerSettings">The settings for <see cref="ConstructorFailureTracer"/>. This value can be null if we don't want to trace them.</param>
        /// <param name="traceGeometryFailures">Indicates whether tracing of geometry failures is on.</param>
        /// <param name="geometryFailureTracerSettings">The settings for <see cref="GeometryFailureTracer"/>. This value can be null if we don't want to trace them.</param>
        /// <param name="traceInvalidInferences">Indicates whether tracing of invalid inferences is on.</param>
        /// <param name="invalidInferenceTracerSettings">The settings for <see cref="InvalidInferenceTracer"/>. This value can be null if we don't want to trace them.</param>
        public Settings(LoggingSettings loggingSettings,
                        ProblemGeneratorInputProviderSettings problemGeneratorInputProviderSettings,
                        InferenceRuleProviderSettings inferenceRuleProviderSettings,
                        SimplificationRuleProviderSettings simplificationRuleProviderSettings,
                        ProblemGenerationRunnerSettings problemGenerationRunnerSettings,
                        ProblemGeneratorSettings problemGeneratorSettings,
                        TheoremSorterSettings theoremSorterSettings,
                        TheoremFindingSettings theoremFindingSettings,
                        TheoremRankingSettings theoremRankingSettings,
                        GenerationSettings generationSettings,
                        bool traceConstructorFailures,
                        ConstructorFailureTracerSettings constructorFailureTracerSettings,
                        bool traceGeometryFailures,
                        GeometryFailureTracerSettings geometryFailureTracerSettings,
                        bool traceInvalidInferences,
                        InvalidInferenceTracerSettings invalidInferenceTracerSettings)

        {
            LoggingSettings = loggingSettings ?? throw new ArgumentNullException(nameof(loggingSettings));
            ProblemGeneratorInputProviderSettings = problemGeneratorInputProviderSettings ?? throw new ArgumentNullException(nameof(problemGeneratorInputProviderSettings));
            InferenceRuleProviderSettings = inferenceRuleProviderSettings ?? throw new ArgumentNullException(nameof(inferenceRuleProviderSettings));
            SimplificationRuleProviderSettings = simplificationRuleProviderSettings ?? throw new ArgumentNullException(nameof(simplificationRuleProviderSettings));
            ProblemGenerationRunnerSettings = problemGenerationRunnerSettings ?? throw new ArgumentNullException(nameof(problemGenerationRunnerSettings));
            ProblemGeneratorSettings = problemGeneratorSettings ?? throw new ArgumentNullException(nameof(problemGeneratorSettings));
            TheoremSorterSettings = theoremSorterSettings ?? throw new ArgumentNullException(nameof(theoremSorterSettings));
            TheoremFindingSettings = theoremFindingSettings ?? throw new ArgumentNullException(nameof(theoremFindingSettings));
            TheoremRankingSettings = theoremRankingSettings ?? throw new ArgumentNullException(nameof(theoremRankingSettings));
            GenerationSettings = generationSettings ?? throw new ArgumentNullException(nameof(generationSettings));
            TraceConstructorFailures = traceConstructorFailures;
            ConstructorFailureTracerSettings = constructorFailureTracerSettings;
            TraceGeometryFailures = traceGeometryFailures;
            GeometryFailureTracerSettings = geometryFailureTracerSettings;
            TraceInvalidInferences = traceInvalidInferences;
            InvalidInferenceTracerSettings = invalidInferenceTracerSettings;

            // Ensure that construction failure tracer settings are set if they are supposed to be traced
            if (TraceConstructorFailures && constructorFailureTracerSettings == null)
                throw new MainLauncherException("The construction failure tracer settings must be set as we are supposed to be tracing them.");

            // Ensure that geometry failure tracer settings are set if they are supposed to be traced
            if (TraceGeometryFailures && geometryFailureTracerSettings == null)
                throw new MainLauncherException("The geometry failure tracer settings must be set as we are supposed to be tracing them.");

            // Ensure that invalid inference tracer settings are set if they are supposed to be traced
            if (TraceInvalidInferences && invalidInferenceTracerSettings == null)
                throw new MainLauncherException("The invalid inference tracer settings must be set as we are supposed to be tracing them.");
        }

        #endregion
    }
}