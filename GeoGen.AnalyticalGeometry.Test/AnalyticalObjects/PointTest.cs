//using System;
//using System.Collections.Generic;
//using System.Linq;
//using GeoGen.Utilities;
//using NUnit.Framework;

//namespace GeoGen.AnalyticalGeometry.Test.AnalyticalObjects
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

//            Assert.IsTrue(2 == sum1.X);
//            Assert.IsTrue(6.5 == sum1.Y);

//            Assert.IsTrue(-0.5 == sum2.X);
//            Assert.IsTrue(1.5 == sum2.Y);

//            Assert.IsTrue(0 == sum3.X);
//            Assert.IsTrue(0 == sum3.Y);
//        }

//        [Test]
//        public void Test_Multiplication_Operator()
//        {
//            var points = Points();

//            var p1 = points[0] * 5;
//            Assert.IsTrue(25 == p1.X);
//            Assert.IsTrue(20 == p1.Y);

//            var p2 = points[1] * 0;
//            Assert.IsTrue(0 == p2.X);
//            Assert.IsTrue(0 == p2.Y);

//            var p3 = points[2] * -2;
//            Assert.IsTrue(-5 == p3.X);
//            Assert.IsTrue(2 == p3.Y);
//        }

//        [Test]
//        public void Test_Division_Operator()
//        {
//            var points = Points();

//            var p1 = points[0] / 5;
//            Assert.IsTrue(1 == p1.X);
//            Assert.IsTrue(0.8 == p1.Y);

//            var p2 = points[1] / -2;
//            Assert.IsTrue(1.5 == p2.X);
//            Assert.IsTrue(-1.25 == p2.Y);
//        }

//        [Test]
//        public void Test_Equals_Method()
//        {
//            var points1 = new List<Point>
//            {
//                new Point(1.5, 1.7),
//                new Point(-1.2, 1.8),
//                new Point(1.0 / 3, 2.5),
//                new Point(1e-45, -1e-43)
//            };

//            var points2 = new List<Point>
//            {
//                new Point(3.0 / 2, 1.7),
//                new Point(1.2, 1.8),
//                new Point(0.333333333, 2.5),
//                new Point(1e-45, -1e-43)
//            };

//            var expected = new List<bool>
//            {
//                true,
//                false,
//                true,
//                true
//            };

//            for (var i = 0; i < points1.Count; i++)
//            {
//                var result = points1[i] == points2[i];
//                Assert.AreEqual(expected[i], result, $"{i}");
//                Assert.IsTrue(expected[i] == result, $"{i}");
//            }
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
//            var p1 = new Point(0.7 / 0.025, 1);
//            var p2 = new Point(28, 1);

//            Assert.AreEqual(p1.GetHashCode(), p2.GetHashCode());
//        }

//        [Test]
//        public void Test_Midpoint_Distint_Points()
//        {
//            var p1 = new Point(1, 7);
//            var p2 = new Point(4.5, -3);
//            var p3 = new Point(-4, 5);

//            var midpoint1 = p1.Midpoint(p2);
//            var midpoint2 = p1.Midpoint(p3);

//            Assert.IsTrue(2.75 == midpoint1.X);
//            Assert.IsTrue(2 == midpoint1.Y);

//            Assert.IsTrue(-1.5 == midpoint2.X);
//            Assert.IsTrue(6 == midpoint2.Y);
//        }

//        [Test]
//        public void Test_Midpoint_Same_Point()
//        {
//            var p = new Point(42, 666);
//            var midpoint = p.Midpoint(p);

//            Assert.AreEqual(p, midpoint);
//        }

//        [Test]
//        public void Test_Distance_To_Distint_Points()
//        {
//            var p1 = new Point(0, -7);
//            var p2 = new Point(1, 5);
//            var p3 = new Point(3, -3);

//            var d1 = p1.DistanceTo(p2);
//            var d2 = p2.DistanceTo(p1);

//            var d3 = p2.DistanceTo(p3);
//            var d4 = p1.DistanceTo(p3);

//            Assert.IsTrue(d1 == Math.Sqrt(145));
//            Assert.IsTrue(d2 == d1);
//            Assert.IsTrue(d3 == Math.Sqrt(68));
//            Assert.IsTrue(d4 == 5);
//        }

//        [Test]
//        public void Test_Distance_To_Same_Point()
//        {
//            var p = new Point(42, 666);
//            RoundedDouble distance = (RoundedDouble) p.DistanceTo(p);

//            Assert.IsTrue(distance == 0);
//        }

//        [Test]
//        public void Test_Rotate_Point_Around_Itself()
//        {
//            var point = new Point(3, 4);

//            for (var angle = 0; angle <= 360; angle += 10)
//            {
//                var result = point.Rotate(point, angle);
//                Assert.AreEqual(result, point);
//            }
//        }

//        [Test]
//        public void Test_Rotate_Point_Around_Other_Point()
//        {
//            var point = new Point(7, 5);
//            var center = new Point(3, 4);

//            var angles = new List<double> {0, 30, 45, 60, 90, 180, 210, 360};

//            var expected = new List<Point>
//            {
//                new Point(7, 5),
//                new Point((5 + 4 * Math.Sqrt(3)) / 2, (12 + Math.Sqrt(3)) / 2),
//                new Point((6 + 3 * Math.Sqrt(2)) / 2, (8 + 5 * Math.Sqrt(2)) / 2),
//                new Point((10 - Math.Sqrt(3)) / 2, (9 + 4 * Math.Sqrt(3)) / 2),
//                new Point(2, 8),
//                new Point(-1, 3),
//                new Point((7 - 4 * Math.Sqrt(3)) / 2, (4 - Math.Sqrt(3)) / 2),
//                new Point(7, 5)
//            };

//            for (var i = 0; i <= 3; i++)
//            {
//                var result = point.Rotate(center, angles[i]);

//                Assert.AreEqual(expected[i], result);
//            }
//        }

//        [Test]
//        public void Test_Perpendicular_Bisector_Equal_Points()
//        {
//            Assert.Throws<ArgumentException>
//            (
//                () =>
//                {
//                    var p1 = new Point(1.0 / 3, 2.0 / 3);
//                    var p2 = new Point(0.1 / 0.3, 0.2 / 0.3);

//                    p1.PerpendicularBisector(p2);
//                }
//            );
//        }

//        [Test]
//        public void Test_Perpedicular_Bisector_Distint_Points()
//        {
//            var tests = new List<Tuple<Point, Point>>
//            {
//                new Tuple<Point, Point>(new Point(0, 0), new Point(1, 0)),
//                new Tuple<Point, Point>(new Point(0, 0), new Point(0, 1)),
//                new Tuple<Point, Point>(new Point(7, 9.5), new Point(7, 9.6)),
//                new Tuple<Point, Point>(new Point(0, 0), new Point(4, 4)),
//                new Tuple<Point, Point>(new Point(17, 4), new Point(11, 2))
//            };

//            var unnormalizedCoefficients = new List<List<double>>
//            {
//                new List<double> {1, 0, -0.5},
//                new List<double> {0, 1, -0.5},
//                new List<double> {0, 1, -9.55},
//                new List<double> {1, 1, -4},
//                new List<double> {3, 1, -45}
//            };

//            foreach (var i in Enumerable.Range(0, tests.Count))
//            {
//                var test = tests[i];
//                var line = test.Item1.PerpendicularBisector(test.Item2);

//                var coefficients = unnormalizedCoefficients[i];
//                var sumOfSquares = Math.Sqrt(coefficients.Sum(c => c * c));
//                var normalized = coefficients.Select(c => c / sumOfSquares).ToList();

//                Assert.IsTrue(line.A == normalized[0], $"{i}");
//                Assert.IsTrue(line.B == normalized[1], $"{i}");
//                Assert.IsTrue(line.C == normalized[2], $"{i}");
//            }
//        }

//        [Test]
//        public void Test_Projection()
//        {
//            var a = new Point(-4, -2);
//            var b = new Point(6, -2);
//            var c = new Point(-4, 6);

//            var aXbc = a.Project(new Line(b, c));
//            var bXac = b.Project(new Line(a, c));
//            var cXab = c.Project(new Line(a, b));

//            Assert.IsTrue(aXbc == new Point(-4.0 / 41, 118.0 / 41));
//            Assert.IsTrue(bXac == a);
//            Assert.IsTrue(cXab == a);

//            var aXbcCopy = aXbc.Project(new Line(b, c));
//            Assert.IsTrue(aXbcCopy == aXbc);
//        }

//        [Test]
//        public void Test_Internal_Angle_Bisector_With_Intencter()
//        {
//            var a = new Point(1, 3);
//            var b = new Point(2, 5);
//            var c = new Point(7, 7);

//            var alfa = a.InternalAngleBisector(b, c);
//            var betta = b.InternalAngleBisector(a, c);
//            var gamma = c.InternalAngleBisector(a, b);

//            Assert.IsTrue(alfa.Contains(a));
//            Assert.IsTrue(betta.Contains(b));
//            Assert.IsTrue(gamma.Contains(c));
            
//            var i1 = alfa.IntersectionWith(betta);
//            var i2 = betta.IntersectionWith(gamma);
//            var i3 = gamma.IntersectionWith(alfa);

//            Assert.AreEqual(i1, i2);
//            Assert.AreEqual(i2, i3);
//            Assert.AreEqual(i3, i1);
//        }

//        [Test]
//        public void Test_Internal_Angle_Bisector_With__Svrcek_Point()
//        {
//            var a = new Point(1, 3);
//            var b = new Point(2, 5);
//            var c = new Point(7, 7);

//            var alfa = a.InternalAngleBisector(b, c);
//            var bcBisector = b.PerpendicularBisector(c);
//            var circumCircle = new Circle(a, b, c);
//            var intersection = alfa.IntersectionWith(bcBisector);

//            Assert.IsTrue(circumCircle.Contains(intersection));
//        }
//    }
//}