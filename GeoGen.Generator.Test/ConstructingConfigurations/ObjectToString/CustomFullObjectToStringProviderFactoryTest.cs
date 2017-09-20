using System;
using System.Collections.Generic;
using GeoGen.Generator.ConstructingConfigurations.ObjectToString;
using GeoGen.Generator.ConstructingConfigurations.ObjectToString.ObjectIdResolving;
using GeoGen.Generator.ConstructingObjects.Arguments.ArgumentsListToString;
using NUnit.Framework;
using static GeoGen.Generator.Test.TestHelpers.Utilities;

namespace GeoGen.Generator.Test.ConstructingConfigurations.ObjectToString
{
    [TestFixture]
    public class CustomFullObjectToStringProviderFactoryTest
    {
        private static CustomFullObjectToStringProviderFactory Factory()
        {
            var provider = SimpleMock<IArgumentsListToStringProvider>();

            return new CustomFullObjectToStringProviderFactory(provider);
        }

        [Test]
        public void Test_Arguments_To_String_Provider_Cant_Be_Null()
        {
            Assert.Throws<ArgumentNullException>(() => new CustomFullObjectToStringProviderFactory(null));
        }


        [Test]
        public void Test_Passed_Objects_Resolver_Cant_Be_Null()
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