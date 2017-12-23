using System;
using GeoGen.Core.Utilities;
using GeoGen.Utilities;
using NUnit.Framework;

namespace GeoGen.Core.Test.Utilities
{
    [TestFixture]
    public class ThrowHelperTest
    {
        [Test]
        public void Test_With_True_Condition()
        {
            Assert.Throws<Exception>(() => ThrowHelper.ThrowExceptionIfNotTrue(false));
        }

        [Test]
        public void Test_With_False_Condition()
        {
            ThrowHelper.ThrowExceptionIfNotTrue(true);
        }
    }
}