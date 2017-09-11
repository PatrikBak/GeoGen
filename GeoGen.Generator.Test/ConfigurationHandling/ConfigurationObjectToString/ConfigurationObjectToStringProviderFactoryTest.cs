using System;
using GeoGen.Generator.ConfigurationHandling.ConfigurationObjectToString;
using GeoGen.Generator.ConfigurationHandling.ConfigurationObjectToString.LooseObjectIdResolving;
using GeoGen.Generator.Constructing.Arguments.ArgumentsToString;
using Moq;
using NUnit.Framework;

namespace GeoGen.Generator.Test.ConfigurationHandling.ConfigurationObjectToString
{
    [TestFixture]
    public class ConfigurationObjectToStringProviderFactoryTest
    {
        private static IArgumentsToStringProvider Provider()
        {
            var mock = new Mock<IArgumentsToStringProvider>();

            return mock.Object;
        }

        private static ConfigurationObjectToStringProviderFactory Factory()
        {
            return new ConfigurationObjectToStringProviderFactory(Provider());
        }

        [Test]
        public void Arguments_To_String_Provider_Cannot_Be_Null()
        {
            Assert.Throws<ArgumentNullException>(() => new ConfigurationObjectToStringProviderFactory(null));
        }

        [Test]
        public void Default_Provider_Is_Returned()
        {
            var defaultProvider = Factory().CreateDefaultProvider();

            Assert.NotNull(defaultProvider);
        }

        [Test]
        public void Loose_Objects_Resolver_Is_Not_Null()
        {
            Assert.Throws<ArgumentNullException>(() => Factory().CreateCustomProvider(null));
        }

        [Test]
        public void Loose_Objects_Resolver_Is_Not_Default()
        {
            Assert.Throws<GeneratorException>(() => Factory().CreateCustomProvider(new DefaultLooseConfigurationObjectIdResolver()));
        }

        [Test]
        public void Custom_Provider_Is_Returned()
        {
            var resolver = new Mock<ILooseConfigurationObjectIdResolver>().Object;
            var provider = Factory().CreateCustomProvider(resolver);

            Assert.NotNull(provider);
        }
    }
}