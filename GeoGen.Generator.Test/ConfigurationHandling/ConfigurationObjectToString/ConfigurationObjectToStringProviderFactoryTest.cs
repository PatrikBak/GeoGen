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
        private static CustomFullObjectToStringProviderFactory Factory()
        {
            var provider = SimpleMock<IArgumentsToStringProvider>();

            return new CustomFullObjectToStringProviderFactory(provider);
        }

        [Test]
        public void Arguments_To_String_Provider_Cannot_Be_Null()
        {
            Assert.Throws<ArgumentNullException>(() => new CustomFullObjectToStringProviderFactory(null));
        }

        [Test]
        public void Objects_Resolver_Is_Not_Null()
        {
            Assert.Throws<ArgumentNullException>(() => Factory().GetCustomProvider(null));
        }

        [Test]
        public void Custom_Provider_Is_Returned()
        {
            var resolver = new DictionaryObjectIdResolver();
            var factory = Factory();
            var provider1 = factory.GetCustomProvider(resolver);
            var provider2 = factory.GetCustomProvider(resolver);

            Assert.NotNull(provider1);
            Assert.NotNull(provider2);
            Assert.AreSame(provider1, provider2);
        }
    }
}