using FluentAssertions;
using GeoGen.Utilities;
using NUnit.Framework;
using System;
using System.Linq;
using static System.Math;

namespace GeoGen.AnalyticGeometry.Tests
{
    /// <summary>
    /// The test class for <see cref="Line"/>.
    /// </summary>
    [TestFixture]
    public class LineTest
    {
        [Test]
        public void Test_Construction_From_Two_Distinct_Points()
        {
            // Prepare some test cases
            new[]
            {
                (points: (new Point(3, 0), new Point(4, 0)), coefficients: new[] {0d, 1, 0}),
                (points: (new Point(0, 5), new Point(0, 6)), coefficients: new[] {1d, 0, 0}),
                (points: (new Point(1, 1), new Point(7, 2)), coefficients: new[] {1 / Sqrt(37), -6 / Sqrt(37), 5 / Sqrt(37)}),
                (points: (new Point(-4, 3), new Point(8, 2)), coefficients: new[] {1 / Sqrt(145), 12 / Sqrt(145), -32 / Sqrt(145)}),
                (points: (new Point(7, 5), new Point(7, 6)), coefficients: new[] {1d , 0, -7}),
                (points: (new Point(1, 1), new Point(2, 1)), coefficients: new[] {0d, 1, -1})
            }
            // For each assert
            .ForEach((pair, i) =>
            {
                // Get the points
                var (point1, point2) = pair.points;

                // Create the line
                var line = new Line(point1, point2);

                // Get its rounded coefficients
                new[] { line.A, line.B, line.C }.Select(coefficient => coefficient.Rounded())
                    // Assert they are equal to the precalculated ones
                    .SequenceEqual(pair.coefficients.Select(coefficient => coefficient.Rounded())).Should().BeTrue();
            });
        }

        [Test]
        public void Test_Construction_From_Equal_Points()
        {
            // Prepare equal points
            var point1 = new Point(1, 4);
            var point2 = new Point(8.0 / 8.0, 8.0 / 2.0);

            // Assert there is an exception for these equal points
            Assert.Throws<AnalyticException>(() => new Line(point1, point2));
            Assert.Throws<AnalyticException>(() => new Line(point1, point1));
            Assert.Throws<AnalyticException>(() => new Line(point2, point2));
        }

        [Test]
        public void Test_Equals_Simple_Test()
        {
            // Create some points
            var A = new Point(1, 2);
            var B = new Point(2, 3);
            var C = new Point(-4, 5);
            var D = (A + B) / 2;

            // Create some lines
            var AB = new Line(A, B);
            var AC = new Line(A, C);
            var BA = new Line(B, A);
            var AD = new Line(A, D);

            // Test the equals operator
            (AB == AC).Should().BeFalse();
            (AB == AD).Should().BeTrue();
            (AD == BA).Should().BeTrue();

            // Test the equals method
            AB.Equals(AC).Should().BeFalse();
            AB.Equals(AD).Should().BeTrue();
            AD.Equals(BA).Should().BeTrue();
        }

        [Test]
        public void Test_Equals_Complex_Test()
        {
            // Prepare some non-collinear points
            var A = new Point(42, 666);
            var B = new Point(666, 42);
            var C = new Point(32, 64);

            // Take these points, the midpoints of the lines they form, and the centroid
            new[] { A, B, C, (A + B) / 2, (B + C) / 2, (C + A) / 2, (A + B + C) / 3 }
                // Create lines from each two
                .UnorderedPairs().Select(pair => new Line(pair.Item1, pair.Item2))
                // Take distinct ones
                .Distinct()
                // There should be 9 of them
                .Count().Should().Be(9);
        }

        [Test]
        public void Test_Line_Intersections()
        {
            // Prepare some simple configuration
            var A = new Point(1, 2);
            var B = new Point(2, 3);
            var C = new Point(-4, 5);
            var D = (A + B) / 2;
            var E = (A + C) / 2;

            // Create some lines
            var AB = new Line(A, B);
            var AC = new Line(A, C);
            var BC = new Line(B, C);
            var DE = new Line(D, E);
            var CD = new Line(C, D);
            var BE = new Line(B, E);

            // Assert some existing intersections
            AB.IntersectionWith(AC).Should().Be(A);
            BC.IntersectionWith(AB).Should().Be(B);
            CD.IntersectionWith(BE).Should().Be(new Point(-1.0 / 3, 10.0 / 3));

            // Assert that interesting equal lines leads to an exception
            Assert.Throws<AnalyticException>(() => AB.IntersectionWith(AB));
            Assert.Throws<AnalyticException>(() => AB.IntersectionWith(new Line(A, D)));

            // Assert that intersection of parallel lines is null
            BC.IntersectionWith(DE).Should().BeNull();
        }

        [Test]
        public void Test_Line_Contains_Point()
        {
            // Prepare some non-collinear points
            var A = new Point(7, 11);
            var B = new Point(8, 19);
            var C = new Point(-666, 42);

            // Assert that each side contains the side points
            new Line(A, B).Contains(A).Should().BeTrue();
            new Line(A, B).Contains(B).Should().BeTrue();
            new Line(B, C).Contains(B).Should().BeTrue();
            new Line(B, C).Contains(C).Should().BeTrue();
            new Line(C, A).Contains(A).Should().BeTrue();
            new Line(C, A).Contains(C).Should().BeTrue();

            // Find the perpendicular bisectors of the sides
            var bisectorAB = AnalyticHelpers.PerpendicularBisector(A, B);
            var bisectorAC = AnalyticHelpers.PerpendicularBisector(A, C);
            var bisectorBC = AnalyticHelpers.PerpendicularBisector(B, C);

            // Find the circumcenter
            var O = bisectorAB.IntersectionWith(bisectorAC).Value;

            // Assert that each bisector contains it
            bisectorBC.Contains(O).Should().BeTrue();
            bisectorAB.Contains(O).Should().BeTrue();
            bisectorAC.Contains(O).Should().BeTrue();

            // Assert that each bisector contains the particular midpoint
            bisectorBC.Contains((B + C) / 2).Should().BeTrue();
            bisectorAC.Contains((A + C) / 2).Should().BeTrue();
            bisectorAB.Contains((A + B) / 2).Should().BeTrue();
        }

        [Test]
        public void Test_Perpendicular_Line()
        {
            // Prepare the test inputs
            new[]
            {
                (line: new Line(new Point(3, 0), new Point(4, 0)), point: new Point(5, 0), result: new[] { 1d, 0, -5 }),
                (line: new Line(new Point(0, 5), new Point(0, 6)), point: new Point(4, 7), result: new[] { 0d, 1, -7 }),
                (line: new Line(new Point(1, 1), new Point(7, 2)), point: new Point(2, 5), result: new[] { 6 / Sqrt(37), 1 / Sqrt(37), -17 / Sqrt(37) }),
                (line: new Line(new Point(-4, 3), new Point(8, 2)), point: new Point(0, 0), result: new[] { 12 / Sqrt(145), -1 / Sqrt(145), 0 }),
                (line: new Line(new Point(7, 5), new Point(7, 6)), point: new Point(7, 5), result: new[] { 0d, 1, -5 }),
                (line: new Line(new Point(1, 2), new Point(2, 1)), point: new Point(3, 3), result: new[] { 1 / Sqrt(2), -1 / Sqrt(2), 0 })
            }
            // Test each
            .ForEach(tuple =>
            {
                // Create the perpendicular line
                var line = tuple.line.PerpendicularLineThroughPoint(tuple.point);

                // Get its rounded coefficients
                new[] { line.A, line.B, line.C }.Select(coefficient => coefficient.Rounded())
                    // Assert they are equal to the precalculated ones
                    .SequenceEqual(tuple.result.Select(coefficient => coefficient.Rounded())).Should().BeTrue();

                // Assert they are perpendicular
                line.IsPerpendicularTo(tuple.line).Should().BeTrue();
                tuple.line.IsPerpendicularTo(line).Should().BeTrue();
            });
        }

        [Test]
        public void Test_Parallelity()
        {
            // Create some triangle
            var A = new Point(42, 666);
            var B = new Point(9, 50);
            var C = new Point(5, 27);

            // Create some midpoints
            var mAB = (A + B) / 2;
            var mAC = (A + C) / 2;

            // Assert some parallelities
            new Line(B, C).IsParallelTo(new Line(mAC, mAB)).Should().BeTrue();
            new Line(mAC, mAB).IsParallelTo(new Line(B, C)).Should().BeTrue();
        }
    }
}