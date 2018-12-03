using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace GeoGen.Utilities.Test.SubsetsProviding
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