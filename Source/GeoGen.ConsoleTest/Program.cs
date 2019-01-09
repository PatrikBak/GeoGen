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

        private static Stopwatch _stopwatch = new Stopwatch();

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

            var points = Enumerable.Range(0, 3)
                    .Select(_ => new LooseConfigurationObject(ConfigurationObjectType.Point))
                    .ToList();

            var configuration = new Configuration(points, new List<ConstructedConfigurationObject>(), LooseObjectsLayout.ScaleneAcuteAngledTriangled);
            var constructions = Constructions();

            var input = new GeneratorInput
            {
                InitialConfiguration = configuration,
                Constructions = constructions,
                NumberOfIterations = 3,
                NumberOfContainers = 8,
                MinimalNumberOfTrueContainers = 7,
                MinimalNumberOfTrueContainersToRevalidate = 2,
                MaximalNumberOfAttemptsToReconstruct = 100,
                MaximalAttemptsToReconstructOneContainer = 100,
                MaximalAttemptsToReconstructAllContainers = 1000
            };

            _stopwatch = new Stopwatch();
            _stopwatch.Start();
            var result = _kernel.Get<IGeneratorFactory>().CreateGenerator(input).Generate().ToList();
            _stopwatch.Stop();

            PrintResult(input, result, new StreamWriter("output.txt") { AutoFlush = true });
            _inconstructibleObjects.WriteReport(new StreamWriter("inconstructible_objects.txt") { AutoFlush = true });
            _equalObjects.WriteReport(new StreamWriter("equal_objects.txt") { AutoFlush = true });
        }

        private static List<Construction> Constructions()
        {
            return new List<Construction>
            {
                //ComposedConstructions.IncenterFromPoints(),
                //ComposedConstructions.CentroidFromPoints(),
                PredefinedConstructionsFactory.Get(IntersectionOfLinesFromPoints),
                PredefinedConstructionsFactory.Get(IntersectionOfLinesFromLineAndPoints),
                PredefinedConstructionsFactory.Get(IntersectionOfLines),
                PredefinedConstructionsFactory.Get(MidpointFromPoints),
                PredefinedConstructionsFactory.Get(CircumcenterFromPoints),
                PredefinedConstructionsFactory.Get(PerpendicularLineFromPoints),
                PredefinedConstructionsFactory.Get(InternalAngleBisectorFromPoints),
            };
        }

        private static void PrintResult(GeneratorInput input, List<GeneratorOutput> result, TextWriter writer)
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
            writer.WriteLine($"Elapsed milliseconds: {_stopwatch.ElapsedMilliseconds}");
            writer.WriteLine($"Generated configuration: {result.Count}");
            writer.WriteLine($"Generated configuration with theorems: {result.Count(r => r.AnalyzerOutput.Theorems.Any())}");
            writer.WriteLine($"Total number of theorems: {result.Sum(output => output.AnalyzerOutput.Theorems.Count)}");
            writer.WriteLine();
            writer.WriteLine($"Results:");
            writer.WriteLine();
            writer.WriteLine("[8] means the theorem was true in this 8 pictures");
            writer.WriteLine("[7,8] means the theorem was initially true in 7 pictures, and after revalidation in 8 pictures");
            writer.WriteLine();

            var i = 1;

            foreach (var generatorOutput in result)
            {
                if (generatorOutput.AnalyzerOutput.Theorems.Count == 0)
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