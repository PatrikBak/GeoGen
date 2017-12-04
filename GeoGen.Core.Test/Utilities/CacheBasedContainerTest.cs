using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Core.Utilities;
using NUnit.Framework;

namespace GeoGen.Core.Test.Utilities
{
    [TestFixture]
    public class CacheBasedContainerTest
    {
        private class Z5Comparer : IEqualityComparer<int>
        {
            public int HashCodeCalls;

            public bool Equals(int x, int y)
            {
                return x % 5 == y % 5;
            }

            public int GetHashCode(int obj)
            {
                HashCodeCalls++;
                return obj % 5;
            }
        }

        [Test]
        public void Test_Equality_Comparer_Cant_Be_Null()
        {
            Assert.Throws<ArgumentNullException>(() => new CacheBasedContainer<int>(null));
        }

        [Test]
        public void Test_Added_Value_Cant_Be_Null()
        {
            var container = new CacheBasedContainer<string>(EqualityComparer<string>.Default);

            Assert.Throws<ArgumentNullException>(() => container.Add(null));
        }

        [Test]
        public void Test_Add_With_Correct_Values()
        {
            var container = new CacheBasedContainer<string>(EqualityComparer<string>.Default)
            {
                "ab",
                "ab",
                "ba",
                "a"
            };

            Assert.AreEqual(3, container.Count());
            Assert.IsTrue(container.Any(c => c == "ab"));
            Assert.IsTrue(container.Any(c => c == "ba"));
            Assert.IsTrue(container.Any(c => c == "a"));
        }

        [Test]
        public void Test_Contains_Value_Cant_Be_Null()
        {
            var container = new CacheBasedContainer<string>(EqualityComparer<string>.Default);

            Assert.Throws<ArgumentNullException>(() => container.Contains(null));
        }

        [Test]
        public void Test_Contains_With_Correct_Values()
        {
            var container = new CacheBasedContainer<string>(EqualityComparer<string>.Default)
            {
                "ab",
                "ab",
                "ba",
                "a"
            };

            Assert.AreEqual(3, container.Count());
            Assert.IsTrue(container.Contains("ab"));
            Assert.IsTrue(container.Contains("ba"));
            Assert.IsTrue(container.Contains("a"));

            Assert.IsFalse(container.Contains("c"));
            Assert.IsFalse(container.Contains("d"));
            Assert.IsFalse(container.Contains("p"));
        }

        [Test]
        public void Test_Hash_Code_Is_Invoked_One_For_Each_Element()
        {
            var comparer = new Z5Comparer();

            var container = new CacheBasedContainer<int>(comparer) { 1, 7, 5, 0, 7, 2, 3 };

            Assert.AreEqual(7, comparer.HashCodeCalls);
            Assert.AreEqual(4, container.Count());

            Assert.IsTrue(container.Contains(0));
            Assert.IsTrue(container.Contains(1));
            Assert.IsTrue(container.Contains(2));
            Assert.IsTrue(container.Contains(3));

            Assert.AreEqual(11, comparer.HashCodeCalls);
        }
    }
}