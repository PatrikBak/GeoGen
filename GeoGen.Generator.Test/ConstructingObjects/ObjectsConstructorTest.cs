using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Core.Configurations;
using GeoGen.Core.Constructions.Arguments;
using GeoGen.Generator.ArgumentsGenerator;
using GeoGen.Generator.Test.TestHelpers;
using Moq;
using NUnit.Framework;
using static GeoGen.Generator.Test.TestHelpers.ConfigurationObjects;
using static GeoGen.Generator.Test.TestHelpers.Utilities;

namespace GeoGen.Generator.Test.ConstructingObjects
{
    [TestFixture]
    public class ObjectsConstructorTest
    {
        private static IArgumentsListContainer ArgumentsListContainer(IEnumerable<ConfigurationObject> objects)
        {
            var argsList = objects
                    .Select(obj => new List<ConstructionArgument> {new ObjectConstructionArgument(obj)})
                    .Cast<IReadOnlyList<ConstructionArgument>>()
                    .ToList();

            var mock = new Mock<IArgumentsListContainer>();
            mock.Setup(s => s.GetEnumerator()).Returns(() => argsList.GetEnumerator());
            
            return mock.Object;
        }

        private static IConstructionsContainer ConstructionsContainer(int count, int outputCount)
        {
            var constructions = Constructions.ConstructionWrappers(count, outputCount);
            var mock = new Mock<IConstructionsContainer>();
            mock.Setup(c => c.GetEnumerator()).Returns(constructions.GetEnumerator());

            return mock.Object;
        }

        private static IArgumentsGenerator ArgumentsGenerator(List<LooseConfigurationObject> objects)
        {
            var mock = new Mock<IArgumentsGenerator>();
            mock.Setup(g => g.GenerateArguments(It.IsAny<ConfigurationWrapper>(), It.IsAny<ConstructionWrapper>()))
                    .Returns(() => ArgumentsListContainer(objects));

            return mock.Object;
        }

        [Test]
        public void Test_Constructor_Constructions_Container_Cant_Be_Null()
        {
            var generator = SimpleMock<IArgumentsGenerator>();

            Assert.Throws<ArgumentNullException>(() => new ObjectsConstructor(null, generator));
        }

        [Test]
        public void Test_Constructor_Arguments_Generator_Cant_Be_Null()
        {
            var container = SimpleMock<IConstructionsContainer>();

            Assert.Throws<ArgumentNullException>(() => new ObjectsConstructor(container, null));
        }

        [Test]
        public void Test_Configuration_Wrapper_Not_Cant_Be_Null()
        {
            var container = SimpleMock<IConstructionsContainer>();
            var generator = SimpleMock<IArgumentsGenerator>();

            Assert.Throws<ArgumentNullException>
            (
                () => new ObjectsConstructor(container, generator).GenerateOutput(null)
            );
        }

        [TestCase(1, 2, 3, 6)]
        [TestCase(2, 7, 4, 56)]
        [TestCase(8, 1, 3, 24)]
        [TestCase(42, 4, 666, 111888)]
        [TestCase(42, 4, 1, 168)]
        [TestCase(0, 0, 666, 0)]
        [TestCase(0, 1, 666, 0)]
        [TestCase(1, 0, 1, 0)]
        public void Test_Constructions_Count(int constructions, int arguments, int perConstruction, int expected)
        {
            var objects = Objects(arguments, ConfigurationObjectType.Point).ToList();
            var container = ConstructionsContainer(constructions, perConstruction);
            var argumentsGenerator = ArgumentsGenerator(objects);
            var wrapper = new ConfigurationWrapper();
            var testConstructor = new ObjectsConstructor(container, argumentsGenerator);

            var result = testConstructor.GenerateOutput(wrapper).Sum(output => output.ConstructedObjects.Count);

            Assert.AreEqual(expected, result);
        }
    }
}