using System;
using GeoGen.Generator.ConfigurationHandling.ConfigurationObjectToString;
using GeoGen.Generator.ConfigurationHandling.ConfigurationObjectToString.ObjectIdResolving;
using GeoGen.Generator.Constructing.Arguments.ArgumentsToString;
using NUnit.Framework;
using static GeoGen.Generator.Test.TestHelpers.Utilities;

namespace GeoGen.Generator.Test.ConfigurationHandling.ConfigurationObjectToString
{
    [TestFixture]
    public class ConfigurationObjectToStringProviderFactoryTest
    {
        private static ConfigurationObjectToStringProviderFactory Factory()
        {
            var provider = SimpleMock<IArgumentsToStringProvider>();

            return new ConfigurationObjectToStringProviderFactory(provider);
        }

        [Test]
        public void Arguments_To_String_Provider_Cannot_Be_Null()
        {
            Assert.Throws<ArgumentNullException>(() => new ConfigurationObjectToStringProviderFactory(null));
        }

        [Test]
        public void Objects_Resolver_Is_Not_Null()
        {
            Assert.Throws<ArgumentNullException>(() => Factory().CreateCustomProvider(null));
            Assert.Throws<ArgumentNullException>(() => Factory().CreateDefaltProvider(null));
        }

        [Test]
        public void Custom_Provider_Is_Returned()
        {
            var resolver = SimpleMock<IObjectIdResolver>();
            var provider1 = Factory().CreateCustomProvider(resolver);
            var provider2 = Factory().CreateDefaltProvider(resolver);

            Assert.NotNull(provider1);
            Assert.NotNull(provider2);
        }
    }
}