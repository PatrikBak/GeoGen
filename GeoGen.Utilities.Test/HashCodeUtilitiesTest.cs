using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace GeoGen.Utilities.Test
{
    [TestFixture]
    public class HashCodeUtilitiesTest
    {
        private static Func<int, int> HashCoder() => i => EqualityComparer<int>.Default.GetHashCode(i);

        [Test]
        public void Test_Order_Independant_Hash()
        {
            var list1 = new List<int> {1, 2, 3, 2};
            var list2 = new List<int> {2, 2, 1, 3};
            var list3 = new List<int> {2, 1, 2, 3};
            var list4 = new List<int> {3, 2, 1, 2};

            var hashCodes = new HashSet<int>
            {
                list1.GetOrderIndependentHashCode(HashCoder()),
                list2.GetOrderIndependentHashCode(HashCoder()),
                list3.GetOrderIndependentHashCode(HashCoder()),
                list4.GetOrderIndependentHashCode(HashCoder())
            };

            Assert.IsTrue(hashCodes.Count == 1);
        }

        //[Test]
        //public void Test_Order_Dependant_Hash()
        //{
        //    var list1 = new List<int> {1, 2, 3, 4, 42, 666};
        //    var list2 = new List<int> {1, 2, 3, 4, 42, 666};

        //    var hash1 = list1.GetOrderDependentHashCode(HashCoder());
        //    var hash2 = list2.GetOrderDependentHashCode(HashCoder());

        //    Assert.AreEqual(hash1, hash2);
        //}
    }
}