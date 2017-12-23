using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Utilities;
using NUnit.Framework;

namespace GeoGen.Core.Test.Utilities.VariationsProviding
{
    [TestFixture]
    public class VariationsProviderTest
    {
        private static IVariationsProvider Provider()
        {
            return new VariationsProvider();
        }

        [Test]
        public void Test_Variantions_2_of_4()
        {
            var provider = Provider();
            var variations = provider.GetVariations(new List<int> {1, 2, 3, 4}, 2)
                    .Select(variaton => variaton.ToList())
                    .ToList();

            var contains = variations.Any
            (
                v =>
                {
                    var list = v.ToList();
                    return list[0] == 3 && list[1] == 2;
                }
            );

            var allHaveSize2 = variations.All(v => v.Count() == 2);

            Assert.AreEqual(12, variations.Count);
            Assert.IsTrue(allHaveSize2);
            Assert.IsTrue(contains);
        }

        [Test]
        public void Test_Variantions_2_of_3()
        {
            var provider = Provider();
            var variations = provider.GetVariations(new List<int> {1, 2, 3}, 2).ToList();

            var contains = variations.Any
            (
                v =>
                {
                    var list = v.ToList();
                    return list[0] == 3 && list[1] == 3;
                }
            );

            var allHaveSize2 = variations.All(v => v.Count() == 2);

            Assert.AreEqual(6, variations.Count);
            Assert.IsTrue(allHaveSize2);
            Assert.IsFalse(contains);
        }

        [Test]
        public void Test_Variantions_1_of_1()
        {
            var provider = Provider();
            var variations = provider.GetVariations(new List<int> {1}, 1).ToList();

            var contains = variations.Any
            (
                v =>
                {
                    var list = v.ToList();
                    return list[0] == 1;
                }
            );

            var allHaveSize1 = variations.All(v => v.Count() == 1);

            Assert.AreEqual(1, variations.Count);
            Assert.IsTrue(allHaveSize1);
            Assert.IsTrue(contains);
        }

        [Test]
        public void Test_Variantions_4_of_10()
        {
            var provider = Provider();

            var variations = provider.GetVariations(Enumerable.Range(1, 10).ToList(), 4).ToList();
            var allHaveSize4 = variations.All(v => v.Count() == 4);

            Assert.AreEqual(5040, variations.Count);
            Assert.IsTrue(allHaveSize4);
        }

        [Test]
        public void Test_Reinitialization()
        {
            var provider = Provider();

            var variations = provider.GetVariations(Enumerable.Range(1, 15).ToList(), 4).ToList();
            var allHaveSize4 = variations.All(v => v.Count() == 4);

            Assert.AreEqual(32760, variations.Count);
            Assert.IsTrue(allHaveSize4);

            variations = provider.GetVariations(Enumerable.Range(1, 10).ToList(), 3).ToList();
            var allHaveSize3 = variations.All(v => v.Count() == 3);

            Assert.AreEqual(720, variations.Count);
            Assert.IsTrue(allHaveSize3);
        }

        [Test]
        public void Test_List_Is_Not_Null()
        {
            Assert.Throws<ArgumentNullException>(() => Provider().GetVariations<int>(null, 42));
        }

        [Test]
        public void Test_List_Is_Not_Empty()
        {
            Assert.Throws<ArgumentException>(() => Provider().GetVariations(new List<int>(), 42));
        }

        [TestCase(-42)]
        [TestCase(-1)]
        [TestCase(0)]
        public void Test_Number_Of_Elements_Is_At_Least_One(int count)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Provider().GetVariations(new List<int> {1, 2}, count));
        }

        [TestCase(3)]
        [TestCase(4)]
        [TestCase(42)]
        public void Test_Number_Of_Elements_Is_At_Most_Count_Of_List(int count)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Provider().GetVariations(new List<int> {1, 2}, count));
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        public void Test_Number_Of_Elements_Is_Correct(int count)
        {
            Provider().GetVariations(new List<int> {1, 2, 3}, count);
        }
    }
}