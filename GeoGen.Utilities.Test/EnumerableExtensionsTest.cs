using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace GeoGen.Utilities.Test
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
            var enumerable = 1.AsEnumerable();
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

        [Test]
        public void Test_To_Set_Equality_Provider_Cant_Be_Null()
        {
            Assert.Throws<ArgumentNullException>(() => new[] {1, 2}.ToSet(null));
        }

        private class Z3Comparer : IEqualityComparer<int>
        {
            public bool Equals(int x, int y)
            {
                return x % 3 == y % 3;
            }

            public int GetHashCode(int obj)
            {
                return obj % 3;
            }
        }

        [Test]
        public void Test_To_Set_With_Custom_Equality_Provider()
        {
            var comparer = new Z3Comparer();

            var set = Enumerable.Range(66, 42).ToSet(comparer);

            Assert.AreEqual(3, set.Count);
            Assert.AreSame(comparer, set.Comparer);
        }

        //[Test]
        //public void Test_Concat_Item()
        //{
        //    var list = new[] {1, 2, 3}.ConcatItem(4).ToList();

        //    Assert.AreEqual(4, list.Count);
        //    Assert.AreEqual(3, list[2]);
        //    Assert.AreEqual(4, list[3]);
        //}
    }
}