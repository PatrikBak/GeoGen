using System;
using GeoGen.Core.Configurations;
using GeoGen.Generator.ConfigurationHandling.ConfigurationObjectToString;
using GeoGen.Generator.ConfigurationHandling.ConfigurationObjectToString.ObjectIdResolving;
using Moq;
using NUnit.Framework;

namespace GeoGen.Generator.Test.ConfigurationHandling.ConfigurationObjectToString
{
    [TestFixture]
    public class DefaultConfigurationObjectIdToStringProviderTest
    {
        private static DefaultConfigurationObjectToStringProvider Provider(bool withResolver = false)
        {
            return withResolver
                ? new DefaultConfigurationObjectToStringProvider(Resolver())
                : new DefaultConfigurationObjectToStringProvider();
        }

        private static IObjectIdResolver Resolver()
        {
            var mock = new Mock<IObjectIdResolver>();
            mock.Setup(s => s.ResolveId(It.IsAny<ConfigurationObject>()))
                    .Returns<ConfigurationObject>(o => 2 * o.Id ?? throw new Exception());

            return mock.Object;
        }

        [Test]
        public void Custom_Id_Resolver_Cant_Be_Null()
        {
            Assert.Throws<ArgumentNullException>(() => new DefaultConfigurationObjectToStringProvider(null));
        }

        [Test]
        public void Configuration_Object_Not_Null()
        {
            Assert.Throws<ArgumentNullException>(() => Provider().ConvertToString(null));
            Assert.Throws<ArgumentNullException>(() => Provider(true).ConvertToString(null));
        }

        [Test]
        public void Configuration_Object_Must_Have_Set_Id()
        {
            var obj = new LooseConfigurationObject(ConfigurationObjectType.Point);

            Assert.Throws<GeneratorException>(() => Provider().ConvertToString(obj));
            Assert.Throws<GeneratorException>(() => Provider(true).ConvertToString(obj));
        }

        [Test]
        public void Configurutation_Object_Id_To_String()
        {
            var obj = new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 42};
            var asString1 = Provider().ConvertToString(obj);
            var asString2 = Provider(true).ConvertToString(obj);

            Assert.AreEqual("42", asString1);
            Assert.AreEqual("84", asString2);
        }
    }
}