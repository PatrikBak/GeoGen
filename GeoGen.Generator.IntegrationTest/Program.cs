using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using GeoGen.AnalyticalGeometry.Ninject;
using GeoGen.Analyzer;
using GeoGen.Analyzer.Drawing;
using GeoGen.Analyzer.NInject;
using GeoGen.Core.Configurations;
using GeoGen.Core.Constructions;
using GeoGen.Core.Constructions.Arguments;
using GeoGen.Core.Constructions.PredefinedConstructions;
using GeoGen.Core.Generator;
using GeoGen.Core.NInject;
using GeoGen.Utilities;
using Ninject;
using Ninject.Planning.Bindings.Resolvers;

namespace GeoGen.Generator.IntegrationTest
{
    public class Program
    {
        private static Midpoint _midpoint;

        private static Intersection _intersection;

        private static void Main()
        {
            _midpoint = new Midpoint {Id = 0};
            _intersection = new Intersection {Id = 1};

            var kernel = new StandardKernel
            (
                new GeneratorModule(),
                new CoreModule(),
                new AnalyerModule(),
                new AnalyticalGeometryModule()
            );

            kernel.Components.RemoveAll<IMissingBindingResolver>();

            //kernel.Rebind<IGradualAnalyzer>().ToConstant(new DummyGradualAnalyzer());
            //kernel.Rebind<IGeometryRegistrar>().ToConstant(new DummyGeometryRegistrar());

            var factory = kernel.Get<IGeneratorFactory>();

            var points = Enumerable.Range(0, 3)
                                   .Select(i => new LooseConfigurationObject(ConfigurationObjectType.Point))
                                   .ToList();

            // Ok, let's make some midpoints
            var m1 = CreateMidpoint(points[0], points[1]);
            var m2 = CreateMidpoint(points[1], points[2]);
            var m3 = CreateMidpoint(points[2], points[0]);

            // And intersection
            var g = CreateIntersection(points[0], m2, points[1], m3);

            //var constructedObjects = new List<ConstructedConfigurationObject>();
            var constructedObjects = new List<ConstructedConfigurationObject> {m1, m2, m3};


            var configuration = new Configuration(points.ToSet(), constructedObjects);
            var constructions = new List<Construction> {_midpoint, _intersection};

            var input = new GeneratorInput
            {
                InitialConfiguration = configuration,
                Constructions = constructions,
                MaximalNumberOfIterations = 2
            };

            var generator = factory.CreateGenerator(input);
            var stopwatch = new Stopwatch();

            stopwatch.Start();
            var result = generator.Generate().ToList();
            stopwatch.Stop();

            Console.WriteLine($"Elapsed: {stopwatch.ElapsedMilliseconds}");
            Console.WriteLine($"Generated: {result.Count}");

            //PrintTheorems(result);
        }

        private static ConstructedConfigurationObject CreateIntersection(params ConfigurationObject[] objects)
        {
            var argument = new SetConstructionArgument(new HashSet<ConstructionArgument>
            {
                new SetConstructionArgument(new HashSet<ConstructionArgument>
                {
                    new ObjectConstructionArgument(objects[0]),
                    new ObjectConstructionArgument(objects[1])
                }),
                new SetConstructionArgument(new HashSet<ConstructionArgument>
                {
                    new ObjectConstructionArgument(objects[2]),
                    new ObjectConstructionArgument(objects[3])
                })
            });

            var argumentsList = new List<ConstructionArgument> {argument};

            return new ConstructedConfigurationObject(_intersection, argumentsList, 0);
        }

        private static ConstructedConfigurationObject CreateMidpoint(ConfigurationObject o1, ConfigurationObject o2)
        {
            var argument = new SetConstructionArgument(new HashSet<ConstructionArgument>
            {
                new ObjectConstructionArgument(o1),
                new ObjectConstructionArgument(o2)
            });

            var argumentsList = new List<ConstructionArgument> {argument};

            return new ConstructedConfigurationObject(_midpoint, argumentsList, 0);
        }

        private static void PrintTheorems(List<GeneratorOutput> result)
        {
            var formatter = new OutputFormatter();

            Console.WriteLine("Results:\n");

            var i = 1;

            foreach (var generatorOutput in result)
            {
                Console.Clear();
                Console.WriteLine($"{i++}.");
                Console.WriteLine("-------------------\n");
                Console.WriteLine(formatter.Format(generatorOutput.Configuration));
                Console.WriteLine("-------------------\n");
                Console.WriteLine("Theorems:");
                Console.WriteLine(formatter.Format(generatorOutput.Theorems));
                //Console.ReadKey(true);
            }

            //var s = $"{string.Join(", ", result.Select(o => o.Theorems.Count))}";

            //if (s != "1, 3, 8, 2, 4, 4, 2, 3, 2, 1, 1, 2, 1, 1")
            //{
            //    Console.WriteLine("Still inconsistency..");
            //    Console.WriteLine("Old s: 1, 3, 8, 2, 4, 4, 2, 3, 2, 1, 1, 2, 1, 1");
            //    Console.WriteLine($"New s: {s}");
            //}
        }
    }
}