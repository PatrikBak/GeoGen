using GeoGen.Generator.Constructing.Arguments.SignatureMatching;
using NUnit.Framework;

namespace GeoGen.Generator.Test.Constructing.Arguments.SignatureMatching
{
    [TestFixture]
    public class ConstructionSingatureMatcherFactoryTest
    {
        private static ConstructionSignatureMatcherFactory Factory()
        {
            return new ConstructionSignatureMatcherFactory();
        }

        [Test]
        public void Resulting_Matcher_Is_Correct()
        {
            var obj = Factory().CreateMatcher();

            Assert.NotNull(obj);
        }
    }
}