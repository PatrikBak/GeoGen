using GeoGen.Constructor;
using System.Collections.Generic;

namespace GeoGen.ConsoleLauncher
{
    /// <summary>
    /// A default settings for the application.
    /// </summary>
    public class DefaultSettings : Settings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultSettings"/> class.
        /// </summary>
        public DefaultSettings()
        {
            // Create the loggers
            Loggers = new List<BaseLoggerSettings>
            {
                // Console logger
                new ConsoleLoggerSettings
                {
                    IncludeLoggingOrigin = false,
                    IncludeTime = true,
                    LogOutputLevel = LogOutputLevel.Debug
                },

                // File logger
                new FileLoggerSettings
                {
                    IncludeLoggingOrigin = true,
                    IncludeTime = false,
                    LogOutputLevel = LogOutputLevel.Debug,
                    FileLogPath = "log.txt"
                }
            };

            // Create default manager settings
            PicturesManagerSettings = new PicturesManagerSettings
            {
                NumberOfPictures = 5,
                MaximalAttemptsToReconstructOnePicture = 5,
                MaximalAttemptsToReconstructAllPictures = 5
            };

            // Create default folder settings
            FolderSettings = new FolderSettings
            {
                WorkingFolder = "Data",
                TemplateTheoremsFolder = "TemplateTheorems",
                FilesExtention = "txt",
                InputFilePrefix = "input",
                OutputFilePrefix = "output"
            };

            // Create default algorithm runner settings
            AlgorithmRunnerSettings = new AlgorithmRunnerSettings
            {
                GenerationProgresLoggingFrequency = 100,
                LogProgress = true
            };
        }
    }
}
