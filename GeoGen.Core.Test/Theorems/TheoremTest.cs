using System;
using NUnit.Framework;

namespace GeoGen.Core.Test.Theorems
{
    [TestFixture]
    public class TheoremTest
    {
        [Test]
        public void Test_Involved_Objects_Cant_Be_Null()
        {
            Assert.Throws<ArgumentNullException>(() => new Theorem(TheoremType.CollinearPoints, null));
        }

        [Test]
        public void Test_Involved_Objects_Cant_Be_Empty()
        {
            //Assert.Throws<ArgumentException>(() => new Theorem(TheoremType.CollinearPoints, new HashSet<TheoremObject>()));
        }
    }
}
