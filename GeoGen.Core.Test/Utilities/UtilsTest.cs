using GeoGen.Core.Configurations;
using GeoGen.Core.Utilities;
using NUnit.Framework;

namespace GeoGen.Core.Test.Utilities
{
    [TestFixture]
    public class UtilsTest
    {
        [Test]
        public void Test_Swap_With_Int()
        {
            var i = 1;
            var j = 2;
            Utils.Swap(ref i, ref j);

            Assert.AreEqual(2, i);
            Assert.AreEqual(1, j);
        }

        [Test]
        public void Test_Swap_With_Null()
        {
            var o1 = (LooseConfigurationObject) null;
            var o2 = new LooseConfigurationObject(ConfigurationObjectType.Point);
            Utils.Swap(ref o1, ref o2);

            Assert.NotNull(o1);
            Assert.IsNull(o2);
        }
    }
}