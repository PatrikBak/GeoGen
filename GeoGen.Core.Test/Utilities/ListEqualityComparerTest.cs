using System;
using System.Collections.Generic;
using GeoGen.Core.Utilities;
using NUnit.Framework;

namespace GeoGen.Core.Test.Utilities
{
    [TestFixture]
    public class ListEqualityComparerTest
    {
        private static ListEqualityComparer<int> GetTestComparer()
        {
            return new ListEqualityComparer<int>(EqualityComparer<int>.Default);
        }

        [Test]
        public void Test_Comparer_With_Null_Constructor_Argument()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                new ListEqualityComparer<int>(null);
            });
        }

        [Test]
        public void Test_List_With_Different_Counts()
        {
            var list1 = new List<int> {1, 2, 3};
            var list2 = new List<int> { 1, 2 };

            Assert.IsFalse(GetTestComparer().Equals(list1, list2));
        }

        [Test]
        public void Test_List_With_Same_Counts_But_Different_Items()
        {
            var list1 = new List<int> { 1, 2, 3 };
            var list2 = new List<int> { 1, 2, 4 };

            Assert.IsFalse(GetTestComparer().Equals(list1, list2));
        }

        [Test]
        public void Test_Same_Lists()
        {
            var list1 = new List<int> { 1, 2, 3 };
            var list2 = new List<int> { 1, 2, 3 };

            Assert.IsTrue(GetTestComparer().Equals(list1, list2));
        }
    }
}