using System;
using GeoGen.Core.Configurations;
using GeoGen.Generator.ConfigurationsHandling.ConfigurationObjectToString.ConfigurationObjectIdResolving;
using NUnit.Framework;

namespace GeoGen.Generator.Test.ConfigurationsHandling.ConfigurationObjectToString.ConfigurationObjectIdResolving
{
    [TestFixture]
    public class DefaultObjectIdResolverTest
    {
        private static DefaultObjectIdResolver Resolver()
        {
            return new DefaultObjectIdResolver();
        }

        [Test]
        public void Test_Object_Cant_Be_Null()
        {
            Assert.Throws<ArgumentNullException>(() => Resolver().ResolveId(null));
        }

        [Test]
        public void Test_Objects_Id_Must_Be_Set()
        {
            var obj = new LooseConfigurationObject(ConfigurationObjectType.Point);

            Assert.Throws<GeneratorException>(() => Resolver().ResolveId(obj));
        }

        [Test]
        public void Test_Objects_Id_Is_Fine()
        {
            var obj = new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 42};
            var id = Resolver().ResolveId(obj);

            Assert.AreEqual(42, id);
        }

        [Test]
        public void Test_Id_Is_set()
        {
            var id = Resolver().Id;

            Assert.AreEqual(DefaultObjectIdResolver.DefaultId, id);
        }
    }
}