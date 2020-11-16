using FluentAssertions;
using GeoGen.Constructor;
using GeoGen.Core;
using GeoGen.Infrastructure;
using GeoGen.Utilities;
using Ninject;
using NUnit.Framework;
using System.Linq;
using static GeoGen.Core.ComposedConstructions;
using static GeoGen.Core.ConfigurationObjectType;
using static GeoGen.Core.LooseObjectLayout;
using static GeoGen.Core.PredefinedConstructions;
using static GeoGen.Core.TheoremType;

namespace GeoGen.TheoremFinder.Tests
{
    /// <summary>
    /// The test class for <see cref="TheoremFinder"/>.
    /// </summary>
    [TestFixture]
    public class TheoremFinderTest
    {
        #region Running algorithm

        /// <summary>
        /// Runs the algorithm on the configuration to find old theorems (i.e. those who are true in the configuration
        /// with the last object removed), the new theorems (i.e. those that use the last object), the invalidated old
        /// theorems (see <see cref="ITheoremFinder.FindNewTheorems(ContextualPicture, TheoremMap, out Theorem[])"/>,
        /// and all theorems find via <see cref="ITheoremFinder.FindAllTheorems(ContextualPicture)"/>.
        /// </summary>
        /// <param name="configuration">The configuration where we're looking for theorems.</param>
        /// <returns>The old, new, invalidated all, and all theorems.</returns>
        private static (TheoremMap oldTheorems, TheoremMap newTheorems, Theorem[] invalidOldTheorems, TheoremMap allTheorems) FindTheorems(Configuration configuration)
        {
            // Prepare the kernel
            var kernel = NinjectUtilities.CreateKernel()
                // With constructor
                .AddConstructor()
                // Look for only some types
                .AddTheoremFinder(new TheoremFindingSettings(soughtTheoremTypes: new[]
                                  {
                                      ParallelLines,
                                      PerpendicularLines,
                                      EqualLineSegments,
                                      TangentCircles,
                                      LineTangentToCircle,
                                      Incidence,
                                      ConcurrentLines
                                  },
                                  // No exclusion of inside-picture tangencies
                                  tangentCirclesTheoremFinderSettings: new TangentCirclesTheoremFinderSettings(excludeTangencyInsidePicture: false),
                                  // Exclusion of inside-picture tangencies
                                  lineTangentToCircleTheoremFinderSettings: new LineTangentToCircleTheoremFinderSettings(excludeTangencyInsidePicture: true)));

            // Create the finder
            var finder = kernel.Get<ITheoremFinder>();

            // Create the old configuration, so we can find its theorems
            var oldConfiguration = new Configuration(configuration.LooseObjectsHolder,
                // We just don't want to include the last object
                configuration.ConstructedObjects.Except(new[] { configuration.LastConstructedObject }).ToList());

            // Construct the old configuration
            var oldPictures = kernel.Get<IGeometryConstructor>().ConstructWithUniformLayout(oldConfiguration, numberOfPictures: 5).pictures;

            // Construct the old contextual picture
            var oldContextualPicture = new ContextualPicture(oldPictures);

            // Finally get the old theorems
            var oldTheorems = finder.FindAllTheorems(oldContextualPicture);

            // Create the pictures for the current configuration
            var pictures = kernel.Get<IGeometryConstructor>().ConstructWithUniformLayout(configuration, numberOfPictures: 5).pictures;

            // Create the contextual picture for the current configuration
            var contextualPicture = new ContextualPicture(pictures);

            // Run both algorithms
            var newTheorems = finder.FindNewTheorems(contextualPicture, oldTheorems, out var invalidOldTheorems);
            var allTheorems = finder.FindAllTheorems(contextualPicture);

            // Return everything
            return (oldTheorems, newTheorems, invalidOldTheorems, allTheorems);
        }

        #endregion

        [Test]
        public void Test_Orthocenter_And_Intersection()
        {
            // Create the objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var H = new ConstructedConfigurationObject(Orthocenter, A, B, C);
            var D = new ConstructedConfigurationObject(IntersectionOfLinesFromPoints, B, C, A, H);

            // Create the configuration
            var configuration = Configuration.DeriveFromObjects(Triangle, H, D);

            // Run
            var (oldTheorems, newTheorems, invalidOldTheorems, allTheorems) = FindTheorems(configuration);

            // Assert that old + new - invalidated = all
            oldTheorems.AllObjects.Concat(newTheorems.AllObjects).Except(invalidOldTheorems).OrderlessEquals(allTheorems.AllObjects).Should().BeTrue();

            // Assert new theorems
            newTheorems.AllObjects.OrderlessEquals(new[]
            {
                new Theorem(PerpendicularLines, new[]
                {
                    new LineTheoremObject(D, H),
                    new LineTheoremObject(D, B)
                }),
                new Theorem(PerpendicularLines, new[]
                {
                    new LineTheoremObject(D, H),
                    new LineTheoremObject(D, C)
                }),
                new Theorem(PerpendicularLines, new[]
                {
                    new LineTheoremObject(D, H),
                    new LineTheoremObject(B, C)
                }),
                new Theorem(PerpendicularLines, new[]
                {
                    new LineTheoremObject(D, A),
                    new LineTheoremObject(D, B)
                }),
                new Theorem(PerpendicularLines, new[]
                {
                    new LineTheoremObject(D, A),
                    new LineTheoremObject(D, C)
                }),
                new Theorem(PerpendicularLines, new[]
                {
                    new LineTheoremObject(D, A),
                    new LineTheoremObject(B, C)
                }),
                new Theorem(PerpendicularLines, new[]
                {
                    new LineTheoremObject(A, H),
                    new LineTheoremObject(D, B)
                }),
                new Theorem(PerpendicularLines, new[]
                {
                    new LineTheoremObject(A, H),
                    new LineTheoremObject(D, C)
                })
            })
            .Should().BeTrue();

            // Assert all theorems
            allTheorems.AllObjects.OrderlessEquals(new[]
            {
                new Theorem(PerpendicularLines, new[]
                {
                    new LineTheoremObject(D, H),
                    new LineTheoremObject(D, B)
                }),
                new Theorem(PerpendicularLines, new[]
                {
                    new LineTheoremObject(D, H),
                    new LineTheoremObject(D, C)
                }),
                new Theorem(PerpendicularLines, new[]
                {
                    new LineTheoremObject(D, H),
                    new LineTheoremObject(B, C)
                }),
                new Theorem(PerpendicularLines, new[]
                {
                    new LineTheoremObject(D, A),
                    new LineTheoremObject(D, B)
                }),
                new Theorem(PerpendicularLines, new[]
                {
                    new LineTheoremObject(D, A),
                    new LineTheoremObject(D, C)
                }),
                new Theorem(PerpendicularLines, new[]
                {
                    new LineTheoremObject(D, A),
                    new LineTheoremObject(B, C)
                }),
                new Theorem(PerpendicularLines, new[]
                {
                    new LineTheoremObject(A, H),
                    new LineTheoremObject(D, B)
                }),
                new Theorem(PerpendicularLines, new[]
                {
                    new LineTheoremObject(A, H),
                    new LineTheoremObject(D, C)
                }),
                new Theorem(PerpendicularLines, new[]
                {
                    new LineTheoremObject(H, A),
                    new LineTheoremObject(B, C)
                }),
                new Theorem(PerpendicularLines, new[]
                {
                    new LineTheoremObject(H, B),
                    new LineTheoremObject(A, C)
                }),
                new Theorem(PerpendicularLines, new[]
                {
                    new LineTheoremObject(H, C),
                    new LineTheoremObject(B, A)
                })
            })
            .Should().BeTrue();
        }

        [Test]
        public void Test_Triangle_With_Midpoints_And_Explicit_Line_At_The_End()
        {
            // Create the objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var D = new ConstructedConfigurationObject(Midpoint, A, B);
            var E = new ConstructedConfigurationObject(Midpoint, A, C);
            var F = new ConstructedConfigurationObject(Midpoint, D, E);
            var l = new ConstructedConfigurationObject(LineFromPoints, B, C);

            // Create the configuration
            var configuration = Configuration.DeriveFromObjects(Triangle, D, E, F, l);

            // Run
            var (oldTheorems, newTheorems, invalidOldTheorems, allTheorems) = FindTheorems(configuration);

            // Assert that old + new - invalidated = all
            oldTheorems.AllObjects.Concat(newTheorems.AllObjects).Except(invalidOldTheorems).OrderlessEquals(allTheorems.AllObjects).Should().BeTrue();

            // Assert new theorems
            newTheorems.AllObjects.OrderlessEquals(new[]
            {
                new Theorem(ParallelLines, new[]
                {
                    new LineTheoremObject(l),
                    new LineTheoremObject(D, E)
                }),
                new Theorem(ParallelLines, new[]
                {
                    new LineTheoremObject(l),
                    new LineTheoremObject(D, F)
                }),
                new Theorem(ParallelLines, new[]
                {
                    new LineTheoremObject(l),
                    new LineTheoremObject(E, F)
                }),
                new Theorem(Incidence, B, l),
                new Theorem(Incidence, C, l)
            })
            .Should().BeTrue();

            // Assert all theorems
            allTheorems.AllObjects.OrderlessEquals(new[]
            {
                new Theorem(ConcurrentLines, new[]
                {
                    new LineTheoremObject(B, E),
                    new LineTheoremObject(C, D),
                    new LineTheoremObject(A, F)
                }),
                new Theorem(ParallelLines, new[]
                {
                    new LineTheoremObject(l),
                    new LineTheoremObject(D, E)
                }),
                new Theorem(ParallelLines, new[]
                {
                    new LineTheoremObject(l),
                    new LineTheoremObject(D, F)
                }),
                new Theorem(ParallelLines, new[]
                {
                    new LineTheoremObject(l),
                    new LineTheoremObject(E, F)
                }),
                new Theorem(ParallelLines, new[]
                {
                    new LineTheoremObject(B, C),
                    new LineTheoremObject(D, E)
                }),
                new Theorem(ParallelLines, new[]
                {
                    new LineTheoremObject(B, C),
                    new LineTheoremObject(D, F)
                }),
                new Theorem(ParallelLines, new[]
                {
                    new LineTheoremObject(B, C),
                    new LineTheoremObject(E, F)
                }),
                new Theorem(TangentCircles, new[]
                {
                    new CircleTheoremObject(A, D, E),
                    new CircleTheoremObject(A, B, C)
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
                }),
                new Theorem(EqualLineSegments, new[]
                {
                    new LineSegmentTheoremObject(D, F),
                    new LineSegmentTheoremObject(E, F)
                }),
                new Theorem(Incidence, B, l),
                new Theorem(Incidence, C, l)
            })
            .Should().BeTrue();
        }

        [Test]
        public void Test_Triangle_With_Midpoints_And_Point_At_The_End()
        {
            // Create the objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var D = new ConstructedConfigurationObject(Midpoint, A, B);
            var E = new ConstructedConfigurationObject(Midpoint, A, C);
            var b = new ConstructedConfigurationObject(InternalAngleBisector, A, B, C);
            var P = new ConstructedConfigurationObject(IntersectionOfLineAndLineFromPoints, b, B, C);
            var l = new ConstructedConfigurationObject(LineFromPoints, B, C);
            var F = new ConstructedConfigurationObject(Midpoint, D, E);

            // Create the configuration
            var configuration = Configuration.DeriveFromObjects(Triangle, D, E, P, l, F);

            // Run
            var (oldTheorems, newTheorems, invalidOldTheorems, allTheorems) = FindTheorems(configuration);

            // Assert that old + new - invalidated = all
            oldTheorems.AllObjects.Concat(newTheorems.AllObjects).Except(invalidOldTheorems).OrderlessEquals(allTheorems.AllObjects).Should().BeTrue();

            // Assert new theorems
            newTheorems.AllObjects.OrderlessEquals(new[]
            {
                new Theorem(ConcurrentLines, new[]
                {
                    new LineTheoremObject(B, E),
                    new LineTheoremObject(C, D),
                    new LineTheoremObject(A, F)
                }),
                new Theorem(ParallelLines, new[]
                {
                    new LineTheoremObject(F, D),
                    new LineTheoremObject(B, C)
                }),
                new Theorem(ParallelLines, new[]
                {
                    new LineTheoremObject(F, E),
                    new LineTheoremObject(B, C)
                }),
                new Theorem(ParallelLines, new[]
                {
                    new LineTheoremObject(F, D),
                    new LineTheoremObject(B, P)
                }),
                new Theorem(ParallelLines, new[]
                {
                    new LineTheoremObject(F, E),
                    new LineTheoremObject(B, P)
                }),
                new Theorem(ParallelLines, new[]
                {
                    new LineTheoremObject(F, D),
                    new LineTheoremObject(C, P)
                }),
                new Theorem(ParallelLines, new[]
                {
                    new LineTheoremObject(F, E),
                    new LineTheoremObject(C, P)
                }),
                new Theorem(ParallelLines, new[]
                {
                    new LineTheoremObject(F, D),
                    new LineTheoremObject(l)
                }),
                new Theorem(ParallelLines, new[]
                {
                    new LineTheoremObject(F, E),
                    new LineTheoremObject(l)
                }),
                new Theorem(EqualLineSegments, new[]
                {
                    new LineSegmentTheoremObject(D, F),
                    new LineSegmentTheoremObject(E, F)
                })
            })
            .Should().BeTrue();

            // Assert all theorems
            allTheorems.AllObjects.OrderlessEquals(new[]
            {
                new Theorem(ConcurrentLines, new[]
                {
                    new LineTheoremObject(B, E),
                    new LineTheoremObject(C, D),
                    new LineTheoremObject(A, F)
                }),
                new Theorem(ParallelLines, new[]
                {
                    new LineTheoremObject(F, D),
                    new LineTheoremObject(B, C)
                }),
                new Theorem(ParallelLines, new[]
                {
                    new LineTheoremObject(F, E),
                    new LineTheoremObject(B, C)
                }),
                new Theorem(ParallelLines, new[]
                {
                    new LineTheoremObject(D, E),
                    new LineTheoremObject(B, C)
                }),
                new Theorem(ParallelLines, new[]
                {
                    new LineTheoremObject(F, D),
                    new LineTheoremObject(B, P)
                }),
                new Theorem(ParallelLines, new[]
                {
                    new LineTheoremObject(F, E),
                    new LineTheoremObject(B, P)
                }),
                new Theorem(ParallelLines, new[]
                {
                    new LineTheoremObject(D, E),
                    new LineTheoremObject(B, P)
                }),
                new Theorem(ParallelLines, new[]
                {
                    new LineTheoremObject(F, D),
                    new LineTheoremObject(C, P)
                }),
                new Theorem(ParallelLines, new[]
                {
                    new LineTheoremObject(F, E),
                    new LineTheoremObject(C, P)
                }),
                new Theorem(ParallelLines, new[]
                {
                    new LineTheoremObject(D, E),
                    new LineTheoremObject(C, P)
                }),
                new Theorem(ParallelLines, new[]
                {
                    new LineTheoremObject(F, D),
                    new LineTheoremObject(l)
                }),
                new Theorem(ParallelLines, new[]
                {
                    new LineTheoremObject(F, E),
                    new LineTheoremObject(l)
                }),
                new Theorem(ParallelLines, new[]
                {
                    new LineTheoremObject(D, E),
                    new LineTheoremObject(l)
                }),
                new Theorem(TangentCircles, new[]
                {
                    new CircleTheoremObject(A, D, E),
                    new CircleTheoremObject(A, B, C)
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
                }),
                new Theorem(EqualLineSegments, new[]
                {
                    new LineSegmentTheoremObject(D, F),
                    new LineSegmentTheoremObject(E, F)
                }),
                new Theorem(Incidence, B, l),
                new Theorem(Incidence, C, l),
                new Theorem(Incidence, P, l),
                new Theorem(Incidence, A, b),
                new Theorem(Incidence, P, b)
            })
            .Should().BeTrue();
        }

        [Test]
        public void Test_Triangle_With_Tangent_Circles_And_Explicit_One_At_The_End()
        {
            // Create the objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var D = new ConstructedConfigurationObject(Midpoint, A, B);
            var O = new ConstructedConfigurationObject(Circumcenter, A, B, C);
            var P = new ConstructedConfigurationObject(PointReflection, A, O);
            var c = new ConstructedConfigurationObject(Circumcircle, P, B, C);

            // Create the configuration
            var configuration = Configuration.DeriveFromObjects(Triangle, D, O, P, c);

            // Run
            var (oldTheorems, newTheorems, invalidOldTheorems, allTheorems) = FindTheorems(configuration);

            // Assert that old + new - invalidated = all
            oldTheorems.AllObjects.Concat(newTheorems.AllObjects).Except(invalidOldTheorems).OrderlessEquals(allTheorems.AllObjects).Should().BeTrue();

            // Assert new theorems
            newTheorems.AllObjects.OrderlessEquals(new[]
            {
                new Theorem(TangentCircles, new[]
                {
                    new CircleTheoremObject(A, D, O),
                    new CircleTheoremObject(c)
                }),
                new Theorem(TangentCircles, new[]
                {
                    new CircleTheoremObject(B, D, O),
                    new CircleTheoremObject(c)
                }),
                new Theorem(Incidence, A, c),
                new Theorem(Incidence, B, c),
                new Theorem(Incidence, C, c),
                new Theorem(Incidence, P, c)
            })
            .Should().BeTrue();

            // Assert all theorems
            allTheorems.AllObjects.OrderlessEquals(new[]
            {
                new Theorem(ParallelLines, new[]
                {
                    new LineTheoremObject(B, P),
                    new LineTheoremObject(O, D)
                }),
                new Theorem(EqualLineSegments, new[]
                {
                    new LineSegmentTheoremObject(A, D),
                    new LineSegmentTheoremObject(D, B)
                }),
                new Theorem(EqualLineSegments, new[]
                {
                    new LineSegmentTheoremObject(O, A),
                    new LineSegmentTheoremObject(O, B)
                }),
                new Theorem(EqualLineSegments, new[]
                {
                    new LineSegmentTheoremObject(O, A),
                    new LineSegmentTheoremObject(O, C)
                }),
                new Theorem(EqualLineSegments, new[]
                {
                    new LineSegmentTheoremObject(O, A),
                    new LineSegmentTheoremObject(O, P)
                }),
                new Theorem(EqualLineSegments, new[]
                {
                    new LineSegmentTheoremObject(O, B),
                    new LineSegmentTheoremObject(O, C)
                }),
                new Theorem(EqualLineSegments, new[]
                {
                    new LineSegmentTheoremObject(O, B),
                    new LineSegmentTheoremObject(O, P)
                }),
                new Theorem(EqualLineSegments, new[]
                {
                    new LineSegmentTheoremObject(O, C),
                    new LineSegmentTheoremObject(O, P)
                }),
                new Theorem(TangentCircles, new[]
                {
                    new CircleTheoremObject(A, D, O),
                    new CircleTheoremObject(A, B, C)
                }),
                new Theorem(TangentCircles, new[]
                {
                    new CircleTheoremObject(A, D, O),
                    new CircleTheoremObject(A, B, P)
                }),
                new Theorem(TangentCircles, new[]
                {
                    new CircleTheoremObject(A, D, O),
                    new CircleTheoremObject(A, C, P)
                }),
                new Theorem(TangentCircles, new[]
                {
                    new CircleTheoremObject(A, D, O),
                    new CircleTheoremObject(B, C, P)
                }),
                new Theorem(TangentCircles, new[]
                {
                    new CircleTheoremObject(A, D, O),
                    new CircleTheoremObject(c)
                }),
                new Theorem(TangentCircles, new[]
                {
                    new CircleTheoremObject(B, D, O),
                    new CircleTheoremObject(A, B, C)
                }),
                new Theorem(TangentCircles, new[]
                {
                    new CircleTheoremObject(B, D, O),
                    new CircleTheoremObject(A, B, P)
                }),
                new Theorem(TangentCircles, new[]
                {
                    new CircleTheoremObject(B, D, O),
                    new CircleTheoremObject(A, C, P)
                }),
                new Theorem(TangentCircles, new[]
                {
                    new CircleTheoremObject(B, D, O),
                    new CircleTheoremObject(B, C, P)
                }),
                new Theorem(TangentCircles, new[]
                {
                    new CircleTheoremObject(B, D, O),
                    new CircleTheoremObject(c)
                }),
                new Theorem(Incidence, A, c),
                new Theorem(Incidence, B, c),
                new Theorem(Incidence, C, c),
                new Theorem(Incidence, P, c),
                new Theorem(PerpendicularLines, new[]
                {
                    new LineTheoremObject(O, D),
                    new LineTheoremObject(B, A)
                }),
                new Theorem(PerpendicularLines, new[]
                {
                    new LineTheoremObject(O, D),
                    new LineTheoremObject(B, D)
                }),
                new Theorem(PerpendicularLines, new[]
                {
                    new LineTheoremObject(O, D),
                    new LineTheoremObject(A, D)
                }),
                new Theorem(PerpendicularLines, new[]
                {
                    new LineTheoremObject(P, B),
                    new LineTheoremObject(B, A)
                }),
                new Theorem(PerpendicularLines, new[]
                {
                    new LineTheoremObject(P, B),
                    new LineTheoremObject(B, D)
                }),
                new Theorem(PerpendicularLines, new[]
                {
                    new LineTheoremObject(P, B),
                    new LineTheoremObject(A, D)
                }),
                new Theorem(PerpendicularLines, new[]
                {
                    new LineTheoremObject(P, C),
                    new LineTheoremObject(A, C)
                })
            })
            .Should().BeTrue();
        }

        [Test]
        public void Test_Triangle_With_Tangent_Circles_And_Point_At_The_End()
        {
            // Create the objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var D = new ConstructedConfigurationObject(Midpoint, A, B);
            var O = new ConstructedConfigurationObject(Circumcenter, A, B, C);
            var c = new ConstructedConfigurationObject(Circumcircle, A, B, C);
            var P = new ConstructedConfigurationObject(PointReflection, A, O);

            // Create the configuration
            var configuration = Configuration.DeriveFromObjects(Triangle, D, O, c, P);

            // Run
            var (oldTheorems, newTheorems, invalidOldTheorems, allTheorems) = FindTheorems(configuration);

            // Assert that old + new - invalidated = all
            oldTheorems.AllObjects.Concat(newTheorems.AllObjects).Except(invalidOldTheorems).OrderlessEquals(allTheorems.AllObjects).Should().BeTrue();

            // Assert new theorems
            newTheorems.AllObjects.OrderlessEquals(new[]
            {
                new Theorem(ParallelLines, new[]
                {
                    new LineTheoremObject(B, P),
                    new LineTheoremObject(O, D)
                }),
                new Theorem(EqualLineSegments, new[]
                {
                    new LineSegmentTheoremObject(O, A),
                    new LineSegmentTheoremObject(O, P)
                }),
                new Theorem(EqualLineSegments, new[]
                {
                    new LineSegmentTheoremObject(O, B),
                    new LineSegmentTheoremObject(O, P)
                }),
                new Theorem(EqualLineSegments, new[]
                {
                    new LineSegmentTheoremObject(O, C),
                    new LineSegmentTheoremObject(O, P)
                }),
                new Theorem(TangentCircles, new[]
                {
                    new CircleTheoremObject(A, D, O),
                    new CircleTheoremObject(A, B, P)
                }),
                new Theorem(TangentCircles, new[]
                {
                    new CircleTheoremObject(A, D, O),
                    new CircleTheoremObject(A, C, P)
                }),
                new Theorem(TangentCircles, new[]
                {
                    new CircleTheoremObject(A, D, O),
                    new CircleTheoremObject(B, C, P)
                }),
                new Theorem(TangentCircles, new[]
                {
                    new CircleTheoremObject(B, D, O),
                    new CircleTheoremObject(A, B, P)
                }),
                new Theorem(TangentCircles, new[]
                {
                    new CircleTheoremObject(B, D, O),
                    new CircleTheoremObject(A, C, P)
                }),
                new Theorem(TangentCircles, new[]
                {
                    new CircleTheoremObject(B, D, O),
                    new CircleTheoremObject(B, C, P)
                }),
                new Theorem(Incidence, P, c),
                new Theorem(PerpendicularLines, new[]
                {
                    new LineTheoremObject(P, B),
                    new LineTheoremObject(B, A)
                }),
                new Theorem(PerpendicularLines, new[]
                {
                    new LineTheoremObject(P, B),
                    new LineTheoremObject(B, D)
                }),
                new Theorem(PerpendicularLines, new[]
                {
                    new LineTheoremObject(P, B),
                    new LineTheoremObject(A, D)
                }),
                new Theorem(PerpendicularLines, new[]
                {
                    new LineTheoremObject(P, C),
                    new LineTheoremObject(A, C)
                })
            })
            .Should().BeTrue();

            // Assert all theorems
            allTheorems.AllObjects.OrderlessEquals(new[]
            {
                new Theorem(ParallelLines, new[]
                {
                    new LineTheoremObject(B, P),
                    new LineTheoremObject(O, D)
                }),
                new Theorem(EqualLineSegments, new[]
                {
                    new LineSegmentTheoremObject(A, D),
                    new LineSegmentTheoremObject(D, B)
                }),
                new Theorem(EqualLineSegments, new[]
                {
                    new LineSegmentTheoremObject(O, A),
                    new LineSegmentTheoremObject(O, B)
                }),
                new Theorem(EqualLineSegments, new[]
                {
                    new LineSegmentTheoremObject(O, A),
                    new LineSegmentTheoremObject(O, C)
                }),
                new Theorem(EqualLineSegments, new[]
                {
                    new LineSegmentTheoremObject(O, A),
                    new LineSegmentTheoremObject(O, P)
                }),
                new Theorem(EqualLineSegments, new[]
                {
                    new LineSegmentTheoremObject(O, B),
                    new LineSegmentTheoremObject(O, C)
                }),
                new Theorem(EqualLineSegments, new[]
                {
                    new LineSegmentTheoremObject(O, B),
                    new LineSegmentTheoremObject(O, P)
                }),
                new Theorem(EqualLineSegments, new[]
                {
                    new LineSegmentTheoremObject(O, C),
                    new LineSegmentTheoremObject(O, P)
                }),
                new Theorem(TangentCircles, new[]
                {
                    new CircleTheoremObject(A, D, O),
                    new CircleTheoremObject(A, B, C)
                }),
                new Theorem(TangentCircles, new[]
                {
                    new CircleTheoremObject(A, D, O),
                    new CircleTheoremObject(A, B, P)
                }),
                new Theorem(TangentCircles, new[]
                {
                    new CircleTheoremObject(A, D, O),
                    new CircleTheoremObject(A, C, P)
                }),
                new Theorem(TangentCircles, new[]
                {
                    new CircleTheoremObject(A, D, O),
                    new CircleTheoremObject(B, C, P)
                }),
                new Theorem(TangentCircles, new[]
                {
                    new CircleTheoremObject(A, D, O),
                    new CircleTheoremObject(c)
                }),
                new Theorem(TangentCircles, new[]
                {
                    new CircleTheoremObject(B, D, O),
                    new CircleTheoremObject(A, B, C)
                }),
                new Theorem(TangentCircles, new[]
                {
                    new CircleTheoremObject(B, D, O),
                    new CircleTheoremObject(A, B, P)
                }),
                new Theorem(TangentCircles, new[]
                {
                    new CircleTheoremObject(B, D, O),
                    new CircleTheoremObject(A, C, P)
                }),
                new Theorem(TangentCircles, new[]
                {
                    new CircleTheoremObject(B, D, O),
                    new CircleTheoremObject(B, C, P)
                }),
                new Theorem(TangentCircles, new[]
                {
                    new CircleTheoremObject(B, D, O),
                    new CircleTheoremObject(c)
                }),
                new Theorem(Incidence, A, c),
                new Theorem(Incidence, B, c),
                new Theorem(Incidence, C, c),
                new Theorem(Incidence, P, c),
                new Theorem(PerpendicularLines, new[]
                {
                    new LineTheoremObject(O, D),
                    new LineTheoremObject(B, A)
                }),
                new Theorem(PerpendicularLines, new[]
                {
                    new LineTheoremObject(O, D),
                    new LineTheoremObject(B, D)
                }),
                new Theorem(PerpendicularLines, new[]
                {
                    new LineTheoremObject(O, D),
                    new LineTheoremObject(A, D)
                }),
                new Theorem(PerpendicularLines, new[]
                {
                    new LineTheoremObject(P, B),
                    new LineTheoremObject(B, A)
                }),
                new Theorem(PerpendicularLines, new[]
                {
                    new LineTheoremObject(P, B),
                    new LineTheoremObject(B, D)
                }),
                new Theorem(PerpendicularLines, new[]
                {
                    new LineTheoremObject(P, B),
                    new LineTheoremObject(A, D)
                }),
                new Theorem(PerpendicularLines, new[]
                {
                    new LineTheoremObject(P, C),
                    new LineTheoremObject(A, C)
                })
            })
            .Should().BeTrue();
        }

        [Test]
        public void Test_That_Old_Theorem_Invalidation_Happens_With_Concurrent_Lines()
        {
            // Create the objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var D = new ConstructedConfigurationObject(Midpoint, B, C);
            var E = new ConstructedConfigurationObject(Midpoint, C, A);
            var F = new ConstructedConfigurationObject(Midpoint, A, B);
            var G = new ConstructedConfigurationObject(Centroid, A, B, C);

            // Create the configuration
            var configuration = Configuration.DeriveFromObjects(Triangle, D, E, F, G);

            // Run
            var (oldTheorems, newTheorems, invalidOldTheorems, allTheorems) = FindTheorems(configuration);

            // Assert that old + new - invalidated = all
            oldTheorems.AllObjects.Concat(newTheorems.AllObjects).Except(invalidOldTheorems).OrderlessEquals(allTheorems.AllObjects).Should().BeTrue();

            // Create the invalidated theorem
            var invalidatedTheorem = new Theorem(ConcurrentLines, new[]
            {
                new LineTheoremObject(A, D),
                new LineTheoremObject(B, E),
                new LineTheoremObject(C, F)
            });

            // Assert it is the only one
            invalidOldTheorems.OrderlessEquals(new[] { invalidatedTheorem });

            // Assert it is indeed old
            oldTheorems.AllObjects.Should().Contain(invalidatedTheorem);
        }

        [Test]
        public void Test_That_Old_Theorem_Invalidation_Happens_With_Line_Tangent_To_Circle()
        {
            // Create the objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var c = new ConstructedConfigurationObject(Incircle, A, B, C);
            var I = new ConstructedConfigurationObject(Incenter, A, B, C);
            var D = new ConstructedConfigurationObject(PerpendicularProjectionOnLineFromPoints, I, B, C);

            // Create the configuration
            var configuration = Configuration.DeriveFromObjects(Triangle, c, I, D);

            // Run
            var (oldTheorems, newTheorems, invalidOldTheorems, allTheorems) = FindTheorems(configuration);

            // Assert that old + new - invalidated = all
            oldTheorems.AllObjects.Concat(newTheorems.AllObjects).Except(invalidOldTheorems).OrderlessEquals(allTheorems.AllObjects).Should().BeTrue();

            // Create the invalidated theorem
            var invalidatedTheorem = new Theorem(LineTangentToCircle, new TheoremObject[]
            {
                new LineTheoremObject(B, C),
                new CircleTheoremObject(c)
            });

            // Assert it is the only one
            invalidOldTheorems.OrderlessEquals(new[] { invalidatedTheorem });

            // Assert it is indeed old
            oldTheorems.AllObjects.Should().Contain(invalidatedTheorem);
        }

        [Test]
        public void Test_That_Old_Theorem_Invalidation_Does_Not_Happen_With_Tangent_Circles()
        {
            // Create the objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var l = new ConstructedConfigurationObject(TangentLine, A, B, C);
            var D = new ConstructedConfigurationObject(PerpendicularProjection, B, l);
            var E = new ConstructedConfigurationObject(PerpendicularProjection, C, l);
            var F = new ConstructedConfigurationObject(ReflectionInLineFromPoints, D, B, C);
            var G = new ConstructedConfigurationObject(ReflectionInLineFromPoints, E, B, C);
            var P = new ConstructedConfigurationObject(IntersectionOfLineAndLineFromPoints, l, B, C);

            // Create the configuration
            var configuration = Configuration.DeriveFromObjects(Triangle, F, G, P);

            // Run
            var (oldTheorems, newTheorems, invalidOldTheorems, allTheorems) = FindTheorems(configuration);

            // Assert that old + new - invalidated = all
            oldTheorems.AllObjects.Concat(newTheorems.AllObjects).Except(invalidOldTheorems).OrderlessEquals(allTheorems.AllObjects).Should().BeTrue();

            // Assert only concurrencies have been invalidated
            invalidOldTheorems.OrderlessEquals(new[]
            {
                new Theorem(ConcurrentLines, new[]
                {
                    new LineTheoremObject(F, G),
                    new LineTheoremObject(B, C),
                    new LineTheoremObject(l)
                }),
                new Theorem(ConcurrentLines, new[]
                {
                    new LineTheoremObject(F, G),
                    new LineTheoremObject(B, C),
                    new LineTheoremObject(D, E)
                }),
                new Theorem(ConcurrentLines, new[]
                {
                    new LineTheoremObject(F, G),
                    new LineTheoremObject(B, C),
                    new LineTheoremObject(A, D)
                }),
                new Theorem(ConcurrentLines, new[]
                {
                    new LineTheoremObject(F, G),
                    new LineTheoremObject(B, C),
                    new LineTheoremObject(A, E)
                })
            })
            .Should().BeTrue();

            // Assert there is an old tangency
            oldTheorems.AllObjects.Should().Contain(new Theorem(TangentCircles, new[]
            {
                new CircleTheoremObject(B, D, F),
                new CircleTheoremObject(C, E, G)
            }));
        }
    }
}