using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Core.Configurations;
using GeoGen.Core.Constructions.Arguments;
using GeoGen.Core.Utilities;
using GeoGen.Generator.ConfigurationHandling.ConfigurationObjectToString;
using GeoGen.Generator.ConfigurationHandling.ConfigurationToString;
using NUnit.Framework;
using static GeoGen.Generator.Test.TestHelpers.ConfigurationObjects;

namespace GeoGen.Generator.Test.ConfigurationHandling.ConfigurationToString
{
    [TestFixture]
    public class ConfigurationToStringProviderTest
    {
        private static IObjectToStringProvider DefaultObjectProvider()
        {
            return new DefaultObjectToStringProvider();
        }

        private static ConfigurationToStringProvider ConfigurationProvider()
        {
            return new ConfigurationToStringProvider(" | ");
        }

        [Test]
        public void Configuration_Cant_Be_Null()
        {
            var provider = DefaultObjectProvider();
            Assert.Throws<ArgumentNullException>(() => ConfigurationProvider().ConvertToString(null, provider));
        }

        [Test]
        public void Object_To_String_Provider_Cant_Be_Null()
        {
            var looseObjects = Objects(2, ConfigurationObjectType.Point).ToSet();
            var configuration = new Configuration(looseObjects, new List<ConstructedConfigurationObject>());

            Assert.Throws<ArgumentNullException>(() => ConfigurationProvider().ConvertToString(configuration, null));
        }

        [Test]
        public void Simple_Configuration_To_String()
        {
            var looseObjects = Objects(2, ConfigurationObjectType.Point).ToSet();
            var configuration = new Configuration(looseObjects, new List<ConstructedConfigurationObject>());
            var asString = ConfigurationProvider().ConvertToString(configuration, DefaultObjectProvider());

            Assert.AreEqual("", asString);
        }

        [Test]
        public void Complex_Configuration_To_String()
        {
            var looseObjects = Objects(2, ConfigurationObjectType.Point).ToSet();

            var args1 = new List<ConstructionArgument> {new ObjectConstructionArgument(looseObjects.First())};
            var obj1 = ConstructedObject(42, 1, args1, 7);

            var args2 = new List<ConstructionArgument> {new ObjectConstructionArgument(obj1)};
            var obj2 = ConstructedObject(42, 1, args2, 14);

            var configuration = new Configuration(looseObjects, new List<ConstructedConfigurationObject> {obj1, obj2});
            var asString = ConfigurationProvider().ConvertToString(configuration, DefaultObjectProvider());

            Assert.AreEqual("14 | 7", asString);
        }
    }
}