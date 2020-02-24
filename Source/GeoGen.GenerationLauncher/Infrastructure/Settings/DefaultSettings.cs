using GeoGen.Algorithm;
using GeoGen.ConsoleLauncher;
using GeoGen.Generator;
using GeoGen.Infrastructure;
using System.Collections.Generic;

namespace GeoGen.GenerationLauncher
{
    /// <summary>
    /// The default settings for the application.
    /// </summary>
    public class DefaultSettings : Settings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultSettings"/> class.
        /// </summary>
        public DefaultSettings() : base
        (
            loggingSettings: new LoggingSettings
            (
                loggers: new List<BaseLoggerSettings>
                {
                    // Console logger
                    new ConsoleLoggerSettings
                    (
                        includeLoggingOrigin: false,
                        includeTime: false,
                        logOutputLevel: LogOutputLevel.Info
                    ),

                    // File logger
                    new FileLoggerSettings
                    (
                        includeLoggingOrigin: true,
                        includeTime: true,
                        logOutputLevel: LogOutputLevel.Debug,
                        fileLogPath: "log.txt"
                    )
                }
            ),
            inputFolderSettings: new InputFolderSettings
            (
                inputFolderPath: "..\\..\\..\\Data\\Inputs",
                inputFilePrefix: "input",
                fileExtension: "txt"
            ),
            generationAlgorithmRunnerSettings: new GenerationAlgorithmRunnerSettings
            (
                generationProgresLoggingFrequency: 100,
                logProgress: true
            ),
            algorithmFacadeSettings: new AlgorithmFacadeSettings
            (
                numberOfPictures: 5,
                excludeAsymmetricConfigurations: false
            ),
            generationSettings: new GenerationSettings
            (
                configurationFilterType: ConfigurationFilterType.Fast
            ),
            constructorFailureTracerSettings: new ConstructorFailureTracerSettings
            (
                failureFilePath: "..\\..\\..\\Data\\Tracing\\constructor_failures.txt",
                logFailures: true
            ),
            geometryFailureTracerSettings: new GeometryFailureTracerSettings
            (
                failureFilePath: "..\\..\\..\\Data\\Tracing\\geometry_failures.txt",
                logFailures: true
            )
        )
        { }
    }
}