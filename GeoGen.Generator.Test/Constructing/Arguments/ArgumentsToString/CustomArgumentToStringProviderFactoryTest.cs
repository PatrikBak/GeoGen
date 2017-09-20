using System;
using GeoGen.Generator.ConfigurationsHandling.ConfigurationObjectToString;
using GeoGen.Generator.ConfigurationsHandling.ConfigurationObjectToString.ConfigurationObjectIdResolving;
using GeoGen.Generator.ConstructingObjects.Arguments.ArgumentsToString;
using Moq;
using NUnit.Framework;

namespace GeoGen.Generator.Test.Constructing.Arguments.ArgumentsToString
{
    [TestFixture]
    public class CustomArgumentToStringProviderFactoryTest
    {
        private static CustomArgumentToStringProviderFactory Factory()
        {
            return new CustomArgumentToStringProviderFactory();
        }

        private static IObjectToStringProvider Provider(int id)
        {
            var resolver = new Mock<IObjectIdResolver>();
            resolver.Setup(s => s.Id).Returns(id);

            var mock = new Mock<IObjectToStringProvider>();
            mock.Setup(s => s.Resolver).Returns(resolver.Object);

            return mock.Object;
        }

        [Test]
        public void Test_Passed_Provider_Cant_Be_Null()
        {
            Assert.Throws<ArgumentNullException>(() => Factory().GetProvider(null));
        }

        [Test]
        public void Test_Caching_Is_Really_Happening()
        {
            var factory = Factory();
            var provider1 = Provider(1);
            var provider2 = Provider(2);

            var a1 = factory.GetProvider(provider1);
            var a2 = factory.GetProvider(provider2);
            var a3 = factory.GetProvider(provider1);
            var a4 = factory.GetProvider(provider2);

            Assert.NotNull(a1);
            Assert.NotNull(a2);
            Assert.AreSame(a1, a3);
            Assert.AreSame(a2, a4);
            Assert.AreNotSame(a1, a2);
        }
    }
}