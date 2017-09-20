using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Core.Configurations;
using GeoGen.Core.Constructions.Arguments;
using GeoGen.Core.Utilities;
using GeoGen.Generator.ConstructingConfigurations.ConfigurationToString;
using GeoGen.Generator.ConstructingConfigurations.ObjectToString;
using GeoGen.Generator.ConstructingConfigurations.ObjectToString.ObjectIdResolving;
using NUnit.Framework;
using static GeoGen.Generator.Test.TestHelpers.ConfigurationObjects;
using static GeoGen.Generator.Test.TestHelpers.Configurations;

namespace GeoGen.Generator.Test.ConstructingConfigurations.ConfigurationToString
{
    public class ConfigurationToStringProviderTest
    {
        private static ConfigurationToStringProvider ConfigurationProvider()
        {
            return new ConfigurationToStringProvider();
        }

        private static IObjectToStringProvider DefaultProvider()
        {
            var resolver = new DefaultObjectIdResolver();

            return new DefaultObjectToStringProvider(resolver);
        }

        [Test]
        public void Test_Configuration_Cant_Be_Null()
        {
            var provider = DefaultProvider();

            Assert.Throws<ArgumentNullException>(() => ConfigurationProvider().ConvertToString(null, provider));
        }

        [Test]
        public void Test_Object_To_String_Provider_Cant_Be_Null()
        {
            var looseObjects = Objects(2, ConfigurationObjectType.Point).ToSet();
            var configuration = AsConfiguration(looseObjects);

            Assert.Throws<ArgumentNullException>(() => ConfigurationProvider().ConvertToString(configuration, null));
        }

        [Test]
        public void Test_Simple_Configuration_To_String()
        {
            var looseObjects = Objects(2, ConfigurationObjectType.Point).ToSet();
            var configuration = AsConfiguration(looseObjects);
            var asString = ConfigurationProvider().ConvertToString(configuration, DefaultProvider());

            Assert.AreEqual("", asString);
        }

        [Test]
        public void Test_Complex_Configuration_To_String()
        {
            var looseObjects = Objects(2, ConfigurationObjectType.Point).ToSet();

            var args1 = new List<ConstructionArgument> {new ObjectConstructionArgument(looseObjects.First())};
            var obj1 = ConstructedObject(42, 1, args1, 7);

            var args2 = new List<ConstructionArgument> {new ObjectConstructionArgument(obj1)};
            var obj2 = ConstructedObject(42, 1, args2, 14);

            var configuration = new Configuration(looseObjects, new List<ConstructedConfigurationObject> {obj1, obj2});
            var asString = ConfigurationProvider().ConvertToString(configuration, DefaultProvider());

            Assert.AreEqual("14|7", asString);
        }
    }
}