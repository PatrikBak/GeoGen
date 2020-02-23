using GeoGen.Infrastructure;
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
        public InputFolderSettings InputFolderSettings { get; }

        /// <summary>
        /// The settings for <see cref="InferenceRuleProvider"/>.
        /// </summary>
        public InferenceRuleFolderSettings InferenceRuleFolderSettings { get; }

        /// <summary>
        /// The settings for <see cref="SimplificationRulesProvider"/>.
        /// </summary>
        public SimplificationRulesProviderSettings SimplificationRulesProviderSettings { get; }

        /// <summary>
        /// The settings for <see cref="DebugAlgorithmRunner"/>.
        /// </summary>
        public DebugAlgorithmRunnerSettings DebugAlgorithmRunnerSettings { get; }

        /// <summary>
        /// The settings related to the algorithm part of the application.
        /// </summary>
        public AlgorithmSettings AlgorithmSettings { get; }

        /// <summary>
        /// The settings for all tracers used per batch.
        /// </summary>
        public TracersSettings TracersSettings { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Settings"/> class.
        /// </summary>
        /// <param name="loggingSettings">The settings for the logging system.</param>
        /// <param name="inputFolderSettings">The settings for <see cref="AlgorithmInputProvider"/>.</param>
        /// <param name="inferenceRuleFolderSettings">The settings for <see cref="InferenceRuleProvider"/>.</param>
        /// <param name="simplificationRulesProviderSettings">The settings for <see cref="SimplificationRulesProvider"/>.</param>
        /// <param name="debugAlgorithmRunnerSettings">The settings for <see cref="DebugAlgorithmRunner"/>.</param>
        /// <param name="algorithmSettings">The settings related to the algorithm part of the application.</param>
        /// <param name="tracersSettings">The settings for all tracers used per batch.</param>
        public Settings(LoggingSettings loggingSettings,
                        InputFolderSettings inputFolderSettings,
                        InferenceRuleFolderSettings inferenceRuleFolderSettings,
                        SimplificationRulesProviderSettings simplificationRulesProviderSettings,
                        DebugAlgorithmRunnerSettings debugAlgorithmRunnerSettings,
                        AlgorithmSettings algorithmSettings,
                        TracersSettings tracersSettings)
        {
            LoggingSettings = loggingSettings ?? throw new ArgumentNullException(nameof(loggingSettings));
            InputFolderSettings = inputFolderSettings ?? throw new ArgumentNullException(nameof(inputFolderSettings));
            InferenceRuleFolderSettings = inferenceRuleFolderSettings ?? throw new ArgumentNullException(nameof(inferenceRuleFolderSettings));
            SimplificationRulesProviderSettings = simplificationRulesProviderSettings ?? throw new ArgumentNullException(nameof(simplificationRulesProviderSettings));
            DebugAlgorithmRunnerSettings = debugAlgorithmRunnerSettings ?? throw new ArgumentNullException(nameof(debugAlgorithmRunnerSettings));
            AlgorithmSettings = algorithmSettings ?? throw new ArgumentNullException(nameof(algorithmSettings));
            TracersSettings = tracersSettings ?? throw new ArgumentNullException(nameof(tracersSettings));
        }

        #endregion
    }
}