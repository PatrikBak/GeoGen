using FluentAssertions;
using GeoGen.Core;
using GeoGen.Utilities;
using NUnit.Framework;
using System.Linq;
using static GeoGen.Core.ComposedConstructions;
using static GeoGen.Core.ConfigurationObjectType;
using static GeoGen.Core.LooseObjectsLayout;
using static GeoGen.Core.PredefinedConstructionType;
using static GeoGen.Core.TheoremType;

namespace GeoGen.TheoremsFinder.Tests
{
    /// <summary>
    /// The test class for <see cref="ConcurrentObjectsTheoremsFinder"/>.
    /// </summary>
    [TestFixture]
    public class ConcurrentObjectsTheoremsFinderTest : TheoremsFinderTestBase<ConcurrentObjectsTheoremsFinder>
    {
        [Test]
        public void Test_Triangle_With_Midpoints()
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
            newTheorems.Count.Should().Be(8);
            allTheorems.Count.Should().Be(8);

            // Assert new theorems
            newTheorems.ToSet().SetEquals(new[]
            {
                new Theorem(configuration, ConcurrentObjects, new[]
                {
                    new LineTheoremObject(A, F),
                    new LineTheoremObject(B, E),
                    new LineTheoremObject(C, D)
                }),
                new Theorem(configuration, ConcurrentObjects, new[]
                {
                    new CircleTheoremObject(A, D, E),
                    new CircleTheoremObject(B, D, F),
                    new CircleTheoremObject(C, E, F)
                }),
                new Theorem(configuration, ConcurrentObjects, new TheoremObject[]
                {
                    new CircleTheoremObject(B, C, E),
                    new CircleTheoremObject(B, D, F),
                    new LineTheoremObject(D, E)
                }),
                new Theorem(configuration, ConcurrentObjects, new TheoremObject[]
                {
                    new CircleTheoremObject(C, E, F),
                    new CircleTheoremObject(B, C, D),
                    new LineTheoremObject(D, E)
                }),
                new Theorem(configuration, ConcurrentObjects, new TheoremObject[]
                {
                    new CircleTheoremObject(A, B, E),
                    new CircleTheoremObject(B, D, F),
                    new LineTheoremObject(E, F)
                }),
                new Theorem(configuration, ConcurrentObjects, new TheoremObject[]
                {
                    new CircleTheoremObject(A, B, F),
                    new CircleTheoremObject(A, D, E),
                    new LineTheoremObject(E, F)
                }),
                new Theorem(configuration, ConcurrentObjects, new TheoremObject[]
                {
                    new CircleTheoremObject(A, C, D),
                    new CircleTheoremObject(C, E, F),
                    new LineTheoremObject(D, F)
                }),
                new Theorem(configuration, ConcurrentObjects, new TheoremObject[]
                {
                    new CircleTheoremObject(A, C, F),
                    new CircleTheoremObject(A, D, E),
                    new LineTheoremObject(D, F)
                })
            })
            .Should().BeTrue();

            // Assert all theorems
            allTheorems.ToSet().SetEquals(newTheorems).Should().BeTrue();
        }

        [Test]
        public void Test_Triangle_With_Altitude_Feets()
        {
            // Create the objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var D = new ConstructedConfigurationObject(PerpendicularProjectionOnLineFromPoints, B, A, C);
            var E = new ConstructedConfigurationObject(PerpendicularProjectionOnLineFromPoints, C, A, B);
            var H = new ConstructedConfigurationObject(IntersectionOfLinesFromPoints, B, D, C, E);

            // Create the configuration
            var configuration = Configuration.DeriveFromObjects(ThreePoints, H);

            // Run
            var (newTheorems, allTheorems) = FindTheorems(configuration);

            // Assert counts
            newTheorems.Count.Should().Be(19);
            allTheorems.Count.Should().Be(20);

            // Assert new theorems
            newTheorems.ToSet().SetEquals(new[]
            {
                new Theorem(configuration, ConcurrentObjects, new TheoremObject[]
                {
                    new LineTheoremObject(A, H),
                    new LineTheoremObject(B, C),
                    new CircleTheoremObject(A, B, D)
                }),
                new Theorem(configuration, ConcurrentObjects, new TheoremObject[]
                {
                    new LineTheoremObject(A, H),
                    new LineTheoremObject(B, C),
                    new CircleTheoremObject(A, C, E)
                }),
                new Theorem(configuration, ConcurrentObjects, new TheoremObject[]
                {
                    new LineTheoremObject(A, H),
                    new LineTheoremObject(B, C),
                    new CircleTheoremObject(B, E, H),
                }),
                new Theorem(configuration, ConcurrentObjects, new TheoremObject[]
                {
                    new LineTheoremObject(A, H),
                    new LineTheoremObject(B, C),
                    new CircleTheoremObject(C, D, H)
                }),
                new Theorem(configuration, ConcurrentObjects, new TheoremObject[]
                {
                    new LineTheoremObject(A, H),
                    new CircleTheoremObject(A, B, D),
                    new CircleTheoremObject(A, C, E)
                }),
                new Theorem(configuration, ConcurrentObjects, new TheoremObject[]
                {
                    new LineTheoremObject(A, H),
                    new CircleTheoremObject(A, B, D),
                    new CircleTheoremObject(B, E, H)
                }),
                new Theorem(configuration, ConcurrentObjects, new TheoremObject[]
                {
                    new LineTheoremObject(A, H),
                    new CircleTheoremObject(A, B, D),
                    new CircleTheoremObject(C, D, H)
                }),
                new Theorem(configuration, ConcurrentObjects, new TheoremObject[]
                {
                    new LineTheoremObject(A, H),
                    new CircleTheoremObject(A, C, E),
                    new CircleTheoremObject(B, E, H),
                }),
                new Theorem(configuration, ConcurrentObjects, new TheoremObject[]
                {
                    new LineTheoremObject(A, H),
                    new CircleTheoremObject(A, C, E),
                    new CircleTheoremObject(C, D, H)
                }),
                new Theorem(configuration, ConcurrentObjects, new TheoremObject[]
                {
                    new LineTheoremObject(A, H),
                    new CircleTheoremObject(B, E, H),
                    new CircleTheoremObject(C, D, H)
                }),
                new Theorem(configuration, ConcurrentObjects, new TheoremObject[]
                {
                    new LineTheoremObject(B, C),
                    new CircleTheoremObject(A, B, D),
                    new CircleTheoremObject(B, E, H)
                }),
                new Theorem(configuration, ConcurrentObjects, new TheoremObject[]
                {
                    new LineTheoremObject(B, C),
                    new CircleTheoremObject(A, B, D),
                    new CircleTheoremObject(C, D, H)
                }),
                new Theorem(configuration, ConcurrentObjects, new TheoremObject[]
                {
                    new LineTheoremObject(B, C),
                    new CircleTheoremObject(A, C, E),
                    new CircleTheoremObject(B, E, H)
                }),
                new Theorem(configuration, ConcurrentObjects, new TheoremObject[]
                {
                    new LineTheoremObject(B, C),
                    new CircleTheoremObject(A, C, E),
                    new CircleTheoremObject(C, D, H)
                }),
                new Theorem(configuration, ConcurrentObjects, new TheoremObject[]
                {
                    new LineTheoremObject(B, C),
                    new CircleTheoremObject(B, E, H),
                    new CircleTheoremObject(C, D, H)
                }),
                new Theorem(configuration, ConcurrentObjects, new TheoremObject[]
                {
                    new CircleTheoremObject(A, B, D),
                    new CircleTheoremObject(A, C, E),
                    new CircleTheoremObject(B, E, H),
                }),
                new Theorem(configuration, ConcurrentObjects, new TheoremObject[]
                {
                    new CircleTheoremObject(A, B, D),
                    new CircleTheoremObject(A, C, E),
                    new CircleTheoremObject(C, D, H)
                }),
                new Theorem(configuration, ConcurrentObjects, new TheoremObject[]
                {
                    new CircleTheoremObject(A, B, D),
                    new CircleTheoremObject(B, E, H),
                    new CircleTheoremObject(C, D, H)
                }),
                new Theorem(configuration, ConcurrentObjects, new TheoremObject[]
                {
                    new CircleTheoremObject(A, C, E),
                    new CircleTheoremObject(B, E, H),
                    new CircleTheoremObject(C, D, H)
                })
            })
            .Should().BeTrue();

            // Assert all theorems
            allTheorems.ToSet().SetEquals(newTheorems.Concat(
                // This is the only theorem that can be stated without H
                new Theorem(configuration, ConcurrentObjects, new TheoremObject[]
                {
                    new LineTheoremObject(B, C),
                    new CircleTheoremObject(A, B, D),
                    new CircleTheoremObject(A, C, E)
                })
            ))
            .Should().BeTrue();
        }

        [Test]
        public void Test_When_Three_Objects_Have_Two_Interesting_Intersections()
        {
            // Create the objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var M = new ConstructedConfigurationObject(Midpoint, A, B);
            var l = new ConstructedConfigurationObject(PerpendicularLineAtPointOfLine, M, A);
            var c1 = new ConstructedConfigurationObject(CircleWithCenterThroughPoint, A, B);
            var c2 = new ConstructedConfigurationObject(CircleWithCenterThroughPoint, B, A);

            // Create the configuration
            var configuration = Configuration.DeriveFromObjects(TwoPoints, A, B, M, l, c1, c2);

            // Run
            var (newTheorems, allTheorems) = FindTheorems(configuration);

            // Assert counts
            newTheorems.Count.Should().Be(1);
            allTheorems.Count.Should().Be(1);

            // Assert new theorems
            newTheorems.ToSet().SetEquals(new[]
            {
                new Theorem(configuration, ConcurrentObjects, new TheoremObject[]
                {
                    new LineTheoremObject(l),
                    new CircleTheoremObject(c1),
                    new CircleTheoremObject(c2)
                })
            })
            .Should().BeTrue();

            // Assert all theorems
            allTheorems.ToSet().SetEquals(newTheorems).Should().BeTrue();
        }
    }
}