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
    /// The test class for <see cref="ParallelLinesTheoremsFinder"/>.
    /// </summary>
    [TestFixture]
    public class ParallelLinesTheoremsFinderTest : TheoremsFinderTestBase<ParallelLinesTheoremsFinder>
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
                    new LineTheoremObject(B, C, D)
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
            })
            .Should().BeTrue();

            // Assert all theorems
            allTheorems.OrderlessEquals(new[]
            {
                new Theorem(configuration, ParallelLines, new[]
                {
                    new LineTheoremObject(H, G),
                    new LineTheoremObject(B, C, D)
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
                    new LineTheoremObject(B, C, D)
                }),
                new Theorem(configuration, ParallelLines, new[]
                {
                    new LineTheoremObject(G, D),
                    new LineTheoremObject(C, E)
                }),
                new Theorem(configuration, ParallelLines, new[]
                {
                    new LineTheoremObject(A, E, B, G),
                    new LineTheoremObject(F, D)
                }),
                new Theorem(configuration, ParallelLines, new[]
                {
                    new LineTheoremObject(A, F, H, G),
                    new LineTheoremObject(D, E)
                })
            })
            .Should().BeTrue();
        }
    }
}
