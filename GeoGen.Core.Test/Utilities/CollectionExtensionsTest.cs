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
        public void Test_Set_Items_Items_Cant_Be_Null()
        {
            Assert.Throws<ArgumentNullException>
            (
                () =>
                {
                    var list = new List<int>();
                    list.SetItems(null);
                }
            );
        }

        [Test]
        public void Test_Set_Items_Items_Empty()
        {
            var list = new List<int> {1, 2};
            list.SetItems(new List<int>());
            Assert.IsTrue(list.Empty());
        }

        [Test]
        public void Test_Set_Items_Items_Are_NotEmpty()
        {
            var list = new List<int> {1, 2};
            list.SetItems(new List<int> {1, 3});
            Assert.IsTrue(list.Contains(1));
            Assert.IsTrue(list.Contains(3));
            Assert.IsTrue(list.Count == 2);
        }
    }
}