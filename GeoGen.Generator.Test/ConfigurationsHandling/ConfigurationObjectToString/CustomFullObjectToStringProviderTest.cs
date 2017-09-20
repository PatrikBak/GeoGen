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
    public class CustomFullObjectToStringProviderTest
    {
        private static IObjectIdResolver _resolver;

        private static IObjectIdResolver Resolver()
        {
            var mock = new Mock<IObjectIdResolver>();
            mock.Setup(s => s.ResolveId(It.IsAny<LooseConfigurationObject>()))
                    .Returns<ConfigurationObject>(o => 10 + o.Id ?? throw new Exception());
            mock.Setup(s => s.Id).Returns(1);

            return mock.Object;
        }

        private static CustomFullObjectToStringProvider Provider(IObjectIdResolver resolver = null)
        {
            resolver = resolver ?? Resolver();
            _resolver = resolver;

            var defaultResolver = new DefaultObjectIdResolver();
            var defaultProvider = new DefaultObjectToStringProvider(defaultResolver);
            var defaultArgument = new DefaultArgumentToStringProvider(defaultProvider);

            CustomArgumentToStringProvider provider = null;

            var mock = new Mock<ICustomArgumentToStringProviderFactory>();
            mock.Setup(s => s.GetProvider(It.IsAny<IObjectToStringProvider>()))
                    .Returns<IObjectToStringProvider>
                    (
                        stringProvider => provider ?? (provider = new CustomArgumentToStringProvider(stringProvider, "; "))
                    );

            var argumentsProvider = new ArgumentsListToStringProvider(defaultArgument, ", ");

            return new CustomFullObjectToStringProvider(mock.Object, argumentsProvider, resolver);
        }

        [Test]
        public void Test_Factory_Cant_Be_Null()
        {
            var resolver = SimpleMock<IObjectIdResolver>();
            var provider = SimpleMock<IArgumentsListToStringProvider>();

            Assert.Throws<ArgumentNullException>
            (
                () => new CustomFullObjectToStringProvider(null, provider, resolver)
            );
        }

        [Test]
        public void Test_Arguments_Provider_Cant_Be_Null()
        {
            var factory = SimpleMock<ICustomArgumentToStringProviderFactory>();
            var resolver = SimpleMock<IObjectIdResolver>();

            Assert.Throws<ArgumentNullException>
            (
                () => new CustomFullObjectToStringProvider(factory,null, resolver)
            );
        }

        [Test]
        public void Test_Arguments_Resolver_Cant_Be_Null()
        {
            var factory = SimpleMock<ICustomArgumentToStringProviderFactory>();
            var provider = SimpleMock<IArgumentsListToStringProvider>();

            Assert.Throws<ArgumentNullException>
            (
                () => new CustomFullObjectToStringProvider(factory,provider, null)
            );
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

            Assert.AreEqual("52", Provider().ConvertToString(looseObject));
        }

        [Test]
        public void Test_Constructed_Object_Must_Have_Set_Id()
        {
            var args = Objects(4, ConfigurationObjectType.Point, 0)
                    .Select(o => new ObjectConstructionArgument(o))
                    .Cast<ConstructionArgument>()
                    .ToList();

            var constructedObject = ConstructedObject(42, 1, args);

            Assert.Throws<GeneratorException>(() => Provider().ConvertToString(constructedObject));
        }

        [Test]
        public void Test_Constructed_Object_To_String()
        {
            var args = Objects(4, ConfigurationObjectType.Point, 0)
                    .Select(o => new ObjectConstructionArgument(o) {Id = o.Id})
                    .Cast<ConstructionArgument>()
                    .ToList();

            var constructedObject = ConstructedObject(42, 1, args, 1);

            Assert.AreEqual("42(10, 11, 12, 13)[1]", Provider().ConvertToString(constructedObject));
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

            var firstObject = ConstructedObject(42, 0, firstArgs, 42);

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

            var secondObject = ConstructedObject(42, 1, secondArgs, 43);

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

            var thirdObject = ConstructedObject(42, 1, thirdArgs, 44);
            var stringVersion = provider.ConvertToString(thirdObject);
            const string expected = "42(42(10, 11), 42({11; 12; 42(10, 11)})[1], {{10; 11}; {12; 42(10, 11)}})[1]";

            Assert.AreEqual(expected, stringVersion);
        }

        [Test]
        public void Test_That_Caching_Is_Really_Happening()
        {
            var callCounter = 0;

            var resolverMock = new Mock<IObjectIdResolver>();
            resolverMock.Setup(s => s.ResolveId(It.IsAny<LooseConfigurationObject>()))
                    .Returns<LooseConfigurationObject>
                    (
                        o =>
                        {
                            callCounter++;
                            return o.Id ?? throw new Exception();
                        }
                    );

            var provider = Provider(resolverMock.Object);

            var args = Objects(4, ConfigurationObjectType.Point, 0)
                    .Select(o => new ObjectConstructionArgument(o) {Id = o.Id})
                    .Cast<ConstructionArgument>()
                    .ToList();

            var constructedObject = ConstructedObject(42, 1, args, 42);

            var newArgs = new List<ConstructionArgument>
            {
                new ObjectConstructionArgument(constructedObject) {Id = 5},
                new ObjectConstructionArgument(constructedObject) {Id = 6}
            };

            var newObject = ConstructedObject(42, 0, newArgs, 43);

            provider.ConvertToString(newObject);

            Assert.AreEqual(4, callCounter);
        }

        [Test]
        public void Test_Resolver_Is_Returned()
        {
            var resolver = Provider().Resolver;

            Assert.AreEqual(_resolver, resolver);
        }
    }
}