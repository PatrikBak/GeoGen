using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management.Instrumentation;
using System.Threading.Tasks;
using GeoGen.Core.Configurations;
using GeoGen.Core.Constructions;
using GeoGen.Core.Utilities;
using GeoGen.Core.Utilities.Combinator;
using GeoGen.Core.Utilities.Variations;
using GeoGen.Generator.ConfigurationHandling;
using GeoGen.Generator.ConfigurationHandling.ConfigurationObjectToString;
using GeoGen.Generator.ConfigurationHandling.ConfigurationObjectToString.ObjectIdResolving;
using GeoGen.Generator.ConfigurationHandling.ConfigurationsConstructing;
using GeoGen.Generator.ConfigurationHandling.ConfigurationsConstructing.IdsFixing;
using GeoGen.Generator.ConfigurationHandling.ConfigurationsConstructing.LeastConfigurationFinding;
using GeoGen.Generator.ConfigurationHandling.ConfigurationsContainer;
using GeoGen.Generator.ConfigurationHandling.ConfigurationToString;
using GeoGen.Generator.ConfigurationHandling.ObjectsContainer;
using GeoGen.Generator.Constructing;
using GeoGen.Generator.Constructing.Arguments;
using GeoGen.Generator.Constructing.Arguments.ArgumentsToString;
using GeoGen.Generator.Constructing.Arguments.Container;
using GeoGen.Generator.Constructing.Arguments.SignatureMatching;
using GeoGen.Generator.Constructing.Container;
using Moq;
using NUnit.Framework;
using static GeoGen.Generator.Test.TestHelpers.ConfigurationObjects;
using static GeoGen.Generator.Test.TestHelpers.Constructions;
using static GeoGen.Generator.Test.TestHelpers.ToStringHelper;

namespace GeoGen.Generator.Test
{
    [TestFixture]
    public class GeneratorIntegrationTest
    {
        private static IConfigurationObjectsContainer container;

        private static Generator Generator(GeneratorInput input)
        {
            var constructionsContainer = new ConstructionsContainer();
            constructionsContainer.Initialize(input.Constructions);
            var configurationsHandler = new ConfigurationsHandler();
            var combinator = new Combinator<ConfigurationObjectType, List<ConfigurationObject>>();
            var constructionSignatureMatcherFactory = new ConstructionSignatureMatcherFactory();
            var variationsProvider1 = new VariationsProvider<ConfigurationObject>();
            var defaultConfigurationObjectToStringProvider = new DefaultConfigurationObjectToStringProvider();
            var argumentsToStringProvider = new ArgumentsToStringProvider(defaultConfigurationObjectToStringProvider);
            var argumentsContainerFactory = new ArgumentsContainerFactory(argumentsToStringProvider);
            var argumentsGenerator = new ArgumentsGenerator(combinator, constructionSignatureMatcherFactory, variationsProvider1, argumentsContainerFactory);
            var objectsConstructor = new ObjectsConstructor(constructionsContainer, argumentsGenerator);
            var variationsProvider2 = new VariationsProvider<int>();
            var configurationToStringProvider = new ConfigurationToStringProvider();
            var configurationObjectToStringProviderFactory = new ConfigurationObjectToStringProviderFactory(argumentsToStringProvider);
            var leastConfigurationFinder = new LeastConfigurationFinder(variationsProvider2, configurationToStringProvider, configurationObjectToStringProviderFactory);
            var defaultObjectIdResolver = new DefaultObjectIdResolver();
            var defaultComplexConfigurationObjectToStringProvider = new DefaultComplexConfigurationObjectToStringProvider(argumentsToStringProvider, defaultObjectIdResolver);
            container = new ConfigurationObjectsContainer(defaultComplexConfigurationObjectToStringProvider);
            container.Initialize(input.InitialConfiguration.LooseObjects);
            var idsFixer = new IdsFixer(container);
            var configurationConstructor = new ConfigurationConstructor(leastConfigurationFinder, idsFixer, argumentsContainerFactory);
            var configurationObjectsContainer = new ConfigurationObjectsContainer(defaultComplexConfigurationObjectToStringProvider);
            var configurationContainer = new ConfigurationContainer(argumentsContainerFactory, configurationConstructor, configurationToStringProvider, configurationObjectsContainer);
            configurationContainer.Initialize(input.InitialConfiguration);

            return new Generator(configurationContainer, objectsConstructor, configurationsHandler, input.MaximalNumberOfIterations);
        }

        [Test]
        public void Triangle_And_Midpoint_Test()
        {
            var points = Objects(3, ConfigurationObjectType.Point, includeIds: false);
            var configuration = new Configuration(points.ToSet(), new List<ConstructedConfigurationObject>());
            var constructions = Midpoint().SingleItemAsEnumerable().Select(c => c.Construction).ToList();
            var input = new GeneratorInput
            {
                InitialConfiguration = configuration,
                Constructions = constructions,
                MaximalNumberOfIterations = 5
            };

            var count = 0;

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            foreach (var output in Generator(input).Generate())
            {
                //var asString = ConfigurationAsString(output.GeneratedConfiguration, false);
                //Console.WriteLine(asString);
                count++;
            }
            stopwatch.Stop();

            Console.WriteLine($"Configurations: {count}");
            Console.WriteLine($"Container: {container.Count()}");
            Console.WriteLine($"Time: {stopwatch.ElapsedMilliseconds}");
        }

        class Hm
        {
            public List<int> list;

            public Hm()
            {
                list = Enumerable.Range(0, 100).ToList();
            }
        }

        [Test]
        public void Test()
        {
            var numbers = Enumerable.Range(0, 100).ToList();

            var hm = new Hm();
            var bag = new ConcurrentBag<List<int>>();

            Parallel.ForEach(
                numbers, i =>
                {
                    bag.Add(hm.list.Select(j => j*j).ToList());
                });

            Assert.AreEqual(numbers.Count, bag.Count);

            foreach (var ints in bag)
            {
                for (int i = 0; i < 100; i++)
                {
                    Assert.AreEqual(ints[i], i * i);
                }
            }
        }
    }
}