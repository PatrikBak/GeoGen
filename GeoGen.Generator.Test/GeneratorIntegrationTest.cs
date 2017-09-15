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
using GeoGen.Generator.ConfigurationsHandling;
using GeoGen.Generator.ConfigurationsHandling.ConfigurationObjectToString;
using GeoGen.Generator.ConfigurationsHandling.ConfigurationObjectToString.ConfigurationObjectIdResolving;
using GeoGen.Generator.ConfigurationsHandling.ConfigurationsConstructing;
using GeoGen.Generator.ConfigurationsHandling.ConfigurationsConstructing.IdsFixing;
using GeoGen.Generator.ConfigurationsHandling.ConfigurationsConstructing.LeastConfigurationFinding;
using GeoGen.Generator.ConfigurationsHandling.ConfigurationsContainer;
using GeoGen.Generator.ConfigurationsHandling.ConfigurationToString;
using GeoGen.Generator.ConfigurationsHandling.ObjectsContainer;
using GeoGen.Generator.Constructing;
using GeoGen.Generator.Constructing.Arguments;
using GeoGen.Generator.Constructing.Arguments.ArgumentsToString;
using GeoGen.Generator.Constructing.Arguments.Container;
using GeoGen.Generator.Constructing.Arguments.SignatureMatching;
using GeoGen.Generator.Constructing.Container;
using Moq;
using NUnit.Framework;
using static GeoGen.Generator.ConfigurationsHandling.ConfigurationsConstructing.ConfigurationConstructor;
using static GeoGen.Generator.ConfigurationsHandling.ConfigurationsConstructing.LeastConfigurationFinding.LeastConfigurationFinder;
using static GeoGen.Generator.ConfigurationsHandling.ConfigurationsContainer.ConfigurationContainer;
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
            var defaultConfigurationObjectToStringProvider = new DefaultObjectToStringProvider();
            var argumentsToStringProvider = new ArgumentsToStringProvider(defaultConfigurationObjectToStringProvider);
            var argumentsContainerFactory = new ArgumentsContainerFactory(argumentsToStringProvider);
            var argumentsGenerator = new ArgumentsGenerator(combinator, constructionSignatureMatcherFactory, variationsProvider1, argumentsContainerFactory);
            var objectsConstructor = new ObjectsConstructor(constructionsContainer, argumentsGenerator);
            var variationsProvider2 = new VariationsProvider<int>();
            var configurationToStringProvider = new ConfigurationToStringProvider();
            var configurationObjectToStringProviderFactory = new CustomFullObjectToStringProviderFactory(argumentsToStringProvider);
            var dictionaryObjectIdResolversContainer = new DictionaryObjectIdResolversContainer(variationsProvider2);
            var leastConfigurationFinder = new LeastConfigurationFinder(configurationToStringProvider, configurationObjectToStringProviderFactory, dictionaryObjectIdResolversContainer);
            var defaultObjectIdResolver = new DefaultObjectIdResolver();
            var defaultComplexConfigurationObjectToStringProvider = new DefaultFullObjectToStringProvider(argumentsToStringProvider, defaultObjectIdResolver);
            container = new ConfigurationObjectsContainer(defaultComplexConfigurationObjectToStringProvider);
            var idsFixer = new IdsFixerFactory(container);
            var configurationConstructor = new ConfigurationConstructor(leastConfigurationFinder, idsFixer, argumentsContainerFactory);
            var configurationContainer = new ConfigurationContainer(argumentsContainerFactory, configurationConstructor, configurationToStringProvider, container);
            configurationContainer.Initialize(input.InitialConfiguration);
            dictionaryObjectIdResolversContainer.Initialize(input.InitialConfiguration.LooseObjects);
            return new Generator(configurationContainer, objectsConstructor, configurationsHandler, input.MaximalNumberOfIterations);
        }

        [Test]
        public void Triangle_And_Midpoint_Test()
        {
            int it = 7;
            int max = 7;
            for (int i = it; i <= max; i++)
            {
                var points = Objects(3, ConfigurationObjectType.Point, includeIds: false);
                var configuration = new Configuration(points.ToSet(), new List<ConstructedConfigurationObject>());
                var constructions = Midpoint().SingleItemAsEnumerable().Select(c => c.Construction).ToList();
                var input = new GeneratorInput
                {
                    InitialConfiguration = configuration,
                    Constructions = constructions,
                    MaximalNumberOfIterations = i
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

                Console.WriteLine($"Iterations: {input.MaximalNumberOfIterations}");
                Console.WriteLine($"Configurations: {count}");
                Console.WriteLine($"Container: {container.Count()}");
                Console.WriteLine($"Time: {stopwatch.ElapsedMilliseconds}\n");

                Console.WriteLine($"New objects: {s_newObjects.ElapsedMilliseconds}");
                Console.WriteLine($"Wrapper: {s_constructingWrapper.ElapsedMilliseconds}");
                Console.WriteLine($"Construction: {s_AddingConfiguration.ElapsedMilliseconds}");
                Console.WriteLine("--------");

                Console.WriteLine($"Balast: {s_balast.ElapsedMilliseconds}");
                Console.WriteLine($"Fixing arguments: {s_arguments.ElapsedMilliseconds}");
                Console.WriteLine($"Least resolver: {s_leastResolver.ElapsedMilliseconds}");
                Console.WriteLine($"Objects map: {s_typeMap.ElapsedMilliseconds}");
                Console.WriteLine($"Cloning config: {s_cloningConfig.ElapsedMilliseconds}");
                Console.WriteLine("--------");

                Console.WriteLine($"Iterating: {s_iterating.ElapsedMilliseconds}");
                Console.WriteLine($"Converting to string: {s_toString.ElapsedMilliseconds}");
                Console.WriteLine("--------");

                Console.WriteLine($"Cache used: {CustomFullObjectToStringProvider.i}");
            }
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
                numbers, i => { bag.Add(hm.list.Select(j => j * j).ToList()); });

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