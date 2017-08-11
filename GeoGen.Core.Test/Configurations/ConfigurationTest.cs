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
        public void Configuration_Construction_Null_LooseObjects()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var constructedObjects = new HashSet<ConstructedConfigurationObject>();
                var configuration = new Configuration(null, constructedObjects);
            });
        }

        [Test]
        public void Configuration_Construction_Null_ConstructedObjects()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var looseObject = new LooseConfigurationObject(ConfigurationObjectType.Point);
                var looseObjects = new HashSet<LooseConfigurationObject> {looseObject};
                var configuration = new Configuration(looseObjects, null);
            });
        }

        [Test]
        public void Configuration_Construction_Empty_LooseObjects()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                var looseObjects = new HashSet<LooseConfigurationObject>();
                var constructedObjects = new HashSet<ConstructedConfigurationObject>();
                var configuration = new Configuration(looseObjects, constructedObjects);
            });
        }
    }
}