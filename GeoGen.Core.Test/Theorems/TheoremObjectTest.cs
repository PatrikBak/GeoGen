using System;
using GeoGen.Core.Configurations;
using GeoGen.Core.Theorems;
using NUnit.Framework;

namespace GeoGen.Core.Test.Theorems
{
    [TestFixture]
    public class TheoremObjectTest
    {
        [Test]
        public void Test_Objects_Cant_Be_Null()
        {
            Assert.Throws<ArgumentNullException>(() => new TheoremObject(null, TheoremObjectSignature.LineGivenByPoints));
        }

        [Test]
        public void Test_Object_Cant_Be_Null()
        {
            Assert.Throws<ArgumentNullException>(() => new TheoremObject(null));
        }

        [Test]
        public void Test_Constructor_For_Single_Object()
        {
            var obj = new LooseConfigurationObject(ConfigurationObjectType.Point);
            var theoremObject = new TheoremObject(obj);

            Assert.AreEqual(TheoremObjectSignature.SingleObject, theoremObject.Type);
            Assert.AreEqual(1, theoremObject.InternalObjects.Count);
            Assert.IsTrue(theoremObject.InternalObjects.Contains(obj));
        }
    }
}