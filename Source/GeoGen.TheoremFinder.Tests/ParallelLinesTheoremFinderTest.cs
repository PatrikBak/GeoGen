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
    /// The test class for <see cref="ParallelLinesTheoremFinder"/>.
    /// </summary>
    [TestFixture]
    public class ParallelLinesTheoremFinderTest : TypedTheoremFinderTestBase<ParallelLinesTheoremFinder>
    {
        [Test]
        public void Test_Triangle_With_Many_Midpoints()
        {
            // Create the objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var D = new ConstructedConfigurationObject(Midpoint, B, C);
            var E = new ConstructedConfigurationObject(Midpoint, A, B);
            var F = new ConstructedConfigurationObject(Midpoint, A, C);
            var G = new ConstructedConfigurationObject(Midpoint, B, E);
            var H = new ConstructedConfigurationObject(Midpoint, C, F);

            // Create the configuration
            var configuration = Configuration.DeriveFromObjects(ThreePoints, D, E, F, G, H);

            // Run
            var (newTheorems, allTheorems) = FindTheorems(configuration);

            // Assert new theorems
            newTheorems.OrderlessEquals(new[]
            {
                new Theorem(configuration, ParallelLines, new[]
                {
                    new LineTheoremObject(H, G),
                    new LineTheoremObject(B, C)
                }),
                new Theorem(configuration, ParallelLines, new[]
                {
                    new LineTheoremObject(H, G),
                    new LineTheoremObject(C, D)
                }),
                new Theorem(configuration, ParallelLines, new[]
                {
                    new LineTheoremObject(H, G),
                    new LineTheoremObject(B, D)
                }),
                new Theorem(configuration, ParallelLines, new[]
                {
                    new LineTheoremObject(H, G),
                    new LineTheoremObject(E, F)
                }),
                new Theorem(configuration, ParallelLines, new[]
                {
                    new LineTheoremObject(H, D),
                    new LineTheoremObject(F, B)
                })
            })
            .Should().BeTrue();

            // Assert all theorems
            allTheorems.OrderlessEquals(new[]
            {
                new Theorem(configuration, ParallelLines, new[]
                {
                    new LineTheoremObject(H, G),
                    new LineTheoremObject(B, C)
                }),
                new Theorem(configuration, ParallelLines, new[]
                {
                    new LineTheoremObject(H, G),
                    new LineTheoremObject(C, D)
                }),
                new Theorem(configuration, ParallelLines, new[]
                {
                    new LineTheoremObject(H, G),
                    new LineTheoremObject(B, D)
                }),
                new Theorem(configuration, ParallelLines, new[]
                {
                    new LineTheoremObject(H, G),
                    new LineTheoremObject(E, F)
                }),
                new Theorem(configuration, ParallelLines, new[]
                {
                    new LineTheoremObject(H, D),
                    new LineTheoremObject(F, B)
                }),
                new Theorem(configuration, ParallelLines, new[]
                {
                    new LineTheoremObject(E, F),
                    new LineTheoremObject(B, C)
                }),
                new Theorem(configuration, ParallelLines, new[]
                {
                    new LineTheoremObject(E, F),
                    new LineTheoremObject(B, D)
                }),
                new Theorem(configuration, ParallelLines, new[]
                {
                    new LineTheoremObject(E, F),
                    new LineTheoremObject(C, D)
                }),
                new Theorem(configuration, ParallelLines, new[]
                {
                    new LineTheoremObject(G, D),
                    new LineTheoremObject(C, E)
                }),
                new Theorem(configuration, ParallelLines, new[]
                {
                    new LineTheoremObject(A, E),
                    new LineTheoremObject(F, D)
                }),
                new Theorem(configuration, ParallelLines, new[]
                {
                    new LineTheoremObject(A, B),
                    new LineTheoremObject(F, D)
                }),
                new Theorem(configuration, ParallelLines, new[]
                {
                    new LineTheoremObject(A, G),
                    new LineTheoremObject(F, D)
                }),
                new Theorem(configuration, ParallelLines, new[]
                {
                    new LineTheoremObject(B, G),
                    new LineTheoremObject(F, D)
                }),
                new Theorem(configuration, ParallelLines, new[]
                {
                    new LineTheoremObject(E, G),
                    new LineTheoremObject(F, D)
                }),
                new Theorem(configuration, ParallelLines, new[]
                {
                    new LineTheoremObject(E, B),
                    new LineTheoremObject(F, D)
                }),
                new Theorem(configuration, ParallelLines, new[]
                {
                    new LineTheoremObject(A, F),
                    new LineTheoremObject(D, E)
                }),
                new Theorem(configuration, ParallelLines, new[]
                {
                    new LineTheoremObject(A, H),
                    new LineTheoremObject(D, E)
                }),
                new Theorem(configuration, ParallelLines, new[]
                {
                    new LineTheoremObject(A, C),
                    new LineTheoremObject(D, E)
                }),
                new Theorem(configuration, ParallelLines, new[]
                {
                    new LineTheoremObject(F, H),
                    new LineTheoremObject(D, E)
                }),
                new Theorem(configuration, ParallelLines, new[]
                {
                    new LineTheoremObject(F, C),
                    new LineTheoremObject(D, E)
                }),
                new Theorem(configuration, ParallelLines, new[]
                {
                    new LineTheoremObject(C, H),
                    new LineTheoremObject(D, E)
                })
            })
            .Should().BeTrue();
        }

        [Test]
        public void Test_Triangle_With_Few_Midpoint_And_Explicit_lines()
        {
            // Create the objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var D = new ConstructedConfigurationObject(Midpoint, A, B);
            var E = new ConstructedConfigurationObject(Midpoint, A, C);
            var M = new ConstructedConfigurationObject(Midpoint, D, E);
            var l1 = new ConstructedConfigurationObject(LineFromPoints, D, E);
            var l2 = new ConstructedConfigurationObject(LineFromPoints, B, C);

            // Create the configuration
            var configuration = Configuration.DeriveFromObjects(ThreePoints, D, E, M, l1, l2);

            // Run
            var (newTheorems, allTheorems) = FindTheorems(configuration);

            // Assert new theorems
            newTheorems.Should().BeEmpty();

            // Assert all theorems
            allTheorems.OrderlessEquals(new[]
            {
                new Theorem(configuration, ParallelLines, new[]
                {
                    new LineTheoremObject(l2),
                    new LineTheoremObject(l1)
                }),
                new Theorem(configuration, ParallelLines, new[]
                {
                    new LineTheoremObject(l2),
                    new LineTheoremObject(D, E)
                }),
                new Theorem(configuration, ParallelLines, new[]
                {
                    new LineTheoremObject(l2),
                    new LineTheoremObject(D, M)
                }),
                new Theorem(configuration, ParallelLines, new[]
                {
                    new LineTheoremObject(l2),
                    new LineTheoremObject(M, E)
                }),
                new Theorem(configuration, ParallelLines, new[]
                {
                    new LineTheoremObject(B, C),
                    new LineTheoremObject(l1)
                }),
                new Theorem(configuration, ParallelLines, new[]
                {
                    new LineTheoremObject(B, C),
                    new LineTheoremObject(D, E)
                }),
                new Theorem(configuration, ParallelLines, new[]
                {
                    new LineTheoremObject(B, C),
                    new LineTheoremObject(D, M)
                }),
                new Theorem(configuration, ParallelLines, new[]
                {
                    new LineTheoremObject(B, C),
                    new LineTheoremObject(M, E)
                })
            })
            .Should().BeTrue();
        }
    }
}
