using GeoGen.Constructor;
using GeoGen.Core;
using GeoGen.TheoremFinder;
using GeoGen.TheoremRanker;
using System.Collections.Generic;

namespace GeoGen.ConsoleLauncher
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
            },
            algorithmSettings: new AlgorithmSettings
            (
                geometryConstructorSettings: new GeometryConstructorSettings(numberOfPictures: 5),
                theoremFindingSettings: new TheoremFindingSettings
                (
                    tangentCirclesTheoremFinderSettings: new TangentCirclesTheoremFinderSettings(excludeTangencyInsidePicture: true),
                    lineTangentToCircleTheoremFinderSettings: new LineTangentToCircleTheoremFinderSettings(excludeTangencyInsidePicture: true),
                    soughtTheoremTypes: new[]
                    {
                        TheoremType.CollinearPoints,
                        TheoremType.ConcurrentLines,
                        TheoremType.ConcyclicPoints,
                        TheoremType.EqualLineSegments,
                        TheoremType.LineTangentToCircle,
                        TheoremType.ParallelLines,
                        TheoremType.PerpendicularLines,
                        TheoremType.TangentCircles,
                        TheoremType.Incidence
                    }
                ),
                theoremRankingSettings: new TheoremRankingSettings
                (
                    theoremRankerSettings: new TheoremRankerSettings
                    (
                        rankingCoefficients: new Dictionary<RankedAspect, double>
                        {
                            { RankedAspect.Symmetry, 100},
                            { RankedAspect.Type, 10}
                        }
                    ),
                    typeRankerSettings: new TypeRankerSettings
                    (
                        typeRankings: new Dictionary<TheoremType, double>
                        {
                            { TheoremType.TangentCircles, 10 },
                            { TheoremType.LineTangentToCircle, 10 },
                            { TheoremType.ConcurrentLines, 4 },
                            { TheoremType.CollinearPoints, 2 },
                            { TheoremType.ConcyclicPoints, 2 },
                            { TheoremType.EqualLineSegments, 2 },
                            { TheoremType.ParallelLines, 2 },
                            { TheoremType.PerpendicularLines, 2 },
                            { TheoremType.Incidence, 1 }
                        }
                    )
                )
            ),
            inputFolderSettings: new InputFolderSettings
            (
                inputFolderPath: "..\\..\\..\\Data\\Inputs",
                inputFilePrefix: "input",
                filesExtention: "txt"
            ),
            templateTheoremsFolderSettings: new TemplateTheoremsFolderSettings
            (
                theoremsFolderPath: "..\\..\\..\\Data\\TemplateTheorems",
                filesExtention: "txt"
            ),
            simplificationRulesProviderSettings: new SimplificationRulesProviderSettings
            (
                filePath: "..\\..\\..\\Data\\simplifications.txt"
            ),
            algorithmRunnerSettings: new AlgorithmRunnerSettings
            (
                outputFolder: "..\\..\\..\\Data\\Outputs",
                outputWithAttemptsFolder: "..\\..\\..\\Data\\Outputs\\WithAttempts",
                writeOutputWithAttempts: true,
                outputWithAttemptsAndProofsFolder: "..\\..\\..\\Data\\Outputs\\WithAttemptsAndProofs",
                writeOutputWithAttemptsAndProofs: true,
                outputFilePrefix: "output",
                filesExtention: "txt",
                generationProgresLoggingFrequency: 15,
                logProgress: true,
                includeUnprovenDiscoveredTheorems: true
            ),
            tracersSettings: new TracersSettings
            (
                constructorFailureTracerSettings: new ConstructorFailureTracerSettings
                (
                    failuresFilePath: "..\\..\\..\\Data\\Tracing\\constructor_failures.txt",
                    logFailures: true
                ),
                geometryFailureTracerSettings: new GeometryFailureTracerSettings
                (
                    failuresFilePath: "..\\..\\..\\Data\\Tracing\\geometry_failures.txt",
                    logFailures: true
                ),
                subtheoremDeriverGeometryFailureTracerSettings: new SubtheoremDeriverGeometryFailureTracerSettings
                (
                    failuresFilePath: "..\\..\\..\\Data\\Tracing\\subtheorem_failures.txt",
                    logFailures: true
                )
            ),
            bestTheoremsTrackerSettings: new BestTheoremTrackerSettings(numberOfTheorems: 42),
            fileTheoremDataWriterSettings: new FileTheoremDataWriterSettings(theoremFilePath: "..\\..\\..\\Data\\best_theorems.txt")
        )
        { }
    }
}