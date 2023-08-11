using FluentAssertions;
using GeoGen.Utilities;
using NUnit.Framework;
using static System.Math;

namespace GeoGen.AnalyticGeometry.Tests
{
    /// <summary>
    /// The test class for <see cref="Circle"/>.
    /// </summary>
    [TestFixture]
    public class CircleTest
    {
        [Test]
        public void Test_Constructor_From_Points()
        {
            // Prepare the test cases
            new[]
            {
                (points: (new Point(0, 0), new Point(1, 1), new Point(3, 4)), result: (center: new Point(-8.5, 9.5), radius: Sqrt(162.5))),
                (points: (new Point(1, 0), new Point(1, -1), new Point(2, 1)), result: (center: new Point(2.5, -0.5), radius: Sqrt(2.5))),
                (points: (new Point(-7, 3), new Point(1, 9), new Point(3, 5)), result: (center: new Point(-24.0 / 11, 54.0 / 11), radius: 5 * Sqrt(130) / 11)),
                (points: (new Point(-4, -2), new Point(6, -2), new Point(-4, 6)), result: (center: new Point(1, 2), radius: Sqrt(41)))
            }
            // Test each
            .ForEach(tuple =>
            {
                // Create the circle
                var circle = new Circle(tuple.points.Item1, tuple.points.Item2, tuple.points.Item3);

                // Assert the center and the radius
                circle.Center.Should().Be(tuple.result.center);
                circle.Radius.Rounded().Should().Be(tuple.result.radius.Rounded());
            });
        }

        [Test]
        public void Test_Constructor_From_Equal_Points()
        {
            // Create some random points
            var A = new Point(1, 5);
            var B = new Point(1, 5);
            var C = new Point(7, 8);

            // We expect an exception when there are equal points passed
            Assert.Throws<AnalyticException>(() => new Circle(A, B, C));
            Assert.Throws<AnalyticException>(() => new Circle(A, C, B));
            Assert.Throws<AnalyticException>(() => new Circle(C, B, A));
            Assert.Throws<AnalyticException>(() => new Circle(A, B, A));
            Assert.Throws<AnalyticException>(() => new Circle(A, A, A));
        }

        [Test]
        public void Test_Constructor_From_Collinear_Points()
        {
            // Prepare some triples of collinear points
            new[]
            {
                (new Point(0, 0), new Point(0, 7), new Point(0, 8)),
                (new Point(7, 4), new Point(8, 4), new Point(0, 4)),
                (new Point(1, 1), new Point(2, 2), new Point(3, 3)),
                (new Point(9, 11), new Point(-4, 7), new Point(-17, 3))
            }
            // Test each triple that the constructor throws an exception
            .ForEach(points => Assert.Throws<AnalyticException>(() => new Circle(points.Item1, points.Item2, points.Item3)));
        }

        [Test]
        public void Test_Equals_On_Nine_Points_Circle()
        {
            // Prepare the triangle
            var A = new Point(0, 0);
            var B = new Point(7, 4);
            var C = new Point(10, 11);

            // Prepare the midpoints
            var mAB = (A + B) / 2;
            var mBC = (B + C) / 2;
            var mCA = (C + A) / 2;

            // Prepare the projections
            var pA = A.Project(new Line(B, C));
            var pB = B.Project(new Line(A, C));
            var pC = C.Project(new Line(A, B));

            // Prepare the orthocenter
            var H = new Line(A, pA).IntersectionWith(new Line(B, pB)).Value;

            // Prepare the midpoints between the vertices and the orthocenter
            var mAH = (H + A) / 2;
            var mBH = (H + B) / 2;
            var mCH = (H + C) / 2;

            // Prepare the Nine-point circle points
            new[] { mAB, mBC, mCA, mAH, mBH, mCH, pA, pB, pC }
                // Make a circle from each three
                .UnorderedTriples().Select(triple => new Circle(triple.Item1, triple.Item2, triple.Item3))
                // Get their pair and test each
                .ToList().UnorderedPairs().ForEach(pair =>
                {
                    // Get the circles
                    var (circle1, circle2) = pair;

                    // Test the equals method and operator
                    (circle1 == circle2).Should().BeTrue();
                    circle1.Equals(circle2).Should().BeTrue();
                });

            // All circles are equal to one, as expected
            var ninePointCircle = new Circle(mAB, mBC, mCA);

            // It's center is the midpoint of OH and it's radius it's a half of the radius of (ABC)
            ninePointCircle.Center.Should().Be((H + new Circle(A, B, C).Center) / 2);
            ninePointCircle.Radius.Rounded().Should().Be((new Circle(A, B, C).Radius / 2).Rounded());
        }

        [Test]
        public void Test_Contains()
        {
            // Prepare some center and some circle
            var center = new Point(7, 4);
            var circle = new Circle(center, 5);

            // Assert some precalculated containing points
            circle.Contains(new Point(4, 0)).Should().BeTrue();
            circle.Contains(new Point(4, 8)).Should().BeTrue();
            circle.Contains(new Point(2, 4)).Should().BeTrue();
            circle.Contains(new Point(11.1, 4 + 3 * Sqrt(91) / 10)).Should().BeTrue();
            circle.Contains(new Point(11.1, 4 - 3 * Sqrt(91) / 10)).Should().BeTrue();

            // Assert some non-containing points
            circle.Contains(center).Should().BeFalse();
            circle.Contains(new Point(17, 1)).Should().BeFalse();
            circle.Contains(new Point(42, 666)).Should().BeFalse();

            // Rotate a point of the circle around the center
            Enumerable.Range(0, 361).Select(angle => new Point(2, 4).Rotate(center, angle))
                // This point should line on the circle too
                .ForEach(point => circle.Contains(point).Should().BeTrue());
        }

        [Test]
        public void Test_Intersection_With_Line_That_Doesnt_Intersect()
        {
            // Prepare some circle
            var circle = new Circle(new Point(-4, 7), 4);

            // Prepare some lines
            new[]
            {
                new Line(new Point(-7, 13), new Point(-1, 11)),
                new Line(new Point(-8.01, 3), new Point(-8.01, 4)),
                new Line(new Point(1, 11.01), new Point(-1, 11.01))
            }
            // Test each
            .ForEach(line => circle.IntersectWith(line).Should().BeEmpty());
        }

        [Test]
        public void Test_Intersection_With_Line_That_Is_Tangent()
        {
            // Prepare some circle
            var circle = new Circle(new Point(-4, 7), 4);

            // Prepare some precalculated intersections
            new[]
            {
                (line: new Line(new Point(-8, 3), new Point(-8, 4)), result: new Point(-8, 7)),
                (line: new Line(new Point(1, 11), new Point(-1, 11)), result:  new Point(-4, 11)),
                (line: new Line(new Point(-1, 7 - Sqrt(7)), new Point(-1 - Sqrt(7), 4 - Sqrt(7))),  result: new Point(-1, 7 - Sqrt(7)))
            }
            // Test each case
            .ForEach(pair =>
            {
                // Test intersection
                circle.IntersectWith(pair.line).Should().BeEquivalentTo(new[] { pair.result });

                // Test tangency
                circle.IsTangentTo(pair.line).Should().BeTrue();
            });
        }

        [Test]
        public void Test_Intersection_With_Line_That_Interesects_At_Two_Points()
        {
            // Prepare some circle
            var circle = new Circle(new Point(-4, 7), 4);

            // Prepare some precalculated intersections
            new[]
            {
                (line: new Line(new Point(-4, 11), new Point(-4, 7)), result: new[] {new Point(-4, 3), new Point(-4, 11)}),
                (line: new Line(new Point(-8, 7), new Point(0, 7)), result: new[] {new Point(-8, 7), new Point(0, 7)}),
                (line: new Line(new Point(-6, 9), new Point(-3, 8)), result: new[] {new Point(0, 7), new Point(-36.0 / 5, 47.0 / 5)}),
                (line: new Line(new Point(-5, 12), new Point(-9, 7)), result: new[] {new Point((-289 - 4 * Sqrt(31)) / 41, (387 - 5 * Sqrt(31)) / 41), new Point((4 * Sqrt(31) - 289) / 41, (387 + 5 * Sqrt(31)) / 41)})
            }
            // Test each case
            .ForEach(pair => circle.IntersectWith(pair.line).Should().BeEquivalentTo(pair.result));
        }

        [Test]
        public void Test_Intersection_With_Equal_Circle()
        {
            // Prepare some circle
            var circle = new Circle(new Point(1, 1), 5);

            // Prepare some points that lie on it 
            new[]
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
            }
            // Get their triples
            .UnorderedTriples()
            // Make sure there is an exception when intersecting our circle with them
            .ForEach(triple => Assert.Throws<AnalyticException>(() => circle.IntersectWith(new Circle(triple.Item1, triple.Item2, triple.Item3))));
        }

        [Test]
        public void Test_Inteserction_With_Circle_That_Doesnt_Intersect()
        {
            // Prepare some circle
            var circle = new Circle(new Point(1, 1), 5);

            // Prepare some non-intersecting circles
            new[]
            {
                new Circle(new Point(1, 1), 11),
                new Circle(new Point(1, 1), 4),
                new Circle(new Point(1, 1), 8),
                new Circle(new Point(-2, 2), 1),
                new Circle(new Point(0, 0), 3),
                new Circle(new Point(7, 1), 0.999)
            }
            // Test each
            .ForEach(_circle => circle.IntersectWith(_circle).Should().BeEmpty());
        }

        [Test]
        public void Test_Inteserction_With_Circle_That_Is_Tangent()
        {
            // Prepare some circle
            var circle = new Circle(new Point(1, 1), 5);

            // Prepare some tangent circles with the points of tangency
            new[]
            {
                (circle: new Circle(new Point(1, 3.5), 2.5), intersection: new Point(1, 6)),
                (circle: new Circle(new Point(1, 16), 10), intersection: new Point(1, 6)),
                (circle: new Circle(new Point(1.5, 1), 4.5), intersection: new Point(6, 1)),
                (circle: new Circle(new Point(5.8, 4.6), 1), intersection: new Point(5, 4))
            }
            // Test each
            .ForEach(pair =>
            {
                // Test intersections
                circle.IntersectWith(pair.circle).Should().BeEquivalentTo(new[] { pair.intersection });
                pair.circle.IntersectWith(circle).Should().BeEquivalentTo(new[] { pair.intersection });

                // Test tangency
                circle.IsTangentTo(pair.circle).Should().BeTrue();
                pair.circle.IsTangentTo(circle).Should().BeTrue();
            });
        }

        [Test]
        public void Test_Inteserction_With_Circle_That_Intersects_At_Two_Points()
        {
            // Prepare some circle
            var circle = new Circle(new Point(1, 1), 5);

            new[]
            {
                (circle: new Circle(new Point(8, 1), Sqrt(18)), intersections: new[] {new Point(5, 4), new Point(5, -2)}),
                (circle: new Circle(new Point(-3, -5), Sqrt(5)), intersections: new[] {new Point(-2, -3), new Point(-20.0 / 13, -43.0 / 13)}),
                (circle: new Circle(new Point(1, 2), Sqrt(20)), intersections: new[] {new Point(-3, 4), new Point(5, 4)}),
                (circle: new Circle(new Point(3, -5), Sqrt(40)), intersections: new[] {new Point((13 - 9 * Sqrt(15)) / 8, (-7 - 3 * Sqrt(15)) / 8), new Point((13 + 9 * Sqrt(15)) / 8, (-7 + 3 * Sqrt(15)) / 8)})
            }
            // Test each
            .ForEach(pair =>
            {
                // Test intersections
                circle.IntersectWith(pair.circle).Should().BeEquivalentTo(pair.intersections);
                pair.circle.IntersectWith(circle).Should().BeEquivalentTo(pair.intersections);
            });
        }
    }
}