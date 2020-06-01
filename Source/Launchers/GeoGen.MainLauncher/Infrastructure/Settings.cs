using GeoGen.ConfigurationGenerator;
using GeoGen.Infrastructure;
using GeoGen.ProblemGenerator;
using GeoGen.ProblemGenerator.InputProvider;
using GeoGen.TheoremFinder;
using GeoGen.TheoremProver.InferenceRuleProvider;
using GeoGen.TheoremProver.ObjectIntroductionRuleProvider;
using GeoGen.TheoremRanker;
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
        /// The settings for <see cref="ObjectIntroductionRuleProvider"/>.
        /// </summary>
        public ObjectIntroductionRuleProviderSettings ObjectIntroductionRuleProviderSettings { get; }

        /// <summary>
        /// The settings for <see cref="ProblemGenerationRunner"/>.
        /// </summary>
        public ProblemGenerationRunnerSettings ProblemGenerationRunnerSettings { get; }

        /// <summary>
        /// The settings for <see cref="TheoremSorterTypeResolver"/>.
        /// </summary>
        public TheoremSorterTypeResolverSettings TheoremSorterTypeResolverSettings { get; }

        /// <summary>
        /// The settings for <see cref="ProblemGenerator.ProblemGenerator"/>.
        /// </summary>
        public ProblemGeneratorSettings ProblemGeneratorSettings { get; }

        /// <summary>
        /// The settings for the theorem finder module.
        /// </summary>
        public TheoremFindingSettings TheoremFindingSettings { get; }

        /// <summary>
        /// The settings for <see cref="TheoremRanker.TheoremRanker"/>.
        /// </summary>
        public TheoremRankerSettings TheoremRankerSettings { get; }

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

        /// <summary>
        /// Indicates whether tracing of sorting geometry failures is on.
        /// </summary>
        public bool TraceSortingGeometryFailures { get; }

        /// <summary>
        /// The settings for <see cref="SortingGeometryFailureTracer"/>. This value can be null if we don't want to trace them.
        /// </summary>
        public SortingGeometryFailureTracerSettings SortingGeometryFailureTracerSettings { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Settings"/> class.
        /// </summary>
        /// <param name="loggingSettings">The settings for the logging system.</param>
        /// <param name="problemGeneratorInputProviderSettings">The settings for <see cref="ProblemGeneratorInputProvider"/>.</param>
        /// <param name="inferenceRuleProviderSettings">The settings for <see cref="InferenceRuleProvider"/>.</param>
        /// <param name="objectIntroductionRuleProviderSettings">The settings for <see cref="ObjectIntroductionRuleProvider"/>.</param>
        /// <param name="problemGenerationRunnerSettings">The settings for <see cref="ProblemGenerationRunner"/>.</param>
        /// <param name="theoremSorterTypeResolverSettings">The settings for <see cref="TheoremSorterTypeResolver"/>.</param>
        /// <param name="problemGeneratorSettings">The settings for <see cref="ProblemGenerator.ProblemGenerator"/>.</param>
        /// <param name="theoremFindingSettings">The settings for the theorem finder module.</param>
        /// <param name="theoremRankerSettings">The settings for <see cref="TheoremRanker.TheoremRanker"/>.</param>
        /// <param name="generationSettings">The settings for the generator module.</param>
        /// <param name="traceConstructorFailures">Indicates whether tracing of constructor failures is on.</param>
        /// <param name="constructorFailureTracerSettings">The settings for <see cref="ConstructorFailureTracer"/>. This value can be null if we don't want to trace them.</param>
        /// <param name="traceGeometryFailures">Indicates whether tracing of geometry failures is on.</param>
        /// <param name="geometryFailureTracerSettings">The settings for <see cref="GeometryFailureTracer"/>. This value can be null if we don't want to trace them.</param>
        /// <param name="traceInvalidInferences">Indicates whether tracing of invalid inferences is on.</param>
        /// <param name="invalidInferenceTracerSettings">The settings for <see cref="InvalidInferenceTracer"/>. This value can be null if we don't want to trace them.</param>
        /// <param name="traceSortingGeometryFailures">Indicates whether tracing of sorting geometry failures is on.</param>
        /// <param name="sortingGeometryFailureTracerSettings">The settings for <see cref="SortingGeometryFailureTracer"/>. This value can be null if we don't want to trace them.</param>
        public Settings(LoggingSettings loggingSettings,
                        ProblemGeneratorInputProviderSettings problemGeneratorInputProviderSettings,
                        InferenceRuleProviderSettings inferenceRuleProviderSettings,
                        ObjectIntroductionRuleProviderSettings objectIntroductionRuleProviderSettings,
                        ProblemGenerationRunnerSettings problemGenerationRunnerSettings,
                        TheoremSorterTypeResolverSettings theoremSorterTypeResolverSettings,
                        ProblemGeneratorSettings problemGeneratorSettings,
                        TheoremFindingSettings theoremFindingSettings,
                        TheoremRankerSettings theoremRankerSettings,
                        GenerationSettings generationSettings,
                        bool traceConstructorFailures,
                        ConstructorFailureTracerSettings constructorFailureTracerSettings,
                        bool traceGeometryFailures,
                        GeometryFailureTracerSettings geometryFailureTracerSettings,
                        bool traceInvalidInferences,
                        InvalidInferenceTracerSettings invalidInferenceTracerSettings,
                        bool traceSortingGeometryFailures,
                        SortingGeometryFailureTracerSettings sortingGeometryFailureTracerSettings)

        {
            LoggingSettings = loggingSettings ?? throw new ArgumentNullException(nameof(loggingSettings));
            ProblemGeneratorInputProviderSettings = problemGeneratorInputProviderSettings ?? throw new ArgumentNullException(nameof(problemGeneratorInputProviderSettings));
            InferenceRuleProviderSettings = inferenceRuleProviderSettings ?? throw new ArgumentNullException(nameof(inferenceRuleProviderSettings));
            ObjectIntroductionRuleProviderSettings = objectIntroductionRuleProviderSettings ?? throw new ArgumentNullException(nameof(objectIntroductionRuleProviderSettings));
            ProblemGenerationRunnerSettings = problemGenerationRunnerSettings ?? throw new ArgumentNullException(nameof(problemGenerationRunnerSettings));
            TheoremSorterTypeResolverSettings = theoremSorterTypeResolverSettings ?? throw new ArgumentNullException(nameof(theoremSorterTypeResolverSettings));
            ProblemGeneratorSettings = problemGeneratorSettings ?? throw new ArgumentNullException(nameof(problemGeneratorSettings));
            TheoremFindingSettings = theoremFindingSettings ?? throw new ArgumentNullException(nameof(theoremFindingSettings));
            TheoremRankerSettings = theoremRankerSettings ?? throw new ArgumentNullException(nameof(theoremRankerSettings));
            GenerationSettings = generationSettings ?? throw new ArgumentNullException(nameof(generationSettings));
            TraceConstructorFailures = traceConstructorFailures;
            ConstructorFailureTracerSettings = constructorFailureTracerSettings;
            TraceGeometryFailures = traceGeometryFailures;
            GeometryFailureTracerSettings = geometryFailureTracerSettings;
            TraceInvalidInferences = traceInvalidInferences;
            InvalidInferenceTracerSettings = invalidInferenceTracerSettings;
            TraceSortingGeometryFailures = traceSortingGeometryFailures;
            SortingGeometryFailureTracerSettings = sortingGeometryFailureTracerSettings;

            // Ensure that construction failure tracer settings are set if they are supposed to be traced
            if (TraceConstructorFailures && constructorFailureTracerSettings == null)
                throw new MainLauncherException("The construction failure tracer settings must be set as we are supposed to be tracing them.");

            // Ensure that geometry failure tracer settings are set if they are supposed to be traced
            if (TraceGeometryFailures && geometryFailureTracerSettings == null)
                throw new MainLauncherException("The geometry failure tracer settings must be set as we are supposed to be tracing them.");

            // Ensure that invalid inference tracer settings are set if they are supposed to be traced
            if (TraceInvalidInferences && invalidInferenceTracerSettings == null)
                throw new MainLauncherException("The invalid inference tracer settings must be set as we are supposed to be tracing them.");

            // Ensure that sorting geometry failure tracer settings are set if they are supposed to be traced
            if (TraceSortingGeometryFailures && sortingGeometryFailureTracerSettings == null)
                throw new MainLauncherException("The sorting geometry failure tracer settings must be set as we are supposed to be tracing them.");
        }

        #endregion
    }
}