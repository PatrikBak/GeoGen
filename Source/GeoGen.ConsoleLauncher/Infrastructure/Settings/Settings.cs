using System;
using System.Collections.Generic;

namespace GeoGen.ConsoleLauncher
{
    /// <summary>
    /// Represents the settings of the application.
    /// </summary>
    public class Settings
    {
        #region Public properties

        /// <summary>
        /// The list of the loggers settings.
        /// </summary>
        public IReadOnlyList<BaseLoggerSettings> Loggers { get; }

        /// <summary>
        /// The settings of the folder containing inputs.
        /// </summary>
        public InputFolderSettings InputFolderSettings { get; }

        /// <summary>
        /// The settings for the folder containing template theorems.
        /// </summary>
        public TemplateTheoremsFolderSettings TemplateTheoremsFolderSettings { get; }

        /// <summary>
        /// The settings for the algorithm runner.
        /// </summary>
        public AlgorithmRunnerSettings AlgorithmRunnerSettings { get; }

        /// <summary>
        /// The settings related to the algorithm.
        /// </summary>
        public AlgorithmSettings AlgorithmSettings { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Settings"/> class.
        /// </summary>
        /// <param name="loggers">The list of the loggers settings.</param>
        /// <param name="inputFolderSettings">The settings of the folder containing inputs.</param>
        /// <param name="templateTheoremsFolderSettings">The settings for the folder containing template theorems.</param>
        /// <param name="algorithmRunnerSettings">The settings for the algorithm runner.</param>
        /// <param name="algorithmSettings">The settings related to the algorithm.</param>
        public Settings(IReadOnlyList<BaseLoggerSettings> loggers,
                        InputFolderSettings inputFolderSettings,
                        TemplateTheoremsFolderSettings templateTheoremsFolderSettings,
                        AlgorithmRunnerSettings algorithmRunnerSettings,
                        AlgorithmSettings algorithmSettings)
        {
            Loggers = loggers ?? throw new ArgumentNullException(nameof(loggers));
            InputFolderSettings = inputFolderSettings ?? throw new ArgumentNullException(nameof(inputFolderSettings));
            TemplateTheoremsFolderSettings = templateTheoremsFolderSettings ?? throw new ArgumentNullException(nameof(templateTheoremsFolderSettings));
            AlgorithmRunnerSettings = algorithmRunnerSettings ?? throw new ArgumentNullException(nameof(algorithmRunnerSettings));
            AlgorithmSettings = algorithmSettings ?? throw new ArgumentNullException(nameof(algorithmSettings));
        }

        #endregion
    }
}
