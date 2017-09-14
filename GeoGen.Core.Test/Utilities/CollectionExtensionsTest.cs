using System;
using System.Collections.Generic;
using GeoGen.Core.Utilities;
using NUnit.Framework;

namespace GeoGen.Core.Test.Utilities
{
    [TestFixture]
    public class CollectionExtensionsTest
    {
        [Test]
        public void Set_Items_Test_Items_Not_null()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var list = new List<int>();
                list.SetItems(null);
            });
        }

        [Test]
        public void Set_Items_Test_Items_When_Items_Are_Empty()
        {
            var list = new List<int> {1, 2};
            list.SetItems(new List<int>());
            Assert.IsTrue(list.Empty());
        }

        [Test]
        public void Set_Items_Items_Are_Not_Empty()
        {
            var list = new List<int> {1, 2};
            list.SetItems(new List<int> {1, 3});
            Assert.IsTrue(list.Contains(1));
            Assert.IsTrue(list.Contains(3));
            Assert.IsTrue(list.Count == 2);
        }
    }
}