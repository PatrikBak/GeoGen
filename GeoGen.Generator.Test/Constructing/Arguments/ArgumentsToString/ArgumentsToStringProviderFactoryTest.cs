using System;
using GeoGen.Generator.ConfigurationsHandling.ConfigurationObjectToString;
using GeoGen.Generator.Constructing.Arguments.ArgumentsToString;
using NUnit.Framework;

namespace GeoGen.Generator.Test.Constructing.Arguments.ArgumentsToString
{
    [TestFixture]
    public class ArgumentsToStringProviderFactoryTest
    {
        private ArgumentsToStringProviderFactory Factory()
        {
            return new ArgumentsToStringProviderFactory();
        }

        [Test]
        public void Provider_Cant_Be_Null()
        {
            Assert.Throws<ArgumentNullException>(() => Factory().CreateProvider(null));
        }

        [Test]
        public void Resulting_Provider_Is_Correct()
        {
            var provider = new DefaultObjectToStringProvider();
            var obj = Factory().CreateProvider(provider);

            Assert.NotNull(obj);
        }
    }
}