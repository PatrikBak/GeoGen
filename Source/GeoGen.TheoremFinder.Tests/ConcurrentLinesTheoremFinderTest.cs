using FluentAssertions;
using GeoGen.Core;
using GeoGen.Utilities;
using NUnit.Framework;
using static GeoGen.Core.ComposedConstructions;
using static GeoGen.Core.ConfigurationObjectType;
using static GeoGen.Core.LooseObjectsLayout;
using static GeoGen.Core.TheoremType;

namespace GeoGen.TheoremFinder.Tests
{
    /// <summary>
    /// The test class for <see cref="ConcurrentLinesTheoremFinder"/>.
    /// </summary>
    [TestFixture]
    public class ConcurrentLinesTheoremFinderTest : TypedTheoremFinderTestBase<ConcurrentLinesTheoremFinder>
    {
        [Test]
        public void Test_Pendicular_Bisectors_Of_Four_Chords()
        {
            // Create the objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var D = new ConstructedConfigurationObject(MidpointOfOppositeArc, A, B, C);
            var l1 = new ConstructedConfigurationObject(PerpendicularBisector, A, B);
            var l2 = new ConstructedConfigurationObject(PerpendicularBisector, B, C);
            var l3 = new ConstructedConfigurationObject(PerpendicularBisector, C, A);
            var l4 = new ConstructedConfigurationObject(PerpendicularBisector, A, D);

            // Create the configuration
            var configuration = Configuration.DeriveFromObjects(ThreePoints, l1, l2, l3, l4);

            // Run
            var (newTheorems, allTheorems) = FindTheorems(configuration);

            // Assert new theorems
            newTheorems.OrderlessEquals(new[]
            {
                new Theorem(ConcurrentLines, new[]
                {
                    new LineTheoremObject(l4),
                    new LineTheoremObject(l1),
                    new LineTheoremObject(l2)
                }),
                new Theorem(ConcurrentLines, new[]
                {
                    new LineTheoremObject(l4),
                    new LineTheoremObject(l1),
                    new LineTheoremObject(l3)
                }),
                new Theorem(ConcurrentLines, new[]
                {
                    new LineTheoremObject(l4),
                    new LineTheoremObject(l2),
                    new LineTheoremObject(l3)
                })
            })
            .Should().BeTrue();

            // Assert all theorems
            allTheorems.OrderlessEquals(new[]
            {
                new Theorem(ConcurrentLines, new[]
                {
                    new LineTheoremObject(l4),
                    new LineTheoremObject(l1),
                    new LineTheoremObject(l2)
                }),
                new Theorem(ConcurrentLines, new[]
                {
                    new LineTheoremObject(l4),
                    new LineTheoremObject(l1),
                    new LineTheoremObject(l3)
                }),
                new Theorem(ConcurrentLines, new[]
                {
                    new LineTheoremObject(l4),
                    new LineTheoremObject(l2),
                    new LineTheoremObject(l3)
                }),
                new Theorem(ConcurrentLines, new[]
                {
                    new LineTheoremObject(l1),
                    new LineTheoremObject(l2),
                    new LineTheoremObject(l3)
                })
            })
            .Should().BeTrue();
        }
    }
}