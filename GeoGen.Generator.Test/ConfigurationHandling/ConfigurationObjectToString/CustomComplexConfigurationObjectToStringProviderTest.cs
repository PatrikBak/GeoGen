using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Core.Configurations;
using GeoGen.Core.Constructions;
using GeoGen.Core.Constructions.Arguments;
using GeoGen.Generator.ConfigurationHandling.ConfigurationObjectToString;
using GeoGen.Generator.ConfigurationHandling.ConfigurationObjectToString.LooseObjectIdResolving;
using GeoGen.Generator.Constructing.Arguments.ArgumentsToString;
using Moq;
using NUnit.Framework;

namespace GeoGen.Generator.Test.ConfigurationHandling.ConfigurationObjectToString
{
    [TestFixture]
    public class CustomComplexConfigurationObjectToStringProviderTest
    {
        private static IArgumentsToStringProvider ArgumentsProvider()
        {
            return new ArgumentsToStringProvider(", ", "; ");
        }

        private static ILooseConfigurationObjectIdResolver Resolver()
        {
            var mock = new Mock<ILooseConfigurationObjectIdResolver>();
            mock.Setup(s => s.ResolveId(It.IsAny<LooseConfigurationObject>()))
                    .Returns<ConfigurationObject>(o => 10 + o.Id.Value);

            return mock.Object;
        }

        private static CustomComplexConfigurationObjectToStringProvider Provider()
        {
            return new CustomComplexConfigurationObjectToStringProvider(ArgumentsProvider(), Resolver());
        }

        [Test]
        public void Arguments_Provider_Cant_Be_Null()
        {
            Assert.Throws<ArgumentNullException>(() => new CustomComplexConfigurationObjectToStringProvider(null, Resolver()));
        }

        [Test]
        public void Arguments_Resolver_Cant_Be_Null()
        {
            Assert.Throws<ArgumentNullException>(() => new CustomComplexConfigurationObjectToStringProvider(ArgumentsProvider(), null));
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

            Assert.AreEqual("52", Provider().ConvertToString(looseObject));
        }

        [Test]
        public void Constructed_Object_Doesnt_Have_Set_Id()
        {
            var mock = new Mock<Construction>();
            mock.Setup(s => s.Id).Returns(42);
            mock.Setup(s => s.OutputTypes).Returns(new List<ConfigurationObjectType> {ConfigurationObjectType.Point, ConfigurationObjectType.Point});

            var args = Enumerable.Range(0, 4)
                    .Select(i => new ObjectConstructionArgument(new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = i}))
                    .ToList();

            var constructedObject = new ConstructedConfigurationObject(mock.Object, args, 1);

            Assert.Throws<GeneratorException>(() => Provider().ConvertToString(constructedObject));
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

            var constructedObject = new ConstructedConfigurationObject(mock.Object, args, 1) {Id = 7};

            Assert.AreEqual("42(10, 11, 12, 13)[1]", Provider().ConvertToString(constructedObject));
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

            var firstObject = new ConstructedConfigurationObject(mock.Object, firstArgs, 0) {Id = 42};

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

            var secondObject = new ConstructedConfigurationObject(mock.Object, secondArgs, 1) {Id = 43};

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

            var thirdObject = new ConstructedConfigurationObject(mock.Object, thirdArgs, 1) {Id = 44};
            var stringVersion = provider.ConvertToString(thirdObject);
            const string expected = "42(42(10, 11)[0], 42({11; 12; 42(10, 11)[0]})[1], {{10; 11}; {12; 42(10, 11)[0]}})[1]";
            
            Assert.AreEqual(expected, stringVersion);
        }

        [Test]
        public void Test_That_Caching_Is_Really_Happening()
        {
            var callCounter = 0;

            var resolverMock = new Mock<ILooseConfigurationObjectIdResolver>();
            resolverMock.Setup(s => s.ResolveId(It.IsAny<LooseConfigurationObject>()))
                    .Returns<LooseConfigurationObject>(
                        o =>
                        {
                            callCounter++;
                            return o.Id.Value;
                        });

            var provider = new CustomComplexConfigurationObjectToStringProvider(ArgumentsProvider(), resolverMock.Object);

            var mock = new Mock<Construction>();
            mock.Setup(s => s.Id).Returns(42);
            mock.Setup(s => s.OutputTypes).Returns(new List<ConfigurationObjectType> {ConfigurationObjectType.Point});

            var args = Enumerable.Range(0, 4)
                    .Select(i => new ObjectConstructionArgument(new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = i}))
                    .ToList();

            var constructedObject = new ConstructedConfigurationObject(mock.Object, args, 0) {Id = 42};

            var newArgs = new List<ConstructionArgument>
            {
                new ObjectConstructionArgument(constructedObject),
                new ObjectConstructionArgument(constructedObject)
            };

            var newObject = new ConstructedConfigurationObject(mock.Object, newArgs, 0) {Id = 43};

            provider.ConvertToString(newObject);

            Assert.AreEqual(4, callCounter);
        }
    }
}