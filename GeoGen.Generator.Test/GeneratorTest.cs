using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GeoGen.Core.Configurations;
using GeoGen.Core.Constructions;
using GeoGen.Core.Constructions.Arguments;
using GeoGen.Core.Generator;
using GeoGen.Core.Utilities;
using GeoGen.Generator.Constructor;
using GeoGen.Generator.Container;
using GeoGen.Generator.Handler;
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

            // mock constructed configuration object
            var mock = new Mock<Construction>();
            var constructon = mock.Object;
            var constructedConfigurationObject = new ConstructedConfigurationObject(constructon,
                new List<ConstructionArgument> {new ObjectConstructionArgument(looseObject)});

            var configurations = new ObservableCollection<Configuration> {configuration};

            // setup container mock so it returns configurations and overrides them when we're
            // adding a new layer
            var containterMock = new Mock<IConfigurationContainer>();
            containterMock.Setup(c => c.GetEnumerator()).Returns(() => configurations.GetEnumerator());
            containterMock.Setup(c => c.AddLayer(It.IsAny<List<ConstructorOutput>>()))
                .Callback<List<ConstructorOutput>>(c => configurations.SetItems(c.Select(i => i.InitialConfiguration)));
            var configurationContainer = containterMock.Object;

            // setup configuration handler mock that converts all generated configurations 
            // except for one into the generator output
            var configurationHandlerMock = new Mock<IConfigurationsHandler>();
            configurationHandlerMock.Setup(h => h.GenerateFinalOutput())
                .Returns(() => configurations.Skip(1).Select(c => new GeneratorOutput(c)));
            var configurationHandler = configurationHandlerMock.Object;

            // setup configuration constructor that generates new configuration by repeating
            // the provided one by the given number of times
            var configurationConstructorMock = new Mock<IConfigurationConstructor>();
            configurationConstructorMock.Setup(c => c.GenerateNewConfigurationObjects(It.IsAny<Configuration>()))
                .Returns<Configuration>(c => Enumerable.Repeat(configuration, constructorDuplicationCount)
                    .Select(cc => new ConstructorOutput(configuration, constructedConfigurationObject)));
            var congigurationConstructer = configurationConstructorMock.Object;

            // setup a generator context mock
            var generatorContextMock = new Mock<IGeneratorContext>();
            generatorContextMock.Setup(g => g.ConfigurationContainer).Returns(configurationContainer);
            generatorContextMock.Setup(g => g.ConfigurationConstructor).Returns(congigurationConstructer);
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
            Assert.Throws<ArgumentNullException>(() => new Generator(null, 42));
        }

        [TestCase(-42)]
        [TestCase(-1)]
        [TestCase(0)]
        public void Generator_Number_Of_Iterations_Is_At_Least_One(int number)
        {
            var contextMock = new Mock<IGeneratorContext>();
            Assert.Throws<ArgumentOutOfRangeException>(() => new Generator(contextMock.Object, number));
        }
    }
}