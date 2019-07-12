using GeoGen.TheoremsFinder;
using GeoGen.Constructor;
using GeoGen.Core;
using GeoGen.Generator;
using GeoGen.Utilities;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;

namespace GeoGen.ConsoleTest
{
    /// <summary>
    /// The entry class of the application.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The entry point of the application.
        /// </summary>
        public static void Main()
        {
            // This makes sure that doubles in the VS debugger will be displayed with a decimal point
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            // Prepare IoC
            IoC.Bootstrap();

            // Prepare theorem analyzer settings
            var theoremAnalysisSettings = new TheoremAnalysisSettings
            {
                MinimalNumberOfTruePictures = 5,
                MinimalNumberOfTruePicturesToRevalidate = 5,
            };

            // Prepare contextual picture settings
            var contextualPictureSettings = new ContextualPictureSettings
            {
                MaximalNumberOfAttemptsToReconstruct = 5,
            };

            // Prepare pictures manager settings
            var picturesManagerSettings = new PicturesManagerSettings
            {
                NumberOfPictures = 5,
                MaximalAttemptsToReconstructOnePicture = 5,
                MaximalAttemptsToReconstructAllPictures = 5
            };

            // Prepare input
            var input = new GeneratorInput
            {
                InitialConfiguration = InitialConfiguration(),
                Constructions = Constructions(),
                NumberOfIterations = 1,
            };

            var result = IoC.Get<IAlgorithm>(theoremAnalysisSettings, contextualPictureSettings, picturesManagerSettings).Execute(input);

            // Perform the algorithm
            GenerateAndPrintResults(input, theoremAnalysisSettings, contextualPictureSettings, picturesManagerSettings, "output.txt");
        }

        private static Configuration InitialConfiguration()
        {
            var A = new LooseConfigurationObject(ConfigurationObjectType.Point);
            var B = new LooseConfigurationObject(ConfigurationObjectType.Point);
            var C = new LooseConfigurationObject(ConfigurationObjectType.Point);

            return Configuration.DeriveFromObjects(LooseObjectsLayout.ScaleneAcuteAngledTriangled, A, B, C);
        }

        private static List<Construction> Constructions() => new List<Construction>
        {
            PredefinedConstructions.CenterOfCircle,
            PredefinedConstructions.CircleWithCenterThroughPoint,
            PredefinedConstructions.Circumcircle,
            PredefinedConstructions.InternalAngleBisector,
            PredefinedConstructions.IntersectionOfLines,
            PredefinedConstructions.LineFromPoints,
            PredefinedConstructions.Midpoint,
            PredefinedConstructions.PerpendicularLine,
            PredefinedConstructions.PerpendicularProjection,
            PredefinedConstructions.PointReflection,
            PredefinedConstructions.SecondIntersectionOfCircleAndLineFromPoints,
            PredefinedConstructions.SecondIntersectionOfCircleWithCenterAndLineFromPoints,
            PredefinedConstructions.SecondIntersectionOfTwoCircumcircles,
            ComposedConstructions.Centroid,
            ComposedConstructions.Incenter,
            ComposedConstructions.Circumcenter,
            ComposedConstructions.PerpendicularProjectionOnLineFromPoints,
            ComposedConstructions.Orthocenter,
            ComposedConstructions.IntersectionOfLinesFromPoints,
            ComposedConstructions.IntersectionOfLineAndLineFromPoints,
            ComposedConstructions.Parallelogram,
            ComposedConstructions.PerpendicularLineAtPointOfLine,
            ComposedConstructions.PerpendicularLineToLineFromPoints,
            ComposedConstructions.ReflectionInLine,
            ComposedConstructions.ReflectionInLineFromPoints
        };

        private static void GenerateAndPrintResults(GeneratorInput input,
                                                    TheoremAnalysisSettings theoremAnalysisSettings,
                                                    ContextualPictureSettings contextualPictureSettings,
                                                    PicturesManagerSettings picturesManagerSettings,
                                                    string fileName,
                                                    bool measureTime = false,
                                                    bool analyzeInitialTheorems = true,
                                                    bool skipConfigurationsWithoutTheormems = true)
        {

            using (var writer = new StreamWriter(fileName))
            {
                var initialConfigurationCopy = InitialConfiguration();
                var initialFormatter = new OutputFormatter(initialConfigurationCopy);

                writer.WriteLine("Initial configuration:");
                writer.WriteLine("------------------------------------------------");
                writer.WriteLine(initialFormatter.FormatConfiguration());

                void FormatOutput(OutputFormatter formatter, TheoremAnalysisOutput output)
                {
                    var theorems = output.Theorems;
                    if (theorems.Count != 0)
                    {
                        writer.WriteLine("\nTheorems:\n");
                        writer.WriteLine(formatter.FormatTheorems(theorems));
                    }

                    var possibleFalseNegatives = output.PotentialFalseNegatives;
                    if (possibleFalseNegatives.Count != 0)
                    {
                        writer.WriteLine("\nPossible false negatives:\n");
                        writer.WriteLine(formatter.FormatTheorems(possibleFalseNegatives));
                    }
                }

                if (analyzeInitialTheorems)
                {
                    var initialOutput = IoC.Get<SimpleCompleteTheoremAnalyzer>(theoremAnalysisSettings, contextualPictureSettings, picturesManagerSettings).Analyze(initialConfigurationCopy);

                    writer.WriteLine();
                    FormatOutput(initialFormatter, initialOutput);
                    writer.WriteLine("------------------------------------------------");
                    writer.WriteLine();

                    writer.WriteLine($"Iterations: {input.NumberOfIterations}");
                    writer.WriteLine($"Pictures per configuration: {picturesManagerSettings.NumberOfPictures}");
                    writer.WriteLine($"Number of pictures where a theorem must hold: {theoremAnalysisSettings.MinimalNumberOfTruePictures}");
                    writer.WriteLine($"Number of pictures where a theorem must hold before revalidation: {theoremAnalysisSettings.MinimalNumberOfTruePicturesToRevalidate}");
                    writer.WriteLine();
                    writer.WriteLine($"Constructions:");
                    writer.WriteLine();
                    input.Constructions.ForEach(construction => writer.WriteLine($" - {construction}"));
                    writer.WriteLine();
                }

                var result = IoC.Get<IAlgorithm>(theoremAnalysisSettings, contextualPictureSettings, picturesManagerSettings).Execute(input);

                if (measureTime)
                {
                    var stopwatch = new Stopwatch();
                    stopwatch.Start();
                    var list = result.ToList();
                    result = list;
                    stopwatch.Stop();

                    var generatedConfiguratonsString = list.GroupBy(output => output.GeneratorOutput.IterationIndex)
                                                           .Select(grouping => $"[{grouping.Key}: {grouping.Count()}]")
                                                           .ToJoinedString();

                    writer.WriteLine($"Elapsed milliseconds: {stopwatch.ElapsedMilliseconds}");
                    writer.WriteLine($"Configurations generated on particular iterations: {generatedConfiguratonsString}");
                    writer.WriteLine($"Total number of generated configuration: {list.Count}");
                    writer.WriteLine($"Generated configuration with theorems: {list.Count(r => r.AnalyzerOutput.Theorems.Any())}");
                    writer.WriteLine($"Total number of theorems: {list.Sum(output => output.AnalyzerOutput.Theorems.Count)}");
                    writer.WriteLine();
                }

                writer.WriteLine($"Results:");
                writer.WriteLine();
                writer.WriteLine("[8] means the theorem was true in this 8 pictures");
                writer.WriteLine("[7,8] means the theorem was initially true in 7 pictures, and after revalidation in 8 pictures");
                writer.WriteLine();

                var i = 1;

                foreach (var algorithmOutput in result)
                {
                    if (skipConfigurationsWithoutTheormems && algorithmOutput.AnalyzerOutput.Theorems.Count == 0)
                        continue;

                    var formatter = new OutputFormatter(algorithmOutput.GeneratorOutput.Configuration);

                    writer.WriteLine("------------------------------------------------");
                    writer.WriteLine($"{i++}");
                    writer.WriteLine("------------------------------------------------\n");
                    writer.WriteLine(formatter.FormatConfiguration());

                    FormatOutput(formatter, algorithmOutput.AnalyzerOutput);
                    writer.WriteLine();
                }
            }
        }
    }
}