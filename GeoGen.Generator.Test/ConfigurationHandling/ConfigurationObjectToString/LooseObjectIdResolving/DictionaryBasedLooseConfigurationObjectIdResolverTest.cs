using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Core.Configurations;
using GeoGen.Generator.ConfigurationHandling.ConfigurationObjectToString.LooseObjectIdResolving;
using NUnit.Framework;

namespace GeoGen.Generator.Test.ConfigurationHandling.ConfigurationObjectToString.LooseObjectIdResolving
{
    [TestFixture]
    public class DictionaryBasedLooseConfigurationObjectIdResolverTest
    {
        private static DictionaryBasedLooseConfigurationObjectIdResolver Resolver()
        {
            var dictionary = Enumerable.Range(0, 42)
                    .ToDictionary(i => i, i => i * i);

            return new DictionaryBasedLooseConfigurationObjectIdResolver(dictionary);
        }

        [Test]
        public void Test_Dictionary_Cant_Be_Null()
        {
            Assert.Throws<ArgumentNullException>(() => new DictionaryBasedLooseConfigurationObjectIdResolver(null));
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
            var objs = Enumerable.Range(0, 42)
                    .Select(i => new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = i});

            var resolver = Resolver();

            foreach (var looseConfigurationObject in objs)
            {
                var id = resolver.ResolveId(looseConfigurationObject);
                var realId = looseConfigurationObject.Id.Value;
                Assert.AreEqual(realId * realId, id);
            }
        }
    }
}