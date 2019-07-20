using FluentAssertions;
using GeoGen.Constructor;
using GeoGen.Core;
using GeoGen.DependenciesResolver;
using GeoGen.Generator;
using GeoGen.Utilities;
using NUnit.Framework;
using static GeoGen.Core.ComposedConstructions;
using static GeoGen.Core.ConfigurationObjectType;
using static GeoGen.Core.PredefinedConstructionType;

namespace GeoGen.TheoremsAnalyzer.Test
{
    /// <summary>
    /// The test class for <see cref="SubtheoremAnalyzer"/>.
    /// </summary>
    [TestFixture]
    public class SubtheoremAnalyzerTest
    {
        #region Service instances

        /// <summary>
        /// The instance of the analyzer.
        /// </summary>
        private ISubtheoremAnalyzer _analyzer;

        /// <summary>
        /// The instance of the objects constructor.
        /// </summary>
        private IGeometryConstructor _constructor;

        #endregion

        #region SetUp

        [OneTimeSetUp]
        public void Initialize()
        {
            // Initialize IoC
            IoC.Kernel.AddGenerator().AddConstructor().AddTheoremsFinder();

            // Get the constructor
            _constructor = IoC.Get<IGeometryConstructor>(new PicturesManagerSettings
            {
                NumberOfPictures = 8,
                MaximalAttemptsToReconstructOnePicture = 100,
                MaximalAttemptsToReconstructAllPictures = 1000
            });

            // Create the analyzer
            _analyzer = new SubtheoremAnalyzer(_constructor);
        }

        #endregion

        #region Helper methods

        /// <summary>
        /// Runs the theorem analysis for a given template theorem and a given examined theorem.
        /// </summary>
        /// <param name="templateTheorem">The template theorem.</param>
        /// <param name="examinedTheorem">The examined theorem.</param>
        /// <returns>The output of the analysis.</returns>
        private SubtheoremAnalyzerOutput Run(Theorem templateTheorem, Theorem examinedTheorem)
        {
            // Draw the examined configuration
            var geometryData = _constructor.Construct(examinedTheorem.Configuration);

            // Make sure the configuration is okay
            if (geometryData.Manager == null)
                throw new GeoGenException("Incorrect configuration");

            // Draw the contextual picture
            var contextualPicture = IoC.Get<IContextualPictureFactory>(new ContextualPictureSettings
            {
                MaximalNumberOfAttemptsToReconstruct = 100
            })
            .Create(examinedTheorem.Configuration, geometryData.Manager);

            // Create the container
            var objectsContainer = IoC.Get<IConfigurationObjectsContainerFactory>().CreateContainer();

            // Fill it with objects of the examined configuration
            examinedTheorem.Configuration.ObjectsMap.AllObjects.ForEach(objectsContainer.Add);

            // Run the algorithm
            return _analyzer.Analyze(new SubtheoremAnalyzerInput
            {
                TemplateTheorem = templateTheorem,
                ExaminedTheorem = examinedTheorem,
                ExaminedConfigurationManager = geometryData.Manager,
                ExaminedConfigurationContexualPicture = contextualPicture,
                ExaminedConfigurationObjectsContainer = objectsContainer
            });
        }

        #endregion

        [Test]
        public void Test_Explicit_Midpoint_Is_Equally_Distanced_From_Segment_Endpoints()
        {
            // Create the template configuration's objects
            var A_ = new LooseConfigurationObject(Point);
            var B_ = new LooseConfigurationObject(Point);
            var M_ = new ConstructedConfigurationObject(Midpoint, A_, B_);

            // Create the template configuration
            var templateConfiguration = Configuration.DeriveFromObjects(M_);

            // Create the template theorem
            var templateTheorem = new Theorem(templateConfiguration, TheoremType.EqualLineSegments, A_, M_, B_, M_);

            // Create the examined configuration's objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var M = new ConstructedConfigurationObject(Midpoint, B, C);
            var N = new ConstructedConfigurationObject(Midpoint, A, M);

            // Create the examined configuration
            var examinedConfiguration = Configuration.DeriveFromObjects(LooseObjectsLayout.ScaleneAcuteAngledTriangled, N);

            // Create the examined theorem
            var examinedTheorem = new Theorem(examinedConfiguration, TheoremType.EqualLineSegments, N, A, N, M);

            // Analyze
            var result = Run(templateTheorem, examinedTheorem);

            // Assert
            result.IsSubtheorem.Should().BeTrue();
            result.UsedEqualities.Should().BeNullOrEmpty();
            result.UsedFacts.Should().BeNullOrEmpty();
        }

        [Test]
        public void Test_Implicit_Midpoint_Is_Equally_Distanced_From_Segment_Endpoints()
        {
            // Create the template configuration's objects
            var A_ = new LooseConfigurationObject(Point);
            var B_ = new LooseConfigurationObject(Point);
            var M_ = new ConstructedConfigurationObject(Midpoint, A_, B_);

            // Create the template configuration
            var templateConfiguration = Configuration.DeriveFromObjects(M_);

            // Create the template theorem
            var templateTheorem = new Theorem(templateConfiguration, TheoremType.EqualLineSegments, A_, M_, B_, M_);

            // Create the examined configuration's objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var O = new ConstructedConfigurationObject(Circumcenter, A, B, C);
            var M = new ConstructedConfigurationObject(PerpendicularProjectionOnLineFromPoints, O, B, C);

            // Create the examined configuration
            var examinedConfiguration = Configuration.DeriveFromObjects(LooseObjectsLayout.ScaleneAcuteAngledTriangled, M);

            // Create the examined theorem
            var examinedTheorem = new Theorem(examinedConfiguration, TheoremType.EqualLineSegments, M, B, M, C);

            // Analyze
            var result = Run(templateTheorem, examinedTheorem);

            // Assert
            result.IsSubtheorem.Should().BeTrue();
            result.UsedEqualities.Should().BeEquivalentTo(new (ConfigurationObject, ConfigurationObject)[]
            {
                // We need to discover M is the midpoint of BC
                (M, new ConstructedConfigurationObject(Midpoint, B, C))
            });
            result.UsedFacts.Should().BeNullOrEmpty();
        }

        [Test]
        public void Test_Equal_Angles_In_Situation_With_Explicit_Circumcenter()
        {
            // Create the template configuration's objects
            var A_ = new LooseConfigurationObject(Point);
            var B_ = new LooseConfigurationObject(Point);
            var C_ = new LooseConfigurationObject(Point);
            var O_ = new ConstructedConfigurationObject(Circumcenter, A_, B_, C_);

            // Create the template configuration
            var templateConfiguration = Configuration.DeriveFromObjects(O_);

            // Create the template theorem
            var templateTheorem = new Theorem(templateConfiguration, TheoremType.EqualAngles, new[]
            {
                new TheoremObjectWithPoints(Line, B_, C_),
                new TheoremObjectWithPoints(Line, B_, O_),
                new TheoremObjectWithPoints(Line, C_, O_),
                new TheoremObjectWithPoints(Line, B_, C_)
            });

            // Create the examined configuration's objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var D = new ConstructedConfigurationObject(Midpoint, A, B);
            var E = new ConstructedConfigurationObject(Midpoint, A, C);
            var O = new ConstructedConfigurationObject(Circumcenter, A, D, E);

            // Create the examined configuration
            var examinedConfiguration = Configuration.DeriveFromObjects(LooseObjectsLayout.ScaleneAcuteAngledTriangled, O);

            // Create the examined theorem
            var examinedTheorem = new Theorem(examinedConfiguration, TheoremType.EqualAngles, new[]
            {
                new TheoremObjectWithPoints(Line, A, D),
                new TheoremObjectWithPoints(Line, A, O),
                new TheoremObjectWithPoints(Line, D, O),
                new TheoremObjectWithPoints(Line, A, D)
            });

            // Analyze
            var result = Run(templateTheorem, examinedTheorem);

            // Assert
            result.IsSubtheorem.Should().BeTrue();
            result.UsedEqualities.Should().BeNullOrEmpty();
            result.UsedFacts.Should().BeNullOrEmpty();
        }

        [Test]
        public void Test_Equal_Angles_In_Situation_With_Implicit_Circumcenter()
        {
            // Create the template configuration's objects
            var A_ = new LooseConfigurationObject(Point);
            var B_ = new LooseConfigurationObject(Point);
            var C_ = new LooseConfigurationObject(Point);
            var O_ = new ConstructedConfigurationObject(Circumcenter, A_, B_, C_);

            // Create the template configuration
            var templateConfiguration = Configuration.DeriveFromObjects(O_);

            // Create the template theorem
            var templateTheorem = new Theorem(templateConfiguration, TheoremType.EqualAngles, new[]
            {
                new TheoremObjectWithPoints(Line, B_, C_),
                new TheoremObjectWithPoints(Line, B_, O_),
                new TheoremObjectWithPoints(Line, C_, O_),
                new TheoremObjectWithPoints(Line, B_, C_)
            });

            // Create the examined configuration's objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var D = new ConstructedConfigurationObject(Midpoint, A, B);
            var E = new ConstructedConfigurationObject(Midpoint, A, C);
            var o1 = new ConstructedConfigurationObject(PerpendicularBisector, A, D);
            var o2 = new ConstructedConfigurationObject(PerpendicularBisector, D, E);
            var O = new ConstructedConfigurationObject(IntersectionOfLines, o1, o2);

            // Create the examined configuration
            var examinedConfiguration = Configuration.DeriveFromObjects(LooseObjectsLayout.ScaleneAcuteAngledTriangled, O);

            // Create the examined theorem
            var examinedTheorem = new Theorem(examinedConfiguration, TheoremType.EqualAngles, new[]
            {
                new TheoremObjectWithPoints(Line, A, D, B),
                new TheoremObjectWithPoints(Line, A, O),
                new TheoremObjectWithPoints(Line, D, O),
                new TheoremObjectWithPoints(Line, A, B, D)
            });

            // Analyze
            var result = Run(templateTheorem, examinedTheorem);

            // Assert
            result.IsSubtheorem.Should().BeTrue();
            result.UsedEqualities.Should().BeEquivalentTo(new (ConfigurationObject, ConfigurationObject)[]
            {
                // We need to discover O is the circumcircle of ABC
                (O, new ConstructedConfigurationObject(Circumcenter, A, B, C))
            });
            result.UsedFacts.Should().BeNullOrEmpty();
        }

        [Test]
        public void Test_Medians_Are_Concurrent_With_Explicit_Definitions_Of_Midpoints()
        {
            // Create the template configuration's objects
            var A_ = new LooseConfigurationObject(Point);
            var B_ = new LooseConfigurationObject(Point);
            var C_ = new LooseConfigurationObject(Point);
            var D_ = new ConstructedConfigurationObject(Midpoint, B_, C_);
            var E_ = new ConstructedConfigurationObject(Midpoint, C_, A_);
            var F_ = new ConstructedConfigurationObject(Midpoint, A_, B_);

            // Create original configuration
            var templateConfiguration = Configuration.DeriveFromObjects(D_, E_, F_);

            // Create the template theorem
            var templateTheorem = new Theorem(templateConfiguration, TheoremType.ConcurrentObjects, new[]
            {
                new TheoremObjectWithPoints(Line, A_, D_),
                new TheoremObjectWithPoints(Line, B_, E_),
                new TheoremObjectWithPoints(Line, C_, F_)
            });

            // Create the examined configuration's objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var D = new ConstructedConfigurationObject(Midpoint, B, C);
            var E = new ConstructedConfigurationObject(Midpoint, A, C);
            var F = new ConstructedConfigurationObject(Midpoint, A, D);
            var G = new ConstructedConfigurationObject(Midpoint, D, C);

            // Create the examined configuration
            var examinedConfiguration = Configuration.DeriveFromObjects(LooseObjectsLayout.ScaleneAcuteAngledTriangled, E, F, G);

            // Create the examined theorem
            var examinedTheorem = new Theorem(examinedConfiguration, TheoremType.ConcurrentObjects, new[]
            {
                new TheoremObjectWithPoints(Line, A, G),
                new TheoremObjectWithPoints(Line, D, E),
                new TheoremObjectWithPoints(Line, C, F)
            });

            // Analyze
            var result = Run(templateTheorem, examinedTheorem);

            // Assert
            result.IsSubtheorem.Should().BeTrue();
            result.UsedEqualities.Should().BeNullOrEmpty();
            result.UsedFacts.Should().BeNullOrEmpty();
        }

        [Test]
        public void Test_Medians_Are_Concurrent_With_Implicit_Definitions_Of_Midpoints()
        {
            // Create the template configuration's objects
            var A_ = new LooseConfigurationObject(Point);
            var B_ = new LooseConfigurationObject(Point);
            var C_ = new LooseConfigurationObject(Point);
            var D_ = new ConstructedConfigurationObject(Midpoint, B_, C_);
            var E_ = new ConstructedConfigurationObject(Midpoint, C_, A_);
            var F_ = new ConstructedConfigurationObject(Midpoint, A_, B_);

            // Create the template configuration
            var templateConfiguration = Configuration.DeriveFromObjects(D_, E_, F_);

            // Create the template theorem
            var templateTheorem = new Theorem(templateConfiguration, TheoremType.ConcurrentObjects, new[]
            {
                new TheoremObjectWithPoints(Line, A_, D_),
                new TheoremObjectWithPoints(Line, B_, E_),
                new TheoremObjectWithPoints(Line, C_, F_)
            });

            // Create the examined configuration's objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var O = new ConstructedConfigurationObject(Circumcenter, A, B, C);
            var D = new ConstructedConfigurationObject(PerpendicularProjectionOnLineFromPoints, O, B, C);
            var E = new ConstructedConfigurationObject(PerpendicularProjectionOnLineFromPoints, O, C, A);
            var F = new ConstructedConfigurationObject(PerpendicularProjectionOnLineFromPoints, O, A, B);

            // Create the examined configuration
            var examinedConfiguration = Configuration.DeriveFromObjects(LooseObjectsLayout.ScaleneAcuteAngledTriangled, D, E, F);

            // Create the examined theorem
            var examinedTheorem = new Theorem(examinedConfiguration, TheoremType.ConcurrentObjects, new[]
            {
                new TheoremObjectWithPoints(Line, A, D),
                new TheoremObjectWithPoints(Line, B, E),
                new TheoremObjectWithPoints(Line, C, F)
            });

            // Analyze
            var result = Run(templateTheorem, examinedTheorem);

            // Assert
            result.IsSubtheorem.Should().BeTrue();
            result.UsedEqualities.Should().BeEquivalentTo(new (ConfigurationObject, ConfigurationObject)[]
            {
                // We need to discover the projections of O on lines of ABC are midpoints 
                (D, new ConstructedConfigurationObject(Midpoint, B, C)),
                (E, new ConstructedConfigurationObject(Midpoint, C, A)),
                (F, new ConstructedConfigurationObject(Midpoint, A, B))
            });
            result.UsedFacts.Should().BeNullOrEmpty();
        }

        [Test]
        public void Test_Reflection_Of_H_In_Midpoint_Of_BC_Is_Point_Opposite_To_A_On_Circumcircle_ABC()
        {
            // Create the template configuration's objects
            var A_ = new LooseConfigurationObject(Point);
            var M_ = new LooseConfigurationObject(Point);
            var B_ = new ConstructedConfigurationObject(PointReflection, A_, M_);

            // Create the template configuration
            var templateConfiguration = Configuration.DeriveFromObjects(B_);

            // Create the template theorem
            var templateTheorem = new Theorem(templateConfiguration, TheoremType.CollinearPoints, A_, B_, M_);

            // Create the examined configuration's objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var H = new ConstructedConfigurationObject(Orthocenter, A, B, C);
            var O = new ConstructedConfigurationObject(Circumcenter, A, B, C);
            var D = new ConstructedConfigurationObject(PointReflection, A, O);
            var M = new ConstructedConfigurationObject(Midpoint, B, C);

            // Create the examined configuration
            var examinedConfiguration = Configuration.DeriveFromObjects(LooseObjectsLayout.ScaleneAcuteAngledTriangled, M, D, H);

            // Create the examined theorem
            var examinedTheorem = new Theorem(examinedConfiguration, TheoremType.CollinearPoints, H, D, M);

            // Analyze
            var result = Run(templateTheorem, examinedTheorem);

            // Assert
            result.IsSubtheorem.Should().BeTrue();
            result.UsedFacts.Should().BeNullOrEmpty();
            result.UsedEqualities.Should().HaveCount(1, "Exactly one of the following two potential reasons should hold true");
            new[]
            {
                // We need to discover D is the reflection of O in M
                (D, new ConstructedConfigurationObject(PointReflection, H, M)),

                // Or the other way around
                (H, new ConstructedConfigurationObject(PointReflection, D, M))
            }
            .Should().ContainEquivalentOf(result.UsedEqualities[0]);

        }

        [Test]
        public void Test_Second_Intersection_Of_Two_Circumcircles_Creates_Concyclic_Points()
        {
            // Create the template configuration's objects
            var A_ = new LooseConfigurationObject(Point);
            var B_ = new LooseConfigurationObject(Point);
            var C_ = new LooseConfigurationObject(Point);
            var D_ = new LooseConfigurationObject(Point);
            var E_ = new LooseConfigurationObject(Point);
            var X_ = new ConstructedConfigurationObject(SecondIntersectionOfTwoCircumcircles, A_, B_, C_, D_, E_);

            // Create the template configuration
            var templateConfiguration = Configuration.DeriveFromObjects(X_);

            // Create the template theorem
            var templateTheorem = new Theorem(templateConfiguration, TheoremType.ConcyclicPoints, A_, B_, C_, X_);

            // Create the examined configuration's objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var D = new ConstructedConfigurationObject(Midpoint, B, C);
            var E = new ConstructedConfigurationObject(Midpoint, A, B);
            var X = new ConstructedConfigurationObject(SecondIntersectionOfTwoCircumcircles, A, E, D, B, C);

            // Create the examined configuration
            var examinedConfiguration = Configuration.DeriveFromObjects(LooseObjectsLayout.ScaleneAcuteAngledTriangled, X);

            // Create the examined theorem
            var examinedTheorem = new Theorem(examinedConfiguration, TheoremType.ConcyclicPoints, A, X, D, E);

            // Analyze
            var result = Run(templateTheorem, examinedTheorem);

            // Assert
            result.IsSubtheorem.Should().BeTrue();
            result.UsedEqualities.Should().BeEmpty();
            result.UsedFacts.Should().BeNullOrEmpty();
        }

        [Test]
        public void Test_Tangent_Circles_Because_Of_Homothety()
        {
            // Create the template configuration's objects
            var A_ = new LooseConfigurationObject(Point);
            var B_ = new LooseConfigurationObject(Point);
            var C_ = new LooseConfigurationObject(Point);
            var D_ = new LooseConfigurationObject(Point);
            var E_ = new ConstructedConfigurationObject(IntersectionOfLinesFromPoints, A_, C_, B_, D_);

            // Create the template configuration
            var templateConfiguration = new Configuration(LooseObjectsLayout.Trapezoid, A_, B_, C_, D_, E_);

            // Create the template theorem
            var templateTheorem = new Theorem(templateConfiguration, TheoremType.TangentCircles, new[]
            {
                new TheoremObjectWithPoints(Circle, E_, A_, B_),
                new TheoremObjectWithPoints(Circle, E_, C_, D_)
            });

            // Create the examined configuration's objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var D = new ConstructedConfigurationObject(Midpoint, A, B);
            var E = new ConstructedConfigurationObject(Midpoint, A, C);

            // Create the examined configuration
            var examinedConfiguration = Configuration.DeriveFromObjects(LooseObjectsLayout.ScaleneAcuteAngledTriangled, D, E);

            // Create the examined theorem
            var examinedTheorem = new Theorem(examinedConfiguration, TheoremType.TangentCircles, new[]
            {
                new TheoremObjectWithPoints(Circle, A, D, E),
                new TheoremObjectWithPoints(Circle, A, B, C)
            });

            // Analyze
            var result = Run(templateTheorem, examinedTheorem);

            // Assert
            result.IsSubtheorem.Should().BeTrue();
            result.UsedEqualities.Should().BeEquivalentTo(new (ConfigurationObject, ConfigurationObject)[]
            {
                // When the algorithm construct mapped E_, it intersects BD and CE
                // After constructing this intersection it finds out that it is actually A
                (A, new ConstructedConfigurationObject(IntersectionOfLinesFromPoints, B, D, C, E))
            });
            result.UsedFacts.ToSet(Theorem.EquivalencyComparer).SetEquals(new[]
            {
                // We need to use the fact that BC || DE 
                new Theorem(examinedConfiguration, TheoremType.ParallelLines, new[]
                {
                    new TheoremObjectWithPoints(Line, B, C),
                    new TheoremObjectWithPoints(Line, D, E)
                })
            }).Should().BeTrue();
        }

        [Test]
        public void Test_Two_Lines_Perpendicular_To_Some_Line_Are_Parallel()
        {
            // Create the template configuration's objects
            var A_ = new LooseConfigurationObject(Point);
            var B_ = new LooseConfigurationObject(Point);
            var C_ = new LooseConfigurationObject(Point);
            var D_ = new LooseConfigurationObject(Point);
            var l1_ = new ConstructedConfigurationObject(PerpendicularLineToLineFromPoints, A_, C_, D_);
            var l2_ = new ConstructedConfigurationObject(PerpendicularLineToLineFromPoints, B_, C_, D_);

            // Create the template configuration
            var templateConfiguration = Configuration.DeriveFromObjects(l1_, l2_);

            // Create the template theorem
            var templateTheorem = new Theorem(templateConfiguration, TheoremType.ParallelLines, l1_, l2_);

            // Create the examined configuration's objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var G = new ConstructedConfigurationObject(Centroid, A, B, C);
            var l1 = new ConstructedConfigurationObject(PerpendicularLineToLineFromPoints, A, B, C);
            var l2 = new ConstructedConfigurationObject(PerpendicularLineToLineFromPoints, G, B, C);

            // Create the examined configuration
            var examinedConfiguration = Configuration.DeriveFromObjects(LooseObjectsLayout.ScaleneAcuteAngledTriangled, l1, l2);

            // Create the examined theorem
            var examinedTheorem = new Theorem(examinedConfiguration, TheoremType.ParallelLines, l1, l2);

            // Analyze
            var result = Run(templateTheorem, examinedTheorem);

            // Assert
            result.IsSubtheorem.Should().BeTrue();
            result.UsedEqualities.Should().BeNullOrEmpty();
            result.UsedFacts.Should().BeNullOrEmpty();
        }

        [Test]
        public void Test_Orthocentric_Situation_Has_Perpendicular_Lines()
        {
            // Create the template configuration's objects
            var A_ = new LooseConfigurationObject(Point);
            var B_ = new LooseConfigurationObject(Point);
            var C_ = new LooseConfigurationObject(Point);
            var H_ = new ConstructedConfigurationObject(Orthocenter, A_, B_, C_);

            // Create the template configuration
            var templateConfiguration = Configuration.DeriveFromObjects(H_);

            // Create the template theorem
            var templateTheorem = new Theorem(templateConfiguration, TheoremType.PerpendicularLines, new[]
            {
                new TheoremObjectWithPoints(Line, A_, H_),
                new TheoremObjectWithPoints(Line, B_, C_)
            });

            // Create the examined configuration's objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var A0 = new ConstructedConfigurationObject(PerpendicularProjectionOnLineFromPoints, A, B, C);
            var D = new ConstructedConfigurationObject(SecondIntersectionOfCircleAndLineFromPoints, A, A0, B, C);
            var H = new ConstructedConfigurationObject(ReflectionInLineFromPoints, D, B, C);

            // Create the examined configuration
            var examinedConfiguration = Configuration.DeriveFromObjects(LooseObjectsLayout.ScaleneAcuteAngledTriangled, H);

            // Create the examined theorem
            var examinedTheorem = new Theorem(examinedConfiguration, TheoremType.PerpendicularLines, new[]
            {
                new TheoremObjectWithPoints(Line, B, H),
                new TheoremObjectWithPoints(Line, A, C)
            });

            // Analyze
            var result = Run(templateTheorem, examinedTheorem);

            // Assert
            result.IsSubtheorem.Should().BeTrue();
            result.UsedEqualities.Should().BeEquivalentTo(new (ConfigurationObject, ConfigurationObject)[]
            {
                // We need to discover H is the orthocenter of ABC
                (H, new ConstructedConfigurationObject(Orthocenter, A, B, C))
            });
            result.UsedFacts.Should().BeNullOrEmpty();
        }

        [Test]
        public void Test_Miquels_Theorem()
        {
            // Create the template configuration's objects
            var A_ = new LooseConfigurationObject(Point);
            var B_ = new LooseConfigurationObject(Point);
            var C_ = new LooseConfigurationObject(Point);
            var D_ = new ConstructedConfigurationObject(RandomPointOnLineFromPoints, A_, B_);
            var E_ = new ConstructedConfigurationObject(RandomPointOnLineFromPoints, B_, C_);
            var F_ = new ConstructedConfigurationObject(RandomPointOnLineFromPoints, C_, A_);

            // Create the template configuration
            var templateConfiguration = Configuration.DeriveFromObjects(D_, E_, F_);

            // Create the template theorem
            var templateTheorem = new Theorem(templateConfiguration, TheoremType.ConcurrentObjects, new[]
            {
                new TheoremObjectWithPoints(Circle, A_, E_, F_),
                new TheoremObjectWithPoints(Circle, B_, F_, D_),
                new TheoremObjectWithPoints(Circle, C_, D_, E_),
            });

            // Create the examined configuration's objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var D = new ConstructedConfigurationObject(Midpoint, A, B);
            var E = new ConstructedConfigurationObject(Midpoint, B, C);
            var F = new ConstructedConfigurationObject(Midpoint, C, A);

            // Create the examined configuration
            var examinedConfiguration = Configuration.DeriveFromObjects(LooseObjectsLayout.ScaleneAcuteAngledTriangled, D, E, F);

            // Create the examined theorem
            var examinedTheorem = new Theorem(examinedConfiguration, TheoremType.ConcurrentObjects, new[]
            {
                new TheoremObjectWithPoints(Circle, A, E, F),
                new TheoremObjectWithPoints(Circle, B, F, D),
                new TheoremObjectWithPoints(Circle, C, D, E)
            });

            // Analyze
            var result = Run(templateTheorem, examinedTheorem);

            // Assert
            result.IsSubtheorem.Should().BeTrue();
            result.UsedEqualities.Should().BeNullOrEmpty();
            result.UsedFacts.Should().BeNullOrEmpty();
        }

        [Test]
        public void Test_Reflected_Tangent_Line_Is_Still_Tangent_To_Circle()
        {
            // Create the template configuration's objects
            var c_ = new LooseConfigurationObject(Circle);
            var P1_ = new LooseConfigurationObject(Point);
            var P2_ = new LooseConfigurationObject(Point);
            var O_ = new ConstructedConfigurationObject(CenterOfCircle, c_);
            var Q1_ = new ConstructedConfigurationObject(PointReflection, P1_, O_);
            var Q2_ = new ConstructedConfigurationObject(PointReflection, P2_, O_);

            // Create the template configuration
            var templateConfiguration = Configuration.DeriveFromObjects(LooseObjectsLayout.CircleAndItsTangentLine, c_, P1_, P2_, Q1_, Q2_);

            // Create the template theorem
            var templateTheorem = new Theorem(templateConfiguration, TheoremType.LineTangentToCircle, new[]
            {
                new TheoremObjectWithPoints(Line, Q1_, Q2_),
                new TheoremObjectWithPoints(c_)
            });

            // Create the examined configuration's objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var c = new ConstructedConfigurationObject(Incircle, A, B, C);
            var I = new ConstructedConfigurationObject(Incenter, A, B, C);
            var B1 = new ConstructedConfigurationObject(PointReflection, B, I);
            var C1 = new ConstructedConfigurationObject(PointReflection, C, I);

            // Create the examined configuration
            var examinedConfiguration = Configuration.DeriveFromObjects(LooseObjectsLayout.ScaleneAcuteAngledTriangled, B1, C1, c);

            // Create the examined theorem
            var examinedTheorem = new Theorem(examinedConfiguration, TheoremType.LineTangentToCircle, new[]
            {
                new TheoremObjectWithPoints(Line, B1, C1),
                new TheoremObjectWithPoints(Circle, c)
            });

            // Analyze
            var result = Run(templateTheorem, examinedTheorem);

            // Assert
            result.IsSubtheorem.Should().BeTrue();
            result.UsedEqualities.Should().BeEquivalentTo(new (ConfigurationObject, ConfigurationObject)[]
            {
                // We need to discover I is really the center of the incircle
                (I, new ConstructedConfigurationObject(CenterOfCircle, c))
            });
            result.UsedFacts.ToSet(Theorem.EquivalencyComparer).SetEquals(new[]
            {
                // We're using the fact that BC is tangent to 'c'
                new Theorem(examinedConfiguration, TheoremType.LineTangentToCircle, new[]
                {
                    new TheoremObjectWithPoints(Line, B, C),
                    new TheoremObjectWithPoints(c)
                })
            })
            .Should().BeTrue();
        }

        [Test]
        public void Test_Radical_Axis_Theorem()
        {
            // Create the template configuration's objects
            var A_ = new LooseConfigurationObject(Point);
            var B_ = new LooseConfigurationObject(Point);
            var C_ = new LooseConfigurationObject(Point);
            var D_ = new LooseConfigurationObject(Point);
            var E_ = new LooseConfigurationObject(Point);
            var F_ = new LooseConfigurationObject(Point);

            // Create the template configuration
            var templateConfiguration = new Configuration(LooseObjectsLayout.ThreeCyclicQuadrilatersOnSixPoints, A_, B_, C_, D_, E_, F_);

            // Create the template theorem
            var templateTheorem = new Theorem(templateConfiguration, TheoremType.ConcurrentObjects, new[]
            {
                new TheoremObjectWithPoints(Line, A_, B_),
                new TheoremObjectWithPoints(Line, C_, D_),
                new TheoremObjectWithPoints(Line, E_, F_)
            });

            // Create the examined configuration's objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var H = new ConstructedConfigurationObject(Orthocenter, A, B, C);
            var H1 = new ConstructedConfigurationObject(ReflectionInLineFromPoints, H, A, B);
            var D = new ConstructedConfigurationObject(SecondIntersectionOfTwoCircumcircles, H, H1, A, B, C);

            // Create the examined configuration
            var examinedConfiguration = Configuration.DeriveFromObjects(LooseObjectsLayout.ScaleneAcuteAngledTriangled, D);

            // Create the examined theorem
            var examinedTheorem = new Theorem(examinedConfiguration, TheoremType.ConcurrentObjects, new[]
            {
                new TheoremObjectWithPoints(Line, A, H1),
                new TheoremObjectWithPoints(Line, H, D),
                new TheoremObjectWithPoints(Line, B, C),
            });

            // Analyze
            var result = Run(templateTheorem, examinedTheorem);

            // Assert
            result.IsSubtheorem.Should().BeTrue();
            result.UsedEqualities.Should().BeNullOrEmpty();
            result.UsedFacts.ToSet(Theorem.EquivalencyComparer).SetEquals(new[]
            {
                // We're using all these concyclic theorems 
                new Theorem(examinedConfiguration, TheoremType.ConcyclicPoints, A, H1, H, D),
                new Theorem(examinedConfiguration, TheoremType.ConcyclicPoints, A, H1, B, C),
                new Theorem(examinedConfiguration, TheoremType.ConcyclicPoints, H, D, B, C),
            })
            .Should().BeTrue();
        }

        [Test]
        public void Test_Equal_Angles_Because_Of_Parallel_Lines()
        {
            // Create the template configuration's objects
            var A_ = new LooseConfigurationObject(Point);
            var B_ = new LooseConfigurationObject(Point);
            var C_ = new LooseConfigurationObject(Point);
            var D_ = new LooseConfigurationObject(Point);
            var E_ = new ConstructedConfigurationObject(RandomPoint);
            var F_ = new ConstructedConfigurationObject(RandomPoint);

            // Create the template configuration
            var templateConfiguration = new Configuration(LooseObjectsLayout.Trapezoid, A_, B_, C_, D_, E_, F_);

            // Create the template theorem
            var templateTheorem = new Theorem(templateConfiguration, TheoremType.EqualAngles, new[]
            {
                new TheoremObjectWithPoints(Line, A_, B_),
                new TheoremObjectWithPoints(Line, E_, F_),
                new TheoremObjectWithPoints(Line, C_, D_),
                new TheoremObjectWithPoints(Line, E_, F_)
            });

            // Create the examined configuration's objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var D = new ConstructedConfigurationObject(Midpoint, A, B);
            var E = new ConstructedConfigurationObject(Midpoint, C, D);
            var F = new ConstructedConfigurationObject(Midpoint, A, E);

            // Create the examined configuration
            var examinedConfiguration = Configuration.DeriveFromObjects(LooseObjectsLayout.ScaleneAcuteAngledTriangled, F);

            // Create the examined theorem
            var examinedTheorems = new[]
            {
                new Theorem(examinedConfiguration, TheoremType.EqualAngles, new[]
                {
                    new TheoremObjectWithPoints(Line, B, E),
                    new TheoremObjectWithPoints(Line, C, F),
                    new TheoremObjectWithPoints(Line, D, F),
                    new TheoremObjectWithPoints(Line, C, F)
                }),
                new Theorem(examinedConfiguration, TheoremType.EqualAngles, new[]
                {
                    new TheoremObjectWithPoints(Line, A, C),
                    new TheoremObjectWithPoints(Line, B, E),
                    new TheoremObjectWithPoints(Line, A, C),
                    new TheoremObjectWithPoints(Line, D, F)
                }),
                new Theorem(examinedConfiguration, TheoremType.EqualAngles, new[]
                {
                    new TheoremObjectWithPoints(Line, B, C),
                    new TheoremObjectWithPoints(Line, B, E),
                    new TheoremObjectWithPoints(Line, B, C),
                    new TheoremObjectWithPoints(Line, D, F)
                })
            };

            // Analyze all theorems
            examinedTheorems.ForEach(examinedTheorem =>
            {
                // Analyze
                var result = Run(templateTheorem, examinedTheorem);

                // Assert
                result.IsSubtheorem.Should().BeTrue();
                result.UsedEqualities.Should().BeNullOrEmpty();
                result.UsedFacts.ToSet(Theorem.EquivalencyComparer).SetEquals(new[]
                {
                    // We're using that BE || DF
                    new Theorem(examinedConfiguration, TheoremType.ParallelLines, new[]
                    {
                        new TheoremObjectWithPoints(Line, B, E),
                        new TheoremObjectWithPoints(Line, D, F)
                    })
                }).Should().BeTrue();
            });
        }
    }
}
