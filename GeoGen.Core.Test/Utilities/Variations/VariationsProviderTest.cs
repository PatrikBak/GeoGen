using System.Collections.Generic;
using System.Linq;
using GeoGen.Core.Utilities.Variations;
using NUnit.Framework;

namespace GeoGen.Core.Test.Utilities.Variations
{
    [TestFixture]
    public class VariationsProviderTest
    {
        [Test]
        public void Test_Variantions_2_of_4()
        {
            var provider = new VariationsProvider<int>(new SubsetsGenerator<int>());
            var variations = provider.GetVariations(new List<int> {1, 2, 3, 4}, 2).
            ToList();

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
            var provider = new VariationsProvider<int>(new SubsetsGenerator<int>());
            var variations = provider.GetVariations(new List<int> {1, 2, 3}, 2).
            ToList();

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
            var provider = new VariationsProvider<int>(new SubsetsGenerator<int>());
            var variations = provider.GetVariations(new List<int> {1}, 1).
            ToList();

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
        public void Test_Variantions_8_of_8()
        {
            var provider = new VariationsProvider<int>(new SubsetsGenerator<int>());
            var variations = provider.GetVariations(Enumerable.Range(1, 8).ToList(), 8).ToList();

            var allHaveSize10 = variations.All(v => v.Count() == 8);

            Assert.AreEqual(40320, variations.Count);
            Assert.IsTrue(allHaveSize10);
        }
    }
}