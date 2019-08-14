using GeoGen.Constructor;
using System.Collections.Generic;
using System.IO;

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
            // The folder in which we're working by default
            var workingFolder = "..\\..\\..\\Data";

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
            PicturesManagerSettings = new PicturesSettings
            {
                NumberOfPictures = 5,
                MaximalAttemptsToReconstructOnePicture = 5,
                MaximalAttemptsToReconstructAllPictures = 5
            };

            // Create default folder settings
            InputFolderSettings = new InputFolderSettings
            {
                InputFolderPath = workingFolder,
                InputFilePrefix = "input",
                FilesExtention = "txt",
            };

            // Create default algorithm runner settings
            AlgorithmRunnerSettings = new AlgorithmRunnerSettings
            {
                OutputFolder = workingFolder,
                OutputFileExtention = "txt",
                OutputFilePrefix = "output",
                GenerationProgresLoggingFrequency = 100,
                LogProgress = true,
                GenerateFullReport = true,
                FullReportSuffix = " (full)"
            };

            // Create default template theorems folder settings
            TemplateTheoremsFolderSettings = new TemplateTheoremsFolderSettings
            {
                TheoremsFolderPath = Path.Combine(workingFolder, "TemplateTheorems"),
                FilesExtention = "txt"
            };
        }
    }
}