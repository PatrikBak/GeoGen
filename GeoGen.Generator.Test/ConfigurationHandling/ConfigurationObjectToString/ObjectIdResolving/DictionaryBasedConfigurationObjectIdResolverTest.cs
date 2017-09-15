using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Core.Configurations;
using GeoGen.Generator.ConfigurationHandling.ConfigurationObjectToString.ObjectIdResolving;
using GeoGen.Generator.Test.TestHelpers;
using NUnit.Framework;

namespace GeoGen.Generator.Test.ConfigurationHandling.ConfigurationObjectToString.ObjectIdResolving
{
    [TestFixture]
    public class DictionaryBasedConfigurationObjectIdResolverTest
    {
        private static DictionaryObjectIdResolver Resolver()
        {
            var dictionary = Enumerable.Range(0, 42).ToDictionary(i => i, i => i * i);

            return new DictionaryObjectIdResolver(dictionary, 0);
        }

        [Test]
        public void Test_Dictionary_Cant_Be_Null()
        {
            Assert.Throws<ArgumentNullException>(() => new DictionaryObjectIdResolver(null, 0));
        }

        [Test]
        public void Test_Object_Cant_Be_Null()
        {
            Assert.Throws<ArgumentNullException>(() => Resolver().ResolveId(null));
        }

        [Test]
        public void Test_Object_Must_Have_Id()
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
            var objs = ConfigurationObjects.Objects(42, ConfigurationObjectType.Point, 0);

            var resolver = Resolver();

            foreach (var looseConfigurationObject in objs)
            {
                var id = resolver.ResolveId(looseConfigurationObject);
                var realId = looseConfigurationObject.Id ?? throw new Exception();
                Assert.AreEqual(realId * realId, id);
            }
        }
    }
}