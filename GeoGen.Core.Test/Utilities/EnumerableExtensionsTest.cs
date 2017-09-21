using System.Collections.Generic;
using System.Linq;
using GeoGen.Core.Utilities;
using NUnit.Framework;

namespace GeoGen.Core.Test.Utilities
{
    [TestFixture]
    public class EnumerableExtensionsTest
    {
        [Test]
        public void Test_Empty_For_Empty_Enumerable()
        {
            var list = new List<int>();
            Assert.IsTrue(list.Empty());
        }

        [Test]
        public void Test_Empty_For_Non_Empty_Enumerable()
        {
            var list = new List<int> {1, 2};
            Assert.IsFalse(list.Empty());
        }

        [Test]
        public void Test_Single_Item_As_Enumerable()
        {
            var enumerable = 1.SingleItemAsEnumerable();
            Assert.NotNull(enumerable);

            var list = enumerable.ToList();

            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(1, list[0]);
        }

        [Test]
        public void Test_To_Set_Not_Empty_Enumerable()
        {
            var list = new List<int> {1, 1, 42, 666, 2, 42};
            var set = list.ToSet();

            Assert.NotNull(set);
            Assert.IsTrue(set.Contains(1));
            Assert.IsTrue(set.Contains(2));
            Assert.IsTrue(set.Contains(42));
            Assert.IsTrue(set.Contains(666));
            Assert.AreEqual(4, set.Count);
        }

        [Test]
        public void Test_To_Set_With_Empty_Enumerable()
        {
            var set = Enumerable.Empty<int>().ToSet();

            Assert.NotNull(set);
            Assert.AreEqual(0, set.Count);
        }
    }
}