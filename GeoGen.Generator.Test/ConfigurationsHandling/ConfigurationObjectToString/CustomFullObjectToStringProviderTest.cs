using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Core.Configurations;
using GeoGen.Core.Constructions.Arguments;
using GeoGen.Generator.ConfigurationsHandling.ConfigurationObjectToString;
using GeoGen.Generator.ConfigurationsHandling.ConfigurationObjectToString.ConfigurationObjectIdResolving;
using GeoGen.Generator.Constructing.Arguments.ArgumentsToString;
using GeoGen.Generator.Test.TestHelpers;
using Moq;
using NUnit.Framework;
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
                        stringProvider => provider ?? (provider = new CustomArgumentToStringProvider(stringProvider))
                    );

            var argumentsProvider = new ArgumentsListToStringProvider(mock.Object, defaultArgument, ", ");

            return new CustomFullObjectToStringProvider(argumentsProvider, resolver);
        }

        [Test]
        public void Test_Arguments_Provider_Cant_Be_Null()
        {
            var resolver = SimpleMock<IObjectIdResolver>();

            Assert.Throws<ArgumentNullException>
            (
                () => new CustomFullObjectToStringProvider(null, resolver)
            );
        }

        [Test]
        public void Test_Arguments_Resolver_Cant_Be_Null()
        {
            var provider = SimpleMock<IArgumentsListToStringProvider>();

            Assert.Throws<ArgumentNullException>
            (
                () => new CustomFullObjectToStringProvider(provider, null)
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
        public void Test_Constructed_Object_Doesnt_Have_Set_Id()
        {
            var args = ConfigurationObjects.Objects(4, ConfigurationObjectType.Point, 0)
                    .Select(o => new ObjectConstructionArgument(o))
                    .Cast<ConstructionArgument>()
                    .ToList();

            var constructedObject = ConfigurationObjects.ConstructedObject(42, 1, args);

            Assert.Throws<GeneratorException>(() => Provider().ConvertToString(constructedObject));
        }

        [Test]
        public void Test_Constructed_Object_To_String()
        {
            var args = ConfigurationObjects.Objects(4, ConfigurationObjectType.Point, 0)
                    .Select(o => new ObjectConstructionArgument(o))
                    .Cast<ConstructionArgument>()
                    .ToList();

            var constructedObject = ConfigurationObjects.ConstructedObject(42, 1, args, 7);

            Assert.AreEqual("42(10, 11, 12, 13)[1]", Provider().ConvertToString(constructedObject));
        }

        [Test]
        public void Test_Complex_Nested_Constructed_Object_To_String()
        {
            var provider = Provider();

            var looseObjects = ConfigurationObjects.Objects(3, ConfigurationObjectType.Point, 0);

            var firstArgs = new List<ConstructionArgument>
            {
                new ObjectConstructionArgument(looseObjects[0]),
                new ObjectConstructionArgument(looseObjects[1])
            };

            var firstObject = ConfigurationObjects.ConstructedObject(42, 0, firstArgs, 42);

            var secondArgs = new List<ConstructionArgument>
            {
                new SetConstructionArgument
                (
                    new HashSet<ConstructionArgument>
                    {
                        new ObjectConstructionArgument(looseObjects[1]),
                        new ObjectConstructionArgument(looseObjects[2]),
                        new ObjectConstructionArgument(firstObject)
                    }
                )
            };

            var secondObject = ConfigurationObjects.ConstructedObject(42, 1, secondArgs, 43);

            var thirdArgs = new List<ConstructionArgument>
            {
                new ObjectConstructionArgument(firstObject),
                new ObjectConstructionArgument(secondObject),
                new SetConstructionArgument
                (
                    new HashSet<ConstructionArgument>
                    {
                        new SetConstructionArgument
                        (
                            new HashSet<ConstructionArgument>
                            {
                                new ObjectConstructionArgument(firstObject),
                                new ObjectConstructionArgument(looseObjects[2])
                            }
                        ),
                        new SetConstructionArgument
                        (
                            new HashSet<ConstructionArgument>
                            {
                                new ObjectConstructionArgument(looseObjects[0]),
                                new ObjectConstructionArgument(looseObjects[1])
                            }
                        )
                    })
            };

            var thirdObject = ConfigurationObjects.ConstructedObject(42, 1, thirdArgs, 44);
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

            var args = ConfigurationObjects.Objects(4, ConfigurationObjectType.Point, 0)
                    .Select(o => new ObjectConstructionArgument(o))
                    .Cast<ConstructionArgument>()
                    .ToList();

            var constructedObject = ConfigurationObjects.ConstructedObject(42, 1, args, 42);

            var newArgs = new List<ConstructionArgument>
            {
                new ObjectConstructionArgument(constructedObject),
                new ObjectConstructionArgument(constructedObject)
            };

            var newObject = ConfigurationObjects.ConstructedObject(42, 0, newArgs, 43);

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