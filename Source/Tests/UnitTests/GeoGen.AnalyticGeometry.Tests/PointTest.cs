using FluentAssertions;
using GeoGen.Utilities;
using NUnit.Framework;
using System.Linq;
using static System.Math;

namespace GeoGen.AnalyticGeometry.Tests
{
    /// <summary>
    /// The test class for <see cref="Point"/>.
    /// </summary>
    [TestFixture]
    public class PointTest
    {
        [Test]
        public void Test_Equals()
        {
            // Prepare some points
            new[]
            {
                (point: new Point(1.5, 1.7), equalPoint: new Point(3.0 / 2, 5.1 / 3)),
                (point: new Point(1.0 / 3, 2.5), equalPoint: new Point(0.33333333333333333333333333, 10.0 / 4)),
                (point: new Point(1e-45, -1e-43), equalPoint: new Point(1e-45, -1e-43))
            }
            // Test each pair
            .ForEach(pair =>
            {
                // Test the equals method
                pair.point.Equals(pair.equalPoint).Should().BeTrue();

                // Test the equals operator
                (pair.point == pair.equalPoint).Should().BeTrue();
            });
        }

        [Test]
        public void Test_Rotate_Point_Around_Itself()
        {
            // Prepare a point
            var point = new Point(42, 666);

            // Assert for some angles
            Enumerable.Range(0, 361).ForEach(angle => point.Rotate(point, angle).Should().Be(point));
        }

        [Test]
        public void Test_Rotate_Point_Around_Other_Point()
        {
            // Prepare a point and a center
            var point = new Point(7, 5);
            var center = new Point(3, 4);

            // Prepare some precalculated results
            new[]
            {
                (angle: 0, result: new Point(7, 5)),
                (angle: 30, result: new Point((5 + 4 * Sqrt(3)) / 2, (12 + Sqrt(3)) / 2)),
                (angle: 45, result: new Point((6 + 3 * Sqrt(2)) / 2, (8 + 5 * Sqrt(2)) / 2)),
                (angle: 60, result: new Point((10 - Sqrt(3)) / 2, (9 + 4 * Sqrt(3)) / 2)),
                (angle: 90, result: new Point(2, 8)),
                (angle: 180, result: new Point(-1, 3)),
                (angle: 210, result: new Point((7 - 4 * Sqrt(3)) / 2, (4 - Sqrt(3)) / 2)),
                (angle: 360, result: new Point(7, 5))
            }
            // Test each case 
            .ForEach(pair => point.Rotate(center, pair.angle).Should().Be(pair.result));
        }

        [Test]
        public void Test_Projection()
        {
            // Prepare some points
            var A = new Point(-4, -2);
            var B = new Point(6, -2);
            var C = new Point(-4, 6);

            // Assert precalculated projections
            A.Project(new Line(B, C)).Should().Be(new Point(-4.0 / 41, 118.0 / 41));
            B.Project(new Line(A, C)).Should().Be(A);
            C.Project(new Line(A, B)).Should().Be(A);
        }
    }
}