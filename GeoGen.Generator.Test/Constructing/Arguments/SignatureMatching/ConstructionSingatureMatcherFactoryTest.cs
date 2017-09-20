using System;
using GeoGen.Generator.Constructing.Arguments.SignatureMatching;
using GeoGen.Generator.ConstructingObjects.Arguments.Containers;
using GeoGen.Generator.ConstructingObjects.Arguments.SignatureMatching;
using NUnit.Framework;
using static GeoGen.Generator.Test.TestHelpers.Utilities;

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
        public void Test_Resulting_Matcher_Is_Correct()
        {
            var obj = Factory().CreateMatcher();

            Assert.NotNull(obj);
        }
    }
}