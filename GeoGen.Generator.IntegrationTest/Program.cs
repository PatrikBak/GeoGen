using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using GeoGen.AnalyticalGeometry;
using GeoGen.Analyzer;
using GeoGen.Core;
using GeoGen.Utilities;
using Ninject;
using Ninject.Planning.Bindings.Resolvers;
using static GeoGen.Core.PredefinedConstructionType;

namespace GeoGen.Generator.IntegrationTest
{
    public class Program
    {
        private static ConstructionsContainer _constructionsContainer;

        private static ComposedConstructions _composedConstructions;

        private static ConstructorHelper _constructorHelper;

        private static void Main()
        {
            //while (true)
            //{
            _constructionsContainer = new ConstructionsContainer();
            _composedConstructions = new ComposedConstructions(_constructionsContainer);
            _constructorHelper = new ConstructorHelper(_constructionsContainer);

            var kernel = new StandardKernel
            (
                new GeneratorModule(),
                new UtilitiesModule(),
                new AnalyerModule(),
                new AnalyticalGeometryModule()
            );

            kernel.Components.RemoveAll<IMissingBindingResolver>();
            kernel.Settings.AllowNullInjection = true;
            var tracker = new ConsoleInconsistenciesTracker();
            kernel.Bind<IInconsistenciesTracker>().ToConstant(tracker);

            //kernel.Rebind<ITheoremsAnalyzer>().ToConstant(new DummyTheoremsAnalyzer());
            //kernel.Rebind<IGeometryRegistrar>().ToConstant(new DummyGeometryRegistrar());

            var factory = kernel.Get<IGeneratorFactory>();

            var points = Enumerable.Range(0, 3)
                    .Select(i => new LooseConfigurationObject(ConfigurationObjectType.Point))
                    .ToList();

            var constructedObjects = ConstructedObjects(points);

            var configuration = new Configuration(points, constructedObjects, LooseObjectsLayout.ScaleneAcuteAngledTriangled);
            var constructions = Constructions();

            var input = new GeneratorInput
            {
                InitialConfiguration = configuration,
                Constructions = constructions,
                MaximalNumberOfIterations = 3
            };

            var generator = factory.CreateGenerator(input);
            var stopwatch = new Stopwatch();

            stopwatch.Start();
            var result = generator.Generate()
                    .ToList();
            stopwatch.Stop();

            //var st = result.Sum(output => output.Theorems.Count);
            //Console.WriteLine(st);
            ////if(theoremCounts.Add(st)) Console.WriteLine(st);
            ////if(configurationsCount.Add(result.Count)) Console.WriteLine(result.Count);

            Console.WriteLine($"First elapsed: {stopwatch.ElapsedMilliseconds}");
            Console.WriteLine($"Generated: {result.Count}");
            Console.WriteLine($"Generated with theorems: {result.Count(r => r.Theorems.Any())}");
            Console.WriteLine($"Total number of theorems: {result.Sum(output => output.Theorems.Count)}");
            Console.WriteLine($"Inconsistencies: {tracker.Inconsistencies}");
            Console.WriteLine($"Failed attempts to reconstruct: {tracker.AttemptsToReconstruct}");
            Console.WriteLine($"-------------------------------------------------");
            Console.ReadKey();

            PrintTheorems(result);

            //Console.WriteLine("Starting the second attempt..");
            //stopwatch.Start();
            //var otherResult = factory.CreateGenerator(input).Generate().ToList();
            //stopwatch.Stop();
            //Console.WriteLine($"Second elapsed: {stopwatch.ElapsedMilliseconds}");

            //var formatter = new OutputFormatter(_constructionsContainer);

            //var counter = 0;

            //for (var i = 0; i < result.Count; i++)
            //{
            //    var first = result[i];
            //    var second = otherResult[i];

            //    string CastTheorem(Theorem theorem)
            //    {
            //        return formatter.ConvertToString(theorem);
            //    }

            //    var c1 = formatter.Format(first.Configuration);
            //    var c2 = formatter.Format(second.Configuration);

            //    var firstSet = first.Theorems.Select(CastTheorem).ToSet();

            //    var secondSet = second.Theorems.Select(CastTheorem).ToSet();

            //    var set = Differences(firstSet, secondSet);

            //    if (set.Empty())
            //        continue;

            //    Console.WriteLine($"{++counter}. In configuration: ");
            //    Console.WriteLine("-------------------\n");
            //    Console.WriteLine(formatter.Format(first.Configuration));
            //    Console.WriteLine("-------------------\n");
            //    Console.WriteLine("Theorems generated exactly in one of two runs:");
            //    Console.WriteLine("-------------------\n");

            //    foreach (var s in set)
            //    {
            //        Console.WriteLine(s);
            //    }

            //    Console.WriteLine();
            //    Console.ReadKey();
            //}
            //}
        }

        private static HashSet<string> Differences(HashSet<string> firstSet, HashSet<string> secondSet)
        {
            var all = firstSet.Union(secondSet).ToList();

            var result = new HashSet<string>();

            foreach (var s in all)
            {
                if (firstSet.Contains(s) && secondSet.Contains(s))
                    continue;
                
                result.Add(s);
            }

            return result;
        }

        private static List<string> ConvertTheormes(List<GeneratorOutput> result)
        {
            var formatter = new OutputFormatter(_constructionsContainer);

            var i = 0;

            return result.SelectMany(r =>
            {
                var configurationString = formatter.Format(r.Configuration);

                var theoremsStrings = r.Theorems.Select(formatter.ConvertToString).ToList();

                theoremsStrings.Sort();

                return theoremsStrings.Select(s => $"{i++}---{configurationString}---{s}");
            }).ToList();
        }

        private static List<Construction> Constructions()
        {
            return new List<Construction>
            {
                //_composedConstructions.AddIncenterFromPoints(),
                //_constructionsContainer.Get(IntersectionOfLinesFromPoints),
                //_constructionsContainer.Get(IntersectionOfLinesFromLineAndPoints),
                //_constructionsContainer.Get(IntersectionOfLines),
                _constructionsContainer.Get(MidpointFromPoints),
                _constructionsContainer.Get(CircumcenterFromPoints),
                //_constructionsContainer.Get(PerpendicularLineFromPoints),
                //_constructionsContainer.Get(InternalAngleBisectorFromPoints),
                //_composedConstructions.AddIncenterFromPoints()
                //_constructionsContainer.Get(IntersectionOfLines),
                //_constructionsContainer.Get(IntersectionOfLinesFromLineAndPoints),
                //_constructionsContainer.Get(PerpendicularLineFromPoints),
                //_constructionsContainer.Get(InternalAngleBisectorFromPoints)
            };
        }

        private static List<ConstructedConfigurationObject> ConstructedObjects(List<LooseConfigurationObject> points)
        {
            _composedConstructions.AddIncenterFromPoints();

            //var o = _constructorHelper.CreateCircumcenter(points[0], points[1], points[2]);
            //var i = _constructorHelper.CreateIncenter(points[0], points[1], points[2]);
            //var p = _constructorHelper.CreateIntersection(o, i, points[0], points[1]);

            return new List<ConstructedConfigurationObject>
            {
              //  o
            };
        }

        private static void PrintTheorems(IEnumerable<GeneratorOutput> result)
        {
            var formatter = new OutputFormatter(_constructionsContainer);

            //Console.ReadKey(true);
            Console.WriteLine("Results:\n");

            var i = 1;

            foreach (var generatorOutput in result.Where(t => t.Theorems.Any()))
            {
                //i++;
                //if (i == 71)
                {
                    Console.Clear();
                    Console.WriteLine($"{i++}.");
                    Console.WriteLine("-------------------\n");
                    Console.WriteLine(formatter.Format(generatorOutput.Configuration));
                    Console.WriteLine("-------------------\n");
                    Console.WriteLine("Theorems:");
                    Console.WriteLine(formatter.Format(generatorOutput.Theorems));
                    Console.ReadKey(true);
                }
            }
        }
    }
}