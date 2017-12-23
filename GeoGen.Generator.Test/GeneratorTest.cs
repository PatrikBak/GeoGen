using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Core.Configurations;
using GeoGen.Core.Constructions;
using GeoGen.Core.Constructions.Arguments;
using GeoGen.Core.Generator;
using GeoGen.Core.Utilities;
using GeoGen.Generator.ConfigurationsHandling;
using GeoGen.Generator.ConstructingConfigurations;
using GeoGen.Generator.ConstructingObjects;
using GeoGen.Utilities;
using Moq;
using NUnit.Framework;
using static GeoGen.Generator.Test.TestHelpers.Utilities;

namespace GeoGen.Generator.Test
{
    [TestFixture]
    public class GeneratorTest
    {
        private static Generator Generator(int constructorDuplicationCount, int iterations)
        {
            // create configurations list containing a single dummy configuration
            var looseObject = new LooseConfigurationObject(ConfigurationObjectType.Point);
            var looseObjects = new HashSet<LooseConfigurationObject> {looseObject};
            var constructedObjects = new List<ConstructedConfigurationObject>();
            var configuration = new ConfigurationWrapper
            {
                Configuration = new Configuration(looseObjects, constructedObjects)
            };

            // mock constructed configuration object
            var mock = new Mock<Construction>();
            var outputTypes = new List<ConfigurationObjectType>
            {
                ConfigurationObjectType.Point,
                ConfigurationObjectType.Circle
            };
            mock.Setup(c => c.OutputTypes).Returns(outputTypes);
            var constructon = mock.Object;
            var args = new List<ConstructionArgument> {new ObjectConstructionArgument(looseObject)};
            var constructedConfigurationObject = new ConstructedConfigurationObject(constructon, args, 0);

            var configurations = new List<ConfigurationWrapper> {configuration};

            // setup container mock so it returns configurations and overrides them when we're
            // adding a new layer
            var containterMock = new Mock<IConfigurationsManager>();
            containterMock.Setup(c => c.CurrentLayer).Returns(() => configurations);
            containterMock.Setup(c => c.AddLayer(It.IsAny<IEnumerable<ConstructorOutput>>()))
                    .Returns<IEnumerable<ConstructorOutput>>
                    (
                        c =>
                        {
                            var result = c.Select(output => output.InitialConfiguration).ToList();

                            configurations.SetItems(result);

                            return result;
                        }
                    );
            var configurationContainer = containterMock.Object;

            // setup configuration handler mock that converts all generated configurations 
            // except for one into the generator output
            var configurationHandlerMock = new Mock<IConfigurationsHandler>();
            configurationHandlerMock.Setup(h => h.GenerateFinalOutput(It.IsAny<IEnumerable<ConfigurationWrapper>>()))
                    .Returns<IEnumerable<ConfigurationWrapper>>(w => w.Skip(1).Select(c => new GeneratorOutput()));
            var configurationHandler = configurationHandlerMock.Object;

            // setup configuration constructor that generates new configuration by repeating
            // the provided one by the given number of times
            var configurationConstructorMock = new Mock<IObjectsConstructor>();
            configurationConstructorMock.Setup(c => c.GenerateOutput(It.IsAny<ConfigurationWrapper>()))
                    .Returns<ConfigurationWrapper>
                    (
                        c => Enumerable.Repeat(configuration, constructorDuplicationCount)
                                .Select
                                (
                                    cc => new ConstructorOutput
                                    {
                                        InitialConfiguration = configuration,
                                        ConstructedObjects = new List<ConstructedConfigurationObject> {constructedConfigurationObject}
                                    }
                                )
                    );
            var congigurationConstructer = configurationConstructorMock.Object;

            return new Generator(configurationContainer, congigurationConstructer, configurationHandler, iterations);
        }

        [TestCase(1, 1, 0)]
        [TestCase(1, 2, 0)]
        [TestCase(1, 3, 0)]
        [TestCase(2, 1, 1)]
        [TestCase(42, 1, 41)]
        [TestCase(2, 2, 4)]
        [TestCase(2, 3, 11)]
        [TestCase(3, 2, 10)]
        [TestCase(10, 5, 111105)]
        public void Test_When_We_Generate_One_From_Each(int duplication, int iterations, int expected)
        {
            var generator = Generator(duplication, iterations);
            Assert.AreEqual(expected, generator.Generate().Count());
        }

        [TestCase(-42)]
        [TestCase(-1)]
        [TestCase(0)]
        public void Test_Number_Of_Iterations_Is_At_Least_One(int number)
        {
            var constructor = SimpleMock<IObjectsConstructor>();
            var container = SimpleMock<IConfigurationsManager>();
            var handler = SimpleMock<IConfigurationsHandler>();

            Assert.Throws<ArgumentOutOfRangeException>(() => new Generator(container, constructor, handler, number));
        }

        [Test]
        public void Test_Generator_Container_Cannot_Be_Null()
        {
            var constructor = SimpleMock<IObjectsConstructor>();
            var handler = SimpleMock<IConfigurationsHandler>();

            Assert.Throws<ArgumentNullException>(() => new Generator(null, constructor, handler, 1));
        }

        [Test]
        public void Test_Generator_Constructor_Cannot_Be_Null()
        {
            var container = SimpleMock<IConfigurationsManager>();
            var handler = SimpleMock<IConfigurationsHandler>();

            Assert.Throws<ArgumentNullException>(() => new Generator(container, null, handler, 1));
        }

        [Test]
        public void Test_Generator_Handler_Cannot_Be_Null()
        {
            var constructor = SimpleMock<IObjectsConstructor>();
            var container = SimpleMock<IConfigurationsManager>();

            Assert.Throws<ArgumentNullException>(() => new Generator(container, constructor, null, 1));
        }
    }
}