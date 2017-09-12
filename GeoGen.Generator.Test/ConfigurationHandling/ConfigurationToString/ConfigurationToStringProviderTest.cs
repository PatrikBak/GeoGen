using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Core.Configurations;
using GeoGen.Core.Constructions;
using GeoGen.Core.Constructions.Arguments;
using GeoGen.Generator.ConfigurationHandling.ConfigurationObjectToString;
using GeoGen.Generator.ConfigurationHandling.ConfigurationToString;
using Moq;
using NUnit.Framework;

namespace GeoGen.Generator.Test.ConfigurationHandling.ConfigurationToString
{
    [TestFixture]
    public class ConfigurationToStringProviderTest
    {
        private static IConfigurationObjectToStringProvider DefaultObjectProvider()
        {
            return new DefaultConfigurationObjectIdToStringProvider();
        }

        private static ConfigurationToStringProvider ConfigurationProvider()
        {
            return new ConfigurationToStringProvider(" | ");
        }

        [Test]
        public void Configuration_Cant_Be_Null()
        {
            Assert.Throws<ArgumentNullException>(() => ConfigurationProvider().ConvertToString(null, DefaultObjectProvider()));
        }

        [Test]
        public void Object_To_String_Provider_Cant_Be_Null()
        {
            var looseObjects = new HashSet<LooseConfigurationObject>
            {
                new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 1},
                new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 2}
            };

            var configuration = new Configuration(looseObjects, new List<ConstructedConfigurationObject>());

            Assert.Throws<ArgumentNullException>(() => ConfigurationProvider().ConvertToString(configuration, null));
        }

        [Test]
        public void Simple_Configuration_To_String()
        {
            var looseObjects = new HashSet<LooseConfigurationObject>
            {
                new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 1},
                new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 2}
            };

            var configuration = new Configuration(looseObjects, new List<ConstructedConfigurationObject>());
            var asString = ConfigurationProvider().ConvertToString(configuration, DefaultObjectProvider());

            Assert.AreEqual("1 | 2", asString);
        }

        [Test]
        public void Complex_Configuration_To_String()
        {
            var looseObjects = new HashSet<LooseConfigurationObject>
            {
                new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 1},
                new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 2}
            };

            var mock = new Mock<Construction>();
            mock.Setup(s => s.Id).Returns(42);
            mock.Setup(s => s.OutputTypes).Returns(
                new List<ConfigurationObjectType>
                {
                    ConfigurationObjectType.Point,
                    ConfigurationObjectType.Point
                });

            var args1 = new List<ConstructionArgument> { new ObjectConstructionArgument(looseObjects.First()) };
            var obj1 = new ConstructedConfigurationObject(mock.Object, args1, 1) { Id = 7 };

            var args2 = new List<ConstructionArgument> { new ObjectConstructionArgument(obj1) };
            var obj2 = new ConstructedConfigurationObject(mock.Object, args2, 1) { Id = 14 };

            var configuration = new Configuration(looseObjects, new List<ConstructedConfigurationObject>{obj1, obj2});
            var asString = ConfigurationProvider().ConvertToString(configuration, DefaultObjectProvider());

            Assert.AreEqual("1 | 14 | 2 | 7", asString);
        }
    }
}