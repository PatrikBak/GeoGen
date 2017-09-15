using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Core.Configurations;
using GeoGen.Core.Constructions.Arguments;
using GeoGen.Generator.ConfigurationHandling.ConfigurationObjectToString;
using GeoGen.Generator.ConfigurationHandling.ConfigurationObjectToString.ObjectIdResolving;
using GeoGen.Generator.Constructing.Arguments.ArgumentsToString;
using NUnit.Framework;
using static GeoGen.Generator.Test.TestHelpers.ConfigurationObjects;
using static GeoGen.Generator.Test.TestHelpers.Utilities;

namespace GeoGen.Generator.Test.ConfigurationHandling.ConfigurationObjectToString
{
    [TestFixture]
    public class DefaultComplexConfigurationObjectToStringProviderTest
    {
        private static DefaultFullObjectToStringProvider Provider()
        {
            var objectToStringProvider = new DefaultObjectToStringProvider();
            var argumentsProvider = new ArgumentsToStringProvider(objectToStringProvider, ", ", "; ");
            var defaultResolver = new DefaultObjectIdResolver();

            return new DefaultFullObjectToStringProvider(argumentsProvider, defaultResolver);
        }

        [Test]
        public void Arguments_Provider_Cant_Be_Null()
        {
            var resolver = SimpleMock<DefaultObjectIdResolver>();

            Assert.Throws<ArgumentNullException>
            (
                () => new DefaultFullObjectToStringProvider(null, resolver)
            );
        }

        [Test]
        public void Default_Resolver_Cant_Be_Null()
        {
            var provider = SimpleMock<IArgumentsToStringProvider>();

            Assert.Throws<ArgumentNullException>
            (
                () => new DefaultFullObjectToStringProvider(provider, null)
            );
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
            var args = Objects(4, ConfigurationObjectType.Point, 0)
                    .Select(o => new ObjectConstructionArgument(o))
                    .Cast<ConstructionArgument>()
                    .ToList();

            var constructedObject = ConstructedObject(42, 1, args);

            Assert.AreEqual("42(0, 1, 2, 3)[1]", Provider().ConvertToString(constructedObject));
        }

        [Test]
        public void Complex_Nested_Constructed_Object_To_String_Test()
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
            const string expected = "42(42(0, 1), 42({1; 2; 42(0, 1)})[1], {{0; 1}; {2; 42(0, 1)}})[1]";

            Assert.AreEqual(expected, stringVersion);
        }

        [Test]
        public void Forgotten_Cache_Object_Call()
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
            var constructedObject2 = ConstructedObject(42, 0, args2, 7);

            Assert.Throws<GeneratorException>(() => provider.ConvertToString(constructedObject2));
        }

        [Test]
        public void Calling_To_String_On_Object_With_Id_And_Never_Cached()
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
        public void Clear_Cache_Test()
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
    }
}