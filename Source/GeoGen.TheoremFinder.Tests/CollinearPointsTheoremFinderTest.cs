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
    /// The test class for <see cref="CollinearPointsTheoremFinder"/>.
    /// </summary>
    [TestFixture]
    public class CollinearPointsTheoremFinderTest : TypedTheoremFinderTestBase<CollinearPointsTheoremFinder>
    {
        [Test]
        public void Test_Eulers_Line_In_Triangle()
        {
            // Create the objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var D = new ConstructedConfigurationObject(Midpoint, A, B);
            var E = new ConstructedConfigurationObject(Midpoint, A, C);
            var G = new ConstructedConfigurationObject(IntersectionOfLinesFromPoints, B, E, C, D);
            var H = new ConstructedConfigurationObject(Orthocenter, A, B, C);
            var O = new ConstructedConfigurationObject(Circumcenter, A, B, C);
            var F = new ConstructedConfigurationObject(Midpoint, O, H);

            // Create the configuration
            var configuration = Configuration.DeriveFromObjects(Triangle, G, F);

            // Run
            var (newTheorems, allTheorems) = FindTheorems(configuration);

            // Assert new theorems
            newTheorems.OrderlessEquals(new[]
            {
                new Theorem(CollinearPoints, F, O, H),
                new Theorem(CollinearPoints, F, O, G),
                new Theorem(CollinearPoints, F, H, G)
            })
            .Should().BeTrue();

            // Assert all theorems
            allTheorems.OrderlessEquals(new[]
            {
                new Theorem(CollinearPoints, A, B, D),
                new Theorem(CollinearPoints, A, C, E),
                new Theorem(CollinearPoints, B, E, G),
                new Theorem(CollinearPoints, C, D, G),
                new Theorem(CollinearPoints, F, O, H),
                new Theorem(CollinearPoints, F, O, G),
                new Theorem(CollinearPoints, F, H, G),
                new Theorem(CollinearPoints, O, H, G)
            })
            .Should().BeTrue();
        }

        [Test]
        public void Test_Intersection_Of_Two_Lines_From_Points()
        {
            // Create the objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var D = new LooseConfigurationObject(Point);
            var mAB = new ConstructedConfigurationObject(Midpoint, A, B);
            var mCD = new ConstructedConfigurationObject(Midpoint, C, D);
            var P = new ConstructedConfigurationObject(IntersectionOfLinesFromPoints, A, B, C, D);


            // Create the configuration
            var configuration = Configuration.DeriveFromObjects(Quadrilateral, A, B, C, D, mAB, mCD, P);

            // Run
            var (newTheorems, allTheorems) = FindTheorems(configuration);

            // Assert new theorems
            newTheorems.OrderlessEquals(new[]
            {
                new Theorem(CollinearPoints, P, A, B),
                new Theorem(CollinearPoints, P, A, mAB),
                new Theorem(CollinearPoints, P, B, mAB),
                new Theorem(CollinearPoints, P, C, D),
                new Theorem(CollinearPoints, P, C, mCD),
                new Theorem(CollinearPoints, P, D, mCD)
            })
            .Should().BeTrue();

            // Assert all theorems
            allTheorems.OrderlessEquals(new[]
            {
                new Theorem(CollinearPoints, P, A, B),
                new Theorem(CollinearPoints, P, A, mAB),
                new Theorem(CollinearPoints, P, B, mAB),
                new Theorem(CollinearPoints, P, C, D),
                new Theorem(CollinearPoints, P, C, mCD),
                new Theorem(CollinearPoints, P, D, mCD),
                new Theorem(CollinearPoints, A, B, mAB),
                new Theorem(CollinearPoints, C, D, mCD)
            })
            .Should().BeTrue();
        }
    }
}
