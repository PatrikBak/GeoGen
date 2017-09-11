using System;
using GeoGen.Core.Configurations;
using GeoGen.Generator.ConfigurationHandling.ConfigurationObjectToString.LooseObjectIdResolving;
using NUnit.Framework;

namespace GeoGen.Generator.Test.ConfigurationHandling.ConfigurationObjectToString.LooseObjectIdResolving
{
    [TestFixture]
    public class DefaultLooseConfigurationObjectIdResolverTest
    {
        private static DefaultLooseConfigurationObjectIdResolver Resolver()
        {
            return new DefaultLooseConfigurationObjectIdResolver();
        }

        [Test]
        public void Configuration_Object_Cant_Be_Null()
        {
            Assert.Throws<ArgumentNullException>(() => Resolver().ResolveId(null));
        }

        [Test]
        public void Configuration_Object_Id_Must_Be_Set()
        {
            var obj = new LooseConfigurationObject(ConfigurationObjectType.Point);

            Assert.Throws<GeneratorException>(() => Resolver().ResolveId(obj));
        }

        [Test]
        public void Configuration_Object_Id_Is_Fine()
        {
            var obj = new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 42};
            var id = Resolver().ResolveId(obj);

            Assert.AreEqual(42, id);
        }
    }
}