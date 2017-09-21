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
                    .Union(Objects(l, ConfigurationObjectType.Line))
                    .Union(Objects(c, ConfigurationObjectType.Circle));
        }

        private static IEnumerable<ConfigurationObject> Objects(int count, ConfigurationObjectType type)
        {
            return Enumerable.Range(0, count).Select(o => new LooseConfigurationObject(type));
        }

        private static Dictionary<ConfigurationObjectType, ConfigurationObjectType> MixDictionary()
        {
            return new Dictionary<ConfigurationObjectType, ConfigurationObjectType>
            {
                {ConfigurationObjectType.Point, ConfigurationObjectType.Circle},
                {ConfigurationObjectType.Line, ConfigurationObjectType.Point},
                {ConfigurationObjectType.Circle, ConfigurationObjectType.Line}
            };
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
            var objects = Objects(1, 2, 3).Cast<LooseConfigurationObject>().ToSet();
            var constructed = new List<ConstructedConfigurationObject>();
            var configuration = new Configuration(objects, constructed);

            var map = new ConfigurationObjectsMap(configuration);
            Assert.AreEqual(3, map.Count);
            Assert.AreEqual(1, map[ConfigurationObjectType.Point].Count);
            Assert.AreEqual(2, map[ConfigurationObjectType.Line].Count);
            Assert.AreEqual(3, map[ConfigurationObjectType.Circle].Count);
        }

        [Test]
        public void Test_Add_All_Objects_Enumerable_Cant_Be_Null()
        {
            var map = new ConfigurationObjectsMap();

            Assert.Throws<ArgumentNullException>(() => map.AddAll((IEnumerable<ConfigurationObject>) null));
        }

        [Test]
        public void Test_Add_All_Empty_Enumerable()
        {
            var map = new ConfigurationObjectsMap();
            Assert.AreEqual(0, map.Count);

            map.AddAll(new List<ConfigurationObject>());
            Assert.AreEqual(0, map.Count);

            map.AddAll(Objects(1, 0, 0));
            Assert.AreEqual(1, map.Count);
            Assert.AreEqual(1, map[ConfigurationObjectType.Point].Count);

            map.AddAll(new List<ConfigurationObject>());
            Assert.AreEqual(1, map.Count);
            Assert.AreEqual(1, map[ConfigurationObjectType.Point].Count);
        }

        [Test]
        public void Test_Add_All_Objects_Enumerable()
        {
            var map = new ConfigurationObjectsMap(Objects(3, 2, 1));
            map.AddAll(Objects(1, 2, 3));

            Assert.AreEqual(3, map.Count);
            Assert.AreEqual(4, map[ConfigurationObjectType.Point].Count);
            Assert.AreEqual(4, map[ConfigurationObjectType.Line].Count);
            Assert.AreEqual(4, map[ConfigurationObjectType.Circle].Count);
        }

        [Test]
        public void Test_Add_All_Objects_And_Custom_Selector()
        {
            var map = new ConfigurationObjectsMap(Objects(3, 2, 1));
            var mixDictionary = MixDictionary();

            ConfigurationObject Selector(ConfigurationObject o)
            {
                return new LooseConfigurationObject(mixDictionary[o.ObjectType]);
            }

            map.AddAll(Objects(666, 42, 1000), Selector);

            Assert.AreEqual(3, map.Count);
            Assert.AreEqual(45, map[ConfigurationObjectType.Point].Count);
            Assert.AreEqual(1002, map[ConfigurationObjectType.Line].Count);
            Assert.AreEqual(667, map[ConfigurationObjectType.Circle].Count);
        }

        [Test]
        public void Test_Add_All_Map_Cant_Be_Null()
        {
            var map = new ConfigurationObjectsMap();

            Assert.Throws<ArgumentNullException>(() => map.AddAll((ConfigurationObjectsMap) null));
        }

        [Test]
        public void Test_Add_All_Map()
        {
            var map = new ConfigurationObjectsMap(Objects(1, 0, 0));
            var newMap = new ConfigurationObjectsMap(Objects(0, 4, 2));
            map.AddAll(newMap);

            Assert.AreEqual(3, map.Count);
            Assert.AreEqual(1, map[ConfigurationObjectType.Point].Count);
            Assert.AreEqual(4, map[ConfigurationObjectType.Line].Count);
            Assert.AreEqual(2, map[ConfigurationObjectType.Circle].Count);
        }

        [Test]
        public void Test_Add_All_Map_And_Custom_Selector()
        {
            var map = new ConfigurationObjectsMap(Objects(1, 0, 0));
            var newMap = new ConfigurationObjectsMap(Objects(1, 0, 0));
            var mixDictionary = MixDictionary();

            ConfigurationObject Selector(ConfigurationObject o)
            {
                return new LooseConfigurationObject(mixDictionary[o.ObjectType]);
            }

            map.AddAll(newMap, Selector);

            Assert.AreEqual(2, map.Count);
            Assert.AreEqual(1, map[ConfigurationObjectType.Point].Count);
            Assert.AreEqual(1, map[ConfigurationObjectType.Circle].Count);
        }
    }
}