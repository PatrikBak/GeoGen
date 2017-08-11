using System.Collections.Generic;
using GeoGen.Core.Utilities;
using NUnit.Framework;

namespace GeoGen.Core.Test.Utilities
{
    [TestFixture]
    public class EnumerableExtensionsTest
    {
        [Test]
        public void EmptyEnumerableTest_NotEmptyEnumerable()
        {
            var list = new List<int>();
            Assert.IsTrue(list.Empty());
        }

        [Test]
        public void EmptyEnumerableTest_Empty_Enumerable()
        {
            var list = new List<int> {1, 2};
            Assert.IsFalse(list.Empty());
        }
    }
}