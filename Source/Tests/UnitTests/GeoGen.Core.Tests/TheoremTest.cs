using FluentAssertions;
using NUnit.Framework;
using static GeoGen.Core.ConfigurationObjectType;
using static GeoGen.Core.LooseObjectLayout;
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
        public void Test_GetUnnecessaryObjects_ConcurrentMedians()
        {
            // Draw configuration with medians
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var mBC = new ConstructedConfigurationObject(Midpoint, B, C);
            var mAC = new ConstructedConfigurationObject(Midpoint, C, A);
            var mAB = new ConstructedConfigurationObject(Midpoint, A, B);

            // Create the configuration
            var configuration = Configuration.DeriveFromObjects(Triangle, mBC, mAC, mAB);

            // This can't be stated in a smaller configuration
            new Theorem(ConcurrentObjects, new TheoremObject[]
            {
                new LineTheoremObject(A, mBC),
                new LineTheoremObject(B, mAC),
                new LineTheoremObject(C, mAB)
            })
            .GetUnnecessaryObjects(configuration).Should().BeEmpty();
        }

        [Test]
        public void Test_GetUnnecessaryObjects_Midline_Is_Half_Of_Side()
        {
            // Draw configuration with medians
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var mBC = new ConstructedConfigurationObject(Midpoint, B, C);
            var mAC = new ConstructedConfigurationObject(Midpoint, C, A);
            var mAB = new ConstructedConfigurationObject(Midpoint, A, B);

            // Create the configuration
            var configuration = Configuration.DeriveFromObjects(Triangle, mBC, mAC, mAB);

            // This can't be stated in a smaller configuration either
            new Theorem(EqualLineSegments, new[]
            {
                new LineSegmentTheoremObject(mAB, mAC),
                new LineSegmentTheoremObject(B, mBC)
            })
            .GetUnnecessaryObjects(configuration).Should().BeEmpty();

            // Analogously this couldn't be stated in a smaller configuration either
            new Theorem(EqualLineSegments, new[]
            {
                new LineSegmentTheoremObject(mAB, mAC),
                new LineSegmentTheoremObject(C, mBC)
            })
            .GetUnnecessaryObjects(configuration).Should().BeEmpty();
        }

        [Test]
        public void Test_GetUnnecessaryObjects_Midline_Is_Parallel_To_Side()
        {
            // Draw configuration with medians
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var mBC = new ConstructedConfigurationObject(Midpoint, B, C);
            var mAC = new ConstructedConfigurationObject(Midpoint, C, A);
            var mAB = new ConstructedConfigurationObject(Midpoint, A, B);

            // Create the configuration
            var configuration = Configuration.DeriveFromObjects(Triangle, mBC, mAC, mAB);

            // This theorem can be stated without the midpoint of BC
            new Theorem(ParallelLines, new TheoremObject[]
            {
                new LineTheoremObject(B, C),
                new LineTheoremObject(mAB, mAC)
            })
            .GetUnnecessaryObjects(configuration).Should().BeEquivalentTo(new[] { mBC });
        }
    }
}
