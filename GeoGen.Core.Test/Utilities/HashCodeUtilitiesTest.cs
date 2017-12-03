using System;
using System.Collections.Generic;
using GeoGen.Core.Utilities;
using NUnit.Framework;

namespace GeoGen.Core.Test.Utilities
{
    [TestFixture]
    public class HashCodeUtilitiesTest
    {
        private static Func<int, int> HashCoder() => i => EqualityComparer<int>.Default.GetHashCode(i);

        [Test]
        public void Test_Order_Independant_Hash()
        {
            var list1 = new List<int> { 1, 2, 3, 2 };
            var list2 = new List<int> { 2, 2, 1, 3 };
            var list3 = new List<int> { 2, 1, 2, 3 };
            var list4 = new List<int> { 3, 2, 1, 2 };

            var hashCodes = new HashSet<int>
            {
                HashCodeUtilities.GetOrderIndependentHashCode(list1, HashCoder()),
                HashCodeUtilities.GetOrderIndependentHashCode(list2, HashCoder()),
                HashCodeUtilities.GetOrderIndependentHashCode(list3, HashCoder()),
                HashCodeUtilities.GetOrderIndependentHashCode(list4, HashCoder())
            };

            Assert.IsTrue(hashCodes.Count == 1);
        }

        [Test]
        public void Test_Order_Dependant_Hash()
        {
            var list1 = new List<int> { 1, 2, 3, 4, 42, 666 };
            var list2 = new List<int> { 1, 2, 3, 4, 42, 666 };

            var hash1 = HashCodeUtilities.GetOrderDependentHashCode(list1, HashCoder());
            var hash2 = HashCodeUtilities.GetOrderDependentHashCode(list2, HashCoder());

            Assert.AreEqual(list1, list2);
        }
    }
}