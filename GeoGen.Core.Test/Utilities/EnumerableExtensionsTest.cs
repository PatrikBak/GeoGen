using System.Collections.Generic;
using GeoGen.Core.Utilities;
using NUnit.Framework;

namespace GeoGen.Core.Test.Utilities
{
    [TestFixture]
    public class EnumerableExtensionsTest
    {
        [Test]
        public void Empty_Enumerable_Test_Not_EmptyEnumerable()
        {
            var list = new List<int>();
            Assert.IsTrue(list.Empty());
        }

        [Test]
        public void Empty_Enumerable_Test_Empty_Enumerable()
        {
            var list = new List<int> {1, 2};
            Assert.IsFalse(list.Empty());
        }
    }
}