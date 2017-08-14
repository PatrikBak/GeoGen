using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Core.Configurations;
using GeoGen.Core.Constructions;
using GeoGen.Core.Constructions.Arguments;
using GeoGen.Generator.Constructor;
using GeoGen.Generator.Constructor.Arguments;
using GeoGen.Generator.Constructor.Container;
using GeoGen.Generator.Wrappers;
using Moq;
using NUnit.Framework;

namespace GeoGen.Generator.Test.Constructor
{
    public class ConfigurationConstructorTest
    {
        private static T SimpleMock<T>() where T : class
        {
            return new Mock<T>().Object;
        }

        private static ConfigurationConstructor TestConstructor(int numberOfConstructions, int numberOfArguments)
        {
            var constructions = Enumerable.Range(1, numberOfConstructions)
                .Select(i => new ConstructionWrapper {Construction = new Mock<Construction>().Object})
                .ToList();

            var containerMock = new Mock<IConstructionsContainer>();
            containerMock.Setup(c => c.GetEnumerator()).Returns(() => constructions.GetEnumerator());
            var container = containerMock.Object;

            var generatorMock = new Mock<IArgumentsGenerator>();
            generatorMock.Setup(g => g.GenerateArguments(It.IsAny<ConfigurationWrapper>(), It.IsAny<ConstructionWrapper>()))
                .Returns(() => Enumerable.Range(1, numberOfArguments).Select(i => new List<ConstructionArgument>()));
            var generator = generatorMock.Object;

            return new ConfigurationConstructor(container, generator);
        }

        [Test]
        public void Test_Constructor_Constructions_Container_Null()
        {
            Assert.Throws<ArgumentNullException>(() => { new ConfigurationConstructor(null, SimpleMock<IArgumentsGenerator>()); });
        }

        [Test]
        public void Test_Constructor_Arguments_Generator_Null()
        {
            Assert.Throws<ArgumentNullException>(() => { new ConfigurationConstructor(SimpleMock<IConstructionsContainer>(), null); });
        }

        [TestCase(1, 2, 2)]
        [TestCase(1, 4, 4)]
        [TestCase(2, 1, 2)]
        [TestCase(4, 1, 4)]
        [TestCase(1, 2, 2)]
        [TestCase(0, 10, 0)]
        [TestCase(17, 90, 1530)]
        [TestCase(42, 666, 27972)]
        public void Test_Construction_Count(int numberOfConstructions, int argumentsPerConstruction, int expectedCount)
        {
            var looseObject = new LooseConfigurationObject(ConfigurationObjectType.Point);
            var looseObjects = new HashSet<LooseConfigurationObject> { looseObject };
            var configuration = new Configuration(looseObjects, new HashSet<ConstructedConfigurationObject>());
            var wrapper = new ConfigurationWrapper {Configuration = configuration};

            var testConstructor = TestConstructor(numberOfConstructions, argumentsPerConstruction);
            var count = testConstructor.GenerateNewConfigurationObjects(wrapper).Count();

            Assert.AreEqual(expectedCount, count);
        }
    }
}