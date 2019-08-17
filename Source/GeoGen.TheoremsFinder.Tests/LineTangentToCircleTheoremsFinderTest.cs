using FluentAssertions;
using GeoGen.Core;
using GeoGen.Utilities;
using NUnit.Framework;
using static GeoGen.Core.ComposedConstructions;
using static GeoGen.Core.ConfigurationObjectType;
using static GeoGen.Core.LooseObjectsLayout;
using static GeoGen.Core.TheoremType;

namespace GeoGen.TheoremsFinder.Tests
{
    /// <summary>
    /// The test class for <see cref="LineTangentToCircleTheoremsFinder"/>.
    /// </summary>
    [TestFixture]
    public class LineTangentToCircleTheoremsFinderTest : TheoremsFinderTestBase<LineTangentToCircleTheoremsFinder>
    {
        [Test]
        public void Test_Right_Triangle_With_Perpendicular_Projections()
        {
            // Create the objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var D = new ConstructedConfigurationObject(PerpendicularProjectionOnLineFromPoints, A, B, C);
            var E = new ConstructedConfigurationObject(PerpendicularProjectionOnLineFromPoints, D, A, B);

            // Create the configuration
            var configuration = Configuration.DeriveFromObjects(RightTriangle, A, E);

            // Run
            var (newTheorems, allTheorems) = FindTheorems(configuration);

            // Assert new theorems
            newTheorems.ToSet(Theorem.EquivalencyComparer).SetEquals(new[]
            {
                new Theorem(configuration, LineTangentToCircle, new TheoremObject[]
                {
                    new LineTheoremObject(B, D, C),
                    new CircleTheoremObject(A, E, D)
                }),
                new Theorem(configuration, LineTangentToCircle, new TheoremObject[]
                {
                    new LineTheoremObject(A, D),
                    new CircleTheoremObject(B, E, D)
                })
            })
            .Should().BeTrue();

            // Assert all theorems
            allTheorems.ToSet(Theorem.EquivalencyComparer).SetEquals(new[]
            {
                new Theorem(configuration, LineTangentToCircle, new TheoremObject[]
                {
                    new LineTheoremObject(B, D, C),
                    new CircleTheoremObject(A, E, D)
                }),
                new Theorem(configuration, LineTangentToCircle, new TheoremObject[]
                {
                    new LineTheoremObject(A, D),
                    new CircleTheoremObject(B, E, D)
                }),
                new Theorem(configuration, LineTangentToCircle, new TheoremObject[]
                {
                    new LineTheoremObject(A, C),
                    new CircleTheoremObject(A, B, D)
                }),
                new Theorem(configuration, LineTangentToCircle, new TheoremObject[]
                {
                    new LineTheoremObject(A, B),
                    new CircleTheoremObject(A, C, D)
                })
            })
            .Should().BeTrue();
        }
    }
}
