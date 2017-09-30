using System;
using GeoGen.Core.Configurations;
using GeoGen.Core.Theorems;
using NUnit.Framework;

namespace GeoGen.Core.Test.Theorems
{
    [TestFixture]
    public class SingleTheoremObjectTest
    {
        [Test]
        public void Test_Configuration_Object_Cant_Be_Null()
        {
            Assert.Throws<ArgumentNullException>(() => new SingleTheoremObject(null));
        }

        [Test]
        public void Test_Everything_Is_Set_Up_Correctly()
        {
            var obj = new LooseConfigurationObject(ConfigurationObjectType.Point);
            var theoremObj = new SingleTheoremObject(obj);

            Assert.AreEqual(ConfigurationObjectType.Point, theoremObj.Type);
            Assert.AreEqual(1, theoremObj.InternalObjects.Count);
            Assert.AreSame(obj, theoremObj.InternalObjects[0]);
        }
    }
}