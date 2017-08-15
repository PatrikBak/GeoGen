using System.Collections.Generic;
using System.Linq;
using GeoGen.Core.Utilities.Variations;
using NUnit.Framework;

namespace GeoGen.Core.Test.Utilities.Variations
{
    [TestFixture]
    public class SubsetsGeneratorTest
    {
        [Test]
        public void TestSubset_6_over_2()
        {
            var subsetsGenerator = new SubsetsGenerator<int>();
            var data = new List<int> {1, 2, 3, 4, 5, 6};
            var sets = subsetsGenerator.Generate(data, 2).ToList();

            var contains = sets.Any(s => s.Contains(2) && s.Contains(4));
            var allHaveSize2 = sets.All(s => s.Count == 2);

            Assert.AreEqual(15, sets.Count);
            Assert.IsTrue(contains);
            Assert.IsTrue(allHaveSize2);
        }

        [Test]
        public void TestSubset_1_over_1()
        {
            var subsetsGenerator = new SubsetsGenerator<int>();
            var data = new List<int> { 1 };
            var sets = subsetsGenerator.Generate(data, 1).ToList();

            var contains = sets.Any(s => s.Contains(1));
            var allHaveSize1 = sets.All(s => s.Count == 1);

            Assert.AreEqual(1, sets.Count);
            Assert.IsTrue(contains);
            Assert.IsTrue(allHaveSize1);
        }

        [Test]
        public void TestSubset_1_over_0()
        {
            var subsetsGenerator = new SubsetsGenerator<int>();
            var data = new List<int> { 1 };
            var sets = subsetsGenerator.Generate(data, 0).ToList();

            var allHaveSize0 = sets.All(s => s.Count == 0);

            Assert.AreEqual(1, sets.Count);
            Assert.IsTrue(allHaveSize0);
        }

        [Test]
        public void TestSubset_10_over_4()
        {
            var subsetsGenerator = new SubsetsGenerator<int>();
            var data = new List<int>(Enumerable.Range(1, 10));
            var sets = subsetsGenerator.Generate(data, 4).ToList();

            var contains = sets.Any(s => s.Contains(2) && s.Contains(4) && s.Contains(10) && s.Contains(5));
            var allOfThemSize4 = sets.All(s => s.Count == 4);

            Assert.AreEqual(210, sets.Count);
            Assert.IsTrue(contains);
            Assert.IsTrue(allOfThemSize4);
        }
    }
}