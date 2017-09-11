using System;
using GeoGen.Core.Configurations;
using GeoGen.Generator.ConfigurationHandling.ConfigurationObjectToString;
using NUnit.Framework;

namespace GeoGen.Generator.Test.ConfigurationHandling.ConfigurationObjectToString
{
    [TestFixture]
    public class DefaultConfigurationObjectIdToStringProviderTest
    {
        private static DefaultConfigurationObjectIdToStringProvider Provider()
        {
            return new DefaultConfigurationObjectIdToStringProvider();
        }

        [Test]
        public void Configuration_Object_Not_Null()
        {
            Assert.Throws<ArgumentNullException>(() => Provider().ConvertToString(null));
        }

        [Test]
        public void Configuration_Object_Must_Have_Set_Id()
        {
            var obj = new LooseConfigurationObject(ConfigurationObjectType.Point);

            Assert.Throws<GeneratorException>(() => Provider().ConvertToString(obj));
        }

        [Test]
        public void Configurutation_Object_Id_To_String()
        {
            var obj = new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 42};
            var asString = Provider().ConvertToString(obj);

            Assert.AreEqual("42", asString);
        }
    }
}