using GeoGen.Algorithm;
using GeoGen.Core;
using GeoGen.Generator;
using GeoGen.Infrastructure;
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
            loggingSettings: new LoggingSettings
            (
                loggers: new List<BaseLoggerSettings>
                {
                    new ConsoleLoggerSettings
                    (
                        includeLoggingOrigin: false,
                        includeTime: false,
                        logOutputLevel: LogOutputLevel.Info
                    ),

                    new FileLoggerSettings
                    (
                        includeLoggingOrigin: true,
                        includeTime: true,
                        logOutputLevel: LogOutputLevel.Debug,
                        fileLogPath: "log.txt"
                    )
                }
            ),
            algorithmSettings: new AlgorithmSettings
            (
                algorithmFacadeSettings: new AlgorithmFacadeSettings
                (
                    numberOfPictures: 5,
                    excludeAsymmetricConfigurations: false
                ),
                bestTheoremsFinderSettings: new BestTheoremsFinderSettings
                (
                    numberOfTheorems: 42
                ),
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
                            { RankedAspect.Symmetry, 200},
                            { RankedAspect.Type, 10},
                            { RankedAspect.TheoremsPerObject, 3},
                            { RankedAspect.CirclesPerObject, 1}
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
                ),
                generationSettings: new GenerationSettings
                (
                    configurationFilterType: ConfigurationFilterType.Fast
                )
            ),
            inputFolderSettings: new InputFolderSettings
            (
                inputFolderPath: "..\\..\\..\\Data\\Inputs",
                inputFilePrefix: "input",
                fileExtension: "txt"
            ),
            inferenceRuleFolderSettings: new InferenceRuleFolderSettings
            (
                ruleFolderPath: "..\\..\\..\\Data\\InferenceRules",
                fileExtension: "txt"
            ),
            simplificationRulesProviderSettings: new SimplificationRulesProviderSettings
            (
                filePath: "..\\..\\..\\Data\\simplifications.txt"
            ),
            debugAlgorithmRunnerSettings: new DebugAlgorithmRunnerSettings
            (
                outputFolder: "..\\..\\..\\Data\\Outputs\\Readable",
                outputFolderWithProofs: "..\\..\\..\\Data\\Outputs\\WithAttempts",
                writeOutputWithProofs: true,
                outputJsonFolder: "..\\..\\..\\Data\\Outputs\\Json",
                outputFilePrefix: "output",
                fileExtension: "txt",
                generationProgresLoggingFrequency: 15,
                logProgress: true,
                bestTheoremsReadableFilePath: "..\\..\\..\\Data\\best_theorems.txt",
                bestTheoremsJsonFilePath: "..\\..\\..\\Data\\best_theorems.json",
                inferenceRuleUsageFile: "..\\..\\..\\Data\\rule_usages.json"
            ),
            tracersSettings: new TracersSettings
            (
                traceConstructorFailures: true,
                constructorFailureTracerSettings: new ConstructorFailureTracerSettings
                (
                    failuresFilePath: "..\\..\\..\\Data\\Tracing\\constructor_failures.txt",
                    logFailures: true
                ),
                traceGeometryFailures: true,
                geometryFailureTracerSettings: new GeometryFailureTracerSettings
                (
                    failuresFilePath: "..\\..\\..\\Data\\Tracing\\geometry_failures.txt",
                    logFailures: true
                )
            )
        )
        { }
    }
}