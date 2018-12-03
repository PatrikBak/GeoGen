using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace GeoGen.AnalyticalGeometry.Test.AnalyticalObjects
{
    [TestFixture]
    public class LineTest
    {
        [Test]
        public void Test_Construction_From_Two_Distinct_Points()
        {
            var points = new List<Tuple<Point, Point>>
            {
                new Tuple<Point, Point>(new Point(3, 0), new Point(4, 0)),
                new Tuple<Point, Point>(new Point(0, 5), new Point(0, 6)),
                new Tuple<Point, Point>(new Point(1, 1), new Point(7, 2)),
                new Tuple<Point, Point>(new Point(-4, 3), new Point(8, 2)),
                new Tuple<Point, Point>(new Point(7, 5), new Point(7, 6)),
                new Tuple<Point, Point>(new Point(1, 1), new Point(2, 1))
            };

            var expected = new List<List<int>>
            {
                new List<int> {0, 1, 0},
                new List<int> {1, 0, 0},
                new List<int> {1, -6, 5},
                new List<int> {1, 12, -32},
                new List<int> {1, 0, -7},
                new List<int> {0, 1, -1}
            };

            foreach (var i in Enumerable.Range(0, points.Count))
            {
                Console.WriteLine(i);

                var point1 = points[i].Item1;
                var point2 = points[i].Item2;

                var line1 = new Line(point1, point2);
                var line2 = new Line(point2, point1);

                Assert.IsTrue(line1.A == line2.A);
                Assert.IsTrue(line1.B == line2.B);
                Assert.IsTrue(line1.C == line2.C);

                var coefficients = expected[i];
                var sumOfSquares = Math.Sqrt(coefficients.Sum(c => c * c));
                var normalized = coefficients.Select(c => c / sumOfSquares).ToList();

                Assert.IsTrue(normalized[0] == line1.A);
                Assert.IsTrue(normalized[1] == line1.B);
                Assert.IsTrue(normalized[2] == line1.C);
            }
        }

        [Test]
        public void Test_Construction_From_Equal_Points()
        {
            var p1 = new Point(1, 4);
            var p2 = new Point(8.0 / 8.0, 8.0 / 2.0);

            Assert.Throws<AnalyticalException>(() => new Line(p1, p2));
            Assert.Throws<AnalyticalException>(() => new Line(p1, p1));
            Assert.Throws<AnalyticalException>(() => new Line(p2, p2));
        }

        [Test]
        public void Test_Line_Intersections()
        {
            var a = new Point(1, 2);
            var b = new Point(2, 3);
            var c = new Point(-4, 5);
            var d = a.Midpoint(b);
            var e = a.Midpoint(c);

            var ab = new Line(a, b);
            var ac = new Line(a, c);
            var bc = new Line(b, c);
            var de = new Line(d, e);
            var cd = new Line(c, d);
            var be = new Line(b, e);

            var abIac = ab.IntersectionWith(ac);
            Assert.IsTrue(abIac == a);

            var abIbc = bc.IntersectionWith(ab);
            Assert.IsTrue(b == abIbc);

            Assert.Throws<AnalyticalException>(() => ab.IntersectionWith(ab));
            Assert.Throws<AnalyticalException>(() => ab.IntersectionWith(new Line(a, d)));

            var bcIde = bc.IntersectionWith(de);
            Assert.IsNull(bcIde);

            var cdIBe = cd.IntersectionWith(be);
            Assert.AreEqual(new Point(-1.0 / 3, 10.0 / 3), cdIBe);
        }

        [Test]
        public void Test_Line_Equals_And_Not_Equals_Operator()
        {
            var a = new Point(1, 2);
            var b = new Point(2, 3);
            var c = new Point(-4, 5);
            var d = a.Midpoint(b);

            var ab = new Line(a, b);
            var ac = new Line(a, c);
            var ba = new Line(b, a);
            var ad = new Line(a, d);

            Assert.IsFalse(ab == ac);
            Assert.IsTrue(ab == ad);
            Assert.IsTrue(ad == ba);

            Assert.IsTrue(ab != ac);
            Assert.IsFalse(ab != ad);
            Assert.IsFalse(ad != ba);

            Assert.IsFalse(ab.Equals(ac));
            Assert.IsTrue(ab.Equals(ad));
            Assert.IsTrue(ad.Equals(ba));
        }

        [Test]
        public void Test_Line_Equality()
        {
            var a = new Point(1, 2);
            var b = new Point(2, 3);
            var c = a.Midpoint(b);

            var set = new HashSet<Line>
            {
                new Line(a, b),
                new Line(b, a),
                new Line(a, c),
                new Line(c, a),
                new Line(b, c),
                new Line(c, b)
            };

            Assert.AreEqual(1, set.Count);
        }

        [Test]
        public void Test_Line_Equality_Complex_Test()
        {
            var a = new Point(1.0 / 3, 2.7 / 11);
            var b = new Point(2.0 / 7, 3.9 / 13);
            var c = new Point(-4.5 / 17, 5.19 / 19);

            var points = new List<Point> { a, b, c }
                    .Concat
                    (
                        new List<Point>
                        {
                            a.Midpoint(b),
                            b.Midpoint(c),
                            c.Midpoint(a),
                            (a + b + c) / 3
                        }
                    )
                    .ToList();

            var lines = new HashSet<Line>();

            for (var i = 0; i < points.Count; i++)
            {
                for (var j = i + 1; j < points.Count; j++)
                {
                    var line1 = new Line(points[i], points[j]);
                    var line2 = new Line(points[j], points[i]);

                    lines.Add(line1);
                    lines.Add(line2);
                }
            }

            Assert.AreEqual(9, lines.Count);
        }

        [Test]
        public void Test_Line_Contains_Point()
        {
            var a = new Point(7, 11);
            var b = new Point(8, 19);
            var c = new Point(-666, 42);

            var xab = a.PerpendicularBisector(b);
            var xac = a.PerpendicularBisector(c);
            var xbc = b.PerpendicularBisector(c);

            var o = xab.IntersectionWith(xac) ?? throw new Exception("No intersection");

            Assert.IsTrue(xbc.Contains(o));

            Assert.IsTrue(xbc.Contains(b.Midpoint(c)));
            Assert.IsTrue(xac.Contains(a.Midpoint(c)));
            Assert.IsTrue(xab.Contains(a.Midpoint(b)));

            var ab = new Line(a, b);
            var bc = new Line(b, c);
            var ac = new Line(c, a);

            Assert.IsTrue(ab.Contains(a));
            Assert.IsTrue(ab.Contains(b));
            Assert.IsTrue(bc.Contains(b));
            Assert.IsTrue(bc.Contains(c));
            Assert.IsTrue(ac.Contains(a));
            Assert.IsTrue(ac.Contains(c));
        }

        [Test]
        public void Test_Line_Hash_Code()
        {
            var point1 = new Point(0.7 / 0.025, Math.Sqrt(13) * Math.Sqrt(13));
            var point2 = new Point(28, 13);

            var point3 = new Point(1.0 / 3, 0);
            var point4 = new Point(10.0 / 30, -1e-15);

            var line1 = new Line(point1, point3);
            var line2 = new Line(point2, point4);

            Assert.AreEqual(line1.GetHashCode(), line2.GetHashCode());
        }

        [Test]
        public void Test_Perpendicular_Line()
        {
            var points = new List<Tuple<Point, Point, Point>>
            {
                new Tuple<Point, Point, Point>(new Point(3, 0), new Point(4, 0), new Point(5, 0)),
                new Tuple<Point, Point, Point>(new Point(0, 5), new Point(0, 6), new Point(4, 7)),
                new Tuple<Point, Point, Point>(new Point(1, 1), new Point(7, 2), new Point(2, 5)),
                new Tuple<Point, Point, Point>(new Point(-4, 3), new Point(8, 2), new Point(0, 0)),
                new Tuple<Point, Point, Point>(new Point(7, 5), new Point(7, 6), new Point(7, 5)),
                new Tuple<Point, Point, Point>(new Point(1, 2), new Point(2, 1), new Point(3, 3))
            };

            var expected = new List<List<int>>
            {
                new List<int> {1, 0, -5},
                new List<int> {0, 1, -7},
                new List<int> {6, 1, -17},
                new List<int> {12, -1, 0},
                new List<int> {0, 1, -5},
                new List<int> {1, -1, 0}
            };

            foreach (var i in Enumerable.Range(0, points.Count))
            {
                Console.WriteLine(i);

                var point1 = points[i].Item1;
                var point2 = points[i].Item2;
                var point3 = points[i].Item3;

                var line = new Line(point1, point2);
                var result = line.PerpendicularLine(point3);

                var coefficients = expected[i];
                var sumOfSquares = Math.Sqrt(coefficients.Sum(c => c * c));
                var normalized = coefficients.Select(c => c / sumOfSquares).ToList();

                Assert.IsTrue((RoundedDouble)normalized[0] == result.A);
                Assert.IsTrue((RoundedDouble)normalized[1] == result.B);
                Assert.IsTrue((RoundedDouble)normalized[2] == result.C);
            }
        }

        [Test]
        public void Test_Angle_Between_Lines_With_Equal_Lines()
        {
            // Diagonal lines
            var line1 = new Line(new Point(0, 0), new Point(1, 1));
            var line2 = new Line(new Point(2, 2), new Point(3, 3));

            Assert.AreEqual(RoundedDouble.Zero, (RoundedDouble)line1.AngleBetween(line2));

            // Vertical lines
            line1 = new Line(new Point(0, 1), new Point(0, 2));
            line2 = new Line(new Point(0, 3), new Point(0, 4));

            Assert.AreEqual(RoundedDouble.Zero, (RoundedDouble)line1.AngleBetween(line2));

            // Horizontal lines
            line1 = new Line(new Point(1, 0), new Point(3, 0));
            line2 = new Line(new Point(2, 0), new Point(4, 0));

            Assert.AreEqual(RoundedDouble.Zero, (RoundedDouble)line1.AngleBetween(line2));
        }

        [Test]
        public void Test_Angle_Between_Lines_With_Parallel_Lines()
        {
            // Diagonal lines
            var line1 = new Line(new Point(0, 0), new Point(1, 1));
            var line2 = new Line(new Point(-1, 0), new Point(0, 1));

            Assert.AreEqual(RoundedDouble.Zero, (RoundedDouble)line1.AngleBetween(line2));

            // Vertical lines
            line1 = new Line(new Point(0, 1), new Point(0, 2));
            line2 = new Line(new Point(1, 3), new Point(1, 4));

            Assert.AreEqual(RoundedDouble.Zero, (RoundedDouble)line1.AngleBetween(line2));

            // Horizontal lines
            line1 = new Line(new Point(1, 1), new Point(3, 1));
            line2 = new Line(new Point(2, 0), new Point(4, 0));

            Assert.AreEqual(RoundedDouble.Zero, (RoundedDouble)line1.AngleBetween(line2));
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

            // Prepare pre-calculted results
            var results = new List<RoundedDouble>
            {
                (RoundedDouble)Math.Atan(8),
                (RoundedDouble)Math.Atan(8.0/5),
                (RoundedDouble)(Math.PI - Math.Atan(8) - Math.Atan(8.0/5))
            };

            // Assert
            Assert.AreEqual(results[0], (RoundedDouble)lines[0].AngleBetween(lines[1]));
            Assert.AreEqual(results[1], (RoundedDouble)lines[0].AngleBetween(lines[2]));
            Assert.AreEqual(results[2], (RoundedDouble)lines[1].AngleBetween(lines[2]));
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

            // Prepare pre-calculted results
            var results = new List<RoundedDouble>
            {
                (RoundedDouble)(Math.PI/4),
                (RoundedDouble)(Math.PI/4),
                (RoundedDouble)(Math.PI/2)
            };

            // Assert
            Assert.AreEqual(results[0], (RoundedDouble)lines[0].AngleBetween(lines[1]));
            Assert.AreEqual(results[1], (RoundedDouble)lines[0].AngleBetween(lines[2]));
            Assert.AreEqual(results[2], (RoundedDouble)lines[1].AngleBetween(lines[2]));
        }
    }
}