using System;
using System.CodeDom;
using System.Collections.Generic;
using GeoGen.AnalyticalGeometry.AnalyticalObjects;
using NUnit.Framework;

namespace GeoGen.AnalyticalGeometry.Test.AnalyticalObjects
{
    [TestFixture]
    public class CircleTest
    {
        [Test]
        public void Test_Constructor_Radius_Is_Non_Negative()
        {
            var center = new Point(0, 0);

            Assert.Throws<ArgumentOutOfRangeException>(() => new Circle(center, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => new Circle(center, -1));
            Assert.Throws<ArgumentOutOfRangeException>(() => new Circle(center, -1e10));
        }

        [Test]
        public void Test_Constructor_From_Three_Points()
        {
            var points = new List<Tuple<Point, Point, Point>>
            {
                new Tuple<Point, Point, Point>(new Point(0, 0), new Point(1, 1), new Point(3, 4)),
                new Tuple<Point, Point, Point>(new Point(1, 0), new Point(1, -1), new Point(2, 1)),
                new Tuple<Point, Point, Point>(new Point(-7, 3), new Point(1, 9), new Point(3, 5)),
                new Tuple<Point, Point, Point>(new Point(-4, -2), new Point(6, -2), new Point(-4, 6))
            };

            var expected = new List<Tuple<Point, double>>
            {
                new Tuple<Point, double>(new Point(-8.5, 9.5), Math.Sqrt(162.5)),
                new Tuple<Point, double>(new Point(2.5, -0.5), Math.Sqrt(2.5)),
                new Tuple<Point, double>(new Point(-24.0 / 11, 54.0 / 11), 5 * Math.Sqrt(130) / 11),
                new Tuple<Point, double>(new Point(1, 2), Math.Sqrt(41))
            };

            for (var i = 0; i < points.Count; i++)
            {
                var tuple = points[i];

                var p1 = tuple.Item1;
                var p2 = tuple.Item2;
                var p3 = tuple.Item3;

                var circle = new Circle(p1, p2, p3);

                Assert.IsTrue(expected[i].Item1 == circle.Center);
                Assert.IsTrue(expected[i].Item2 == circle.Radius);
            }
        }

        [Test]
        public void Test_Constructor_Points_Are_Mutually_Different()
        {
            var p1 = new Point(1, 5);
            var p2 = new Point(1, 5);
            var p3 = new Point(7, 8);

            Assert.Throws<ArgumentException>(() => new Circle(p1, p2, p3));
            Assert.Throws<ArgumentException>(() => new Circle(p1, p3, p2));
            Assert.Throws<ArgumentException>(() => new Circle(p3, p2, p1));
            Assert.Throws<ArgumentException>(() => new Circle(p1, p2, p1));
            Assert.Throws<ArgumentException>(() => new Circle(p1, p1, p1));
        }

        [Test]
        public void Test_Constructor_Points_Are_Not_Collinear()
        {
            var points = new List<Tuple<Point, Point, Point>>
            {
                new Tuple<Point, Point, Point>(new Point(0, 0), new Point(0, 7), new Point(0, 7)),
                new Tuple<Point, Point, Point>(new Point(7, 4), new Point(8, 4), new Point(0, 4)),
                new Tuple<Point, Point, Point>(new Point(1, 1), new Point(2, 2), new Point(3, 3)),
                new Tuple<Point, Point, Point>(new Point(9, 11), new Point(-4, 7), new Point(-17, 3))
            };

            foreach (var tuple in points)
            {
                Assert.Throws<ArgumentException>(() => new Circle(tuple.Item1, tuple.Item2, tuple.Item3));
            }
        }

        [Test]
        public void Test_Equals_And_Hash_Code()
        {
            var a = new Point(0, 0);
            var b = new Point(7, 4);
            var c = new Point(10, 11);

            var midAb = a.Midpoint(b);
            var midBc = b.Midpoint(c);
            var midCa = c.Midpoint(a);

            var projA = a.Project(new Line(b, c));
            var projB = b.Project(new Line(a, c));
            var projC = c.Project(new Line(a, b));

            var h = new Line(a, projA).IntersectionWith(new Line(b, projB)) ?? throw new Exception("Impossible");

            var midAh = h.Midpoint(a);
            var midBh = h.Midpoint(b);
            var midCh = h.Midpoint(c);

            // Nine-points circle
            var points = new List<Point> {midAb, midBc, midCa, midAh, midBh, midCh, projA, projB, projC};

            var circles = new List<Circle>();

            for (var i = 0; i < points.Count; i++)
            {
                for (var j = i + 1; j < points.Count; j++)
                {
                    for (var k = j + 1; k < points.Count; k++)
                    {
                        var circle = new Circle(points[i], points[j], points[k]);
                        circles.Add(circle);
                    }
                }
            }

            foreach (var circle1 in circles)
            {
                foreach (var circle2 in circles)
                {
                    Assert.IsTrue(circle1 == circle2);
                    Assert.IsTrue(circle1.GetHashCode() == circle2.GetHashCode());
                }
            }

            // All circles are equal to one, as expected. It's center is the 
            // midpoint of OH and it's radius it's a half of the radius of (ABC)
            var ninePointCircle = circles[0];

            var abcCircle = new Circle(a, b, c);
            var o = abcCircle.Center;
            var center = o.Midpoint(h);

            Assert.IsTrue(ninePointCircle.Center == center);
            Assert.IsTrue(ninePointCircle.Radius == abcCircle.Radius / 2);
        }
    }
}