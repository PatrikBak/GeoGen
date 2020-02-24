using FluentAssertions;
using GeoGen.Constructor;
using GeoGen.Core;
using GeoGen.Infrastructure;
using GeoGen.TheoremFinder;
using GeoGen.TheoremSimplifier;
using GeoGen.Utilities;
using Ninject;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using static GeoGen.Core.ComposedConstructions;
using static GeoGen.Core.ConfigurationObjectType;
using static GeoGen.Core.LooseObjectLayout;
using static GeoGen.Core.PredefinedConstructions;
using static GeoGen.Core.TheoremType;

namespace GeoGen.TheoremSimplifierTests
{
    /// <summary>
    /// The test class for <see cref="TheoremSimplifier.TheoremSimplifier"/>.
    /// </summary>
    [TestFixture]
    public class TheoremSimplifierTest
    {
        #region Simplify method

        /// <summary>
        /// Performs the simplification algorithm.
        /// </summary>
        /// <param name="configuration">The configuration where the theorem to be simplified holds.</param>
        /// <param name="theorem">The theorem to be simplified.</param>
        /// <returns>The result of the simplification.</returns>
        private static (Configuration newConfiguration, Theorem newTheorem)? Simplify(Configuration configuration, Theorem theorem)
        {
            #region Prepare simplification rules

            // Prepare some loose points used in templates
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var D = new LooseConfigurationObject(Point);

            // Prepare the data
            var data = new TheoremSimplifierData(new ReadOnlyHashSet<SimplificationRule>(new HashSet<SimplificationRule>
            {
                // Median simplification with midpoint
                new SimplificationRule
                (
                    simplifableObject: new LineTheoremObject(A, new ConstructedConfigurationObject(Midpoint, B, C)),
                    simplifiedObject: new LineTheoremObject(new ConstructedConfigurationObject(Median, A, B, C)),
                    assumptions: Enumerable.Empty<Theorem>().ToReadOnlyHashSet()
                ),

                // Internal angle bisector simplification with incenter
                new SimplificationRule
                (
                    simplifableObject: new LineTheoremObject(A, new ConstructedConfigurationObject(Incenter, A, B, C)),
                    simplifiedObject: new LineTheoremObject(new ConstructedConfigurationObject(InternalAngleBisector, A, B, C)),
                    assumptions: Enumerable.Empty<Theorem>().ToReadOnlyHashSet()
                ),

                // Nine-point circle simplification
                new SimplificationRule
                (
                    simplifableObject: new CircleTheoremObject
                    (
                        new ConstructedConfigurationObject(Midpoint, A, B),
                        new ConstructedConfigurationObject(Midpoint, B, C),
                        new ConstructedConfigurationObject(Midpoint, C, A)
                    ),
                    simplifiedObject: new CircleTheoremObject(new ConstructedConfigurationObject(NinePointCircle, A, B, C)),
                    assumptions: Enumerable.Empty<Theorem>().ToReadOnlyHashSet()
                ),

                // Perpendicular bisector
                new SimplificationRule
                (
                    simplifableObject: new LineTheoremObject(A, B),
                    simplifiedObject: new LineTheoremObject(new ConstructedConfigurationObject(PerpendicularBisector, C, D)),
                    assumptions: new[]
                    {
                        new Theorem(EqualLineSegments, new[]
                        {
                            new LineSegmentTheoremObject(A, C),
                            new LineSegmentTheoremObject(A, D)
                        }),
                        new Theorem(EqualLineSegments, new[]
                        {
                            new LineSegmentTheoremObject(B, C),
                            new LineSegmentTheoremObject(B, D)
                        })
                    }
                    .ToReadOnlyHashSet()
                )
            }));

            #endregion

            #region Run algorithm

            // Initialize IoC
            var kernel = IoC.CreateKernel()
                // Add the theorem finder with no restrictions
                .AddTheoremFinder(new TheoremFindingSettings
                                  (
                                     // Look for any type except for EqualObjects
                                     soughtTheoremTypes: typeof(TheoremType).GetEnumValues().Cast<TheoremType>().Except(new[] { EqualObjects }).ToArray(),

                                     // Don't exclude tangencies
                                     new TangentCirclesTheoremFinderSettings(excludeTangencyInsidePicture: false),
                                     new LineTangentToCircleTheoremFinderSettings(excludeTangencyInsidePicture: false)
                                  ))
                // Add constructor
                .AddConstructor()
                // Add theorem simplifier with the data
                .AddTheoremSimplifier(data);

            // Create the constructor
            var constructor = kernel.Get<IGeometryConstructor>();

            // Draw the examined configuration
            var pictures = constructor.Construct(configuration, numberOfPictures: 5).pictures;

            // Draw the contextual picture
            var contextualPicture = new ContextualPicture(pictures);

            // Find the theorems
            var allTheorems = kernel.Get<ITheoremFinder>().FindAllTheorems(contextualPicture);

            // Run the algorithm
            return kernel.Get<ITheoremSimplifier>().Simplify(configuration, theorem, allTheorems);

            #endregion
        }

        #endregion

        [Test]
        public void Test_Nine_Point_Circle()
        {
            // Create the objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var P = new ConstructedConfigurationObject(Midpoint, A, B);
            var Q = new ConstructedConfigurationObject(Midpoint, B, C);
            var R = new ConstructedConfigurationObject(Midpoint, C, A);
            var incircle = new ConstructedConfigurationObject(Incircle, A, B, C);

            // Create the configuration
            var configuration = Configuration.DeriveFromObjects(Triangle, incircle, P, Q, R);

            // Create the theorem
            var theorem = new Theorem(TangentCircles, new[]
            {
                new CircleTheoremObject(incircle),
                new CircleTheoremObject(P, Q, R)
            });

            // Let it be simplified
            var result = Simplify(configuration, theorem);

            // Assert there is some result
            result.Should().NotBeNull();

            // Create the new objects
            var ninePointCircle = new ConstructedConfigurationObject(NinePointCircle, A, B, C);

            // Create the new configuration
            var newConfiguration = Configuration.DeriveFromObjects(Triangle, ninePointCircle, incircle);

            // Create the new theorem
            var newTheorem = new Theorem(TangentCircles, new[]
            {
                new CircleTheoremObject(incircle),
                new CircleTheoremObject(ninePointCircle)
            });

            // Assert
            result.Value.newConfiguration.Should().Be(newConfiguration);
            result.Value.newTheorem.Should().Be(newTheorem);
        }

        [Test]
        public void Test_Median_Defined_Via_Midpoint()
        {
            // Create the objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var Bmedian = new ConstructedConfigurationObject(Median, B, A, C);
            var Cmedian = new ConstructedConfigurationObject(Median, C, A, B);
            var M = new ConstructedConfigurationObject(Midpoint, B, C);

            // Create the configuration
            var configuration = Configuration.DeriveFromObjects(Triangle, A, B, C, Bmedian, Cmedian, M);

            // Create the theorem
            var theorem = new Theorem(ConcurrentLines, new[]
            {
                new LineTheoremObject(Bmedian),
                new LineTheoremObject(Cmedian),
                new LineTheoremObject(A, M)
            });

            // Let it be simplified
            var result = Simplify(configuration, theorem);

            // Assert there is some result
            result.Should().NotBeNull();

            // Create the new objects
            var Amedian = new ConstructedConfigurationObject(Median, A, B, C);

            // Create the new configuration
            var newConfiguration = Configuration.DeriveFromObjects(Triangle, A, B, C, Amedian, Bmedian, Cmedian);

            // Create the new theorem
            var newTheorem = new Theorem(ConcurrentLines, new[]
            {
                new LineTheoremObject(Amedian),
                new LineTheoremObject(Bmedian),
                new LineTheoremObject(Cmedian)
            });

            // Assert
            result.Value.newConfiguration.Should().Be(newConfiguration);
            result.Value.newTheorem.Should().Be(newTheorem);
        }

        [Test]
        public void Test_Median_Defined_Via_Midpoint_And_Parallelogram_Point()
        {
            // Create the objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var Bmedian = new ConstructedConfigurationObject(Median, B, A, C);
            var Cmedian = new ConstructedConfigurationObject(Median, C, A, B);
            var M = new ConstructedConfigurationObject(Midpoint, B, C);
            var N = new ConstructedConfigurationObject(ParallelogramPoint, A, B, C);

            // Create the configuration
            var configuration = Configuration.DeriveFromObjects(Triangle, A, B, C, Bmedian, Cmedian, M, N);

            // Create the theorem
            var theorem = new Theorem(ConcurrentLines, new[]
            {
                new LineTheoremObject(Bmedian),
                new LineTheoremObject(Cmedian),
                new LineTheoremObject(M, N)
            });

            // Let it be simplified
            var result = Simplify(configuration, theorem);

            // Assert there is some result
            result.Should().NotBeNull();

            // Create the new objects
            var Nmedian = new ConstructedConfigurationObject(Median, N, B, C);

            // Create the new configuration
            var newConfiguration = Configuration.DeriveFromObjects(Triangle, A, B, C, Nmedian, Bmedian, Cmedian);

            // Create the new theorem
            var newTheorem = new Theorem(ConcurrentLines, new[]
            {
                new LineTheoremObject(Nmedian),
                new LineTheoremObject(Bmedian),
                new LineTheoremObject(Cmedian)
            });

            // Assert
            result.Value.newConfiguration.Should().Be(newConfiguration);
            result.Value.newTheorem.Should().Be(newTheorem);
        }

        [Test]
        public void Test_More_Medians_Defined_Via_Midpoints()
        {
            // Create the objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var P = new ConstructedConfigurationObject(Midpoint, A, B);
            var Q = new ConstructedConfigurationObject(Midpoint, B, C);
            var R = new ConstructedConfigurationObject(Midpoint, C, A);

            // Create the configuration
            var configuration = Configuration.DeriveFromObjects(Triangle, A, B, C, P, Q, R);

            // Create the theorem
            var theorem = new Theorem(ConcurrentLines, new[]
            {
                new LineTheoremObject(C, P),
                new LineTheoremObject(A, Q),
                new LineTheoremObject(B, R)
            });

            // Let it be simplified
            var result = Simplify(configuration, theorem);

            // Assert there is some result
            result.Should().NotBeNull();

            // Create the new objects
            var Amedian = new ConstructedConfigurationObject(Median, A, B, C);
            var Bmedian = new ConstructedConfigurationObject(Median, B, A, C);
            var Cmedian = new ConstructedConfigurationObject(Median, C, A, B);

            // Create the new configuration
            var newConfiguration = Configuration.DeriveFromObjects(Triangle, A, B, C, Amedian, Bmedian, Cmedian);

            // Create the new theorem
            var newTheorem = new Theorem(ConcurrentLines, new[]
            {
                new LineTheoremObject(Amedian),
                new LineTheoremObject(Bmedian),
                new LineTheoremObject(Cmedian)
            });

            // Assert
            result.Value.newConfiguration.Should().Be(newConfiguration);
            result.Value.newTheorem.Should().Be(newTheorem);
        }

        [Test]
        public void Test_Replacing_Perpendicular_Bisector()
        {
            // Create the objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var O = new ConstructedConfigurationObject(Circumcenter, A, B, C);
            var M = new ConstructedConfigurationObject(MidpointOfOppositeArc, A, B, C);
            var Abisector = new ConstructedConfigurationObject(InternalAngleBisector, A, B, C);

            // Create the configuration
            var configuration = Configuration.DeriveFromObjects(Triangle, A, B, C, O, M, Abisector);

            // Create the theorem
            var theorem = new Theorem(ConcurrentObjects, new TheoremObject[]
            {
                new LineTheoremObject(Abisector),
                new CircleTheoremObject(A, B, C),
                new LineTheoremObject(O, M)
            });

            // Let it be simplified
            var result = Simplify(configuration, theorem);

            // Assert there is some result
            result.Should().NotBeNull();

            // Create the new objects
            var BCbisector = new ConstructedConfigurationObject(PerpendicularBisector, B, C);

            // Create the new configuration
            var newConfiguration = Configuration.DeriveFromObjects(Triangle, A, B, C, Abisector, BCbisector);

            // Create the new theorem
            var newTheorem = new Theorem(ConcurrentObjects, new TheoremObject[]
            {
                new LineTheoremObject(Abisector),
                new CircleTheoremObject(A, B, C),
                new LineTheoremObject(BCbisector)
            });

            // Assert
            result.Value.newConfiguration.Should().Be(newConfiguration);
            result.Value.newTheorem.Should().Be(newTheorem);
        }

        [Test]
        public void Test_That_Simplification_Does_Not_Happen_When_It_Would_Add_More_Objects()
        {
            // Create the objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var D = new ConstructedConfigurationObject(Incenter, A, B, C);
            var E = new ConstructedConfigurationObject(PerpendicularProjectionOnLineFromPoints, D, B, C);
            var F = new ConstructedConfigurationObject(PerpendicularProjectionOnLineFromPoints, D, A, C);
            var G = new ConstructedConfigurationObject(PerpendicularProjectionOnLineFromPoints, D, A, B);
            var l = new ConstructedConfigurationObject(ParallelLineToLineFromPoints, A, E, F);

            // Create the configuration
            var configuration = Configuration.DeriveFromObjects(Triangle, A, B, C, l);

            // Create the theorem
            var theorem = new Theorem(ConcurrentLines, new[]
            {
                new LineTheoremObject(C, D),
                new LineTheoremObject(E, G),
                new LineTheoremObject(l)
            });

            // Let it be simplified. Theoretically speaking, CD is the angle bisector and it could be applied,
            // but neither C nor D could be removed from the configuration, i.e. we would have all the objects 
            // + angle bisector after applying the rule. We don't want that
            var result = Simplify(configuration, theorem);

            // Assert it couldn't be done
            result.Should().BeNull();
        }
    }
}