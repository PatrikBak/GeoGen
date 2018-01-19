using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using GeoGen.AnalyticalGeometry;
using GeoGen.Analyzer;
using GeoGen.Core;
using GeoGen.Utilities;
using Ninject;
using Ninject.Planning.Bindings.Resolvers;

namespace GeoGen.Generator.IntegrationTest
{
    public class Program
    {
        private static ConstructionsContainer _constructionsContainer;

        private static ComposedConstructions _composedConstructions;

        private static ConstructorHelper _constructorHelper;

        private static void Main()
        {
            var hash1 = HashCodeUtilities.GetOrderDependentHashCode((IEnumerable)(new List<int> { 1, 2, 3 }));
            var hash2 = HashCodeUtilities.GetOrderDependentHashCode(1, 2, 3);

            Console.WriteLine(hash1 == hash2);

            //var objects = new List<LooseConfigurationObject>
            //{
            //    new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 1},
            //    new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 2},
            //    new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 3}
            //};

            //var argument = new SetConstructionArgument(new HashSet<ConstructionArgument>
            //{
            //    new SetConstructionArgument(new HashSet<ConstructionArgument>
            //    {
            //        new ObjectConstructionArgument(objects[1]),
            //        new ObjectConstructionArgument(objects[0])
            //    }),
            //    new SetConstructionArgument(new HashSet<ConstructionArgument>
            //    {
            //        new ObjectConstructionArgument(objects[3]),
            //        new ObjectConstructionArgument(objects[2])
            //    })
            //});

            //var sb = new StringBuilder();

            //void Action(ConstructionArgument o)
            //{
            //    if (o is ObjectConstructionArgument)
            //    {

            //    }
            //}

            //var t = new Tree
            //{
            //    Left = new Tree
            //    {
            //        Left = new Tree
            //        {
            //            Left = null,
            //            Right = new Tree()

            //        },
            //        Right = new Tree
            //        {
            //            Left = null,
            //            Right = null
            //        }
            //    },
            //    Right = new Tree
            //    {
            //        Left = new Tree(),
            //        Right = new Tree()
            //    }
            //};

            //foreach (var i in t.Preorder1())
            //{
            //    //Console.WriteLine(i);
            //}

            //Console.WriteLine($"\n{Tree._calls}");

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

            //kernel.Rebind<IGradualAnalyzer>().ToConstant(new DummyGradualAnalyzer());
            //kernel.Rebind<IGeometryRegistrar>().ToConstant(new DummyGeometryRegistrar());

            var factory = kernel.Get<IGeneratorFactory>();

            var points = Enumerable.Range(0, 3)
                    .Select(i => new LooseConfigurationObject(ConfigurationObjectType.Point))
                    .ToList();

            var constructedObjects = ConstructedObjects(points);

            var configuration = new Configuration(points, constructedObjects);
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
            var result = generator.Generate().ToList();
            stopwatch.Stop();

            Console.WriteLine($"Elapsed: {stopwatch.ElapsedMilliseconds}");
            Console.WriteLine($"Generated: {result.Count}");
            Console.WriteLine($"Registration inconsistencies: {Wtf.Inconsistencies}");
            Console.WriteLine($"Maximal attempts to resolve: {Wtf.MaximalNeededAttemps}");
            Console.WriteLine($"Maximal interior re-initializations: {Wtf.MaximalContainerIterations}");
            //PrintTheorems(result);
        }

        private static List<Construction> Constructions()
        {
            return new List<Construction>
            {
                _composedConstructions.AddCentroidFromPoints(),
                _composedConstructions.AddIncenterFromPoints(),
                //_constructionsContainer.Get(MidpointFromPoints),
                //_constructionsContainer.Get(IntersectionOfLinesFromPoints),
                //_constructionsContainer.Get(IntersectionOfLines),
                //_constructionsContainer.Get(PerpendicularLineFromPoints),
                //_constructionsContainer.Get(InternalAngelBisectorFromPoints),
            };
        }

        private static List<ConstructedConfigurationObject> ConstructedObjects(List<LooseConfigurationObject> points)
        {
            return new List<ConstructedConfigurationObject>
            {
            };
        }

        private static void PrintTheorems(IEnumerable<GeneratorOutput> result)
        {
            var formatter = new OutputFormatter(_constructionsContainer);

            Console.ReadKey(true);
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
                Console.ReadKey(true);
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