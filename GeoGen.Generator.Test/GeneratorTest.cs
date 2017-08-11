using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Core.Configurations;
using GeoGen.Core.Generator;
using GeoGen.Core.Utilities;
using Moq;
using NUnit.Framework;

namespace GeoGen.Generator.Test
{
    [TestFixture]
    public class GeneratorTest
    {
        private static Generator CreateTestGenerator(int constructorDuplicationCount, int iterations)
        {
            // create configurations list containing a single dummy configuration
            var looseObject = new LooseConfigurationObject(ConfigurationObjectType.Point);
            var looseObjects = new HashSet<LooseConfigurationObject> {looseObject};
            var constructedObjects = new HashSet<ConstructedConfigurationObject>();
            var configuration = new Configuration(looseObjects, constructedObjects);
            var configurations = new List<Configuration> {configuration};

            // setup container mock so it returns configurations and overrides them when we're
            // adding a new layer
            var containterMock = new Mock<IConfigurationContainer>();
            containterMock.Setup(c => c.Configurations).Returns(configurations);
            containterMock.Setup(c => c.AddNewLayer(It.IsAny<List<Configuration>>()))
                .Callback<List<Configuration>>(c => configurations.SetItems(c));
            var configurationContainer = containterMock.Object;

            // setup configuration handler mock that converts all generated configurations 
            // except for one into the generator outut
            var configurationHandlerMock = new Mock<IConfigurationsHandler>();
            configurationHandlerMock.Setup(h => h.GenerateFinalOutput())
                .Returns(() => configurationContainer.Configurations.Skip(1).Select(c => new GeneratorOutput(c)));
            var configurationHandler = configurationHandlerMock.Object;

            // setup configuration constructor that generates new configuration by repeating
            // the provided one by the given number of times
            var configurationConstructorMock = new Mock<IConfigurationConstructer>();
            configurationConstructorMock.Setup(c => c.GenerateNewConfigurations(It.IsAny<Configuration>()))
                .Returns<Configuration>(c => Enumerable.Repeat(c, constructorDuplicationCount).ToList());
            var congigurationConstructer = configurationConstructorMock.Object;

            // setup a generator context mock
            var generatorContextMock = new Mock<IGeneratorContext>();
            generatorContextMock.Setup(g => g.ConfigurationContainer).Returns(configurationContainer);
            generatorContextMock.Setup(g => g.ConfigurationConstructer).Returns(congigurationConstructer);
            generatorContextMock.Setup(g => g.ConfigurationsHandler).Returns(configurationHandler);

            return new Generator(generatorContextMock.Object, iterations);
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
        public void GenerationTest_When_We_Generate_One_From_Each(int duplication, int iterations, int expected)
        {
            var generator = CreateTestGenerator(duplication, iterations);
            Assert.AreEqual(expected, generator.Generate().Count());
        }

        public void Generator_Constructor_Context_Cannot_Be_Null()
        {
            Assert.Throws<ArgumentNullException>(() => { new Generator(null, 42); });
        }

        [TestCase(-42)]
        [TestCase(-1)]
        [TestCase(0)]
        public void Generator_Number_Of_Iterations_Is_At_Least_One(int number)
        {
            var contextMock = new Mock<IGeneratorContext>();
            Assert.Throws<ArgumentOutOfRangeException>(() => { new Generator(contextMock.Object, number); });
        }
    }
}