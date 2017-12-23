using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using GeoGen.AnalyticalGeometry.Ninject;
using GeoGen.Analyzer;
using GeoGen.Analyzer.NInject;
using GeoGen.Core.Configurations;
using GeoGen.Core.Constructions;
using GeoGen.Core.Constructions.PredefinedConstructions;
using GeoGen.Core.Generator;
using GeoGen.Core.NInject;
using GeoGen.Core.Utilities;
using GeoGen.Utilities;
using Ninject;
using Ninject.Extensions.ContextPreservation;
using Ninject.Extensions.Factory;
using Ninject.Extensions.NamedScope;
using Ninject.Planning.Bindings.Resolvers;

namespace GeoGen.Generator.IntegrationTest
{
    public class Program
    {
        private static void Main()
        {
            var kernel = new StandardKernel
            (
                    new GeneratorModule(),
                    new CoreModule(),
                    new AnalyerModule(),
                    new AnalyticalGeometryModule()
            );

            kernel.Components.RemoveAll<IMissingBindingResolver>();

            kernel.Rebind<IGradualAnalyzer>().ToConstant(new DummyGradualAnalyzer());

            var factory = kernel.Get<IGeneratorFactory>();

            var points = Enumerable.Range(0, 3)
                    .Select(i => new LooseConfigurationObject(ConfigurationObjectType.Point))
                    .ToSet();

            var configuration = new Configuration(points, new List<ConstructedConfigurationObject>());
            var constructions = new List<Construction>
            {
                    new Midpoint {Id = 0}
            };

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

            //PrintResults(result);
        }

        private static void PrintTheorems(List<GeneratorOutput> result)
        {
            var formatter = new OutputFormatter();

            Console.WriteLine("Results:\n");

            foreach (var generatorOutput in result)
            {
                try
                {
                    Console.WriteLine(formatter.Format(generatorOutput.Configuration));
                    Console.WriteLine();
                    Console.WriteLine("Theorems:");
                    Console.WriteLine(formatter.Format(generatorOutput.Theorems));
                    Console.WriteLine("-------------------\n");
                }
                catch (Exception)
                {
                    Console.Error.WriteLine("Exception :(");
                }
            }
        }
    }
}