using GeoGen.Algorithm;
using GeoGen.Generator;
using GeoGen.Infrastructure;
using GeoGen.TheoremFinder;
using GeoGen.TheoremRanker;
using System;

namespace GeoGen.ConsoleLauncher
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
        /// The settings for <see cref="AlgorithmInputProvider"/>.
        /// </summary>
        public AlgorithmInputProviderSettings AlgorithmInputProviderSettings { get; }

        /// <summary>
        /// The settings for <see cref="InferenceRuleProvider"/>.
        /// </summary>
        public InferenceRuleProviderSettings InferenceRuleProviderSettings { get; }

        /// <summary>
        /// The settings for <see cref="SimplificationRuleProvider"/>.
        /// </summary>
        public SimplificationRuleProviderSettings SimplificationRuleProviderSettings { get; }

        /// <summary>
        /// The settings for <see cref="DebugAlgorithmRunner"/>.
        /// </summary>
        public DebugAlgorithmRunnerSettings DebugAlgorithmRunnerSettings { get; }

        /// <summary>
        /// The settings for <see cref="Algorithm.Algorithm"/>.
        /// </summary>
        public AlgorithmSettings AlgorithmSettings { get; }

        /// <summary>
        /// The settings for <see cref="BestTheoremFinder"/>.
        /// </summary>
        public BestTheoremFinderSettings BestTheoremFinderSettings { get; }

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
        /// <param name="algorithmInputProviderSettings">The settings for <see cref="AlgorithmInputProvider"/>.</param>
        /// <param name="inferenceRuleProviderSettings">The settings for <see cref="InferenceRuleProvider"/>.</param>
        /// <param name="simplificationRuleProviderSettings">The settings for <see cref="SimplificationRuleProvider"/>.</param>
        /// <param name="debugAlgorithmRunnerSettings">The settings for <see cref="DebugAlgorithmRunner"/>.</param>
        /// <param name="algorithmSettings">The settings for <see cref="Algorithm.Algorithm"/>.</param>
        /// <param name="bestTheoremFinderSettings">The settings for <see cref="BestTheoremFinder"/>.</param>
        /// <param name="theoremFindingSettings">The settings for the theorem finder module.</param>
        /// <param name="theoremRankingSettings">The settings for the theorem ranker module.</param>
        /// <param name="generationSettings">The settings for the generator module.</param>
        /// <param name="tracingSettings">The settings for all tracers.</param>
        public Settings(LoggingSettings loggingSettings,
                        AlgorithmInputProviderSettings algorithmInputProviderSettings,
                        InferenceRuleProviderSettings inferenceRuleProviderSettings,
                        SimplificationRuleProviderSettings simplificationRuleProviderSettings,
                        DebugAlgorithmRunnerSettings debugAlgorithmRunnerSettings,
                        AlgorithmSettings algorithmSettings,
                        BestTheoremFinderSettings bestTheoremFinderSettings,
                        TheoremFindingSettings theoremFindingSettings,
                        TheoremRankingSettings theoremRankingSettings,
                        GenerationSettings generationSettings,
                        TracingSettings tracingSettings)
        {
            LoggingSettings = loggingSettings ?? throw new ArgumentNullException(nameof(loggingSettings));
            AlgorithmInputProviderSettings = algorithmInputProviderSettings ?? throw new ArgumentNullException(nameof(algorithmInputProviderSettings));
            InferenceRuleProviderSettings = inferenceRuleProviderSettings ?? throw new ArgumentNullException(nameof(inferenceRuleProviderSettings));
            SimplificationRuleProviderSettings = simplificationRuleProviderSettings ?? throw new ArgumentNullException(nameof(simplificationRuleProviderSettings));
            DebugAlgorithmRunnerSettings = debugAlgorithmRunnerSettings ?? throw new ArgumentNullException(nameof(debugAlgorithmRunnerSettings));
            AlgorithmSettings = algorithmSettings ?? throw new ArgumentNullException(nameof(algorithmSettings));
            BestTheoremFinderSettings = bestTheoremFinderSettings ?? throw new ArgumentNullException(nameof(bestTheoremFinderSettings));
            TheoremFindingSettings = theoremFindingSettings ?? throw new ArgumentNullException(nameof(theoremFindingSettings));
            TheoremRankingSettings = theoremRankingSettings ?? throw new ArgumentNullException(nameof(theoremRankingSettings));
            GenerationSettings = generationSettings ?? throw new ArgumentNullException(nameof(generationSettings));
            TracingSettings = tracingSettings ?? throw new ArgumentNullException(nameof(tracingSettings));
        }

        #endregion
    }
}