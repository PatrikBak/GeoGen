using System;
using System.Collections.Generic;
using GeoGen.Generator.ConfigurationsHandling.ConfigurationObjectToString;
using GeoGen.Generator.ConfigurationsHandling.ConfigurationObjectToString.ConfigurationObjectIdResolving;
using GeoGen.Generator.ConstructingObjects.Arguments.ArgumentsToString;
using Moq;
using NUnit.Framework;
using static GeoGen.Generator.Test.TestHelpers.Utilities;

namespace GeoGen.Generator.Test.ConfigurationsHandling.ConfigurationObjectToString
{
    [TestFixture]
    public class CustomFullObjectToStringProviderFactoryTest
    {
        private static CustomFullObjectToStringProviderFactory Factory()
        {
            var provider = SimpleMock<IArgumentsListToStringProvider>();
            var mock = new Mock<ICustomArgumentToStringProviderFactory>();
            mock.Setup(s => s.GetProvider(It.IsAny<IObjectToStringProvider>()))
                    .Returns<IObjectToStringProvider>(p => new CustomArgumentToStringProvider(p));

            return new CustomFullObjectToStringProviderFactory(provider, mock.Object);
        }

        [Test]
        public void Test_Arguments_To_String_Provider_Cant_Be_Null()
        {
            var factory = SimpleMock<ICustomArgumentToStringProviderFactory>();
            Assert.Throws<ArgumentNullException>(() => new CustomFullObjectToStringProviderFactory(null, factory));
        }

        [Test]
        public void Test_Factory_Cant_Be_Null()
        {
            var provider = SimpleMock<IArgumentsListToStringProvider>();
            Assert.Throws<ArgumentNullException>(() => new CustomFullObjectToStringProviderFactory(provider, null));
        }

        [Test]
        public void Test_Objects_Resolver_Is_Not_Null()
        {
            Assert.Throws<ArgumentNullException>(() => Factory().GetCustomProvider(null));
        }

        [Test]
        public void Test_Custom_Full_Provider_Is_Cached()
        {
            var resolver1 = new DictionaryObjectIdResolver(1, new Dictionary<int, int>());
            var resolver2 = new DictionaryObjectIdResolver(2, new Dictionary<int, int>());
            var factory = Factory();

            var provider1 = factory.GetCustomProvider(resolver1);
            var provider2 = factory.GetCustomProvider(resolver2);

            Assert.NotNull(provider1);
            Assert.NotNull(provider2);
            Assert.AreNotSame(provider1, provider2);

            var provider3 = factory.GetCustomProvider(resolver1);
            var provider4 = factory.GetCustomProvider(resolver2);

            Assert.AreSame(provider1, provider3);
            Assert.AreSame(provider2, provider4);
        }
    }
}