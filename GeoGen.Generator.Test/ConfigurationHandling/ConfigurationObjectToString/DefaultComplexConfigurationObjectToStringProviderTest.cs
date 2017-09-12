using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Core.Configurations;
using GeoGen.Core.Constructions;
using GeoGen.Core.Constructions.Arguments;
using GeoGen.Generator.ConfigurationHandling.ConfigurationObjectToString;
using GeoGen.Generator.Constructing.Arguments.ArgumentsToString;
using Moq;
using NUnit.Framework;

namespace GeoGen.Generator.Test.ConfigurationHandling.ConfigurationObjectToString
{
    [TestFixture]
    public class DefaultComplexConfigurationObjectToStringProviderTest
    {
        private static IArgumentsToStringProvider ArgumentsProvider()
        {
            return new ArgumentsToStringProvider(", ", "; ");
        }

        private static DefaultComplexConfigurationObjectToStringProvider Provider()
        {
            return new DefaultComplexConfigurationObjectToStringProvider(ArgumentsProvider());
        }

        [Test]
        public void Arguments_Provider_Cant_Be_Null()
        {
            Assert.Throws<ArgumentNullException>(() => new DefaultComplexConfigurationObjectToStringProvider(null));
        }

        [Test]
        public void Object_Cant_Be_Null()
        {
            Assert.Throws<ArgumentNullException>(() => Provider().ConvertToString(null));
        }

        [Test]
        public void Loose_Objects_To_String_Test()
        {
            var looseObject = new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 42};

            Assert.AreEqual("42", Provider().ConvertToString(looseObject));
        }

        [Test]
        public void Constructed_Object_To_String_Test()
        {
            var mock = new Mock<Construction>();
            mock.Setup(s => s.Id).Returns(42);
            mock.Setup(s => s.OutputTypes).Returns(new List<ConfigurationObjectType> {ConfigurationObjectType.Point, ConfigurationObjectType.Point});

            var args = Enumerable.Range(0, 4)
                    .Select(i => new ObjectConstructionArgument(new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = i}))
                    .ToList();

            var constructedObject = new ConstructedConfigurationObject(mock.Object, args, 1);

            Assert.AreEqual("42(0, 1, 2, 3)[1]", Provider().ConvertToString(constructedObject));
        }

        [Test]
        public void Complex_Nested_Constructed_Object_To_String_Test()
        {
            var provider = Provider();

            var mock = new Mock<Construction>();
            mock.Setup(s => s.Id).Returns(42);
            mock.Setup(s => s.OutputTypes).Returns(new List<ConfigurationObjectType> {ConfigurationObjectType.Point, ConfigurationObjectType.Point});

            var looseObjects = Enumerable.Range(0, 3)
                    .Select(i => new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = i})
                    .ToList();

            var firstArgs = new List<ConstructionArgument>
            {
                new ObjectConstructionArgument(looseObjects[0]),
                new ObjectConstructionArgument(looseObjects[1])
            };

            var firstObject = new ConstructedConfigurationObject(mock.Object, firstArgs, 0);
            provider.CacheObject(3, provider.ConvertToString(firstObject));
            firstObject.Id = 3;

            var secondArgs = new List<ConstructionArgument>
            {
                new SetConstructionArgument(
                    new HashSet<ConstructionArgument>
                    {
                        new ObjectConstructionArgument(looseObjects[1]),
                        new ObjectConstructionArgument(looseObjects[2]),
                        new ObjectConstructionArgument(firstObject)
                    })
            };

            var secondObject = new ConstructedConfigurationObject(mock.Object, secondArgs, 1);
            provider.CacheObject(4, provider.ConvertToString(secondObject));
            secondObject.Id = 4;

            var thirdArgs = new List<ConstructionArgument>
            {
                new ObjectConstructionArgument(firstObject),
                new ObjectConstructionArgument(secondObject),
                new SetConstructionArgument(
                    new HashSet<ConstructionArgument>
                    {
                        new SetConstructionArgument(
                            new HashSet<ConstructionArgument>
                            {
                                new ObjectConstructionArgument(firstObject),
                                new ObjectConstructionArgument(looseObjects[2])
                            }),
                        new SetConstructionArgument(
                            new HashSet<ConstructionArgument>
                            {
                                new ObjectConstructionArgument(looseObjects[0]),
                                new ObjectConstructionArgument(looseObjects[1])
                            })
                    })
            };

            var thirdObject = new ConstructedConfigurationObject(mock.Object, thirdArgs, 1);
            var stringVersion = provider.ConvertToString(thirdObject);
            const string expected = "42(42(0, 1)[0], 42({1; 2; 42(0, 1)[0]})[1], {{0; 1}; {2; 42(0, 1)[0]}})[1]";

            Assert.AreEqual(expected, stringVersion);
        }

        [Test]
        public void Forgotten_Cache_Object_Call()
        {
            var provider = Provider();

            var mock = new Mock<Construction>();
            mock.Setup(s => s.Id).Returns(42);
            mock.Setup(s => s.OutputTypes).Returns(new List<ConfigurationObjectType> {ConfigurationObjectType.Point});

            var args = Enumerable.Range(0, 4)
                    .Select(i => new ObjectConstructionArgument(new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = i}))
                    .ToList();

            var constructedObject = new ConstructedConfigurationObject(mock.Object, args, 0);
            var asString = provider.ConvertToString(constructedObject);
            constructedObject.Id = 42;

            var args2 = new List<ConstructionArgument> {new ObjectConstructionArgument(constructedObject)};
            var constructedObject2 = new ConstructedConfigurationObject(mock.Object, args2, 0);

            Assert.Throws<GeneratorException>(() => provider.ConvertToString(constructedObject2));
        }

        [Test]
        public void Calling_To_String_On_Object_With_Id_And_Never_Cached()
        {
            var provider = Provider();

            var mock = new Mock<Construction>();
            mock.Setup(s => s.Id).Returns(42);
            mock.Setup(s => s.OutputTypes).Returns(new List<ConfigurationObjectType> {ConfigurationObjectType.Point});

            var args = Enumerable.Range(0, 4)
                    .Select(i => new ObjectConstructionArgument(new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = i}))
                    .ToList();

            var constructedObject = new ConstructedConfigurationObject(mock.Object, args, 0) {Id = 42};

            Assert.Throws<GeneratorException>(() => provider.ConvertToString(constructedObject));
        }

        [Test]
        public void Clear_Cache_Test()
        {
            var provider = Provider();

            var mock = new Mock<Construction>();
            mock.Setup(s => s.Id).Returns(42);
            mock.Setup(s => s.OutputTypes).Returns(new List<ConfigurationObjectType> {ConfigurationObjectType.Point});

            var args = Enumerable.Range(0, 4)
                    .Select(i => new ObjectConstructionArgument(new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = i}))
                    .ToList();

            var constructedObject = new ConstructedConfigurationObject(mock.Object, args, 0);
            var asString = provider.ConvertToString(constructedObject);
            provider.CacheObject(1, asString);
            provider.ClearCache();
            constructedObject.Id = 1;

            Assert.Throws<GeneratorException>(() => provider.ConvertToString(constructedObject));
        }
    }
}