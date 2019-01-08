using GeoGen.Utilities;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace GeoGen.AnalyticGeometry.Tests
{
    [TestFixture]
    public class PointTest
    {
        private static List<Point> Points()
        {
            return new List<Point>
            {
                new Point(5, 4),
                new Point(-3, 2.5),
                new Point(2.5, -1),
                new Point(-3, -3),
                new Point(-3, 2.5),
                new Point(5, 4),
                new Point(-2.5, 1)
            };
        }

        [Test]
        public void Test_Plus_Operator()
        {
            var points = Points();

            var sum1 = points[0] + points[1];
            var sum2 = points[1] + points[2];
            var sum3 = points[2] + points[6];

            Assert.IsTrue(2 == sum1.X);
            Assert.IsTrue(6.5 == sum1.Y);

            Assert.IsTrue(-0.5 == sum2.X);
            Assert.IsTrue(1.5 == sum2.Y);

            Assert.IsTrue(0 == sum3.X);
            Assert.IsTrue(0 == sum3.Y);
        }

        [Test]
        public void Test_Multiplication_Operator()
        {
            var points = Points();

            var p1 = points[0] * 5;
            Assert.IsTrue(25 == p1.X);
            Assert.IsTrue(20 == p1.Y);

            var p2 = points[1] * 0;
            Assert.IsTrue(0 == p2.X);
            Assert.IsTrue(0 == p2.Y);

            var p3 = points[2] * -2;
            Assert.IsTrue(-5 == p3.X);
            Assert.IsTrue(2 == p3.Y);
        }

        [Test]
        public void Test_Division_Operator()
        {
            var points = Points();

            var p1 = points[0] / 5;
            Assert.IsTrue(1 == p1.X);
            Assert.IsTrue(0.8 == p1.Y);

            var p2 = points[1] / -2;
            Assert.IsTrue(1.5 == p2.X);
            Assert.IsTrue(-1.25 == p2.Y);
        }

        [Test]
        public void Test_Equals_Method()
        {
            var points1 = new List<Point>
            {
                new Point(1.5, 1.7),
                new Point(-1.2, 1.8),
                new Point(1.0 / 3, 2.5),
                new Point(1e-45, -1e-43)
            };

            var points2 = new List<Point>
            {
                new Point(3.0 / 2, 1.7),
                new Point(1.2, 1.8),
                new Point(0.33333333333333333333333333, 2.5),
                new Point(1e-45, -1e-43)
            };

            var expected = new List<bool>
            {
                true,
                false,
                true,
                true
            };

            for (var i = 0; i < points1.Count; i++)
            {
                var result = points1[i] == points2[i];
                Assert.AreEqual(expected[i], result, $"{i}");
                Assert.IsTrue(expected[i] == result, $"{i}");
            }
        }

        [Test]
        public void Test_Not_Equals_Operator()
        {
            var points = Points();

            Assert.IsTrue(points[0] != points[1]);
            Assert.IsFalse(points[0] != points[5]);
        }

        [Test]
        public void Test_Hash_Code()
        {
            var p1 = new Point(0.7 / 0.025, 1);
            var p2 = new Point(28, 1);

            Assert.AreEqual(p1.GetHashCode(), p2.GetHashCode());
        }

        [Test]
        public void Test_Distance_To_Distint_Points()
        {
            var p1 = new Point(0, -7);
            var p2 = new Point(1, 5);
            var p3 = new Point(3, -3);

            var d1 = p1.DistanceTo(p2);
            var d2 = p2.DistanceTo(p1);

            var d3 = p2.DistanceTo(p3);
            var d4 = p1.DistanceTo(p3);

            Assert.IsTrue(d1 == Math.Sqrt(145));
            Assert.IsTrue(d2 == d1);
            Assert.IsTrue(d3 == Math.Sqrt(68));
            Assert.IsTrue(d4 == 5);
        }

        [Test]
        public void Test_Distance_To_Same_Point()
        {
            var p = new Point(42, 666);

            Assert.IsTrue(p.DistanceTo(p).Rounded() == 0);
        }

        [Test]
        public void Test_Rotate_Point_Around_Itself()
        {
            var point = new Point(3, 4);

            for (var angle = 0; angle <= 360; angle += 10)
            {
                var result = point.Rotate(point, angle);
                Assert.AreEqual(result, point);
            }
        }

        [Test]
        public void Test_Rotate_Point_Around_Other_Point()
        {
            var point = new Point(7, 5);
            var center = new Point(3, 4);

            var angles = new List<double> { 0, 30, 45, 60, 90, 180, 210, 360 };

            var expected = new List<Point>
            {
                new Point(7, 5),
                new Point((5 + 4 * Math.Sqrt(3)) / 2, (12 + Math.Sqrt(3)) / 2),
                new Point((6 + 3 * Math.Sqrt(2)) / 2, (8 + 5 * Math.Sqrt(2)) / 2),
                new Point((10 - Math.Sqrt(3)) / 2, (9 + 4 * Math.Sqrt(3)) / 2),
                new Point(2, 8),
                new Point(-1, 3),
                new Point((7 - 4 * Math.Sqrt(3)) / 2, (4 - Math.Sqrt(3)) / 2),
                new Point(7, 5)
            };

            for (var i = 0; i <= 3; i++)
            {
                var result = point.Rotate(center, angles[i]);

                Assert.AreEqual(expected[i], result);
            }
        }

        [Test]
        public void Test_Projection()
        {
            var a = new Point(-4, -2);
            var b = new Point(6, -2);
            var c = new Point(-4, 6);

            var aXbc = a.Project(new Line(b, c));
            var bXac = b.Project(new Line(a, c));
            var cXab = c.Project(new Line(a, b));

            Assert.IsTrue(aXbc == new Point(-4.0 / 41, 118.0 / 41));
            Assert.IsTrue(bXac == a);
            Assert.IsTrue(cXab == a);

            var aXbcCopy = aXbc.Project(new Line(b, c));
            Assert.IsTrue(aXbcCopy == aXbc);
        }
    }
}