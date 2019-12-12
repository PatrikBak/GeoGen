using FluentAssertions;
using GeoGen.Utilities;
using NUnit.Framework;
using System;
using System.Linq;
using static GeoGen.AnalyticGeometry.AnalyticHelpers;
using static System.Math;

namespace GeoGen.AnalyticGeometry.Tests
{
    /// <summary>
    /// The test class for <see cref="AnalyticHelpers"/>.
    /// </summary>
    [TestFixture]
    public class AnalyticHelpersTest
    {
        [Test]
        public void Test_Lies_On_With_Line()
        {
            // Prepare some line
            var line = new Line(new Point(1, 1), new Point(4, 5));

            // Test some points that are on it
            LiesOn(line, new Point(1, 1)).Should().BeTrue();
            LiesOn(line, new Point(7, 9)).Should().BeTrue();
            LiesOn(line, new Point(4, 5)).Should().BeTrue();
            LiesOn(line, new Point(-2, -3)).Should().BeTrue();
            LiesOn(line, new Point(2.5, 3)).Should().BeTrue();

            // Test some points that are not on it
            LiesOn(line, new Point(7, 2)).Should().BeFalse();
            LiesOn(line, new Point(7, 3)).Should().BeFalse();
            LiesOn(line, new Point(-5, 3)).Should().BeFalse();
            LiesOn(line, new Point(-7, -7)).Should().BeFalse();
        }

        [Test]
        public void Test_Lies_On_With_Circle()
        {
            // Prepare a circle
            var circle = new Circle(new Point(1, 1), 2);

            // Test some points that are on it
            LiesOn(circle, new Point(-1, 1)).Should().BeTrue();
            LiesOn(circle, new Point(-0.2, 2.6)).Should().BeTrue();
            LiesOn(circle, new Point(-0.6, 2.2)).Should().BeTrue();
            LiesOn(circle, new Point(3, 1)).Should().BeTrue();

            // Test some points that are not on it
            LiesOn(circle, new Point(1, 1)).Should().BeFalse();
            LiesOn(circle, new Point(7, -5)).Should().BeFalse();
            LiesOn(circle, new Point(5, -1)).Should().BeFalse();
            LiesOn(circle, new Point(3, -4)).Should().BeFalse();
        }

        [Test]
        public void Test_Intersect_Two_Lines()
        {
            // Intersect some two lines
            Intersect(new IAnalyticObject[]
            {
                new Line(new Point(1, 5), new Point(1, 3)),
                new Line(new Point(1, 5), new Point(2, 4))
            })
            // Test the intersection
            .Should().BeEquivalentTo(new[] { new Point(1, 5) });
        }

        [Test]
        public void Test_Intersect_Two_Circles()
        {
            // Intersect some two circles
            Intersect(new IAnalyticObject[]
            {
                new Circle(new Point(0, 0), new Point(1, 1), new Point(7, 8)),
                new Circle(new Point(0, 0), new Point(1, 1), new Point(8, 7))
            })
            // Test the intersections
            .Should().BeEquivalentTo(new[] { new Point(0, 0), new Point(1, 1) });
        }

        [Test]
        public void Test_Intersect_Line_And_Circle()
        {
            // Intersect some circle and some line
            Intersect(new IAnalyticObject[]
            {
                new Circle(new Point(0, 0), new Point(1, 1), new Point(7, 8)),
                new Line(new Point(0, 0), new Point(1, 1))
            })
            // Test the intersections
            .Should().BeEquivalentTo(new[] { new Point(0, 0), new Point(1, 1) });
        }

        [Test]
        public void Test_Intersect_Three_Lines()
        {
            // Prepare some non-collinear points
            var A = new Point(1, 5);
            var B = new Point(7, 4);
            var C = new Point(1, 77);

            // Intersect the medians
            Intersect(new IAnalyticObject[]
            {
                new Line(A, (B+C)/2),
                new Line(B, (C+A)/2),
                new Line(C, (A+B)/2)
            })
            // Test the intersection, which should be the centroid
            .Should().BeEquivalentTo(new[] { (A + B + C) / 3 });
        }

        [Test]
        public void Test_Intersect_Two_Lines_Two_Circles()
        {
            // Prepare some non-collinear points
            var A = new Point(1, 5);
            var B = new Point(7, 4);
            var C = new Point(1, 77);

            // Intersect some lines and circles
            Intersect(new IAnalyticObject[]
            {
                new Line(B, C),
                new Circle((A+B)/2, A.DistanceTo(B) / 2),
                new Circle((A+C)/2, A.DistanceTo(C) / 2),
                new Line(A, A.Project(new Line(B, C)))
            })
            // Test the intersection, which should be the projection of A on BC
            .Should().BeEquivalentTo(new[] { A.Project(new Line(B, C)) });
        }

        [Test]
        public void Test_Angle_Between_Lines_With_Equal_Lines()
        {
            // Prepare some diagonal lines
            var line1 = new Line(new Point(0, 0), new Point(1, 1));
            var line2 = new Line(new Point(2, 2), new Point(3, 3));

            // Assert their angle is 0
            AngleBetweenLines(line1, line2).Rounded().Should().Be(0);
            AngleBetweenLines(line2, line1).Rounded().Should().Be(0);

            // Prepare some vertical lines
            line1 = new Line(new Point(0, 1), new Point(0, 2));
            line2 = new Line(new Point(0, 3), new Point(0, 4));

            // Assert their angle is 0
            AngleBetweenLines(line1, line2).Rounded().Should().Be(0);
            AngleBetweenLines(line2, line1).Rounded().Should().Be(0);

            // Prepare some horizontal lines
            line1 = new Line(new Point(1, 0), new Point(3, 0));
            line2 = new Line(new Point(2, 0), new Point(4, 0));

            // Assert their angle is 0
            AngleBetweenLines(line1, line2).Rounded().Should().Be(0);
            AngleBetweenLines(line2, line1).Rounded().Should().Be(0);
        }

        [Test]
        public void Test_Angle_Between_Lines_With_Parallel_Lines()
        {
            // Prepare some diagonal lines
            var line1 = new Line(new Point(0, 0), new Point(1, 1));
            var line2 = new Line(new Point(-1, 0), new Point(0, 1));

            // Assert their angle is 0
            AngleBetweenLines(line1, line2).Rounded().Should().Be(0);
            AngleBetweenLines(line2, line1).Rounded().Should().Be(0);

            // Prepare some vertical lines
            line1 = new Line(new Point(0, 1), new Point(0, 2));
            line2 = new Line(new Point(1, 3), new Point(1, 4));

            // Assert their angle is 0
            AngleBetweenLines(line1, line2).Rounded().Should().Be(0);
            AngleBetweenLines(line2, line1).Rounded().Should().Be(0);

            // Prepare some horizontal lines
            line1 = new Line(new Point(1, 1), new Point(3, 1));
            line2 = new Line(new Point(2, 0), new Point(4, 0));

            // Assert their angle is 0
            AngleBetweenLines(line1, line2).Rounded().Should().Be(0);
            AngleBetweenLines(line2, line1).Rounded().Should().Be(0);
        }

        [Test]
        public void Test_Angle_Between_Lines_With_Acute_Angled_Triangle()
        {
            // Prepare some non-collinear points
            var A = new Point(2, -2);
            var B = new Point(8, -2);
            var C = new Point(3, 6);

            // Prepare their sides
            var AB = new Line(A, B);
            var AC = new Line(A, C);
            var BC = new Line(B, C);

            // Assert with some precalculated results
            AngleBetweenLines(AB, AC).Rounded().Should().Be(Atan(8).Rounded());
            AngleBetweenLines(AB, BC).Rounded().Should().Be(Atan(8.0 / 5).Rounded());
            AngleBetweenLines(AC, BC).Rounded().Should().Be((PI - Atan(8) - Atan(8.0 / 5)).Rounded());
        }

        [Test]
        public void Test_Angle_Between_Lines_With_Right_Angled_Triangle()
        {
            // Prepare some non-collinear points
            var A = new Point(0, 3);
            var B = new Point(8, 5);
            var C = new Point(3, 8);

            // Prepare their sides
            var AB = new Line(A, B);
            var AC = new Line(A, C);
            var BC = new Line(B, C);

            // Assert with some precalculated results
            AngleBetweenLines(AB, AC).Rounded().Should().Be((PI / 4).Rounded());
            AngleBetweenLines(AB, BC).Rounded().Should().Be((PI / 4).Rounded());
            AngleBetweenLines(AC, BC).Rounded().Should().Be((PI / 2).Rounded());
        }

        [Test]
        public void Test_Internal_Angle_Bisector_With_Intencter()
        {
            // Prepare some non-collinear points
            var A = new Point(1, 3);
            var B = new Point(2, 5);
            var C = new Point(7, 7);

            // Get the internal angle bisectors
            var Abisector = InternalAngleBisector(A, B, C);
            var Bbisector = InternalAngleBisector(B, A, C);
            var Cbisector = InternalAngleBisector(C, A, B);

            // Make sure they contain the particular vertices
            Abisector.Contains(A).Should().BeTrue();
            Bbisector.Contains(B).Should().BeTrue();
            Cbisector.Contains(C).Should().BeTrue();

            // Intersect each two
            var I1 = Abisector.IntersectionWith(Bbisector);
            var I2 = Bbisector.IntersectionWith(Cbisector);
            var I3 = Cbisector.IntersectionWith(Abisector);

            // Make sure these intersections are equal
            I1.Should().Be(I2);
            I2.Should().Be(I3);
            I3.Should().Be(I1);
        }

        [Test]
        public void Test_Internal_Angle_Bisector_With_Midpoint_Of_Opposite_arc()
        {
            // Prepare some non-collinear points
            var A = new Point(1, 3);
            var B = new Point(2, 5);
            var C = new Point(7, 7);

            // Prepare the three objects that should intersect
            var Abisector = InternalAngleBisector(A, B, C);
            var BCbisector = PerpendicularBisector(B, C);
            var ABCcircumcircle = new Circle(A, B, C);

            // Intersect all of them
            Intersect(new IAnalyticObject[] { Abisector, BCbisector, ABCcircumcircle })
                // Assert there is only one intersection
                .Length.Should().Be(1);
        }

        [Test]
        public void Test_Internal_Angle_Bisector_With_Equal_Points()
        {
            // Prepare some points
            var A = new Point(0, 1);
            var B = new Point(42, 666);

            // Assert there is an exception whenever one point is equal to the main vertex
            Assert.Throws<AnalyticException>(() => InternalAngleBisector(A, A, B));
            Assert.Throws<AnalyticException>(() => InternalAngleBisector(A, B, A));
            Assert.Throws<AnalyticException>(() => InternalAngleBisector(A, A, A));
        }

        [Test]
        public void Test_Perpendicular_Bisector_Equal_Points()
        {
            // Assert there is an exception if it's called for two equal points 
            Assert.Throws<AnalyticException>(() => PerpendicularBisector(new Point(0, 0), new Point(0, 0)));
            Assert.Throws<AnalyticException>(() => PerpendicularBisector(new Point(42, 666), new Point(42, 666)));
        }

        [Test]
        public void Test_Perpedicular_Bisector_Distint_Points()
        {
            // Prepare some precalculated inputs
            new[]
            {
                (points: (new Point(0, 0), new Point(1, 0)), coefficients: new[] {1, 0, -0.5}),
                (points: (new Point(0, 0), new Point(0, 1)), coefficients: new[] {0, 1, -0.5}),
                (points: (new Point(7, 9.5), new Point(7, 9.6)), coefficients: new[] {0, 1, -9.55}),
                (points: (new Point(0, 0), new Point(4, 4)), coefficients: new[] {1 / Sqrt(2), 1 / Sqrt(2), -4 / Sqrt(2)}),
                (points: (new Point(17, 4), new Point(11, 2)), coefficients: new[] {3 / Sqrt(10), 1 / Sqrt(10), -45 / Sqrt(10)})
            }
            // Test each
            .ForEach(pair =>
            {
                // Create the bisector
                var bisector = PerpendicularBisector(pair.points.Item1, pair.points.Item2);

                // Get its rounded coefficients
                new[] { bisector.A, bisector.B, bisector.C }.Select(d => d.Rounded())
                    // Assert they are equal to the precalculated ones
                    .SequenceEqual(pair.coefficients.Select(d => d.Rounded())).Should().BeTrue();
            });
        }

        [Test]
        public void Test_Are_Collinear()
        {
            // Get some collinear points
            var A = new Point(0, 0);
            var B = new Point(2, 2);
            var C = new Point(42, 42);
            var D = new Point(666, 666);

            // Assert they're collinear
            AreCollinear(A, B, C, D).Should().BeTrue();
        }

        [Test]
        public void Test_Shift_Segment()
        {
            // Prepare some point
            var A = new Point(42, 666);
            var B = new Point(-11, -13);

            // Prepare the shift length
            var shiftLength = 4.2;

            // Shift
            var C = ShiftSegment(A, B, shiftLength);

            // Make sure the collinearity is there
            AreCollinear(A, B, C).Should().BeTrue();

            // Make sure the length is okay
            B.DistanceTo(C).Rounded().Should().Be(shiftLength.Rounded());

            // Make sure C is beyond B
            A.DistanceTo(C).Rounded().Should().BeGreaterThan(shiftLength.Rounded());
            A.DistanceTo(C).Rounded().Should().BeGreaterThan(A.DistanceTo(B).Rounded());
        }
    }
}