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
        public void Test_Loose_Objects_Cant_Be_Null()
        {
            Assert.Throws<ArgumentNullException>
            (
                () =>
                {
                    var constructedObjects = new List<ConstructedConfigurationObject>();
                    new Configuration(null, constructedObjects);
                });
        }

        [Test]
        public void Test_Constructed_Objects_Cant_Be_Null()
        {
            Assert.Throws<ArgumentNullException>
            (
                () =>
                {
                    var looseObject = new LooseConfigurationObject(ConfigurationObjectType.Point);
                    var looseObjects = new HashSet<LooseConfigurationObject> {looseObject};
                    new Configuration(looseObjects, null);
                }
            );
        }

        [Test]
        public void Test_Loose_Objects_Cant_Be_Empty()
        {
            Assert.Throws<ArgumentException>
            (
                () =>
                {
                    var looseObjects = new HashSet<LooseConfigurationObject>();
                    var constructedObjects = new List<ConstructedConfigurationObject>();
                    new Configuration(looseObjects, constructedObjects);
                }
            );
        }
    }
}