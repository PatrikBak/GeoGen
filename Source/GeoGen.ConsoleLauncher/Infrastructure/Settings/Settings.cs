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
        /// The settings for <see cref="SimplificationRuleProvider"/>.
        /// </summary>
        public SimplificationRuleProviderSettings SimplificationRuleProviderSettings { get; }

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
        public TracingSettings TracingSettings { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Settings"/> class.
        /// </summary>
        /// <param name="loggingSettings">The settings for the logging system.</param>
        /// <param name="inputFolderSettings">The settings for <see cref="AlgorithmInputProvider"/>.</param>
        /// <param name="inferenceRuleFolderSettings">The settings for <see cref="InferenceRuleProvider"/>.</param>
        /// <param name="simplificationRuleProviderSettings">The settings for <see cref="SimplificationRuleProvider"/>.</param>
        /// <param name="debugAlgorithmRunnerSettings">The settings for <see cref="DebugAlgorithmRunner"/>.</param>
        /// <param name="algorithmSettings">The settings related to the algorithm part of the application.</param>
        /// <param name="tracingSettings">The settings for all tracers used per batch.</param>
        public Settings(LoggingSettings loggingSettings,
                        InputFolderSettings inputFolderSettings,
                        InferenceRuleFolderSettings inferenceRuleFolderSettings,
                        SimplificationRuleProviderSettings simplificationRuleProviderSettings,
                        DebugAlgorithmRunnerSettings debugAlgorithmRunnerSettings,
                        AlgorithmSettings algorithmSettings,
                        TracingSettings tracingSettings)
        {
            LoggingSettings = loggingSettings ?? throw new ArgumentNullException(nameof(loggingSettings));
            InputFolderSettings = inputFolderSettings ?? throw new ArgumentNullException(nameof(inputFolderSettings));
            InferenceRuleFolderSettings = inferenceRuleFolderSettings ?? throw new ArgumentNullException(nameof(inferenceRuleFolderSettings));
            SimplificationRuleProviderSettings = simplificationRuleProviderSettings ?? throw new ArgumentNullException(nameof(simplificationRuleProviderSettings));
            DebugAlgorithmRunnerSettings = debugAlgorithmRunnerSettings ?? throw new ArgumentNullException(nameof(debugAlgorithmRunnerSettings));
            AlgorithmSettings = algorithmSettings ?? throw new ArgumentNullException(nameof(algorithmSettings));
            TracingSettings = tracingSettings ?? throw new ArgumentNullException(nameof(tracingSettings));
        }

        #endregion
    }
}