using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Core.Configurations;
using GeoGen.Core.Utilities;
using NUnit.Framework;

namespace GeoGen.Core.Test.Utilities
{
    [TestFixture]
    public class ConfigurationObjectsMapTest
    {
        private static IEnumerable<ConfigurationObject> Objects(int p, int l, int c)
        {
            return Objects(p, ConfigurationObjectType.Point)
                   .Concat(Objects(l, ConfigurationObjectType.Line))
                   .Concat(Objects(c, ConfigurationObjectType.Circle));
        }

        private static IEnumerable<ConfigurationObject> Objects(int count, ConfigurationObjectType type)
        {
            return Enumerable.Range(0, count).Select(o => new LooseConfigurationObject(type));
        }

        [Test]
        public void Test_Constructor_Objects_Enumerable_Cant_Be_Null()
        {
            Assert.Throws<ArgumentNullException>
            (
                () => new ConfigurationObjectsMap((IEnumerable<ConfigurationObject>) null)
            );
        }

        [Test]
        public void Test_Constructor_Objects_Enumerable()
        {
            var objects = Objects(2, 1, 0);

            var map = new ConfigurationObjectsMap(objects);

            Assert.AreEqual(2, map.Count);
            Assert.AreEqual(2, map[ConfigurationObjectType.Point].Count);
            Assert.AreEqual(1, map[ConfigurationObjectType.Line].Count);
        }

        [Test]
        public void Test_Constructor_Objects_Dictionary_Cant_Be_Null()
        {
            Assert.Throws<ArgumentNullException>
            (
                () => new ConfigurationObjectsMap((IDictionary<ConfigurationObjectType, List<ConfigurationObject>>) null)
            );
        }

        [Test]
        public void Test_Constructor_Objects_Dictionary()
        {
            var dictionary = new Dictionary<ConfigurationObjectType, List<ConfigurationObject>>
            {
                {
                    ConfigurationObjectType.Point, new List<ConfigurationObject>
                    {
                        new LooseConfigurationObject(ConfigurationObjectType.Point),
                        new LooseConfigurationObject(ConfigurationObjectType.Point),
                        new LooseConfigurationObject(ConfigurationObjectType.Point)
                    }
                },
                {
                    ConfigurationObjectType.Line, new List<ConfigurationObject>
                    {
                        new LooseConfigurationObject(ConfigurationObjectType.Line),
                        new LooseConfigurationObject(ConfigurationObjectType.Line),
                        new LooseConfigurationObject(ConfigurationObjectType.Line),
                        new LooseConfigurationObject(ConfigurationObjectType.Line)
                    }
                }
            };

            var map = new ConfigurationObjectsMap(dictionary);
            Assert.AreEqual(2, map.Count);
            Assert.AreEqual(3, map[ConfigurationObjectType.Point].Count);
            Assert.AreEqual(4, map[ConfigurationObjectType.Line].Count);
        }

        [Test]
        public void Test_Constructor_Configuration_Cant_Be_Null()
        {
            Assert.Throws<ArgumentNullException>
            (
                () => new ConfigurationObjectsMap((Configuration) null)
            );
        }

        [Test]
        public void Test_Constructor_Configuration()
        {
            var objects = Objects(1, 2, 3).Cast<LooseConfigurationObject>().ToList();
            var constructed = new List<ConstructedConfigurationObject>();
            var configuration = new Configuration(objects, constructed);

            var map = new ConfigurationObjectsMap(configuration);
            Assert.AreEqual(3, map.Count);
            Assert.AreEqual(1, map[ConfigurationObjectType.Point].Count);
            Assert.AreEqual(2, map[ConfigurationObjectType.Line].Count);
            Assert.AreEqual(3, map[ConfigurationObjectType.Circle].Count);
        }

        [Test]
        public void Test_Count_Of_Type()
        {
            var map = new ConfigurationObjectsMap(Objects(4, 0, 3));

            Assert.AreEqual(4, map.CountOfType(ConfigurationObjectType.Point));
            Assert.AreEqual(0, map.CountOfType(ConfigurationObjectType.Line));
            Assert.AreEqual(3, map.CountOfType(ConfigurationObjectType.Circle));
        }

        [Test]
        public void Test_Merge_Map_Cant_Be_Null()
        {
            var map = new ConfigurationObjectsMap(Objects(4, 0, 3));

            Assert.Throws<ArgumentNullException>(() => map.Merge(null));
        }

        [Test]
        public void Test_Merge()
        {
            var map1 = new ConfigurationObjectsMap(Objects(1, 2, 3));
            var map2 = new ConfigurationObjectsMap(Objects(3, 2, 1));

            var merged = map1.Merge(map2);

            Assert.AreEqual(6, map1.AllObjects.Count);
            Assert.AreEqual(6, map2.AllObjects.Count());

            Assert.AreEqual(12, merged.AllObjects.Count());
            Assert.IsTrue(merged.AllObjects.All(o => map1.AllObjects.Contains(o) || map2.AllObjects.Contains(o)));
        }

        [Test]
        public void Test_All_Objects()
        {
            var map = new ConfigurationObjectsMap(Objects(1, 2, 3));

            Assert.AreEqual(6, map.AllObjects.Count());
            Assert.AreEqual(1, map.AllObjects.Count(o => o.ObjectType == ConfigurationObjectType.Point));
            Assert.AreEqual(2, map.AllObjects.Count(o => o.ObjectType == ConfigurationObjectType.Line));
            Assert.AreEqual(3, map.AllObjects.Count(o => o.ObjectType == ConfigurationObjectType.Circle));
        }

        [Test]
        public void Test_Indexer()
        {
            var map = new ConfigurationObjectsMap(Objects(1, 0, 3));

            Assert.AreEqual(1, map[ConfigurationObjectType.Point].Count);
            Assert.AreEqual(0, map[ConfigurationObjectType.Line].Count);
            Assert.AreEqual(3, map[ConfigurationObjectType.Circle].Count);
        }
    }
}