using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using GeoGen.Core.Utilities.Combinator;

namespace GeoGen.Core.Test.Utilities.Combinator
{
    [TestFixture]
    public class CombinatorTest
    {
        [Test]
        public void Test_With_Four_Lists_To_Be_Combined()
        {
            var combinator = new Combinator<int, int>();

            var testSample = new Dictionary<int, IEnumerable<int>>
            {
                {1, new List<int> {1, 2, 3, 42}},
                {2, new List<int> {1, 2, 3, 25}},
                {3, new List<int> {1, 2, 42, 25}},
                {4, new List<int> {1, 2, 3, 7, 9}}
            };

            var result = combinator.Combine(testSample).ToList();

            var contains = result.Any(dic => dic[1] == 2 && dic[2] == 3 && dic[3] == 1 && dic[4] == 7);
            var count = result.Count;

            Assert.IsTrue(contains);
            Assert.AreEqual(320, count);
        }

        public void Test_With_One_List()
        {
            var combinator = new Combinator<int, int>();

            var testSample = new Dictionary<int, IEnumerable<int>>
            {
                {1, new List<int> {1, 2, 3, 42}}
            };

            var result = combinator.Combine(testSample).ToList();

            var contains = result.Any(dic => dic[1] == 42);
            var count = result.Count;

            Assert.IsTrue(contains);
            Assert.AreEqual(4, count);
        }

        public void Test_With_More_One_Element_Lists()
        {
            var combinator = new Combinator<int, int>();

            var testSample = new Dictionary<int, IEnumerable<int>>
            {
                {1, new List<int> {1}},
                {2, new List<int> {1}},
                {3, new List<int> {2}}
            };

            var result = combinator.Combine(testSample).ToList();

            var contains = result.Any(dic => dic[1] == 1 && dic[2] == 1 && dic[3] == 2);
            var count = result.Count;

            Assert.IsTrue(contains);
            Assert.AreEqual(1, count);
        }

        public void Test_With_One_Element_List_And_Other_List()
        {
            var combinator = new Combinator<int, int>();

            var testSample = new Dictionary<int, IEnumerable<int>>
            {
                {1, new List<int> {1}},
                {2, new List<int> {1, 2}},
            };

            var result = combinator.Combine(testSample).ToList();

            var contains = result.Any(dic => dic[1] == 1 && dic[2] == 2);
            var count = result.Count;

            Assert.IsTrue(contains);
            Assert.AreEqual(2, count);
        }
    }
}