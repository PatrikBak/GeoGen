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
    /// The test class for <see cref="ConcyclicPointsTheoremFinder"/>.
    /// </summary>
    [TestFixture]
    public class ConcyclicPointsTheoremFinderTest : TypedTheoremFinderTestBase<ConcyclicPointsTheoremFinder>
    {
        [Test]
        public void Test_Triangle_With_Altitude_Feets()
        {
            // Create the objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var D = new ConstructedConfigurationObject(PerpendicularProjectionOnLineFromPoints, B, A, C);
            var E = new ConstructedConfigurationObject(PerpendicularProjectionOnLineFromPoints, C, B, A);
            var H = new ConstructedConfigurationObject(IntersectionOfLinesFromPoints, B, D, C, E);
            var F = new ConstructedConfigurationObject(IntersectionOfLinesFromPoints, A, H, B, C);

            // Create the configuration
            var configuration = Configuration.DeriveFromObjects(ThreePoints, F);

            // Run
            var (newTheorems, allTheorems) = FindTheorems(configuration);

            // Assert new theorems
            newTheorems.OrderlessEquals(new[]
            {
                new Theorem(ConcyclicPoints, F, B, E, H),
                new Theorem(ConcyclicPoints, F, C, D, H),
                new Theorem(ConcyclicPoints, F, B, D, A),
                new Theorem(ConcyclicPoints, F, C, E, A)
            })
            .Should().BeTrue();

            // Assert all theorems
            allTheorems.OrderlessEquals(new[]
            {
                new Theorem(ConcyclicPoints, F, B, E, H),
                new Theorem(ConcyclicPoints, F, C, D, H),
                new Theorem(ConcyclicPoints, F, B, D, A),
                new Theorem(ConcyclicPoints, F, C, E, A),
                new Theorem(ConcyclicPoints, B, C, D, E),
                new Theorem(ConcyclicPoints, A, D, E, H)
            })
            .Should().BeTrue();
        }

        [Test]
        public void Test_Triangle_With_Reflected_Orthocenter()
        {
            // Create the objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var H = new ConstructedConfigurationObject(Orthocenter, A, B, C);
            var F = new ConstructedConfigurationObject(ReflectionInLineFromPoints, H, A, B);
            var G = new ConstructedConfigurationObject(ReflectionInLineFromPoints, H, A, C);

            // Create the configuration
            var configuration = Configuration.DeriveFromObjects(ThreePoints, F, G);

            // Run
            var (newTheorems, allTheorems) = FindTheorems(configuration);

            // Assert new theorems
            newTheorems.OrderlessEquals(new[]
            {
                new Theorem(ConcyclicPoints, G, A, B, C),
                new Theorem(ConcyclicPoints, G, A, B, F),
                new Theorem(ConcyclicPoints, G, A, C, F),
                new Theorem(ConcyclicPoints, G, B, C, F)
            })
            .Should().BeTrue();

            // Assert all theorems
            allTheorems.OrderlessEquals(new[]
            {
                new Theorem(ConcyclicPoints, G, A, B, C),
                new Theorem(ConcyclicPoints, G, A, B, F),
                new Theorem(ConcyclicPoints, G, A, C, F),
                new Theorem(ConcyclicPoints, G, B, C, F),
                new Theorem(ConcyclicPoints, A, B, C, F),
            })
            .Should().BeTrue();
        }
    }
}