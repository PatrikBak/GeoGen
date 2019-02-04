using GeoGen.Analyzer;
using GeoGen.Core;
using GeoGen.Generator;
using GeoGen.Runner;
using GeoGen.Utilities;
using Ninject;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using static GeoGen.Core.PredefinedConstructionType;

namespace GeoGen.ConsoleTest
{
    public class Program
    {
        private static IKernel _kernel;

        private static ConsoleInconsistenciesTracker _inconsistencies;

        private static ConsoleInconstructibleObjectsTracer _inconstructibleObjects;

        private static ConsoleEqualObjectsTracer _equalObjects;

        private static void Bootstrap()
        {
            _kernel = new StandardKernel();

            _kernel.Bind<IGeneratorFactory>().To<GeneratorFactory>().WithConstructorArgument(new Action<IKernel>(internalKernel =>
            {
                internalKernel.Rebind<IInconsistentContainersTracer>().ToConstant(_inconsistencies = new ConsoleInconsistenciesTracker());
                internalKernel.Rebind<IEqualObjectsTracer>().ToConstant(_equalObjects = new ConsoleEqualObjectsTracer());
                internalKernel.Rebind<IInconstructibleObjectsTracer>().ToConstant(_inconstructibleObjects = new ConsoleInconstructibleObjectsTracer());
            }));
        }

        private static void Main()
        {
            // This makes sure that doubles in the VS debugger will be displayed with a decimal point
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            Bootstrap();

            var input = new GeneratorInput
            {
                InitialConfiguration = Configuration(),
                Constructions = Constructions(),
                NumberOfIterations = 1,
                NumberOfContainers = 8,
                MinimalNumberOfTrueContainers = 7,
                MinimalNumberOfTrueContainersToRevalidate = 1,
                MaximalNumberOfAttemptsToReconstruct = 100,
                MaximalAttemptsToReconstructOneContainer = 100,
                MaximalAttemptsToReconstructAllContainers = 1000
            };

            GenerateAndPrintResultsWithAtLeastOneTheorem(input, "output.txt", measureTime: true);
            _inconstructibleObjects.WriteReport("inconstructible_objects.txt");
            _equalObjects.WriteReport("equal_objects.txt");
        }

        private static Configuration Configuration()
        {
            var A = new LooseConfigurationObject(ConfigurationObjectType.Point);
            var B = new LooseConfigurationObject(ConfigurationObjectType.Point);
            var C = new LooseConfigurationObject(ConfigurationObjectType.Point);
            var H = new ConstructedConfigurationObject(ComposedConstructions.OrthocenterFromPoints(), A, B, C);
            var Hb = new ConstructedConfigurationObject(ComposedConstructions.ReflectionInLineFromPoints(), H, C, A);
            var Hc = new ConstructedConfigurationObject(ComposedConstructions.ReflectionInLineFromPoints(), H, A, B);

            return new Configuration(LooseObjectsLayout.ScaleneAcuteAngledTriangled, A, B, C, H, Hb, Hc);
        }

        private static List<Construction> Constructions()
        {
            return new List<Construction>
            {
                //ComposedConstructions.IncenterFromPoints(),
                //ComposedConstructions.Parallelogram(),
                //ComposedConstructions.ReflectionInLineFromPoints(),
                //ComposedConstructions.CentroidFromPoints(),
                //ComposedConstructions.OrthocenterFromPoints(),
                //ComposedConstructions.OrthocenterFromPoints(),
                //PredefinedConstructionsFactory.Get(IntersectionOfLinesFromPoints),
                //PredefinedConstructionsFactory.Get(IntersectionOfLinesFromLineAndPoints),
                //PredefinedConstructionsFactory.Get(IntersectionOfLines),
                //PredefinedConstructionsFactory.Get(MidpointFromPoints),
                //PredefinedConstructionsFactory.Get(PointReflection),
                //PredefinedConstructionsFactory.Get(CircumcenterFromPoints),
                //PredefinedConstructionsFactory.Get(PerpendicularLineFromPoints),
                //PredefinedConstructionsFactory.Get(InternalAngleBisectorFromPoints),
                //PredefinedConstructionsFactory.Get(SecondIntersectionOfCircleFromPointsAndLineFromPoints),
                PredefinedConstructionsFactory.Get(SecondIntersectionOfTwoCirclesFromPoints)
            };
        }

        private static void GenerateAndPrintResultsWithAtLeastOneTheorem(GeneratorInput input, string fileName, bool measureTime = false)
        {
            GenerateAndPrintResults(input, fileName, output => output.AnalyzerOutput.Theorems.Count != 0, measureTime);
        }

        private static void GenerateAndPrintResults(GeneratorInput input, string fileName, Func<GeneratorOutput, bool> condition, bool measureTime)
        {
            using (var writer = new StreamWriter(fileName))
            {
                writer.WriteLine($"Initial configuration: Scalene Acute Triangle");
                writer.WriteLine($"Iterations: {input.NumberOfIterations}");
                writer.WriteLine($"Pictures per configuration: {input.NumberOfContainers}");
                writer.WriteLine($"Number of pictures where a theorem must hold: {input.MinimalNumberOfTrueContainers}");
                writer.WriteLine($"Number of pictures where a theorem must hold before revalidation: {input.MinimalNumberOfTrueContainersToRevalidate}");
                writer.WriteLine();
                writer.WriteLine($"Constructions:");
                writer.WriteLine();
                input.Constructions.ForEach(construction => writer.WriteLine($" - {construction}"));
                writer.WriteLine();

                var result = _kernel.Get<IGeneratorFactory>().CreateGenerator(input).Generate();

                if (measureTime)
                {
                    var stopwatch = new Stopwatch();
                    stopwatch.Start();
                    var list = result.ToList();
                    result = list;
                    stopwatch.Stop();

                    writer.WriteLine($"Elapsed milliseconds: {stopwatch.ElapsedMilliseconds}");
                    writer.WriteLine($"Generated configuration: {list.Count}");
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

                foreach (var generatorOutput in result)
                {
                    if (!condition(generatorOutput))
                        continue;

                    var formatter = new OutputFormatter(generatorOutput.Configuration);

                    writer.WriteLine("------------------------------------------------");
                    writer.WriteLine($"{i++}. (id={generatorOutput.Configuration.Id})");
                    writer.WriteLine("------------------------------------------------\n");
                    writer.WriteLine(formatter.FormatConfiguration());

                    var theorems = generatorOutput.AnalyzerOutput.Theorems;
                    if (theorems.Count != 0)
                    {
                        writer.WriteLine("\nTheorems:\n");
                        writer.WriteLine(formatter.FormatTheorems(theorems));
                    }

                    var possibleFalseNegatives = generatorOutput.AnalyzerOutput.PotentialFalseNegatives;
                    if (possibleFalseNegatives.Count != 0)
                    {
                        writer.WriteLine("\nPossible false negatives:\n");
                        writer.WriteLine(formatter.FormatTheorems(generatorOutput.AnalyzerOutput.PotentialFalseNegatives));
                    }
                    writer.WriteLine();
                }
            }
        }
    }
}