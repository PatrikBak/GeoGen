using GeoGen.Analyzer;
using GeoGen.Core;
using GeoGen.Generator;
using GeoGen.Runner;
using Ninject;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using static GeoGen.Core.PredefinedConstructionType;

namespace GeoGen.ConsoleTest
{
    public class Program
    {
        private static IKernel _kernel;

        private static Stopwatch _stopwatch = new Stopwatch();

        private static void Bootstrap()
        {
            _kernel = new StandardKernel();

            _kernel.Bind<IGeneratorFactory>().To<GeneratorFactory>().WithConstructorArgument(new Action<IKernel>(internalKernel =>
            {
                internalKernel.Rebind<IInconsistentContainersTracer>().ToConstant(new ConsoleInconsistenciesTracker());
                internalKernel.Rebind<IEqualObjectsTracer>().ToConstant(new ConsoleEqualObjectsTracer());
                internalKernel.Rebind<IInconstructibleObjectsTracer>().ToConstant(new ConsoleInconstructibleObjectsTracer());
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
                    MaximalNumberOfIterations = 3,
                    NumberOfContainers = 15,
                    MinimalNumberOfTrueContainers = 14,
                    MinimalNumberOfTrueContainersToRevalidate = 2,
                    MaximalNumberOfAttemptsToReconstructContextualContainer = 100,
                    MaximalAttemptsToReconstructOneContainer = 100,
                    MaximalAttemptsToReconstructAllContainers = 1000
                };

                _stopwatch = new Stopwatch();
                _stopwatch.Start();
                var result = _kernel.Get<IGeneratorFactory>().CreateGenerator(input).Generate().ToList();
                _stopwatch.Stop();

                PrintResult(result);
        }

        private static List<Construction> Constructions()
        {
            return new List<Construction>
            {
                //ComposedConstructions.IncenterFromPoints(),
                //ComposedConstructions.CentroidFromPoints(),
                //PredefinedConstructionsFactory.Get(IntersectionOfLinesFromPoints),
                //PredefinedConstructionsFactory.Get(IntersectionOfLinesFromLineAndPoints),
                //PredefinedConstructionsFactory.Get(IntersectionOfLines),
                PredefinedConstructionsFactory.Get(MidpointFromPoints),
                PredefinedConstructionsFactory.Get(CircumcenterFromPoints),
                PredefinedConstructionsFactory.Get(PerpendicularLineFromPoints),
                PredefinedConstructionsFactory.Get(InternalAngleBisectorFromPoints),
            };
        }

        private static void PrintResult(List<GeneratorOutput> result)
        {
            Console.WriteLine($"Elapsed: {_stopwatch.ElapsedMilliseconds}");
            Console.WriteLine($"Generated: {result.Count}");
            Console.WriteLine($"Generated with theorems: {result.Count(r => r.AnalyzerOutput.Theorems.Any())}");
            Console.WriteLine($"Total number of theorems: {result.Sum(output => output.AnalyzerOutput.Theorems.Count)}");
            Console.WriteLine($"Unsuccessful configurations: {result.Count(r => !r.AnalyzerOutput.TheoremAnalysisSuccessful)}");
            Console.WriteLine();
            Console.ReadKey();
            
            Console.WriteLine("Results:\n");
            
            var i = 1;
            
            foreach (var generatorOutput in result.Where(t => t.AnalyzerOutput.Theorems.Any()))
            {
                var formatter = new OutputFormatter(generatorOutput.Configuration);
            
                Console.Clear();
                Console.WriteLine($"{i++}.");
                Console.WriteLine("-------------------\n");
                Console.WriteLine(formatter.FormatConfiguration());
                Console.WriteLine("-------------------\n");
                Console.WriteLine("Theorems:");
                Console.WriteLine(formatter.FormatTheorems(generatorOutput.AnalyzerOutput.Theorems));
                Console.ReadKey(true);
            }
        }
    }
}