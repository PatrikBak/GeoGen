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
            Assert.Throws<ArgumentNullException>(() => map.ContainsLeft(null));
            Assert.Throws<ArgumentNullException>(() => map.ContainsRight(null));
        }

        [Test]
        public void Test_Contains_With_Correct_Key()
        {
            var map = new Map<string, string>();
            map.Add("12", "3");
            map.Add("A", "b");

            Assert.IsTrue(map.ContainsLeft("12"));
            Assert.IsTrue(map.ContainsLeft("A"));
            Assert.IsFalse(map.ContainsLeft("a"));

            Assert.IsTrue(map.ContainsRight("3"));
            Assert.IsTrue(map.ContainsRight("b"));
            Assert.IsFalse(map.ContainsRight("B"));
        }

        [Test]
        public void Test_Get_Object_Cant_Be_Null()
        {
            var map = new Map<string, string>();
            Assert.Throws<ArgumentNullException>(() => map.GetLeft(null));
            Assert.Throws<ArgumentNullException>(() => map.GetRight(null));
        }

        [Test]
        public void Test_Get_Key_Must_Exist()
        {
            var map = new Map<string, int>();
            map.Add("a", 1);

            Assert.Throws<KeyNotFoundException>(() => map.GetLeft(2));
            Assert.Throws<KeyNotFoundException>(() => map.GetRight("b"));
        }

        [Test]
        public void Test_Get_With_Correct_Key()
        {
            var map = new Map<string, int>();
            map.Add("a", 1);
            map.Add("x", 2);

            Assert.AreEqual("a", map.GetLeft(1));
            Assert.AreEqual(1, map.GetRight("a"));
            Assert.AreEqual("x", map.GetLeft(2));
            Assert.AreEqual(2, map.GetRight("x"));
        }
    }
}