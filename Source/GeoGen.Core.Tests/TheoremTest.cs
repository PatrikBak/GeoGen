using FluentAssertions;
using NUnit.Framework;
using static GeoGen.Core.ComposedConstructions;
using static GeoGen.Core.ConfigurationObjectType;
using static GeoGen.Core.LooseObjectsLayout;
using static GeoGen.Core.PredefinedConstructionType;
using static GeoGen.Core.TheoremType;

namespace GeoGen.Core.Tests
{
    /// <summary>
    /// The test class for <see cref="Theorem"/>.
    /// </summary>
    [TestFixture]
    public class TheoremTest
    {
        [Test]
        public void Test_CanBeStatedInSmallerConfiguration_ConcurrentMedians()
        {
            // Draw configuration with medians
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var mBC = new ConstructedConfigurationObject(Midpoint, B, C);
            var mAC = new ConstructedConfigurationObject(Midpoint, C, A);
            var mAB = new ConstructedConfigurationObject(Midpoint, A, B);

            // Create the configuration
            var configuration = Configuration.DeriveFromObjects(ThreePoints, mBC, mAC, mAB);

            // This couldn't be stated in a smaller configuration
            new Theorem(configuration, ConcurrentObjects, new TheoremObject[]
            {
                new LineTheoremObject(A, mBC),
                new LineTheoremObject(B, mAC),
                new LineTheoremObject(C, mAB)
            })
            .CanBeStatedInSmallerConfiguration().Should().BeFalse();
        }

        [Test]
        public void Test_CanBeStatedInSmallerConfiguration_Midline_Is_Half_Of_Side()
        {
            // Draw configuration with medians
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var mBC = new ConstructedConfigurationObject(Midpoint, B, C);
            var mAC = new ConstructedConfigurationObject(Midpoint, C, A);
            var mAB = new ConstructedConfigurationObject(Midpoint, A, B);

            // Create the configuration
            var configuration = Configuration.DeriveFromObjects(ThreePoints, mBC, mAC, mAB);

            // This couldn't be stated in a smaller configuration either
            new Theorem(configuration, EqualLineSegments, new[]
            {
                new LineSegmentTheoremObject(mAB, mAC),
                new LineSegmentTheoremObject(B, mBC)
            })
            .CanBeStatedInSmallerConfiguration().Should().BeFalse();

            // Equivalently this couldn't be stated in a smaller configuration either
            new Theorem(configuration, EqualLineSegments, new[]
            {
                new LineSegmentTheoremObject(mAB, mAC),
                new LineSegmentTheoremObject(C, mBC)
            })
            .CanBeStatedInSmallerConfiguration().Should().BeFalse();
        }

        [Test]
        public void Test_CanBeStatedInSmallerConfiguration_Midline_Is_Parallel_To_Side()
        {
            // Draw configuration with medians
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var mBC = new ConstructedConfigurationObject(Midpoint, B, C);
            var mAC = new ConstructedConfigurationObject(Midpoint, C, A);
            var mAB = new ConstructedConfigurationObject(Midpoint, A, B);

            // Create the configuration
            var configuration = Configuration.DeriveFromObjects(ThreePoints, mBC, mAC, mAB);

            // This theorem can be stated without the midpoint of BC
            new Theorem(configuration, ParallelLines, new TheoremObject[]
            {
                new LineTheoremObject(B, C, mBC),
                new LineTheoremObject(mAC, mBC)
            })
            .CanBeStatedInSmallerConfiguration().Should().BeTrue();
        }

        [Test]
        public void Test_CanBeStatedInSmallerConfiguration_Trapezoid_Parallel_Lines()
        {
            // Prepare points
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var D = new LooseConfigurationObject(Point);
            var l = new ConstructedConfigurationObject(LineFromPoints, A, B);

            // Create the configuration
            var configuration = Configuration.DeriveFromObjects(Trapezoid, A, B, C, D, l);

            // This theorem can be stated without explicitly having line 'l'
            new Theorem(configuration, ParallelLines, new TheoremObject[]
            {
                new LineTheoremObject(l, points: new[] { A, B }),
                new LineTheoremObject(C, D)
            })
            .CanBeStatedInSmallerConfiguration().Should().BeTrue();
        }

        [Test]
        public void Test_CanBeStatedInSmallerConfiguration_Trapezoid_Equal_Angles()
        {
            // Prepare points
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var D = new LooseConfigurationObject(Point);
            var P = new ConstructedConfigurationObject(IntersectionOfLinesFromPoints, A, C, B, D);

            // Create the configuration
            var configuration = Configuration.DeriveFromObjects(Trapezoid, A, B, C, D, P);

            // This theorem can be stated without P
            new Theorem(configuration, EqualAngles, new TheoremObject[]
            {
                new AngleTheoremObject(new LineTheoremObject(A, C, P), new LineTheoremObject(A, B)),
                new AngleTheoremObject(new LineTheoremObject(A, C, P), new LineTheoremObject(C, D))
            })
            .CanBeStatedInSmallerConfiguration().Should().BeTrue();
        }
    }
}
