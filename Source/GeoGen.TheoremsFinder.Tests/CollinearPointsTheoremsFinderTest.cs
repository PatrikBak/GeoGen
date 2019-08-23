using FluentAssertions;
using GeoGen.Core;
using GeoGen.Utilities;
using NUnit.Framework;
using static GeoGen.Core.ComposedConstructions;
using static GeoGen.Core.ConfigurationObjectType;
using static GeoGen.Core.LooseObjectsLayout;
using static GeoGen.Core.PredefinedConstructionType;
using static GeoGen.Core.TheoremType;

namespace GeoGen.TheoremsFinder.Tests
{
    /// <summary>
    /// The test class for <see cref="CollinearPointsTheoremsFinder"/>.
    /// </summary>
    [TestFixture]
    public class CollinearPointsTheoremsFinderTest : TheoremsFinderTestBase<CollinearPointsTheoremsFinder>
    {
        [Test]
        public void Test_Eulers_Line_In_Triangle()
        {
            // Create the objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var G = new ConstructedConfigurationObject(Centroid, A, B, C);
            var H = new ConstructedConfigurationObject(Orthocenter, A, B, C);
            var O = new ConstructedConfigurationObject(Circumcenter, A, B, C);
            var F = new ConstructedConfigurationObject(Midpoint, O, H);

            // Create the configuration
            var configuration = Configuration.DeriveFromObjects(ThreePoints, G, F);

            // Run
            var (newTheorems, allTheorems) = FindTheorems(configuration);

            // Assert new theorems
            newTheorems.ToSet().SetEquals(new[]
            {
                new Theorem(configuration, CollinearPoints, F, O, H),
                new Theorem(configuration, CollinearPoints, F, O, G),
                new Theorem(configuration, CollinearPoints, F, H, G)
            })
            .Should().BeTrue();

            // Assert all theorems
            allTheorems.ToSet().SetEquals(new[]
            {
                new Theorem(configuration, CollinearPoints, F, O, H),
                new Theorem(configuration, CollinearPoints, F, O, G),
                new Theorem(configuration, CollinearPoints, F, H, G),
                new Theorem(configuration, CollinearPoints, O, H, G)
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
            var configuration = Configuration.DeriveFromObjects(FourPoints, A, B, C, D, mAB, mCD, P);

            // Run
            var (newTheorems, allTheorems) = FindTheorems(configuration);

            // Assert new theorems
            newTheorems.ToSet().SetEquals(new[]
            {
                new Theorem(configuration, CollinearPoints, P, A, B),
                new Theorem(configuration, CollinearPoints, P, A, mAB),
                new Theorem(configuration, CollinearPoints, P, B, mAB),
                new Theorem(configuration, CollinearPoints, P, C, D),
                new Theorem(configuration, CollinearPoints, P, C, mCD),
                new Theorem(configuration, CollinearPoints, P, D, mCD)
            })
            .Should().BeTrue();

            // Assert all theorems
            allTheorems.ToSet().SetEquals(new[]
            {
                new Theorem(configuration, CollinearPoints, P, A, B),
                new Theorem(configuration, CollinearPoints, P, A, mAB),
                new Theorem(configuration, CollinearPoints, P, B, mAB),
                new Theorem(configuration, CollinearPoints, P, C, D),
                new Theorem(configuration, CollinearPoints, P, C, mCD),
                new Theorem(configuration, CollinearPoints, P, D, mCD),
                new Theorem(configuration, CollinearPoints, A, B, mAB),
                new Theorem(configuration, CollinearPoints, C, D, mCD)
            })
            .Should().BeTrue();
        }
    }
}
