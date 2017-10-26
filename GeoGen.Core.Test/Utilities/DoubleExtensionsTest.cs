//using System;
//using GeoGen.Core.Utilities;
//using NUnit.Framework;

//namespace GeoGen.Core.Test.Utilities
//{
//    [TestFixture]
//    public class DoubleExtensionsTest
//    {
//        [Test]
//        public void Test_Equal_And_Hash_Code_Square_Roots()
//        {
//            var root = Math.Sqrt(1.0 / 3);
//            var sameRoot = Math.Sqrt(3) / 3;

//            Assert.IsTrue(root.EqualTo(sameRoot));
//            Assert.AreEqual(root.HashCode(), sameRoot.HashCode());
//        }

//        [Test]
//        public void Test_Equal_And_Hash_Code_Tangens()
//        {
//            var a1 = MathUtils.ToRadians(23);
//            var a2 = MathUtils.ToRadians(111);

//            var tanSum = Math.Tan(a1 + a2);
//            var tan1 = Math.Tan(a1);
//            var tan2 = Math.Tan(a2);

//            var sameSum = (tan1 + tan2) / (1 - tan1 * tan2);

//            Assert.IsTrue(tanSum.EqualTo(sameSum));
//            Assert.AreEqual(tanSum.HashCode(), sameSum.HashCode());
//        }

//        [Test]
//        public void Test_Equal_And_Hash_Code_Dividing_Doubles()
//        {
//            const double v1 = 0.7;
//            const double v2 = 0.025;
//            const double result = v1 / v2;

//            Assert.IsTrue(result.EqualTo(28));
//            Assert.AreEqual(28.0.HashCode(), result.HashCode());
//        }
//    }
//}