using GeoGen.Utilities;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.AnalyticGeometry.Tests
{
    /// <summary>
    /// The test class for <see cref="Circle"/>.
    /// 
    /// TODO: Review
    /// 
    /// </summary>
    [TestFixture]
    public class CircleTest
    {
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
                Assert.IsTrue(expected[i].Item2.Rounded() == circle.Radius.Rounded());
            }
        }

        [Test]
        public void Test_Constructor_Points_Are_Mutually_Different()
        {
            var p1 = new Point(1, 5);
            var p2 = new Point(1, 5);
            var p3 = new Point(7, 8);

            Assert.Throws<AnalyticException>(() => new Circle(p1, p2, p3));
            Assert.Throws<AnalyticException>(() => new Circle(p1, p3, p2));
            Assert.Throws<AnalyticException>(() => new Circle(p3, p2, p1));
            Assert.Throws<AnalyticException>(() => new Circle(p1, p2, p1));
            Assert.Throws<AnalyticException>(() => new Circle(p1, p1, p1));
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
                Assert.Throws<AnalyticException>(() => new Circle(tuple.Item1, tuple.Item2, tuple.Item3));
            }
        }

        [Test]
        public void Test_Equals_And_Hash_Code_On_Nine_Points_Circle()
        {
            var a = new Point(0, 0);
            var b = new Point(7, 4);
            var c = new Point(10, 11);

            var midAb = (a + b) / 2;
            var midBc = (b + c) / 2;
            var midCa = (c + a) / 2;

            var projA = a.Project(new Line(b, c));
            var projB = b.Project(new Line(a, c));
            var projC = c.Project(new Line(a, b));

            var h = new Line(a, projA).IntersectionWith(new Line(b, projB)).Value;

            var midAh = (h + a) / 2;
            var midBh = (h + b) / 2;
            var midCh = (h + c) / 2;

            // Nine-points circle points
            var points = new List<Point> { midAb, midBc, midCa, midAh, midBh, midCh, projA, projB, projC };

            var circles = new List<Circle>();

            // Let's generate all triples of points. Each should represent the same circle.
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
                    Assert.IsFalse(circle1 != circle2);
                    Assert.IsTrue(circle1.Equals(circle2));
                    Assert.IsTrue(circle1.GetHashCode() == circle2.GetHashCode());
                }
            }

            // All circles are equal to one, as expected. It's center is the 
            // midpoint of OH and it's radius it's a half of the radius of (ABC)
            var ninePointCircle = circles[0];

            var abcCircle = new Circle(a, b, c);
            var o = abcCircle.Center;
            var center = (o + h) / 2;

            Assert.IsTrue(ninePointCircle.Center == center);
            Assert.IsTrue(ninePointCircle.Radius.Rounded() == (abcCircle.Radius / 2).Rounded());
        }

        [Test]
        public void Test_Contains()
        {
            var center = new Point(7, 4);
            var circle = new Circle(center, 5);

            Assert.IsTrue(circle.Contains(new Point(4, 0)));
            Assert.IsTrue(circle.Contains(new Point(4, 8)));
            Assert.IsTrue(circle.Contains(new Point(2, 4)));
            Assert.IsTrue(circle.Contains(new Point(11.1, 4 + 3 * Math.Sqrt(91) / 10)));
            Assert.IsTrue(circle.Contains(new Point(11.1, 4 - 3 * Math.Sqrt(91) / 10)));

            Assert.IsFalse(circle.Contains(center));
            Assert.IsFalse(circle.Contains(new Point(17, 1)));
            Assert.IsFalse(circle.Contains(new Point(42, 666)));

            for (double a = 0; a <= 360; a++)
            {
                var rotated = new Point(2, 4).Rotate(center, a);
                Assert.IsTrue(circle.Contains(rotated));
            }
        }

        [Test]
        public void Test_Intersection_With_Line_That_Doesnt_Intersect()
        {
            var center = new Point(-4, 7);
            var circle = new Circle(center, 4);

            var points = new List<Tuple<Point, Point>>
            {
                new Tuple<Point, Point>(new Point(-7, 13), new Point(-1, 11)),
                new Tuple<Point, Point>(new Point(-8.01, 3), new Point(-8.01, 4)),
                new Tuple<Point, Point>(new Point(1, 11.01), new Point(-1, 11.01))
            };

            foreach (var tuple in points)
            {
                var line = new Line(tuple.Item1, tuple.Item2);
                var interesections = circle.IntersectWith(line);

                Assert.IsTrue(interesections.IsEmpty());
            }
        }

        [Test]
        public void Test_Intersection_With_Line_That_Is_Tangent()
        {
            var center = new Point(-4, 7);
            var circle = new Circle(center, 4);

            var points = new List<Tuple<Point, Point>>
            {
                new Tuple<Point, Point>(new Point(-8, 3), new Point(-8, 4)),
                new Tuple<Point, Point>(new Point(1, 11), new Point(-1, 11)),
                new Tuple<Point, Point>(new Point(-1, 7 - Math.Sqrt(7)), new Point(-1 - Math.Sqrt(7), 4 - Math.Sqrt(7)))
            };

            var results = new List<Point>
            {
                new Point(-8, 7),
                new Point(-4, 11),
                new Point(-1, 7 - Math.Sqrt(7))
            };

            for (var i = 0; i < points.Count; i++)
            {
                var tuple = points[i];
                var line = new Line(tuple.Item1, tuple.Item2);
                var interesections = circle.IntersectWith(line);

                Assert.AreEqual(1, interesections.Length);
                Assert.AreEqual(results[i], interesections.First());
            }
        }

        [Test]
        public void Test_Intersection_With_Line_That_Interesects_At_Two_Points()
        {
            var center = new Point(-4, 7);
            var circle = new Circle(center, 4);

            var points = new List<Tuple<Point, Point>>
            {
                new Tuple<Point, Point>(new Point(-4, 11), new Point(-4, 7)),
                new Tuple<Point, Point>(new Point(-8, 7), new Point(0, 7)),
                new Tuple<Point, Point>(new Point(-6, 9), new Point(-3, 8)),
                new Tuple<Point, Point>(new Point(-5, 12), new Point(-9, 7))
            };

            var results = new List<List<Point>>
            {
                new List<Point> {new Point(-4, 3), new Point(-4, 11)},
                new List<Point> {new Point(-8, 7), new Point(0, 7)},
                new List<Point> {new Point(0, 7), new Point(-36.0 / 5, 47.0 / 5)},
                new List<Point> {new Point((-289 - 4 * Math.Sqrt(31)) / 41, (387 - 5 * Math.Sqrt(31)) / 41), new Point((4 * Math.Sqrt(31) - 289) / 41, (387 + 5 * Math.Sqrt(31)) / 41)}
            };

            bool EqualPoints(int i, IEnumerable<Point> result)
            {
                return EnumerableExtensions.ToSet(result).SetEquals(EnumerableExtensions.ToSet(results[i]));
            }

            for (var i = 0; i < points.Count; i++)
            {
                Console.WriteLine($"{i}");
                var tuple = points[i];
                var line = new Line(tuple.Item1, tuple.Item2);
                var intersections = circle.IntersectWith(line);

                Assert.AreEqual(2, intersections.Length);
                Assert.IsTrue(EqualPoints(i, intersections));
            }
        }

        [Test]
        public void Test_Intersection_With_Equal_Circle()
        {
            var circle = new Circle(new Point(1, 1), 5);

            var points = new List<Point>
            {
                new Point(1, 6),
                new Point(4, 5),
                new Point(6, 1),
                new Point(5, -2),
                new Point(1, -4),
                new Point(-3, -2),
                new Point(-4, 1),
                new Point(-3, 4),
                new Point(-2, 5),
                new Point(5, 4),
                new Point(4, -3),
                new Point(-2, -3)
            };

            var circles = new List<Circle> { circle };

            for (var i = 0; i < points.Count; i++)
            {
                for (var j = i + 1; j < points.Count; j++)
                {
                    for (var k = j + 1; k < points.Count; k++)
                    {
                        circles.Add(new Circle(points[i], points[j], points[k]));
                    }
                }
            }

            for (var i = 0; i < circles.Count; i++)
            {
                for (var j = i + 1; j < circles.Count; j++)
                {
                    var circle1 = circles[i];
                    var circle2 = circles[j];

                    Assert.Throws<AnalyticException>(() => circle1.IntersectWith(circle2));
                    Assert.Throws<AnalyticException>(() => circle2.IntersectWith(circle1));
                    Assert.Throws<AnalyticException>(() => circle1.IntersectWith(circle1));
                    Assert.Throws<AnalyticException>(() => circle2.IntersectWith(circle2));
                }
            }
        }

        [Test]
        public void Test_Inteserction_With_Circle_That_Doesnt_Intersect()
        {
            var mainCircle = new Circle(new Point(1, 1), 5);

            var circles = new List<Circle>
            {
                new Circle(new Point(1, 1), 11),
                new Circle(new Point(1, 1), 4),
                new Circle(new Point(1, 1), 8),
                new Circle(new Point(-2, 2), 1),
                new Circle(new Point(0, 0), 3),
                new Circle(new Point(7, 1), 0.999)
            };

            for (var i = 0; i < circles.Count; i++)
            {
                Assert.IsTrue(circles[i].IntersectWith(mainCircle).IsEmpty(), i.ToString());
            }
        }

        [Test]
        public void Test_Inteserction_With_Circle_That_Is_Tangent()
        {
            var mainCircle = new Circle(new Point(1, 1), 5);

            var circles = new List<Circle>
            {
                new Circle(new Point(1, 3.5), 2.5),
                new Circle(new Point(1, 16), 10),
                new Circle(new Point(1.5, 1), 4.5),
                new Circle(new Point(5.8, 4.6), 1)
            };

            var results = new List<Point>
            {
                new Point(1, 6),
                new Point(1, 6),
                new Point(6, 1),
                new Point(5, 4)
            };

            for (var i = 0; i < circles.Count; i++)
            {
                Console.WriteLine(i);
                var circle = circles[i];
                var intersections = mainCircle.IntersectWith(circle);

                Assert.AreEqual(1, intersections.Length);
                Assert.AreEqual(results[i], intersections[0]);
                Assert.IsTrue(circle.IsTangentTo(mainCircle));
                Assert.IsTrue(mainCircle.IsTangentTo(circle));
            }
        }

        [Test]
        public void Test_Inteserction_With_Circle_That_Intersect_At_Two_Points()
        {
            var mainCircle = new Circle(new Point(1, 1), 5);

            var circles = new List<Circle>
            {
                new Circle(new Point(8, 1), Math.Sqrt(18)),
                new Circle(new Point(-3, -5), Math.Sqrt(5)),
                new Circle(new Point(1, 2), Math.Sqrt(20)),
                new Circle(new Point(3, -5), Math.Sqrt(40))
            };

            var results = new List<List<Point>>
            {
                new List<Point> {new Point(5, 4), new Point(5, -2)},
                new List<Point> {new Point(-2, -3), new Point(-20.0 / 13, -43.0 / 13)},
                new List<Point> {new Point(-3, 4), new Point(5, 4)},
                new List<Point> {new Point((13 - 9 * Math.Sqrt(15)) / 8, (-7 - 3 * Math.Sqrt(15)) / 8), new Point((13 + 9 * Math.Sqrt(15)) / 8, (-7 + 3 * Math.Sqrt(15)) / 8)}
            };

            bool EqualPoints(int i, IEnumerable<Point> result)
            {
                return EnumerableExtensions.ToSet(result).SetEquals(EnumerableExtensions.ToSet(results[i]));
            }

            for (var i = 0; i < circles.Count; i++)
            {
                Console.WriteLine(i);
                var circle = circles[i];
                var interesections = mainCircle.IntersectWith(circle);

                Assert.AreEqual(2, interesections.Length);
                Assert.IsTrue(EqualPoints(i, interesections));
            }
        }
    }
}