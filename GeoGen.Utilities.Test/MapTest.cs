using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace GeoGen.Utilities.Test
{
    [TestFixture]
    public class MapTest
    {
        [Test]
        public void Test_Add_Object_Cant_Be_Null()
        {
            var map = new Map<string, string>();

            Assert.Throws<ArgumentNullException>(() => map.Add("a", null));
            Assert.Throws<ArgumentNullException>(() => map.Add(null, "b"));
            Assert.Throws<ArgumentNullException>(() => map.Add(null, null));
        }

        [Test]
        public void Test_Added_Object_Cant_Be_In_Map()
        {
            var map = new Map<string, string>();
            map.Add("a", "b");

            Assert.Throws<ArgumentException>(() => map.Add("a", "b"));
            Assert.Throws<ArgumentException>(() => map.Add("a", "x"));
            Assert.Throws<ArgumentException>(() => map.Add("y", "b"));
        }

        [Test]
        public void Test_Contains_Object_Cant_Be_Null()
        {
            var map = new Map<string, string>();
            Assert.Throws<ArgumentNullException>(() => map.ContainsLeftKey(null));
            Assert.Throws<ArgumentNullException>(() => map.ContainsRightKey(null));
        }

        [Test]
        public void Test_Contains_With_Correct_Key()
        {
            var map = new Map<string, string>();
            map.Add("12", "3");
            map.Add("A", "b");

            Assert.IsTrue(map.ContainsLeftKey("12"));
            Assert.IsTrue(map.ContainsLeftKey("A"));
            Assert.IsFalse(map.ContainsLeftKey("a"));

            Assert.IsTrue(map.ContainsRightKey("3"));
            Assert.IsTrue(map.ContainsRightKey("b"));
            Assert.IsFalse(map.ContainsRightKey("B"));
        }

        [Test]
        public void Test_Get_Object_Cant_Be_Null()
        {
            var map = new Map<string, string>();
            Assert.Throws<ArgumentNullException>(() => map.GetLeftValue(null));
            Assert.Throws<ArgumentNullException>(() => map.GetRightValue(null));
        }

        [Test]
        public void Test_Get_Key_Must_Exist()
        {
            var map = new Map<string, int>();
            map.Add("a", 1);

            Assert.Throws<KeyNotFoundException>(() => map.GetLeftValue(2));
            Assert.Throws<KeyNotFoundException>(() => map.GetRightValue("b"));
        }

        [Test]
        public void Test_Get_With_Correct_Key()
        {
            var map = new Map<string, int>();
            map.Add("a", 1);
            map.Add("x", 2);

            Assert.AreEqual("a", map.GetLeftValue(1));
            Assert.AreEqual(1, map.GetRightValue("a"));
            Assert.AreEqual("x", map.GetLeftValue(2));
            Assert.AreEqual(2, map.GetRightValue("x"));
        }
    }
}