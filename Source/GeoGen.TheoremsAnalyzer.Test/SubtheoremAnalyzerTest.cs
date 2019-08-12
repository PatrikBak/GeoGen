using FluentAssertions;
using GeoGen.Constructor;
using GeoGen.Core;
using GeoGen.DependenciesResolver;
using GeoGen.Generator;
using GeoGen.Utilities;
using Ninject;
using NUnit.Framework;
using static GeoGen.Core.ComposedConstructions;
using static GeoGen.Core.ConfigurationObjectType;
using static GeoGen.Core.LooseObjectsLayout;
using static GeoGen.Core.PredefinedConstructionType;
using static GeoGen.Core.TheoremType;

namespace GeoGen.TheoremsAnalyzer.Test
{
    /// <summary>
    /// The test class for <see cref="SubtheoremAnalyzer"/>.
    /// </summary>
    [TestFixture]
    public class SubtheoremAnalyzerTest
    {
        #region IoC container

        /// <summary>
        /// The NInject kernel
        /// </summary>
        private IKernel _kernel;

        #endregion

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
            _kernel = IoCUtilities.CreateKernel().AddGenerator().AddConstructor().AddTheoremsFinder();

            // Create the constructor
            _constructor = _kernel.Get<IGeometryConstructor>(new PicturesSettings
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
            var (manager, data) = _constructor.Construct(examinedTheorem.Configuration);

            // Draw the contextual picture
            var contextualPicture = _kernel.Get<IContextualPictureFactory>().Create(manager);

            // Create the container
            var objectsContainer = _kernel.Get<IConfigurationObjectsContainerFactory>().CreateContainer();

            // Fill it with objects of the examined configuration
            examinedTheorem.Configuration.AllObjects.ForEach(objectsContainer.Add);

            // Run the algorithm
            return _analyzer.Analyze(new SubtheoremAnalyzerInput
            {
                TemplateTheorem = templateTheorem,
                ExaminedTheorem = examinedTheorem,
                ExaminedConfigurationManager = manager,
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
            var templateConfiguration = Configuration.DeriveFromObjects(TwoPoints, M_);

            // Create the template theorem
            var templateTheorem = new Theorem(templateConfiguration, EqualLineSegments, new[]
            {
                new LineSegmentTheoremObject(A_, M_),
                new LineSegmentTheoremObject(B_, M_)
            });

            // Create the examined configuration's objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var M = new ConstructedConfigurationObject(Midpoint, B, C);
            var N = new ConstructedConfigurationObject(Midpoint, A, M);

            // Create the examined configuration
            var examinedConfiguration = Configuration.DeriveFromObjects(ThreePoints, N);

            // Create the examined theorem
            var examinedTheorem = new Theorem(examinedConfiguration, EqualLineSegments, new[]
            {
                new LineSegmentTheoremObject(N, A),
                new LineSegmentTheoremObject(N, M)
            });

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
            var templateConfiguration = Configuration.DeriveFromObjects(TwoPoints, M_);

            // Create the template theorem
            var templateTheorem = new Theorem(templateConfiguration, EqualLineSegments, new[]
            {
                new LineSegmentTheoremObject(A_, M_),
                new LineSegmentTheoremObject(B_, M_)
            });

            // Create the examined configuration's objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var O = new ConstructedConfigurationObject(Circumcenter, A, B, C);
            var M = new ConstructedConfigurationObject(PerpendicularProjectionOnLineFromPoints, O, B, C);

            // Create the examined configuration
            var examinedConfiguration = Configuration.DeriveFromObjects(ThreePoints, M);

            // Create the examined theorem
            var examinedTheorem = new Theorem(examinedConfiguration, EqualLineSegments, new[]
            {
                new LineSegmentTheoremObject(M, B),
                new LineSegmentTheoremObject(M, C)
            });

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
            var templateConfiguration = Configuration.DeriveFromObjects(ThreePoints, O_);

            // Create the template theorem
            var templateTheorem = new Theorem(templateConfiguration, EqualAngles, new[]
            {
                new AngleTheoremObject(new LineTheoremObject(B_, C_), new LineTheoremObject(B_, O_)),
                new AngleTheoremObject(new LineTheoremObject(C_, O_), new LineTheoremObject(B_, C_))
            });

            // Create the examined configuration's objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var D = new ConstructedConfigurationObject(Midpoint, A, B);
            var E = new ConstructedConfigurationObject(Midpoint, A, C);
            var O = new ConstructedConfigurationObject(Circumcenter, A, D, E);

            // Create the examined configuration
            var examinedConfiguration = Configuration.DeriveFromObjects(ThreePoints, O);

            // Create the examined theorem
            var examinedTheorem = new Theorem(examinedConfiguration, EqualAngles, new[]
            {
                new AngleTheoremObject(new LineTheoremObject(A, D), new LineTheoremObject(A, O)),
                new AngleTheoremObject(new LineTheoremObject(D, O), new LineTheoremObject(A, D))
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
            var templateConfiguration = Configuration.DeriveFromObjects(ThreePoints, O_);

            // Create the template theorem
            var templateTheorem = new Theorem(templateConfiguration, EqualAngles, new[]
            {
                new AngleTheoremObject(new LineTheoremObject(B_, C_), new LineTheoremObject(B_, O_)),
                new AngleTheoremObject(new LineTheoremObject(C_, O_), new LineTheoremObject(B_, C_))
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
            var examinedConfiguration = Configuration.DeriveFromObjects(ThreePoints, O);

            // Create the examined theorem
            var examinedTheorem = new Theorem(examinedConfiguration, EqualAngles, new[]
            {
                new AngleTheoremObject(new LineTheoremObject(A, D, B), new LineTheoremObject(A, O)),
                new AngleTheoremObject(new LineTheoremObject(D, O), new LineTheoremObject(A, B, D))
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
            var templateConfiguration = Configuration.DeriveFromObjects(ThreePoints, D_, E_, F_);

            // Create the template theorem
            var templateTheorem = new Theorem(templateConfiguration, ConcurrentObjects, new[]
            {
                new LineTheoremObject(A_, D_),
                new LineTheoremObject(B_, E_),
                new LineTheoremObject(C_, F_)
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
            var examinedConfiguration = Configuration.DeriveFromObjects(ThreePoints, E, F, G);

            // Create the examined theorem
            var examinedTheorem = new Theorem(examinedConfiguration, ConcurrentObjects, new[]
            {
                new LineTheoremObject(A, G),
                new LineTheoremObject(D, E),
                new LineTheoremObject(C, F)
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
            var templateConfiguration = Configuration.DeriveFromObjects(ThreePoints, D_, E_, F_);

            // Create the template theorem
            var templateTheorem = new Theorem(templateConfiguration, ConcurrentObjects, new[]
            {
                new LineTheoremObject(A_, D_),
                new LineTheoremObject(B_, E_),
                new LineTheoremObject(C_, F_)
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
            var examinedConfiguration = Configuration.DeriveFromObjects(ThreePoints, D, E, F);

            // Create the examined theorem
            var examinedTheorem = new Theorem(examinedConfiguration, ConcurrentObjects, new[]
            {
                new LineTheoremObject(A, D),
                new LineTheoremObject(B, E),
                new LineTheoremObject(C, F)
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
            var templateConfiguration = Configuration.DeriveFromObjects(TwoPoints, B_);

            // Create the template theorem
            var templateTheorem = new Theorem(templateConfiguration, CollinearPoints, A_, B_, M_);

            // Create the examined configuration's objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var H = new ConstructedConfigurationObject(Orthocenter, A, B, C);
            var O = new ConstructedConfigurationObject(Circumcenter, A, B, C);
            var D = new ConstructedConfigurationObject(PointReflection, A, O);
            var M = new ConstructedConfigurationObject(Midpoint, B, C);

            // Create the examined configuration
            var examinedConfiguration = Configuration.DeriveFromObjects(ThreePoints, M, D, H);

            // Create the examined theorem
            var examinedTheorem = new Theorem(examinedConfiguration, CollinearPoints, H, D, M);

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
        public void Test_Tangent_Circles_Because_Of_Homothety()
        {
            // Create the template configuration's objects
            var A_ = new LooseConfigurationObject(Point);
            var B_ = new LooseConfigurationObject(Point);
            var C_ = new LooseConfigurationObject(Point);
            var D_ = new LooseConfigurationObject(Point);
            var E_ = new ConstructedConfigurationObject(IntersectionOfLinesFromPoints, A_, C_, B_, D_);

            // Create the template configuration
            var templateConfiguration = Configuration.DeriveFromObjects(Trapezoid, A_, B_, C_, D_, E_);

            // Create the template theorem
            var templateTheorem = new Theorem(templateConfiguration, TangentCircles, new[]
            {
                new CircleTheoremObject(E_, A_, B_),
                new CircleTheoremObject(E_, C_, D_)
            });

            // Create the examined configuration's objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var D = new ConstructedConfigurationObject(Midpoint, A, B);
            var E = new ConstructedConfigurationObject(Midpoint, A, C);
            var F = new ConstructedConfigurationObject(Midpoint, B, E);

            // Create the examined configuration
            var examinedConfiguration = Configuration.DeriveFromObjects(ThreePoints, D, E, F);

            // Create the examined theorem
            var examinedTheorem = new Theorem(examinedConfiguration, TangentCircles, new[]
            {
                new CircleTheoremObject(A, B, E),
                new CircleTheoremObject(B, D, F)
            });

            // Analyze
            var result = Run(templateTheorem, examinedTheorem);

            // Assert
            result.IsSubtheorem.Should().BeTrue();
            result.UsedEqualities.Should().BeEquivalentTo(new (ConfigurationObject, ConfigurationObject)[]
            {
                // When the algorithm construct mapped E_, it intersects AD and EF
                // After constructing this intersection it finds out that it is actually B
                (B, new ConstructedConfigurationObject(IntersectionOfLinesFromPoints, A, D, E, F))
            });
            result.UsedFacts.ToSet(Theorem.EquivalencyComparer).SetEquals(new[]
            {
                // We need to use the fact that AEC || DF 
                new Theorem(examinedConfiguration, ParallelLines, new[]
                {
                    new LineTheoremObject(A, E, C),
                    new LineTheoremObject(D, F)
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
            var templateConfiguration = Configuration.DeriveFromObjects(FourPoints, l1_, l2_);

            // Create the template theorem
            var templateTheorem = new Theorem(templateConfiguration, ParallelLines, l1_, l2_);

            // Create the examined configuration's objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var G = new ConstructedConfigurationObject(Centroid, A, B, C);
            var l1 = new ConstructedConfigurationObject(PerpendicularLineToLineFromPoints, A, B, C);
            var l2 = new ConstructedConfigurationObject(PerpendicularLineToLineFromPoints, G, B, C);

            // Create the examined configuration
            var examinedConfiguration = Configuration.DeriveFromObjects(ThreePoints, l1, l2);

            // Create the examined theorem
            var examinedTheorem = new Theorem(examinedConfiguration, ParallelLines, l1, l2);

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
            var templateConfiguration = Configuration.DeriveFromObjects(ThreePoints, H_);

            // Create the template theorem
            var templateTheorem = new Theorem(templateConfiguration, PerpendicularLines, new[]
            {
                new LineTheoremObject(A_, H_),
                new LineTheoremObject(B_, C_)
            });

            // Create the examined configuration's objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var A0 = new ConstructedConfigurationObject(PerpendicularProjectionOnLineFromPoints, A, B, C);
            var D = new ConstructedConfigurationObject(SecondIntersectionOfCircleAndLineFromPoints, A, A0, B, C);
            var H = new ConstructedConfigurationObject(ReflectionInLineFromPoints, D, B, C);

            // Create the examined configuration
            var examinedConfiguration = Configuration.DeriveFromObjects(ThreePoints, H);

            // Create the examined theorem
            var examinedTheorem = new Theorem(examinedConfiguration, PerpendicularLines, new[]
            {
                new LineTheoremObject(B, H),
                new LineTheoremObject(A, C)
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
            var templateConfiguration = Configuration.DeriveFromObjects(ThreePoints, D_, E_, F_);

            // Create the template theorem
            var templateTheorem = new Theorem(templateConfiguration, ConcurrentObjects, new[]
            {
                new CircleTheoremObject(A_, E_, F_),
                new CircleTheoremObject(B_, F_, D_),
                new CircleTheoremObject(C_, D_, E_),
            });

            // Create the examined configuration's objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var D = new ConstructedConfigurationObject(Midpoint, A, B);
            var E = new ConstructedConfigurationObject(Midpoint, B, C);
            var F = new ConstructedConfigurationObject(Midpoint, C, A);

            // Create the examined configuration
            var examinedConfiguration = Configuration.DeriveFromObjects(ThreePoints, D, E, F);

            // Create the examined theorem
            var examinedTheorem = new Theorem(examinedConfiguration, ConcurrentObjects, new[]
            {
                new CircleTheoremObject(A, E, F),
                new CircleTheoremObject(B, F, D),
                new CircleTheoremObject(C, D, E)
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
            var templateConfiguration = Configuration.DeriveFromObjects(CircleAndItsTangentLineFromPoints, c_, P1_, P2_, Q1_, Q2_);

            // Create the template theorem
            var templateTheorem = new Theorem(templateConfiguration, LineTangentToCircle, new TheoremObject[]
            {
                new LineTheoremObject(Q1_, Q2_),
                new CircleTheoremObject(c_)
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
            var examinedConfiguration = Configuration.DeriveFromObjects(ThreePoints, B1, C1, c);

            // Create the examined theorem
            var examinedTheorem = new Theorem(examinedConfiguration, LineTangentToCircle, new TheoremObject[]
            {
                new LineTheoremObject(B1, C1),
                new CircleTheoremObject(c)
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
                new Theorem(examinedConfiguration, LineTangentToCircle, new TheoremObject[]
                {
                    new LineTheoremObject(B, C),
                    new CircleTheoremObject(c)
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
            var templateConfiguration = Configuration.DeriveFromObjects(ThreeCyclicQuadrilatersOnSixPoints, A_, B_, C_, D_, E_, F_);

            // Create the template theorem
            var templateTheorem = new Theorem(templateConfiguration, ConcurrentObjects, new[]
            {
                new LineTheoremObject(A_, B_),
                new LineTheoremObject(C_, D_),
                new LineTheoremObject(E_, F_)
            });

            // Create the examined configuration's objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var H = new ConstructedConfigurationObject(Orthocenter, A, B, C);
            var H1 = new ConstructedConfigurationObject(ReflectionInLineFromPoints, H, A, B);
            var D = new ConstructedConfigurationObject(SecondIntersectionOfTwoCircumcircles, H, H1, A, B, C);

            // Create the examined configuration
            var examinedConfiguration = Configuration.DeriveFromObjects(ThreePoints, D);

            // Create the examined theorem
            var examinedTheorem = new Theorem(examinedConfiguration, ConcurrentObjects, new[]
            {
                new LineTheoremObject(A, H1),
                new LineTheoremObject(H, D),
                new LineTheoremObject(B, C),
            });

            // Analyze
            var result = Run(templateTheorem, examinedTheorem);

            // Assert
            result.IsSubtheorem.Should().BeTrue();
            result.UsedEqualities.Should().BeNullOrEmpty();
            result.UsedFacts.ToSet(Theorem.EquivalencyComparer).SetEquals(new[]
            {
                // We're using all these concyclic theorems 
                new Theorem(examinedConfiguration, ConcyclicPoints, A, H1, H, D),
                new Theorem(examinedConfiguration, ConcyclicPoints, A, H1, B, C),
                new Theorem(examinedConfiguration, ConcyclicPoints, H, D, B, C),
            })
            .Should().BeTrue();
        }

        [Test]
        public void Test_Equal_Angles_Because_Of_Concylic_Points()
        {
            // Create the template configuration's objects
            var A_ = new LooseConfigurationObject(Point);
            var B_ = new LooseConfigurationObject(Point);
            var C_ = new LooseConfigurationObject(Point);
            var D_ = new LooseConfigurationObject(Point);

            // Create the template configuration
            var templateConfiguration = Configuration.DeriveFromObjects(FourConcyclicPoints, A_, B_, C_, D_);

            // Create the template theorem
            var templateTheorem = new Theorem(templateConfiguration, EqualAngles, new[]
            {
                new AngleTheoremObject(new LineTheoremObject(A_, B_), new LineTheoremObject(A_, D_)),
                new AngleTheoremObject(new LineTheoremObject(C_, B_), new LineTheoremObject(C_, D_)),
            });

            // Create the examined configuration's objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var H = new ConstructedConfigurationObject(Orthocenter, A, B, C);
            var D = new ConstructedConfigurationObject(ReflectionInLineFromPoints, H, B, C);

            // Create the examined configuration
            var examinedConfiguration = Configuration.DeriveFromObjects(ThreePoints, D);

            // Create the examined theorem
            var examinedTheorems = new[]
            {
                new Theorem(examinedConfiguration, EqualAngles, new[]
                {
                    new AngleTheoremObject(new LineTheoremObject(A, B), new LineTheoremObject(A, C)),
                    new AngleTheoremObject(new LineTheoremObject(D, B), new LineTheoremObject(D, C))
                }),
                new Theorem(examinedConfiguration, EqualAngles, new[]
                {
                    new AngleTheoremObject(new LineTheoremObject(C, A), new LineTheoremObject(C, D)),
                    new AngleTheoremObject(new LineTheoremObject(B, A), new LineTheoremObject(B, D))
                }),
                new Theorem(examinedConfiguration, EqualAngles, new[]
                {
                    new AngleTheoremObject(new LineTheoremObject(D, A), new LineTheoremObject(D, C)),
                    new AngleTheoremObject(new LineTheoremObject(B, A), new LineTheoremObject(B, C))
                }),
                new Theorem(examinedConfiguration, EqualAngles, new[]
                {
                    new AngleTheoremObject(new LineTheoremObject(D, A), new LineTheoremObject(D, B)),
                    new AngleTheoremObject(new LineTheoremObject(C, A), new LineTheoremObject(C, B))
                }),
                new Theorem(examinedConfiguration, EqualAngles, new[]
                {
                    new AngleTheoremObject(new LineTheoremObject(A, C), new LineTheoremObject(A, D)),
                    new AngleTheoremObject(new LineTheoremObject(B, C), new LineTheoremObject(B, D))
                }),
                new Theorem(examinedConfiguration, EqualAngles, new[]
                {
                    new AngleTheoremObject(new LineTheoremObject(A, B), new LineTheoremObject(A, D)),
                    new AngleTheoremObject(new LineTheoremObject(C, B), new LineTheoremObject(C, D))
                }),
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
                    // We're using that A, B, C, D are concyclic
                    new Theorem(examinedConfiguration, ConcyclicPoints, A, B, C, D)
                })
                .Should().BeTrue();
            });
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
            var templateConfiguration = Configuration.DeriveFromObjects(Trapezoid, A_, B_, C_, D_, E_, F_);

            // Create the template theorem
            var templateTheorem = new Theorem(templateConfiguration, EqualAngles, new[]
            {
                new AngleTheoremObject(new LineTheoremObject(A_, B_), new LineTheoremObject(E_, F_)),
                new AngleTheoremObject(new LineTheoremObject(C_, D_), new LineTheoremObject(E_, F_))
            });

            // Create the examined configuration's objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var D = new ConstructedConfigurationObject(Midpoint, A, B);
            var E = new ConstructedConfigurationObject(Midpoint, C, D);
            var F = new ConstructedConfigurationObject(Midpoint, A, E);

            // Create the examined configuration
            var examinedConfiguration = Configuration.DeriveFromObjects(ThreePoints, F);

            // Create the examined theorem
            var examinedTheorems = new[]
            {
                new Theorem(examinedConfiguration, EqualAngles, new[]
                {
                    new AngleTheoremObject(new LineTheoremObject(B, E), new LineTheoremObject(C, F)),
                    new AngleTheoremObject(new LineTheoremObject(D, F), new LineTheoremObject(C, F))
                }),
                new Theorem(examinedConfiguration, EqualAngles, new[]
                {
                    new AngleTheoremObject(new LineTheoremObject(A, C), new LineTheoremObject(B, E)),
                    new AngleTheoremObject(new LineTheoremObject(A, C), new LineTheoremObject(D, F))
                }),
                new Theorem(examinedConfiguration, EqualAngles, new[]
                {
                    new AngleTheoremObject(new LineTheoremObject(B, C), new LineTheoremObject(B, E)),
                    new AngleTheoremObject(new LineTheoremObject(B, C), new LineTheoremObject(D, F))
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
                    new Theorem(examinedConfiguration, ParallelLines, new[]
                    {
                        new LineTheoremObject(B, E),
                        new LineTheoremObject(D, F)
                    })
                }).Should().BeTrue();
            });
        }

        [Test]
        public void Test_Trapezoid_Layout_With_Concurrency_Theorem()
        {
            // Create the template configuration's objects
            var A_ = new LooseConfigurationObject(Point);
            var B_ = new LooseConfigurationObject(Point);
            var C_ = new LooseConfigurationObject(Point);
            var D_ = new LooseConfigurationObject(Point);
            var E_ = new ConstructedConfigurationObject(IntersectionOfLinesFromPoints, A_, C_, B_, D_);
            var M_ = new ConstructedConfigurationObject(Midpoint, A_, B_);

            // Create the template configuration
            var templateConfiguration = Configuration.DeriveFromObjects(Trapezoid, A_, B_, C_, D_, E_, M_);

            // Create the template theorem
            var templateTheorem = new Theorem(templateConfiguration, ConcurrentObjects, new[]
            {
                new LineTheoremObject(E_, M_),
                new LineTheoremObject(A_, D_),
                new LineTheoremObject(B_, C_)
            });

            // Create the examined configuration's objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var D = new ConstructedConfigurationObject(Midpoint, A, B);
            var E = new ConstructedConfigurationObject(Midpoint, A, C);
            var F = new ConstructedConfigurationObject(Midpoint, D, E);

            // Create the examined configuration
            var examinedConfiguration = Configuration.DeriveFromObjects(ThreePoints, D, E, F);

            // Create the examined theorem
            var examinedTheorem = new Theorem(examinedConfiguration, ConcurrentObjects, new[]
            {
                new LineTheoremObject(A, F),
                new LineTheoremObject(B, E),
                new LineTheoremObject(C, D)
            });

            // Analyze
            var result = Run(templateTheorem, examinedTheorem);

            // Assert
            result.IsSubtheorem.Should().BeTrue();
            result.UsedEqualities.Should().BeEquivalentTo(new (ConfigurationObject, ConfigurationObject)[]
            {
                // When the algorithm construct mapped E_, it intersects AD and EF
                // After constructing this intersection it finds out that it is actually B
                (A, new ConstructedConfigurationObject(IntersectionOfLinesFromPoints, B, D, C, E))
            });
            result.UsedFacts.ToSet(Theorem.EquivalencyComparer).SetEquals(new[]
            {
                // We need to use the fact that BC || DE
                new Theorem(examinedConfiguration, ParallelLines, new[]
                {
                    new LineTheoremObject(B, C),
                    new LineTheoremObject(D, E)
                })
            }).Should().BeTrue();
        }
    }
}

