using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using GeoGen.Core.Utilities.Variations;
using NUnit.Framework;

namespace GeoGen.Core.Test.Utilities.Variations
{
    [TestFixture]
    public class VariationsProviderTest
    {
        private static IVariationsProvider<int> Provider()
        {
            //return new VariationsProvider<int>(new SubsetsGenerator<int>());
            return new VariationsProvider<int>();
        }

        [Test]
        public void Test_Variantions_2_of_4()
        {
            var provider = Provider();
            var variations = provider.GetVariations(new List<int> {1, 2, 3, 4}, 2);

            var contains = variations.Any
            (
                v =>
                {
                    var list = v.ToList();
                    return list[0] == 3 && list[1] == 2;
                }
            );

            var allHaveSize2 = variations.All(v => v.Count() == 2);

            Assert.AreEqual(12, variations.Count());
            Assert.IsTrue(allHaveSize2);
            Assert.IsTrue(contains);
        }

        [Test]
        public void Test_Variantions_2_of_3()
        {
            var provider = Provider();
            var variations = provider.GetVariations(new List<int> {1, 2, 3}, 2);

            var contains = variations.Any
            (
                v =>
                {
                    var list = v.ToList();
                    return list[0] == 3 && list[1] == 3;
                }
            );

            var allHaveSize2 = variations.All(v => v.Count() == 2);

            Assert.AreEqual(6, variations.Count());
            Assert.IsTrue(allHaveSize2);
            Assert.IsFalse(contains);
        }

        [Test]
        public void Test_Variantions_1_of_1()
        {
            var provider = Provider();
            var variations = provider.GetVariations(new List<int> {1}, 1);

            var contains = variations.Any
            (
                v =>
                {
                    var list = v.ToList();
                    return list[0] == 1;
                }
            );

            var allHaveSize1 = variations.All(v => v.Count() == 1);

            Assert.AreEqual(1, variations.Count());
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
    }
}