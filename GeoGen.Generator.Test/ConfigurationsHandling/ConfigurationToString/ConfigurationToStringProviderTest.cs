using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Core.Configurations;
using GeoGen.Core.Constructions.Arguments;
using GeoGen.Core.Utilities;
using GeoGen.Generator.ConfigurationsHandling.ConfigurationObjectToString;
using GeoGen.Generator.ConfigurationsHandling.ConfigurationObjectToString.ConfigurationObjectIdResolving;
using GeoGen.Generator.ConfigurationsHandling.ConfigurationToString;
using GeoGen.Generator.Test.ConfigurationsHandling.ConfigurationObjectToString.ConfigurationObjectIdResolving;
using GeoGen.Generator.Test.TestHelpers;
using NUnit.Framework;

namespace GeoGen.Generator.Test.ConfigurationsHandling.ConfigurationToString
{
    [TestFixture]
    public class ConfigurationToStringProviderTest
    {
        private static IObjectToStringProvider DefaultObjectProvider()
        {
            var resolver = new DefaultObjectIdResolver();

            return new DefaultObjectToStringProvider(resolver);
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
            var looseObjects = ConfigurationObjects.Objects(2, ConfigurationObjectType.Point).ToSet();
            var configuration = new Configuration(looseObjects, new List<ConstructedConfigurationObject>());

            Assert.Throws<ArgumentNullException>(() => ConfigurationProvider().ConvertToString(configuration, null));
        }

        [Test]
        public void Simple_Configuration_To_String()
        {
            var looseObjects = ConfigurationObjects.Objects(2, ConfigurationObjectType.Point).ToSet();
            var configuration = new Configuration(looseObjects, new List<ConstructedConfigurationObject>());
            var asString = ConfigurationProvider().ConvertToString(configuration, DefaultObjectProvider());

            Assert.AreEqual("", asString);
        }

        [Test]
        public void Complex_Configuration_To_String()
        {
            var looseObjects = ConfigurationObjects.Objects(2, ConfigurationObjectType.Point).ToSet();

            var args1 = new List<ConstructionArgument> {new ObjectConstructionArgument(looseObjects.First())};
            var obj1 = ConfigurationObjects.ConstructedObject(42, 1, args1, 7);

            var args2 = new List<ConstructionArgument> {new ObjectConstructionArgument(obj1)};
            var obj2 = ConfigurationObjects.ConstructedObject(42, 1, args2, 14);

            var configuration = new Configuration(looseObjects, new List<ConstructedConfigurationObject> {obj1, obj2});
            var asString = ConfigurationProvider().ConvertToString(configuration, DefaultObjectProvider());

            Assert.AreEqual("14 | 7", asString);
        }
    }
}