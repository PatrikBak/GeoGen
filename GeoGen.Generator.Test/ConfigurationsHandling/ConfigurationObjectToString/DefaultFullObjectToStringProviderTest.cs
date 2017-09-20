using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Core.Configurations;
using GeoGen.Core.Constructions.Arguments;
using GeoGen.Generator.ConfigurationsHandling.ConfigurationObjectToString;
using GeoGen.Generator.ConfigurationsHandling.ConfigurationObjectToString.ConfigurationObjectIdResolving;
using GeoGen.Generator.ConstructingObjects.Arguments.ArgumentsToString;
using Moq;
using NUnit.Framework;
using static GeoGen.Generator.Test.TestHelpers.ConfigurationObjects;
using static GeoGen.Generator.Test.TestHelpers.Utilities;

namespace GeoGen.Generator.Test.ConfigurationsHandling.ConfigurationObjectToString
{
    [TestFixture]
    public class DefaultFullObjectToStringProviderTest
    {
        private static DefaultObjectIdResolver _defaultResolver;

        private static DefaultFullObjectToStringProvider Provider()
        {
            _defaultResolver = new DefaultObjectIdResolver();
            var defaultProvider = new DefaultObjectToStringProvider(_defaultResolver);
            var defaultArgument = new DefaultArgumentToStringProvider(defaultProvider);

            CustomArgumentToStringProvider provider = null;

            var mock = new Mock<ICustomArgumentToStringProviderFactory>();
            mock.Setup(s => s.GetProvider(It.IsAny<IObjectToStringProvider>()))
                    .Returns<IObjectToStringProvider>
                    (
                        stringProvider => provider ?? (provider = new CustomArgumentToStringProvider(stringProvider, "; "))
                    );

            var argumentsProvider = new ArgumentsListToStringProvider(defaultArgument, ", ");

            return new DefaultFullObjectToStringProvider(mock.Object, argumentsProvider, _defaultResolver);
        }

        [Test]
        public void Test_Factory_Cant_Be_Null()
        {
            var resolver = SimpleMock<DefaultObjectIdResolver>();
            var provider = SimpleMock<IArgumentsListToStringProvider>();

            Assert.Throws<ArgumentNullException>(() => new DefaultFullObjectToStringProvider(null, provider, resolver));
        }

        [Test]
        public void Test_Arguments_Provider_Cant_Be_Null()
        {
            var factory = SimpleMock<ICustomArgumentToStringProviderFactory>();
            var resolver = SimpleMock<DefaultObjectIdResolver>();

            Assert.Throws<ArgumentNullException>(() => new DefaultFullObjectToStringProvider(factory, null, resolver));
        }

        [Test]
        public void Test_Default_Resolver_Cant_Be_Null()
        {
            var factory = SimpleMock<ICustomArgumentToStringProviderFactory>();
            var provider = SimpleMock<IArgumentsListToStringProvider>();

            Assert.Throws<ArgumentNullException>(() => new DefaultFullObjectToStringProvider(factory, provider, null));
        }

        [Test]
        public void Test_Object_Cant_Be_Null()
        {
            Assert.Throws<ArgumentNullException>(() => Provider().ConvertToString(null));
        }

        [Test]
        public void Test_Loose_Objects_To_String()
        {
            var looseObject = new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 42};

            Assert.AreEqual("42", Provider().ConvertToString(looseObject));
        }

        [Test]
        public void Test_Constructed_Object_To_String()
        {
            var args = Objects(4, ConfigurationObjectType.Point, 0)
                    .Select(o => new ObjectConstructionArgument(o) {Id = o.Id})
                    .Cast<ConstructionArgument>()
                    .ToList();

            var constructedObject = ConstructedObject(42, 1, args);

            Assert.AreEqual("42(0, 1, 2, 3)[1]", Provider().ConvertToString(constructedObject));
        }

        [Test]
        public void Test_Complex_Nested_Constructed_Object_To_String()
        {
            var provider = Provider();

            var looseObjects = Objects(3, ConfigurationObjectType.Point, 0);

            var firstArgs = new List<ConstructionArgument>
            {
                new ObjectConstructionArgument(looseObjects[0]) {Id = 1},
                new ObjectConstructionArgument(looseObjects[1]) {Id = 2}
            };

            var firstObject = ConstructedObject(42, 0, firstArgs);
            var firstObjectString = provider.ConvertToString(firstObject);
            provider.CacheObject(3, firstObjectString);
            firstObject.Id = 3;

            var secondArgs = new List<ConstructionArgument>
            {
                new SetConstructionArgument
                (
                    new HashSet<ConstructionArgument>
                    {
                        new ObjectConstructionArgument(looseObjects[1]) {Id = 2},
                        new ObjectConstructionArgument(looseObjects[2]) {Id = 3},
                        new ObjectConstructionArgument(firstObject) {Id = 4}
                    }
                ) {Id = 5}
            };

            var secondObject = ConstructedObject(42, 1, secondArgs);
            var secondObjectString = provider.ConvertToString(secondObject);
            provider.CacheObject(4, secondObjectString);
            secondObject.Id = 4;

            var thirdArgs = new List<ConstructionArgument>
            {
                new ObjectConstructionArgument(firstObject) {Id = 4},
                new ObjectConstructionArgument(secondObject) {Id = 6},
                new SetConstructionArgument
                (
                    new HashSet<ConstructionArgument>
                    {
                        new SetConstructionArgument
                        (
                            new HashSet<ConstructionArgument>
                            {
                                new ObjectConstructionArgument(firstObject) {Id = 4},
                                new ObjectConstructionArgument(looseObjects[2]) {Id = 3}
                            }
                        ) {Id = 7},
                        new SetConstructionArgument
                        (
                            new HashSet<ConstructionArgument>
                            {
                                new ObjectConstructionArgument(looseObjects[0]) {Id = 1},
                                new ObjectConstructionArgument(looseObjects[1]) {Id = 2}
                            }
                        ) {Id = 8}
                    }) {Id = 9}
            };

            var thirdObject = ConstructedObject(42, 1, thirdArgs);
            var stringVersion = provider.ConvertToString(thirdObject);
            const string expected = "42(42(0, 1), 42({1; 2; 42(0, 1)})[1], {{0; 1}; {2; 42(0, 1)}})[1]";

            Assert.AreEqual(expected, stringVersion);
        }

        [Test]
        public void Test_Double_Caching_Call()
        {
            var provider = Provider();

            var args = Objects(4, ConfigurationObjectType.Point, 0)
                    .Select(o => new ObjectConstructionArgument(o) {Id = o.Id})
                    .Cast<ConstructionArgument>()
                    .ToList();

            var constructedObject = ConstructedObject(42, 1, args);
            var asString = provider.ConvertToString(constructedObject);
            constructedObject.Id = 42;
            provider.CacheObject(42, asString);

            Assert.Throws<GeneratorException>(() => provider.CacheObject(42, asString));
        }

        [Test]
        public void Test_Calling_To_String_On_Object_With_Id_And_Never_Cached()
        {
            var provider = Provider();

            var args = Objects(4, ConfigurationObjectType.Point, 0)
                    .Select(o => new ObjectConstructionArgument(o) {Id = o.Id})
                    .Cast<ConstructionArgument>()
                    .ToList();

            var constructedObject = ConstructedObject(42, 1, args, 42);

            Assert.Throws<GeneratorException>(() => provider.ConvertToString(constructedObject));
        }

        [Test]
        public void Test_Forgotten_Cache_Object_Call()
        {
            var provider = Provider();

            var args = Objects(4, ConfigurationObjectType.Point, 0)
                    .Select(o => new ObjectConstructionArgument(o) {Id = o.Id})
                    .Cast<ConstructionArgument>()
                    .ToList();

            var constructedObject = ConstructedObject(42, 1, args);
            provider.ConvertToString(constructedObject);
            constructedObject.Id = 42;

            var args2 = new List<ConstructionArgument> {new ObjectConstructionArgument(constructedObject)};
            var constructedObject2 = ConstructedObject(42, 0, args2);

            Assert.Throws<GeneratorException>(() => provider.ConvertToString(constructedObject2));
        }

        [Test]
        public void Test_Caching_Is_Happening()
        {
            var args = Objects(4, ConfigurationObjectType.Point, 0)
                    .Select(o => new ObjectConstructionArgument(o) {Id = o.Id})
                    .Cast<ConstructionArgument>()
                    .ToList();

            var constructedObject = ConstructedObject(42, 1, args, 2);
            var provider = Provider();

            provider.CacheObject(2, "test");
            var asString = provider.ConvertToString(constructedObject);

            Assert.AreEqual("test", asString);
        }

        [Test]
        public void Test_Resolver_Is_Returned()
        {
            var resolver = Provider().Resolver;

            Assert.AreEqual(_defaultResolver, resolver);
        }
    }
}