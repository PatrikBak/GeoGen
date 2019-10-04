using FluentAssertions;
using GeoGen.Core;
using GeoGen.Utilities;
using NUnit.Framework;
using static GeoGen.Core.ConfigurationObjectType;
using static GeoGen.Core.LooseObjectsLayout;
using static GeoGen.Core.PredefinedConstructionType;
using static GeoGen.Core.TheoremType;

namespace GeoGen.TheoremFinder.Tests
{
    /// <summary>
    /// The test class for <see cref="EqualLineSegmentsTheoremFinder"/>.
    /// </summary>
    [TestFixture]
    public class EqualLineSegmentsTheoremFinderTest : TypedTheoremFinderTestBase<EqualLineSegmentsTheoremFinder>
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
            var configuration = Configuration.DeriveFromObjects(Triangle, D, E, F);

            // Run
            var (newTheorems, allTheorems) = FindTheorems(configuration);

            // Assert new theorems
            newTheorems.OrderlessEquals(new[]
            {
                new Theorem(EqualLineSegments, new[]
                {
                    new LineSegmentTheoremObject(F, B),
                    new LineSegmentTheoremObject(F, C)
                }),
                new Theorem(EqualLineSegments, new[]
                {
                    new LineSegmentTheoremObject(F, B),
                    new LineSegmentTheoremObject(D, E)
                }),
                new Theorem(EqualLineSegments, new[]
                {
                    new LineSegmentTheoremObject(F, C),
                    new LineSegmentTheoremObject(D, E)
                }),
                new Theorem(EqualLineSegments, new[]
                {
                    new LineSegmentTheoremObject(F, D),
                    new LineSegmentTheoremObject(C, E)
                }),
                new Theorem(EqualLineSegments, new[]
                {
                    new LineSegmentTheoremObject(F, D),
                    new LineSegmentTheoremObject(A, E)
                }),
                new Theorem(EqualLineSegments, new[]
                {
                    new LineSegmentTheoremObject(F, E),
                    new LineSegmentTheoremObject(A, D)
                }),
                new Theorem(EqualLineSegments, new[]
                {
                    new LineSegmentTheoremObject(F, E),
                    new LineSegmentTheoremObject(B, D)
                })
            })
            .Should().BeTrue();

            // Assert all theorems
            allTheorems.OrderlessEquals(new[]
            {
                new Theorem(EqualLineSegments, new[]
                {
                    new LineSegmentTheoremObject(F, B),
                    new LineSegmentTheoremObject(F, C)
                }),
                new Theorem(EqualLineSegments, new[]
                {
                    new LineSegmentTheoremObject(F, B),
                    new LineSegmentTheoremObject(D, E)
                }),
                new Theorem(EqualLineSegments, new[]
                {
                    new LineSegmentTheoremObject(F, C),
                    new LineSegmentTheoremObject(D, E)
                }),
                new Theorem(EqualLineSegments, new[]
                {
                    new LineSegmentTheoremObject(F, D),
                    new LineSegmentTheoremObject(C, E)
                }),
                new Theorem(EqualLineSegments, new[]
                {
                    new LineSegmentTheoremObject(F, D),
                    new LineSegmentTheoremObject(A, E)
                }),
                new Theorem(EqualLineSegments, new[]
                {
                    new LineSegmentTheoremObject(F, E),
                    new LineSegmentTheoremObject(A, D)
                }),
                new Theorem(EqualLineSegments, new[]
                {
                    new LineSegmentTheoremObject(F, E),
                    new LineSegmentTheoremObject(B, D)
                }),
                new Theorem(EqualLineSegments, new[]
                {
                    new LineSegmentTheoremObject(A, D),
                    new LineSegmentTheoremObject(B, D)
                }),
                new Theorem(EqualLineSegments, new[]
                {
                    new LineSegmentTheoremObject(A, E),
                    new LineSegmentTheoremObject(C, E)
                })
            })
            .Should().BeTrue();
        }
    }
}
