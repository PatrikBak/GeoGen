using GeoGen.Utilities;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.AnalyticGeometry.Tests
{
    /// <summary>
    /// The test class for <see cref="AnalyticHelpers"/>.
    /// 
    /// TODO: Review
    /// 
    /// </summary>
    [TestFixture]
    public class AnalyticHelpersTest
    {
        [Test]
        public void Test_Lies_On_With_Line()
        {
            var line = new Line(new Point(1, 1), new Point(4, 5));

            Assert.IsTrue(AnalyticHelpers.LiesOn(line, new Point(1, 1)));
            Assert.IsTrue(AnalyticHelpers.LiesOn(line, new Point(7, 9)));
            Assert.IsTrue(AnalyticHelpers.LiesOn(line, new Point(4, 5)));
            Assert.IsTrue(AnalyticHelpers.LiesOn(line, new Point(-2, -3)));
            Assert.IsTrue(AnalyticHelpers.LiesOn(line, new Point(2.5, 3)));

            Assert.IsFalse(AnalyticHelpers.LiesOn(line, new Point(7, 2)));
            Assert.IsFalse(AnalyticHelpers.LiesOn(line, new Point(7, 3)));
            Assert.IsFalse(AnalyticHelpers.LiesOn(line, new Point(-5, 3)));
            Assert.IsFalse(AnalyticHelpers.LiesOn(line, new Point(-7, -7)));
        }

        [Test]
        public void Test_Lies_On_With_Circle()
        {
            var circle = new Circle(new Point(1, 1), 2);

            Assert.IsTrue(AnalyticHelpers.LiesOn(circle, new Point(-1, 1)));
            Assert.IsTrue(AnalyticHelpers.LiesOn(circle, new Point(-0.2, 2.6)));
            Assert.IsTrue(AnalyticHelpers.LiesOn(circle, new Point(-0.6, 2.2)));
            Assert.IsTrue(AnalyticHelpers.LiesOn(circle, new Point(3, 1)));

            Assert.IsFalse(AnalyticHelpers.LiesOn(circle, new Point(1, 1)));
            Assert.IsFalse(AnalyticHelpers.LiesOn(circle, new Point(7, -5)));
            Assert.IsFalse(AnalyticHelpers.LiesOn(circle, new Point(5, -1)));
            Assert.IsFalse(AnalyticHelpers.LiesOn(circle, new Point(3, -4)));
        }

        [Test]
        public void Test_Intersect_Two_Lines()
        {
            var objects = new IAnalyticObject[]
            {
                new Line(new Point(1, 5), new Point(1, 3)),
                new Line(new Point(1, 5), new Point(2, 4))
            };

            var points = AnalyticHelpers.Intersect(objects);

            Assert.AreEqual(1, points.Length);
            Assert.IsTrue(points.Contains(new Point(1, 5)));
        }

        [Test]
        public void Test_Intersect_Two_Circles()
        {
            var objects = new IAnalyticObject[]
            {
                new Circle(new Point(0, 0), new Point(1, 1), new Point(7, 8)),
                new Circle(new Point(0, 0), new Point(1, 1), new Point(8, 7))
            };

            var points = AnalyticHelpers.Intersect(objects);

            Assert.AreEqual(2, points.Length);
            Assert.IsTrue(points.Contains(new Point(0, 0)));
            Assert.IsTrue(points.Contains(new Point(1, 1)));
        }

        [Test]
        public void Test_Intersect_Line_And_Circle()
        {
            var objects = new IAnalyticObject[]
            {
                new Circle(new Point(0, 0), new Point(1, 1), new Point(7, 8)),
                new Line(new Point(0, 0), new Point(1, 1))
            };

            var points = AnalyticHelpers.Intersect(objects);

            Assert.AreEqual(2, points.Length);
            Assert.IsTrue(points.Contains(new Point(0, 0)));
            Assert.IsTrue(points.Contains(new Point(1, 1)));
        }

        [Test]
        public void Test_Intersect_Complex_Test_Medians()
        {
            var a = new Point(1, 5);
            var b = new Point(7, 4);
            var c = new Point(1, 77);

            var objects = new IAnalyticObject[]
            {
                new Line(a, (b+c)/2),
                new Line(b, (c+a)/2),
                new Line(c, (a+b)/2)
            };

            var points = AnalyticHelpers.Intersect(objects);

            Assert.AreEqual(1, points.Length);
            Assert.IsTrue(points.Contains((a + b + c) / 3));
        }

        [Test]
        public void Test_Intersect_Complex_Test_Feet_Of_Altitude()
        {
            var a = new Point(1, 5);
            var b = new Point(7, 4);
            var c = new Point(1, 77);

            var objects = new IAnalyticObject[]
            {
                new Line(b, c),
                new Circle((a+b)/2, a.DistanceTo(b) / 2),
                new Circle((a+c)/2, a.DistanceTo(c) / 2),
                new Line(a, a.Project(new Line(b, c)))
            };

            var points = AnalyticHelpers.Intersect(objects);

            Assert.AreEqual(1, points.Length);
            Assert.IsTrue(points.Contains(a.Project(new Line(b, c))));
        }

        [Test]
        public void Test_Angle_Between_Lines_With_Equal_Lines()
        {
            // Diagonal lines
            var line1 = new Line(new Point(0, 0), new Point(1, 1));
            var line2 = new Line(new Point(2, 2), new Point(3, 3));

            Assert.AreEqual(0, AnalyticHelpers.AngleBetweenLines(line1, line2).Rounded());

            // Vertical lines
            line1 = new Line(new Point(0, 1), new Point(0, 2));
            line2 = new Line(new Point(0, 3), new Point(0, 4));

            Assert.AreEqual(0, AnalyticHelpers.AngleBetweenLines(line1, line2).Rounded());

            // Horizontal lines
            line1 = new Line(new Point(1, 0), new Point(3, 0));
            line2 = new Line(new Point(2, 0), new Point(4, 0));

            Assert.AreEqual(0, AnalyticHelpers.AngleBetweenLines(line1, line2).Rounded());
        }

        [Test]
        public void Test_Angle_Between_Lines_With_Parallel_Lines()
        {
            // Diagonal lines
            var line1 = new Line(new Point(0, 0), new Point(1, 1));
            var line2 = new Line(new Point(-1, 0), new Point(0, 1));

            Assert.AreEqual(0, AnalyticHelpers.AngleBetweenLines(line1, line2).Rounded());

            // Vertical lines
            line1 = new Line(new Point(0, 1), new Point(0, 2));
            line2 = new Line(new Point(1, 3), new Point(1, 4));

            Assert.AreEqual(0, AnalyticHelpers.AngleBetweenLines(line1, line2).Rounded());

            // Horizontal lines
            line1 = new Line(new Point(1, 1), new Point(3, 1));
            line2 = new Line(new Point(2, 0), new Point(4, 0));

            Assert.AreEqual(0, AnalyticHelpers.AngleBetweenLines(line1, line2).Rounded());
        }

        [Test]
        public void Test_Angle_Between_Lines_With_Acute_Angled_Triangle()
        {
            // Prepare points
            var a = new Point(2, -2);
            var b = new Point(8, -2);
            var c = new Point(3, 6);

            // Prepare lines
            var lines = new List<Line>
            {
                new Line(a, b),
                new Line(a, c),
                new Line(b, c)
            };

            // Prepare precalculated results
            var results = new List<double>
            {
                Math.Atan(8),
                Math.Atan(8.0/5),
                Math.PI - Math.Atan(8) - Math.Atan(8.0/5)
            };

            // Assert
            Assert.AreEqual(results[0].Rounded(), AnalyticHelpers.AngleBetweenLines(lines[0], lines[1]).Rounded());
            Assert.AreEqual(results[1].Rounded(), AnalyticHelpers.AngleBetweenLines(lines[0], lines[2]).Rounded());
            Assert.AreEqual(results[2].Rounded(), AnalyticHelpers.AngleBetweenLines(lines[1], lines[2]).Rounded());
        }

        [Test]
        public void Test_Angle_Between_Lines_With_Right_Angled_Triangle()
        {
            // Prepare points
            var a = new Point(0, 3);
            var b = new Point(8, 5);
            var c = new Point(3, 8);

            // Prepare lines
            var lines = new List<Line>
            {
                new Line(a, b),
                new Line(a, c),
                new Line(b, c)
            };

            // Prepare precalculated results
            var results = new List<double>
            {
                Math.PI/4,
                Math.PI/4,
                Math.PI/2
            };

            // Assert
            Assert.AreEqual(results[0].Rounded(), AnalyticHelpers.AngleBetweenLines(lines[0], lines[1]).Rounded());
            Assert.AreEqual(results[1].Rounded(), AnalyticHelpers.AngleBetweenLines(lines[0], lines[2]).Rounded());
            Assert.AreEqual(results[2].Rounded(), AnalyticHelpers.AngleBetweenLines(lines[1], lines[2]).Rounded());
        }


        [Test]
        public void Test_Internal_Angle_Bisector_With_Intencter()
        {
            var a = new Point(1, 3);
            var b = new Point(2, 5);
            var c = new Point(7, 7);

            var alfa = AnalyticHelpers.InternalAngleBisector(a, b, c);
            var betta = AnalyticHelpers.InternalAngleBisector(b, a, c);
            var gamma = AnalyticHelpers.InternalAngleBisector(c, a, b);

            Assert.IsTrue(alfa.Contains(a));
            Assert.IsTrue(betta.Contains(b));
            Assert.IsTrue(gamma.Contains(c));

            var i1 = alfa.IntersectionWith(betta);
            var i2 = betta.IntersectionWith(gamma);
            var i3 = gamma.IntersectionWith(alfa);

            Assert.AreEqual(i1, i2);
            Assert.AreEqual(i2, i3);
            Assert.AreEqual(i3, i1);
        }

        [Test]
        public void Test_Internal_Angle_Bisector_With__Svrcek_Point()
        {
            var a = new Point(1, 3);
            var b = new Point(2, 5);
            var c = new Point(7, 7);

            var alfa = AnalyticHelpers.InternalAngleBisector(a, b, c);
            var bcBisector = AnalyticHelpers.PerpendicularBisector(b, c);
            var circumCircle = new Circle(a, b, c);
            var intersection = alfa.IntersectionWith(bcBisector).Value;

            Assert.IsTrue(circumCircle.Contains(intersection));
        }

        [Test]
        public void Test_Perpendicular_Bisector_Equal_Points()
        {
            Assert.Throws<AnalyticException>
            (
                () =>
                {
                    var p1 = new Point(1.0 / 3, 2.0 / 3);
                    var p2 = new Point(0.1 / 0.3, 0.2 / 0.3);

                    AnalyticHelpers.PerpendicularBisector(p1, p2);
                }
            );
        }

        [Test]
        public void Test_Perpedicular_Bisector_Distint_Points()
        {
            var tests = new List<Tuple<Point, Point>>
            {
                new Tuple<Point, Point>(new Point(0, 0), new Point(1, 0)),
                new Tuple<Point, Point>(new Point(0, 0), new Point(0, 1)),
                new Tuple<Point, Point>(new Point(7, 9.5), new Point(7, 9.6)),
                new Tuple<Point, Point>(new Point(0, 0), new Point(4, 4)),
                new Tuple<Point, Point>(new Point(17, 4), new Point(11, 2))
            };

            var unnormalizedCoefficients = new List<List<double>>
            {
                new List<double> {1, 0, -0.5},
                new List<double> {0, 1, -0.5},
                new List<double> {0, 1, -9.55},
                new List<double> {1, 1, -4},
                new List<double> {3, 1, -45}
            };

            foreach (var i in Enumerable.Range(0, tests.Count))
            {
                var test = tests[i];
                var line = AnalyticHelpers.PerpendicularBisector(test.Item1, test.Item2);

                var coefficients = unnormalizedCoefficients[i];
                var sumOfSquares = Math.Sqrt(coefficients[0].Squared() + coefficients[1].Squared());
                var normalized = coefficients.Select(c => c / sumOfSquares).ToList();

                Assert.IsTrue(line.A.Rounded() == normalized[0].Rounded(), $"{i}");
                Assert.IsTrue(line.B.Rounded() == normalized[1].Rounded(), $"{i}");
                Assert.IsTrue(line.C.Rounded() == normalized[2].Rounded(), $"{i}");
            }
        }
    }
}