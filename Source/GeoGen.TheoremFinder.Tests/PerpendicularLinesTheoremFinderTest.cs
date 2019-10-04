using FluentAssertions;
using GeoGen.Core;
using GeoGen.Utilities;
using NUnit.Framework;
using static GeoGen.Core.ComposedConstructions;
using static GeoGen.Core.ConfigurationObjectType;
using static GeoGen.Core.LooseObjectsLayout;
using static GeoGen.Core.PredefinedConstructionType;
using static GeoGen.Core.TheoremType;

namespace GeoGen.TheoremFinder.Tests
{
    /// <summary>
    /// The test class for <see cref="PerpendicularLinesTheoremFinder"/>.
    /// </summary>
    [TestFixture]
    public class PerpendicularLinesTheoremFinderTest : TypedTheoremFinderTestBase<PerpendicularLinesTheoremFinder>
    {
        [Test]
        public void Test_Reflected_Orthocenter()
        {
            // Create the objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var H = new ConstructedConfigurationObject(Orthocenter, A, B, C);
            var N = new ConstructedConfigurationObject(Midpoint, A, C);
            var M = new ConstructedConfigurationObject(Midpoint, B, C);
            var P = new ConstructedConfigurationObject(PointReflection, H, M);

            // Create the configuration
            var configuration = Configuration.DeriveFromObjects(ThreePoints, H, N, M, P);

            // Run
            var (newTheorems, allTheorems) = FindTheorems(configuration);

            // Assert new theorems
            newTheorems.OrderlessEquals(new[]
            {
                new Theorem(PerpendicularLines, new[]
                {
                    new LineTheoremObject(P, B),
                    new LineTheoremObject(A, B)
                }),
                new Theorem(PerpendicularLines, new[]
                {
                    new LineTheoremObject(P, C),
                    new LineTheoremObject(A, C)
                }),
                new Theorem(PerpendicularLines, new[]
                {
                    new LineTheoremObject(P, C),
                    new LineTheoremObject(A, N)
                }),
                new Theorem(PerpendicularLines, new[]
                {
                    new LineTheoremObject(P, C),
                    new LineTheoremObject(N, C)
                }),
                new Theorem(PerpendicularLines, new[]
                {
                    new LineTheoremObject(P, B),
                    new LineTheoremObject(M, N)
                })
            })
            .Should().BeTrue();

            // Assert all theorems
            allTheorems.OrderlessEquals(new[]
            {
                new Theorem(PerpendicularLines, new[]
                {
                    new LineTheoremObject(P, B),
                    new LineTheoremObject(A, B)
                }),
                new Theorem(PerpendicularLines, new[]
                {
                    new LineTheoremObject(P, C),
                    new LineTheoremObject(A, C)
                }),
                new Theorem(PerpendicularLines, new[]
                {
                    new LineTheoremObject(P, C),
                    new LineTheoremObject(A, N)
                }),
                new Theorem(PerpendicularLines, new[]
                {
                    new LineTheoremObject(P, C),
                    new LineTheoremObject(N, C)
                }),
                new Theorem(PerpendicularLines, new[]
                {
                    new LineTheoremObject(P, B),
                    new LineTheoremObject(M, N)
                }),
                new Theorem(PerpendicularLines, new[]
                {
                    new LineTheoremObject(A, H),
                    new LineTheoremObject(B, C)
                }),
                new Theorem(PerpendicularLines, new[]
                {
                    new LineTheoremObject(A, H),
                    new LineTheoremObject(M, C)
                }),
                new Theorem(PerpendicularLines, new[]
                {
                    new LineTheoremObject(A, H),
                    new LineTheoremObject(B, M)
                }),
                new Theorem(PerpendicularLines, new[]
                {
                    new LineTheoremObject(B, H),
                    new LineTheoremObject(A, C)
                }),
                new Theorem(PerpendicularLines, new[]
                {
                    new LineTheoremObject(B, H),
                    new LineTheoremObject(N, C)
                }),
                new Theorem(PerpendicularLines, new[]
                {
                    new LineTheoremObject(B, H),
                    new LineTheoremObject(A, N)
                }),
                new Theorem(PerpendicularLines, new[]
                {
                    new LineTheoremObject(C, H),
                    new LineTheoremObject(M, N)
                }),
                new Theorem(PerpendicularLines, new[]
                {
                    new LineTheoremObject(C, H),
                    new LineTheoremObject(A, B)
                })
            })
            .Should().BeTrue();
        }
    }
}
