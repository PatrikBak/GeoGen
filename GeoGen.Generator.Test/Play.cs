using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace GeoGen.Generator.Test
{
    [TestFixture]
    internal class Play
    {
        [Test]
        public void Test()
        {
            var lists = new List<List<int>>
            {
                new List<int> {1, 2, 3},
                new List<int> {1, 2, 3},
                new List<int> {1, 2, 3}
            };

            foreach (var combination in Magic(lists, 0, new int[3]))
            {
                Console.WriteLine(string.Join(",", combination));
            }
            Console.WriteLine(calls);
        }

        private static int calls = 0;

        private IEnumerable<int[]> Magic(List<List<int>> lists, int currentindex, int[] currentArray)
        {
            calls++;

            foreach (var element in lists[currentindex])
            {
                currentArray[currentindex] = element;

                if (currentindex == lists.Count - 1)
                {
                    yield return currentArray;
                }
                else
                {
                    foreach (var result in Magic(lists, currentindex + 1, currentArray))
                    {
                        yield return result;
                    }
                }
            }
        }
    }
}