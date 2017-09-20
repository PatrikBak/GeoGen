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
        public void Test_Resulting_Matcher_Is_Correct()
        {
            var obj = Factory().CreateMatcher();

            Assert.NotNull(obj);
        }
    }
}