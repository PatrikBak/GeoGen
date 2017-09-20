using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Core.Configurations;
using GeoGen.Core.Constructions.Arguments;
using GeoGen.Generator.ConstructingConfigurations.ObjectToString;
using GeoGen.Generator.ConstructingConfigurations.ObjectToString.ObjectIdResolving;
using GeoGen.Generator.ConstructingObjects.Arguments.ArgumentsListToString;
using NUnit.Framework;
using static GeoGen.Generator.Test.TestHelpers.ConfigurationObjects;
using static GeoGen.Generator.Test.TestHelpers.Utilities;

namespace GeoGen.Generator.Test.ConstructingConfigurations.ObjectToString
{
    [TestFixture]
    public class DefaultFullObjectToStringProviderTest
    {
        private static DefaultObjectIdResolver _defaultResolver;

        private static DefaultFullObjectToStringProvider Provider()
        {
            _defaultResolver = new DefaultObjectIdResolver();
            var defaultProvider = new DefaultObjectToStringProvider(_defaultResolver);
            var argumentsProvider = new ArgumentsListToStringProvider(defaultProvider);

            return new DefaultFullObjectToStringProvider(argumentsProvider, _defaultResolver);
        }

        [Test]
        public void Test_Arguments_Provider_Cant_Be_Null()
        {
            var resolver = SimpleMock<DefaultObjectIdResolver>();

            Assert.Throws<ArgumentNullException>(() => new DefaultFullObjectToStringProvider(null, resolver));
        }

        [Test]
        public void Test_Default_Resolver_Cant_Be_Null()
        {
            var provider = SimpleMock<IArgumentsListToStringProvider>();

            Assert.Throws<ArgumentNullException>(() => new DefaultFullObjectToStringProvider(provider, null));
        }

        [Test]
        public void Test_Passed_Object_Cant_Be_Null()
        {
            Assert.Throws<ArgumentNullException>(() => Provider().ConvertToString(null));
        }

        [Test]
        public void Test_Loose_Object_To_String()
        {
            var looseObject = new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 42};

            Assert.AreEqual("42", Provider().ConvertToString(looseObject));
        }

        [Test]
        public void Test_Constructed_Object_To_String()
        {
            var args = Objects(4, ConfigurationObjectType.Point, 0)
                    .Select(o => new ObjectConstructionArgument(o))
                    .Cast<ConstructionArgument>()
                    .ToList();

            var constructedObject = ConstructedObject(42, 1, args);

            Assert.AreEqual("42(0,1,2,3)[1]", Provider().ConvertToString(constructedObject));
        }

        [Test]
        public void Test_Complex_Nested_Constructed_Object_To_String()
        {
            var provider = Provider();

            var looseObjects = Objects(3, ConfigurationObjectType.Point, 0);

            var firstArgs = new List<ConstructionArgument>
            {
                new ObjectConstructionArgument(looseObjects[0]),
                new ObjectConstructionArgument(looseObjects[1])
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
                        new ObjectConstructionArgument(looseObjects[1]),
                        new ObjectConstructionArgument(looseObjects[2]),
                        new ObjectConstructionArgument(firstObject)
                    }
                )
            };

            var secondObject = ConstructedObject(42, 1, secondArgs);
            var secondObjectString = provider.ConvertToString(secondObject);
            provider.CacheObject(4, secondObjectString);
            secondObject.Id = 4;

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

            var thirdObject = ConstructedObject(42, 1, thirdArgs);
            var stringVersion = provider.ConvertToString(thirdObject);
            const string expected = "42(42(0,1),42({1;2;42(0,1)})[1],{{0;1};{2;42(0,1)}})[1]";

            Assert.AreEqual(expected, stringVersion);
        }

        [Test]
        public void Test_Cant_Call_Cache_Method_Twice()
        {
            var provider = Provider();

            var args = Objects(4, ConfigurationObjectType.Point, 0)
                    .Select(o => new ObjectConstructionArgument(o))
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
                    .Select(o => new ObjectConstructionArgument(o))
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
                    .Select(o => new ObjectConstructionArgument(o))
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
                    .Select(o => new ObjectConstructionArgument(o))
                    .Cast<ConstructionArgument>()
                    .ToList();

            var constructedObject = ConstructedObject(42, 1, args, 2);
            var provider = Provider();

            provider.CacheObject(2, "test");
            var asString = provider.ConvertToString(constructedObject);

            Assert.AreEqual("test", asString);
        }

        [Test]
        public void Test_Cache_Is_Cleared()
        {
            var provider = Provider();

            var args = Objects(4, ConfigurationObjectType.Point, 0)
                    .Select(o => new ObjectConstructionArgument(o))
                    .Cast<ConstructionArgument>()
                    .ToList();

            var constructedObject = ConstructedObject(42, 1, args);
            var asString = provider.ConvertToString(constructedObject);
            provider.CacheObject(1, asString);
            provider.ClearCache();
            constructedObject.Id = 1;

            Assert.Throws<GeneratorException>(() => provider.ConvertToString(constructedObject));
        }

        [Test]
        public void Test_Resolver_Is_Returned()
        {
            var resolver = Provider().Resolver;

            Assert.AreEqual(_defaultResolver, resolver);
        }
    }
}