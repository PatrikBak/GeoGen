using GeoGen.AnalyticGeometry;
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
        public static int Containers = 15;

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
            var inconsistenciesHandler = new TheoremInconsistenciesHandler();

            //for (var i = 0; i < 150; i++)            //{

            var points = Enumerable.Range(0, 3)
                    .Select(_ => new LooseConfigurationObject(ConfigurationObjectType.Point))
                    .ToList();

            var configuration = new Configuration(points, new List<ConstructedConfigurationObject>(), LooseObjectsLayout.ScaleneAcuteAngledTriangled);
            var constructions = Constructions();

            var input = new GeneratorInput
            {
                InitialConfiguration = configuration,
                Constructions = constructions,
                MaximalNumberOfIterations = 4,
                NumberOfContainers = Containers,
                MaximalAttemptsToReconstructOneContainer = 100,
                MaximalAttemptsToReconstructAllContainers = 1000
            };

            var generator = _kernel.Get<IGeneratorFactory>().CreateGenerator(input);

            _stopwatch.Restart();
            var result = generator.Generate();
            _stopwatch.Stop();

            //Console.WriteLine($"{(i+1)} took {_stopwatch.ElapsedMilliseconds}");
            Console.SetOut(new StreamWriter("weird.txt", append: false) { AutoFlush = true });

            var i = 0;

            result.ForEach(output =>
            {
                i++;
                if (i % 10000 == 0)
                    Console.WriteLine(i);
                //var inconsistencies = inconsistenciesHandler.AddAndReturnInconsistencies(output);
                //
                //if (inconsistencies.Count == 0)
                //    return;
                //
                //var formatter = new OutputFormatter(output.Configuration);
                //Console.WriteLine("-------------------\n");
                //Console.WriteLine(formatter.FormatConfiguration());
                //Console.WriteLine();
                //Console.WriteLine("Inconsistent theorems:");
                //Console.WriteLine();
                //Console.WriteLine(string.Join(Environment.NewLine, inconsistencies));
                //Console.ReadKey(true);
                //Console.Clear();


                var list = output.AnalyzerOutput.AfterRetesting;

                if (list.Count == 0)
                    return;

                
                var formatter = new OutputFormatter(output.Configuration);

                Console.WriteLine();
                Console.WriteLine(formatter.FormatConfiguration());
                Console.WriteLine("-------------------\n");
                Console.WriteLine("Theorems:\n");
                list.ForEach(pair =>
                {
                    Console.WriteLine($"{pair.Item2} {formatter.ConvertTheoremToString(pair.Item1)}");
                });
                Console.WriteLine();

                //Console.ReadKey(true);
                //Console.Clear();
            });
            //}

            //PrintResult(result);
        }

        private static List<Construction> Constructions()
        {
            return new List<Construction>
            {
                ComposedConstructions.IncenterFromPoints(),
                ComposedConstructions.CentroidFromPoints(),
                PredefinedConstructionsFactory.Get(IntersectionOfLinesFromPoints),
                PredefinedConstructionsFactory.Get(IntersectionOfLinesFromLineAndPoints),
                PredefinedConstructionsFactory.Get(IntersectionOfLines),
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
            Console.WriteLine($"Generated with theorems: {result.Count(r => r.Theorems.Any())}");
            Console.WriteLine($"Total number of theorems: {result.Sum(output => output.Theorems.Count)}");
            Console.ReadKey();

            Console.WriteLine("Results:\n");

            var i = 1;

            foreach (var generatorOutput in result.Where(t => t.Theorems.Any()))
            {
                var formatter = new OutputFormatter(generatorOutput.Configuration);

                Console.Clear();
                Console.WriteLine($"{i++}.");
                Console.WriteLine("-------------------\n");
                Console.WriteLine(formatter.FormatConfiguration());
                Console.WriteLine("-------------------\n");
                Console.WriteLine("Theorems:");
                Console.WriteLine(formatter.FormatTheorems(generatorOutput.Theorems));
                Console.ReadKey(true);
            }
        }
    }
}