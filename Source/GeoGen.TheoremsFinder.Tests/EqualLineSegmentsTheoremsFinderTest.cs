using FluentAssertions;
using GeoGen.Core;
using GeoGen.Utilities;
using NUnit.Framework;
using static GeoGen.Core.ConfigurationObjectType;
using static GeoGen.Core.LooseObjectsLayout;
using static GeoGen.Core.PredefinedConstructionType;
using static GeoGen.Core.TheoremType;

namespace GeoGen.TheoremsFinder.Tests
{
    /// <summary>
    /// The test class for <see cref="EqualLineSegmentsTheoremsFinder"/>.
    /// </summary>
    [TestFixture]
    public class EqualLineSegmentsTheoremsFinderTest : TheoremsFinderTestBase<EqualLineSegmentsTheoremsFinder>
    {
        [Test]
        public void Test_Triangle_With_Many_Midpoints()
        {
            // Create the objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var D = new ConstructedConfigurationObject(Midpoint, A, B);
            var E = new ConstructedConfigurationObject(Midpoint, A, C);
            var F = new ConstructedConfigurationObject(Midpoint, B, C);

            // Create the configuration
            var configuration = Configuration.DeriveFromObjects(ThreePoints, D, E, F);

            // Run
            var (newTheorems, allTheorems) = FindTheorems(configuration);

            // Assert counts
            newTheorems.Count.Should().Be(7);
            allTheorems.Count.Should().Be(9);

            // Assert new theorems
            newTheorems.ToSet().SetEquals(new[]
            {
                new Theorem(configuration, EqualLineSegments, new[]
                {
                    new LineSegmentTheoremObject(F, B),
                    new LineSegmentTheoremObject(F, C)
                }),
                new Theorem(configuration, EqualLineSegments, new[]
                {
                    new LineSegmentTheoremObject(F, B),
                    new LineSegmentTheoremObject(D, E)
                }),
                new Theorem(configuration, EqualLineSegments, new[]
                {
                    new LineSegmentTheoremObject(F, C),
                    new LineSegmentTheoremObject(D, E)
                }),
                new Theorem(configuration, EqualLineSegments, new[]
                {
                    new LineSegmentTheoremObject(F, D),
                    new LineSegmentTheoremObject(C, E)
                }),
                new Theorem(configuration, EqualLineSegments, new[]
                {
                    new LineSegmentTheoremObject(F, D),
                    new LineSegmentTheoremObject(A, E)
                }),
                new Theorem(configuration, EqualLineSegments, new[]
                {
                    new LineSegmentTheoremObject(F, E),
                    new LineSegmentTheoremObject(A, D)
                }),
                new Theorem(configuration, EqualLineSegments, new[]
                {
                    new LineSegmentTheoremObject(F, E),
                    new LineSegmentTheoremObject(B, D)
                })
            })
            .Should().BeTrue();

            // Assert all theorems
            allTheorems.ToSet().SetEquals(new[]
            {
                new Theorem(configuration, EqualLineSegments, new[]
                {
                    new LineSegmentTheoremObject(F, B),
                    new LineSegmentTheoremObject(F, C)
                }),
                new Theorem(configuration, EqualLineSegments, new[]
                {
                    new LineSegmentTheoremObject(F, B),
                    new LineSegmentTheoremObject(D, E)
                }),
                new Theorem(configuration, EqualLineSegments, new[]
                {
                    new LineSegmentTheoremObject(F, C),
                    new LineSegmentTheoremObject(D, E)
                }),
                new Theorem(configuration, EqualLineSegments, new[]
                {
                    new LineSegmentTheoremObject(F, D),
                    new LineSegmentTheoremObject(C, E)
                }),
                new Theorem(configuration, EqualLineSegments, new[]
                {
                    new LineSegmentTheoremObject(F, D),
                    new LineSegmentTheoremObject(A, E)
                }),
                new Theorem(configuration, EqualLineSegments, new[]
                {
                    new LineSegmentTheoremObject(F, E),
                    new LineSegmentTheoremObject(A, D)
                }),
                new Theorem(configuration, EqualLineSegments, new[]
                {
                    new LineSegmentTheoremObject(F, E),
                    new LineSegmentTheoremObject(B, D)
                }),
                new Theorem(configuration, EqualLineSegments, new[]
                {
                    new LineSegmentTheoremObject(A, D),
                    new LineSegmentTheoremObject(B, D)
                }),
                new Theorem(configuration, EqualLineSegments, new[]
                {
                    new LineSegmentTheoremObject(A, E),
                    new LineSegmentTheoremObject(C, E)
                })
            })
            .Should().BeTrue();
        }
    }
}
