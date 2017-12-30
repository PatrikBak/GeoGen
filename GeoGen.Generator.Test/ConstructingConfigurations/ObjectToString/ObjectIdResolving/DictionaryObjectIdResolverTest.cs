using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Core.Configurations;
using NUnit.Framework;
using static GeoGen.Generator.Test.TestHelpers.ConfigurationObjects;

namespace GeoGen.Generator.Test.ConstructingConfigurations.ObjectToString.ObjectIdResolving
{
    [TestFixture]
    public class DictionaryObjectIdResolverTest
    {
        private static DictionaryObjectIdResolver Resolver()
        {
            var dictionary = Enumerable.Range(0, 42).ToDictionary(i => i, i => i * i);

            return new DictionaryObjectIdResolver(1, dictionary);
        }

        [Test]
        public void Test_Passed_Dictionary_Cant_Be_Null()
        {
            Assert.Throws<ArgumentNullException>(() => new DictionaryObjectIdResolver(1, null));
        }

        [Test]
        public void Test_Passed_Object_Cant_Be_Null()
        {
            Assert.Throws<ArgumentNullException>(() => Resolver().ResolveId(null));
        }

        [Test]
        public void Test_Passed_Object_Must_Have_Id()
        {
            var obj = new LooseConfigurationObject(ConfigurationObjectType.Point);

            Assert.Throws<GeneratorException>(() => Resolver().ResolveId(obj));
        }

        [Test]
        public void Test_Objects_Id_Isnt_Present_In_Dictionary()
        {
            var obj = new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 42};

            Assert.Throws<KeyNotFoundException>(() => Resolver().ResolveId(obj));
        }

        [Test]
        public void Test_Objects_Id_Is_Fine()
        {
            var objs = Objects(42, ConfigurationObjectType.Point, 0);

            var resolver = Resolver();

            foreach (var looseConfigurationObject in objs)
            {
                var id = resolver.ResolveId(looseConfigurationObject);
                var realId = looseConfigurationObject.Id ?? throw new Exception();
                Assert.AreEqual(realId * realId, id);
            }
        }

        [Test]
        public void Test_Id_Of_Resolver_Is_Set()
        {
            var id = Resolver().Id;

            Assert.AreEqual(1, id);
        }

        [Test]
        public void Test_Id_Of_Resolver_Cant_Be_Default()
        {
            var id = DefaultObjectIdResolver.DefaultId;
            var dictionary = new Dictionary<int, int>();

            Assert.Throws<ArgumentException>(() => new DictionaryObjectIdResolver(id, dictionary));
        }

        [Test]
        public void Test_Dictionaries_Composition_With_Correct_Dictionaries()
        {
            var first = new Dictionary<int, int>
            {
                    {0, 1},
                    {1, 2},
                    {2, 0},
                    {3, 3}
            };

            var second = new Dictionary<int, int>
            {
                    {0, 0},
                    {1, 1},
                    {2, 3},
                    {3, 2}
            };

            var result1 = new DictionaryObjectIdResolver(1, first).Compose(new DictionaryObjectIdResolver(2, second));

            Assert.AreEqual(1, result1[0]);
            Assert.AreEqual(3, result1[1]);
            Assert.AreEqual(0, result1[2]);
            Assert.AreEqual(2, result1[3]);

            Assert.AreEqual(4, result1.Count);

            var result2 = new DictionaryObjectIdResolver(1, second).Compose(new DictionaryObjectIdResolver(2, first));

            Assert.AreEqual(1, result2[0]);
            Assert.AreEqual(2, result2[1]);
            Assert.AreEqual(3, result2[2]);
            Assert.AreEqual(0, result2[3]);

            Assert.AreEqual(4, result2.Count);
        }

        [Test]
        public void Test_Dictionaries_Composition_Dictionary_Cant_Be_Null()
        {
            var first = new Dictionary<int, int>
            {
                    {0, 1},
                    {1, 2},
                    {2, 0},
                    {3, 3}
            };

            Assert.Throws<ArgumentNullException>(() => new DictionaryObjectIdResolver(1, first).Compose(null));
        }

        [Test]
        public void Test_Dictionaries_Composition_With_Dictionaries_With_Distint_Number_Of_items()
        {
            var first = new Dictionary<int, int>
            {
                    {0, 1},
                    {1, 2},
                    {2, 0},
                    {3, 3},
                    {4, 5}
            };

            var second = new Dictionary<int, int>
            {
                    {0, 0},
                    {1, 1},
                    {2, 3},
                    {3, 2}
            };

            Assert.Throws<ArgumentException>(() => new DictionaryObjectIdResolver(1, first).Compose(new DictionaryObjectIdResolver(2, second)));
            Assert.Throws<ArgumentException>(() => new DictionaryObjectIdResolver(1, second).Compose(new DictionaryObjectIdResolver(2, first)));
        }

        [Test]
        public void Test_Dictionaries_Composition_With_Incorrect_Dictionaries_Without_Matching_Elements()
        {
            var first = new Dictionary<int, int>
            {
                    {0, 1},
                    {1, 2},
                    {2, 0},
                    {3, 3}
            };

            var second = new Dictionary<int, int>
            {
                    {0, 0},
                    {4, 1},
                    {2, 3},
                    {3, 2}
            };

            Assert.Throws<ArgumentException>(() => new DictionaryObjectIdResolver(1, first).Compose(new DictionaryObjectIdResolver(2, second)));
        }

        [Test]
        public void Test_Is_Equivalent_To_Dictionary_Cant_Be_Null()
        {
            var first = new Dictionary<int, int>
            {
                    {0, 1},
                    {1, 2},
                    {2, 0},
                    {3, 3}
            };

            Assert.Throws<ArgumentNullException>(() => new DictionaryObjectIdResolver(1, first).IsEquivalentTo(null));
        }

        [Test]
        public void Test_Is_Equivalent_To_With_Equivalent_Dictionary()
        {
            var first = new Dictionary<int, int>
            {
                    {0, 1},
                    {1, 2},
                    {2, 0},
                    {3, 3}
            };

            var second = new Dictionary<int, int>
            {
                    {0, 1},
                    {1, 2},
                    {2, 0},
                    {3, 3}
            };

            Assert.IsTrue(new DictionaryObjectIdResolver(1, first).IsEquivalentTo(second));
            Assert.IsTrue(new DictionaryObjectIdResolver(1, second).IsEquivalentTo(first));
        }

        [Test]
        public void Test_Is_Equivalent_To_With_Not_Equivalent_Dictionaries_With_Distint_Number_Of_Elements()
        {
            var first = new Dictionary<int, int>
            {
                    {0, 1},
                    {1, 2},
                    {2, 0}
            };

            var second = new Dictionary<int, int>
            {
                    {0, 1},
                    {1, 2},
                    {2, 0},
                    {3, 3}
            };

            Assert.IsFalse(new DictionaryObjectIdResolver(1, first).IsEquivalentTo(second));
            Assert.IsFalse(new DictionaryObjectIdResolver(1, second).IsEquivalentTo(first));
        }

        [Test]
        public void Test_Is_Equivalent_To_With_Not_Equivalent_Dictionaries_With_Distint_Elements()
        {
            var first = new Dictionary<int, int>
            {
                    {0, 1},
                    {1, 2},
                    {2, 0},
                    {3, 2}
            };

            var second = new Dictionary<int, int>
            {
                    {0, 1},
                    {1, 2},
                    {2, 0},
                    {3, 3}
            };

            Assert.IsFalse(new DictionaryObjectIdResolver(1, first).IsEquivalentTo(second));
            Assert.IsFalse(new DictionaryObjectIdResolver(1, second).IsEquivalentTo(first));
        }
    }
}