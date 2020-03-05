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
        /// The settings for all tracers.
        /// </summary>
        public TracingSettings TracingSettings { get; }

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
        /// <param name="tracingSettings">The settings for all tracers.</param>
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
                        TracingSettings tracingSettings)
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
            TracingSettings = tracingSettings ?? throw new ArgumentNullException(nameof(tracingSettings));
        }

        #endregion
    }
}