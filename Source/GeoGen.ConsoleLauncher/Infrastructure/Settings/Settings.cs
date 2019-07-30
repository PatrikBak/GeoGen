using GeoGen.Constructor;
using System.Collections.Generic;

namespace GeoGen.ConsoleLauncher
{
    /// <summary>
    /// Represents the settings of the application.
    /// </summary>
    public class Settings
    {
        /// <summary>
        /// The list of the loggers settings.
        /// </summary>
        public List<BaseLoggerSettings> Loggers { get; set; }

        /// <summary>
        /// The settings for the <see cref="PicturesManager"/> used in the algorithm.
        /// </summary>
        public PicturesManagerSettings PicturesManagerSettings { get; set; }

        /// <summary>
        /// The settings of the folder containing inputs.
        /// </summary>
        public InputFolderSettings InputFolderSettings { get; set; }

        /// <summary>
        /// The settings for the algorithm runner.
        /// </summary>
        public AlgorithmRunnerSettings AlgorithmRunnerSettings { get; set; }

        /// <summary>
        /// The settings for the folder containing template theorems.
        /// </summary>
        public TemplateTheoremsFolderSettings TemplateTheoremsFolderSettings { get; set; }
    }
}
