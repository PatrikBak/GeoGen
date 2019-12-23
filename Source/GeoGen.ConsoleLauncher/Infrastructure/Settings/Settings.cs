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
        /// The settings of the folder containing inputs.
        /// </summary>
        public InputFolderSettings InputFolderSettings { get; }

        /// <summary>
        /// The settings for the folder containing template theorems.
        /// </summary>
        public TemplateTheoremsFolderSettings TemplateTheoremsFolderSettings { get; }

        /// <summary>
        /// The settings for the simplification rules provider.
        /// </summary>
        public SimplificationRulesProviderSettings SimplificationRulesProviderSettings { get; }

        /// <summary>
        /// The settings for the algorithm runner.
        /// </summary>
        public AlgorithmRunnerSettings AlgorithmRunnerSettings { get; }

        /// <summary>
        /// The settings related to the algorithm.
        /// </summary>
        public AlgorithmSettings AlgorithmSettings { get; }

        /// <summary>
        /// The settings for all tracers used in the batch runs.
        /// </summary>
        public TracersSettings TracersSettings { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Settings"/> class.
        /// </summary>
        /// <param name="loggingSettings">The settings for the logging system.</param>
        /// <param name="inputFolderSettings">The settings of the folder containing inputs.</param>
        /// <param name="templateTheoremsFolderSettings">The settings for the folder containing template theorems.</param>
        /// <param name="simplificationRulesProviderSettings">The settings for the simplification rules provider.</param>
        /// <param name="algorithmRunnerSettings">The settings for the algorithm runner.</param>
        /// <param name="algorithmSettings">The settings related to the algorithm.</param>
        /// <param name="tracersSettings">The settings for all tracers used in the batch runs.</param>
        public Settings(LoggingSettings loggingSettings,
                        InputFolderSettings inputFolderSettings,
                        TemplateTheoremsFolderSettings templateTheoremsFolderSettings,
                        SimplificationRulesProviderSettings simplificationRulesProviderSettings,
                        AlgorithmRunnerSettings algorithmRunnerSettings,
                        AlgorithmSettings algorithmSettings,
                        TracersSettings tracersSettings)
        {
            LoggingSettings = loggingSettings ?? throw new ArgumentNullException(nameof(loggingSettings));
            InputFolderSettings = inputFolderSettings ?? throw new ArgumentNullException(nameof(inputFolderSettings));
            TemplateTheoremsFolderSettings = templateTheoremsFolderSettings ?? throw new ArgumentNullException(nameof(templateTheoremsFolderSettings));
            SimplificationRulesProviderSettings = simplificationRulesProviderSettings ?? throw new ArgumentNullException(nameof(simplificationRulesProviderSettings));
            AlgorithmRunnerSettings = algorithmRunnerSettings ?? throw new ArgumentNullException(nameof(algorithmRunnerSettings));
            AlgorithmSettings = algorithmSettings ?? throw new ArgumentNullException(nameof(algorithmSettings));
            TracersSettings = tracersSettings ?? throw new ArgumentNullException(nameof(tracersSettings));
        }

        #endregion
    }
}
