using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Core.Utilities.SubsetsProviding;
using NUnit.Framework;

namespace GeoGen.Core.Test.Utilities.SubsetsProviding
{
    [TestFixture]
    public class SubsetsProviderTest
    {
        private static SubsetsProvider Provider()
        {
            return new SubsetsProvider();
        }

        private static List<int> Elements(int count)
        {
            return Enumerable.Range(5, count).ToList();
        }

        [Test]
        public void Test_List_Cant_Be_Null()
        {
            Assert.Throws<ArgumentNullException>(() => new SubsetsProvider().GetSubsets<int>(null, 1));
        }

        [TestCase(-666)]
        [TestCase(-42)]
        [TestCase(-1)]
        public void Test_Elements_Number_Must_Be_NonNegative(int number)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new SubsetsProvider().GetSubsets(Elements(1), number));
        }

        [TestCase(6)]
        [TestCase(7)]
        [TestCase(42)]
        [TestCase(666)]
        public void Test_Elements_Number_Must_Be_At_Most_List_Count(int number)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new SubsetsProvider().GetSubsets(Elements(5), number));
        }

        [TestCase(0, 0, 1)]
        [TestCase(1, 0, 1)]
        [TestCase(10, 0, 1)]
        [TestCase(1, 1, 1)]
        [TestCase(2, 1, 2)]
        [TestCase(42, 1, 42)]
        [TestCase(666, 1, 666)]
        [TestCase(2, 2, 1)]
        [TestCase(4, 2, 6)]
        [TestCase(3, 3, 1)]
        [TestCase(42, 3, 11480)]
        public void Test_Subsets_Of_Size_k(int elements, int size, int expected)
        {
            var count = Provider().GetSubsets(Elements(elements), size).Count();

            Assert.AreEqual(expected, count);
        }

        [Test]
        public void Test_Subsets_Contains_Given_Combination()
        {
            var contains = Provider().GetSubsets(Elements(4), 3)
                    .Select(enumerable => enumerable.ToArray())
                    .Any(array => array[0] == 5 && array[1] == 7 && array[2] == 8);

            Assert.IsTrue(contains);
        }
    }
}