using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using GeoGen.Core.Configurations;
using GeoGen.Core.Constructions;
using GeoGen.Core.Constructions.PredefinedConstructions;
using GeoGen.Core.Utilities;
using GeoGen.Generator.NInject;
using Ninject;

namespace GeoGen.Generator.IntegrationTest
{
    public class Program
    {
        private static void Main()
        {
            var kernel = new StandardKernel(new ConstructionTestModule());
            var factory = kernel.Get<IGeneratorFactory>();

            var points = Enumerable.Range(0, 3)
                    .Select(i => new LooseConfigurationObject(ConfigurationObjectType.Point))
                    .ToSet();

            var configuration = new Configuration(points, new List<ConstructedConfigurationObject>());
            var constructions = new Midpoint {Id = 1}.SingleItemAsEnumerable().Cast<Construction>().ToList();

            var input = new GeneratorInput
            {
                InitialConfiguration = configuration,
                Constructions = constructions,
                MaximalNumberOfIterations = 6
            };

            var generator = factory.CreateGenerator(input);
            var stopwatch = new Stopwatch();

            stopwatch.Start();
            var counter = generator.Generate().Count();
            stopwatch.Stop();

            Console.WriteLine($"Elapsed: {stopwatch.ElapsedMilliseconds}");
            Console.WriteLine($"Generated: {counter}");
        }
    }
}