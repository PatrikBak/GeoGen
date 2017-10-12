//using System;
//using System.Collections.Generic;
//using GeoGen.Analyzer.AnalyticalGeometry;
//using NUnit.Framework;

//namespace GeoGen.Analyzer.Test.AnalyticalGeometry
//{
//    [TestFixture]
//    public class PointTest
//    {
//        private static List<Point> Points()
//        {
//            return new List<Point>
//            {
//                new Point(5, 4),
//                new Point(-3, 2.5),
//                new Point(2.5, -1),
//                new Point(-3, -3),
//                new Point(-3, 2.5),
//                new Point(5, 4),
//                new Point(-2.5, 1)
//            };
//        }

//        [Test]
//        public void Test_Plus_Operator()
//        {
//            var points = Points();

//            var sum1 = points[0] + points[1];
//            var sum2 = points[1] + points[2];
//            var sum3 = points[2] + points[6];

//            Assert.AreEqual(2, sum1.X);
//            Assert.AreEqual(6.5, sum1.Y);

//            Assert.AreEqual(-0.5, sum2.X);
//            Assert.AreEqual(1.5, sum2.Y);

//            Assert.AreEqual(0, sum3.X);
//            Assert.AreEqual(0, sum3.Y);
//        }

//        [Test]
//        public void Test_Minus_Operator()
//        {
//            var points = Points();

//            var dif1 = points[0] - points[1];
//            var dif2 = points[1] - points[2];
//            var dif3 = points[3] - points[6];

//            Assert.AreEqual(8, dif1.X);
//            Assert.AreEqual(1.5, dif1.Y);

//            Assert.AreEqual(-5.5, dif2.X);
//            Assert.AreEqual(3.5, dif2.Y);

//            Assert.AreEqual(-0.5, dif3.X);
//            Assert.AreEqual(-4, dif3.Y);
//        }

//        [Test]
//        public void Test_Multiplication_Operator()
//        {
//            var points = Points();

//            var p1 = points[0] * 5;
//            Assert.AreEqual(25, p1.X);
//            Assert.AreEqual(20, p1.Y);

//            var p2 = points[1] * 0;
//            Assert.AreEqual(0, p2.X);
//            Assert.AreEqual(0, p2.Y);

//            var p3 = points[2] * -2;
//            Assert.AreEqual(-5, p3.X);
//            Assert.AreEqual(2, p3.Y);
//        }

//        [Test]
//        public void Test_Division_Operator()
//        {
//            var points = Points();

//            var p1 = points[0] / 5;
//            Assert.AreEqual(1, p1.X);
//            Assert.AreEqual(0.8, p1.Y);

//            var p2 = points[1] / -2;
//            Assert.AreEqual(1.5, p2.X);
//            Assert.AreEqual(-1.25, p2.Y);
//        }

//        [Test]
//        public void Test_Equals_Operator_And_Method()
//        {
//            var points = Points();

//            Assert.IsTrue(points[0] == points[5]);
//            Assert.IsFalse(points[0].Equals(points[1]));

//            Assert.IsTrue(points[0].Equals(points[0]));
//        }

//        [Test]
//        public void Test_Not_Equals_Operator()
//        {
//            var points = Points();

//            Assert.IsTrue(points[0] != points[1]);
//            Assert.IsFalse(points[0] != points[5]);
//        }

//        [Test]
//        public void Test_Hash_Code()
//        {
//            var p1 = new Point(1, 5);
//            var p2 = new Point(1, 5);

//            Assert.AreEqual(p1.GetHashCode(), p2.GetHashCode());
//        }
//    }
//}