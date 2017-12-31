using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using GeoGen.AnalyticalGeometry;
using GeoGen.Analyzer;
using GeoGen.Core;
using GeoGen.Core.Configurations;
using GeoGen.Core.Constructions;
using GeoGen.Core.Constructions.PredefinedConstructions;
using GeoGen.Core.Generator;
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
            _constructionsContainer = new ConstructionsContainer();
            _composedConstructions = new ComposedConstructions(_constructionsContainer);
            _constructorHelper = new ConstructorHelper(_constructionsContainer);

            var kernel = new StandardKernel
            (
                new GeneratorModule(),
                new CoreModule(),
                new AnalyerModule(),
                new AnalyticalGeometryModule()
            );

            kernel.Components.RemoveAll<IMissingBindingResolver>();

            kernel.Rebind<IGradualAnalyzer>().ToConstant(new DummyGradualAnalyzer());
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
                MaximalNumberOfIterations = 7
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

        private static List<Construction> Constructions()
        {
            return new List<Construction>
            {
                //_composedConstructions.AddCentroidFromPoints(),
               // _composedConstructions.AddIncenterFromPoints(),
                _constructionsContainer.Get<MidpointFromPoints>(),
               // _constructionsContainer.Get<IntersectionFromPoints>()
            };
        }

        private static List<ConstructedConfigurationObject> ConstructedObjects(List<LooseConfigurationObject> points)
        {
            return new List<ConstructedConfigurationObject>
            {
            };
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