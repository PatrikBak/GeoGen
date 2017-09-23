using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using GeoGen.Core.Configurations;
using GeoGen.Core.Utilities;
using GeoGen.Core.Utilities.Combinator;
using GeoGen.Core.Utilities.VariationsProviding;
using GeoGen.Generator.ConfigurationsHandling;
using GeoGen.Generator.ConstructingConfigurations;
using GeoGen.Generator.ConstructingConfigurations.ConfigurationToString;
using GeoGen.Generator.ConstructingConfigurations.IdsFixing;
using GeoGen.Generator.ConstructingConfigurations.LeastConfigurationFinding;
using GeoGen.Generator.ConstructingConfigurations.ObjectsContainer;
using GeoGen.Generator.ConstructingConfigurations.ObjectToString;
using GeoGen.Generator.ConstructingConfigurations.ObjectToString.ObjectIdResolving;
using GeoGen.Generator.ConstructingObjects;
using GeoGen.Generator.ConstructingObjects.Arguments;
using GeoGen.Generator.ConstructingObjects.Arguments.ArgumentsListToString;
using GeoGen.Generator.ConstructingObjects.Arguments.Container;
using GeoGen.Generator.ConstructingObjects.Arguments.SignatureMatching;
using GeoGen.Generator.Test.TestHelpers;
using Moq;
using NUnit.Framework;
using static GeoGen.Generator.Test.TestHelpers.ConfigurationObjects;
using static GeoGen.Generator.Test.TestHelpers.Constructions;

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
            var handler = new Mock<IConfigurationsHandler>();
            handler.Setup(s => s.GenerateFinalOutput(It.IsAny<IEnumerable<ConfigurationWrapper>>()))
                    .Returns<IEnumerable<ConfigurationWrapper>>(s => s.Select(i => new GeneratorOutput()));
            var combinator = new Combinator<ConfigurationObjectType, List<ConfigurationObject>>();
            var variationsProvider1 = new VariationsProvider<ConfigurationObject>();
            var defaultObjectIdResolver = new DefaultObjectIdResolver();
            var defaultObjectToStringProvider = new DefaultObjectToStringProvider(defaultObjectIdResolver);
            var constructionSignatureMatcherFactory = new ConstructionSignatureMatcher();
            var argumentsToStringProvider = new ArgumentsListToStringProvider(defaultObjectToStringProvider);
            var argumentsContainerFactory = new ArgumentsListContainerFactory(argumentsToStringProvider);
            var argumentsGenerator = new ArgumentsGenerator(combinator, constructionSignatureMatcherFactory, variationsProvider1, argumentsContainerFactory);
            var objectsConstructor = new ObjectsConstructor(constructionsContainer, argumentsGenerator);
            var variationsProvider2 = new VariationsProvider<LooseConfigurationObject>();
            var configurationToStringProvider = new ConfigurationToStringProvider();
            var configurationObjectToStringProviderFactory = new CustomFullObjectToStringProviderFactory(argumentsToStringProvider);
            var dictionaryObjectIdResolversContainer = new DictionaryObjectIdResolversContainer(variationsProvider2);
            var leastConfigurationFinder = new LeastConfigurationFinder(configurationToStringProvider, configurationObjectToStringProviderFactory, dictionaryObjectIdResolversContainer);
            var defaultComplexConfigurationObjectToStringProvider = new DefaultFullObjectToStringProvider(argumentsToStringProvider, defaultObjectIdResolver);
            container = new ConfigurationObjectsContainer(defaultComplexConfigurationObjectToStringProvider);
            var idsFixer = new IdsFixerFactory(container);
            var configurationConstructor = new ConfigurationConstructor(leastConfigurationFinder, idsFixer, argumentsContainerFactory);
            var configurationContainer = new ConfigurationsContainer(configurationConstructor, configurationToStringProvider, container, defaultComplexConfigurationObjectToStringProvider);
            configurationContainer.Initialize(input.InitialConfiguration);
            dictionaryObjectIdResolversContainer.Initialize(input.InitialConfiguration.LooseObjects.ToList());
            return new Generator(configurationContainer, objectsConstructor, handler.Object, input.MaximalNumberOfIterations);
        }

        [Test]
        public void Triangle_And_Midpoint_Test()
        {
            int it = 5;
            int max = 5;
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
                
      //Console.WriteLine($"Iterating: {s_iterating.ElapsedMilliseconds}");
                //Console.WriteLine($"Converting to string: {s_toString.ElapsedMilliseconds}");

                //Console.WriteLine($"Converting: {s_converting.ElapsedMilliseconds}");
                //Console.WriteLine($"Sorting: {s_sorting.ElapsedMilliseconds}");
                //Console.WriteLine($"Joining: {s_joining.ElapsedMilliseconds}");

                Console.WriteLine("--------");
            }
        }
    }
}