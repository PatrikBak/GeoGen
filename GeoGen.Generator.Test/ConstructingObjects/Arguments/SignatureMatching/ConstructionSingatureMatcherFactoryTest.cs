using GeoGen.Generator.ConstructingObjects.Arguments.SignatureMatching;
using NUnit.Framework;

namespace GeoGen.Generator.Test.ConstructingObjects.Arguments.SignatureMatching
{
    [TestFixture]
    public class ConstructionSingatureMatcherFactoryTest
    {
        private static ConstructionSignatureMatcherFactory Factory()
        {
            return new ConstructionSignatureMatcherFactory();
        }

        [Test]
        public void Test_Resulting_Matcher_Returns_Distinct_Objects()
        {
            var factory = Factory();

            var matcher1 = factory.CreateMatcher();
            var matcher2 = factory.CreateMatcher();

            Assert.NotNull(matcher1);
            Assert.NotNull(matcher2);
            Assert.AreNotSame(matcher1, matcher2);
        }
    }
}