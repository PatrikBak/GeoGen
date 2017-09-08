using System;
using System.Collections.Generic;
using GeoGen.Core.Configurations;
using NUnit.Framework;

namespace GeoGen.Core.Test.Configurations
{
    [TestFixture]
    public class ConfigurationTest
    {
        [Test]
        public void Loose_Objects_Cannot_Be_Null()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var constructedObjects = new List<ConstructedConfigurationObject>();
                var configuration = new Configuration(null, constructedObjects);
            });
        }

        [Test]
        public void Constructed_Objects_Cannot_Be_Empty()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var looseObject = new LooseConfigurationObject(ConfigurationObjectType.Point);
                var looseObjects = new HashSet<LooseConfigurationObject> {looseObject};
                var configuration = new Configuration(looseObjects, null);
            });
        }

        [Test]
        public void Loose_Objects_Cannot_Be_Empty()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                var looseObjects = new HashSet<LooseConfigurationObject>();
                var constructedObjects = new List<ConstructedConfigurationObject>();
                var configuration = new Configuration(looseObjects, constructedObjects);
            });
        }
    }
}