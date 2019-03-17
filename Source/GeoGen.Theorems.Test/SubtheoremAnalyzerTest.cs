using FluentAssertions;
using FluentAssertions.Equivalency;
using GeoGen.ConsoleTest;
using GeoGen.Constructor;
using GeoGen.Core;
using GeoGen.Generator;
using NUnit.Framework;
using System;
using static GeoGen.Core.ComposedConstructions;
using static GeoGen.Core.ConfigurationObjectType;
using static GeoGen.Core.PredefinedConstructionType;

namespace GeoGen.Theorems.Test
{
    /// <summary>
    /// The test class for <see cref="SubtheoremAnalyzer"/>.
    /// </summary>
    [TestFixture]
    public class SubtheoremAnalyzerTest
    {
        /// <summary>
        /// The instance of the analyzer.
        /// </summary>
        private ISubtheoremAnalyzer _analyzer;

        [SetUp]
        public void InitializeAnalyzer()
        {
            // Initialize IoC
            IoC.Bootstrap();

            // Get the constructor
            var constructor = IoC.Get<IGeometryConstructor>(new ObjectsContainersManagerSettings
            {
                NumberOfContainers = 8,
                MaximalAttemptsToReconstructOneContainer = 100,
                MaximalAttemptsToReconstructAllContainers = 1000
            });

            // Get the factory
            var factory = IoC.Get<IConfigurationObjectsContainerFactory>();

            // Create the analyzer
            _analyzer = new SubtheoremAnalyzer(constructor, factory);
        }

        [Test]
        public void Test_Midpoint_Equally_Distanced_Theorem_With_Direct_Definition()
        {
            // Create original configuration objects
            var A_ = new LooseConfigurationObject(Point);
            var B_ = new LooseConfigurationObject(Point);
            var M_ = new ConstructedConfigurationObject(Midpoint, A_, B_);

            // Create the original configuration
            var originalConfiguration = Configuration.DeriveFromObjects(M_);

            // Create the original theorem
            var originalTheorem = new Theorem(originalConfiguration, TheoremType.EqualLineSegments, A_, M_, B_, M_);

            // Create potential consequence theorem objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var M = new ConstructedConfigurationObject(Midpoint, B, C);
            var N = new ConstructedConfigurationObject(Midpoint, A, M);

            // Create the consequence configuration
            var consequenceConfiguration = Configuration.DeriveFromObjects(LooseObjectsLayout.ScaleneAcuteAngledTriangled, N);

            // Create the potential consequence theorem
            var consequenceTheorem = new Theorem(consequenceConfiguration, TheoremType.EqualLineSegments, N, A, N, M);

            // Analyze
            var result = _analyzer.Analyze(originalTheorem, consequenceTheorem);

            // Assert
            result.IsSubtheorem.Should().BeTrue();
            result.UsedEqualities.Should().BeNullOrEmpty();
        }

        [Test]
        public void Test_Midpoint_Equally_Distanced_Theorem_With_Implicit_Definition()
        {
            // Create original configuration objects
            var A_ = new LooseConfigurationObject(Point);
            var B_ = new LooseConfigurationObject(Point);
            var M_ = new ConstructedConfigurationObject(Midpoint, A_, B_);
            var X = new ConstructedConfigurationObject(Midpoint, A_, B_);

            // Create the original configuration
            var originalConfiguration = Configuration.DeriveFromObjects(M_);

            // Create the original theorem
            var originalTheorem = new Theorem(originalConfiguration, TheoremType.EqualLineSegments, A_, M_, B_, M_);

            // Create potential consequence theorem objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var O = new ConstructedConfigurationObject(Circumcenter, A, B, C);
            var M = new ConstructedConfigurationObject(PerpendicularProjectionOnLineFromPoints, O, B, C);

            // Create the consequence configuration
            var consequenceConfiguration = Configuration.DeriveFromObjects(LooseObjectsLayout.ScaleneAcuteAngledTriangled, M);

            // Create the potential consequence theorem
            var consequenceTheorem = new Theorem(consequenceConfiguration, TheoremType.EqualLineSegments, M, B, M, C);

            // Analyze
            var result = _analyzer.Analyze(originalTheorem, consequenceTheorem);

            // Assert
            result.IsSubtheorem.Should().BeTrue();
            result.UsedEqualities.Should().BeEquivalentTo(new (ConfigurationObject, ConfigurationObject)[]
            {
                // We need to discover M is the midpoint of BC
                (M, new ConstructedConfigurationObject(Midpoint, B, C))
            });
        }

        [Test]
        public void Test_Circumcircle_Equal_Angles_Theorem_With_Direct_Definition()
        {
            // Create original configuration objects
            var A_ = new LooseConfigurationObject(Point);
            var B_ = new LooseConfigurationObject(Point);
            var C_ = new LooseConfigurationObject(Point);
            var O_ = new ConstructedConfigurationObject(Circumcenter, A_, B_, C_);

            // Create the original configuration
            var originalConfiguration = Configuration.DeriveFromObjects(O_);

            // Create the original theorem
            var originalTheorem = new Theorem(originalConfiguration, TheoremType.EqualAngles, new[]
            {
                new TheoremObject(TheoremObjectSignature.LineGivenByPoints, B_, C_),
                new TheoremObject(TheoremObjectSignature.LineGivenByPoints, B_, O_),
                new TheoremObject(TheoremObjectSignature.LineGivenByPoints, C_, O_),
                new TheoremObject(TheoremObjectSignature.LineGivenByPoints, B_, C_)
            });

            // Create potential consequence theorem objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var D = new ConstructedConfigurationObject(Midpoint, A, B);
            var E = new ConstructedConfigurationObject(Midpoint, A, C);
            var O = new ConstructedConfigurationObject(Circumcenter, A, D, E);

            // Create the consequence configuration
            var consequenceConfiguration = Configuration.DeriveFromObjects(LooseObjectsLayout.ScaleneAcuteAngledTriangled, O);

            // Create the potential consequence theorem
            var consequenceTheorem = new Theorem(consequenceConfiguration, TheoremType.EqualAngles, new[]
            {
                new TheoremObject(TheoremObjectSignature.LineGivenByPoints, A, D),
                new TheoremObject(TheoremObjectSignature.LineGivenByPoints, A, O),
                new TheoremObject(TheoremObjectSignature.LineGivenByPoints, D, O),
                new TheoremObject(TheoremObjectSignature.LineGivenByPoints, A, D)
            });

            // Analyze
            var result = _analyzer.Analyze(originalTheorem, consequenceTheorem);

            // Assert
            result.IsSubtheorem.Should().BeTrue();
            result.UsedEqualities.Should().BeNullOrEmpty();
        }

        [Test]
        public void Test_Circumcircle_Equal_Angles_Theorem_With_Implicit_Definition()
        {
            // Create original configuration objects
            var A_ = new LooseConfigurationObject(Point);
            var B_ = new LooseConfigurationObject(Point);
            var C_ = new LooseConfigurationObject(Point);
            var O_ = new ConstructedConfigurationObject(Circumcenter, A_, B_, C_);

            // Create the original configuration
            var originalConfiguration = Configuration.DeriveFromObjects(O_);

            // Create the original theorem
            var originalTheorem = new Theorem(originalConfiguration, TheoremType.EqualAngles, new[]
            {
                new TheoremObject(TheoremObjectSignature.LineGivenByPoints, B_, C_),
                new TheoremObject(TheoremObjectSignature.LineGivenByPoints, B_, O_),
                new TheoremObject(TheoremObjectSignature.LineGivenByPoints, C_, O_),
                new TheoremObject(TheoremObjectSignature.LineGivenByPoints, B_, C_)
            });

            // Create potential consequence theorem objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var D = new ConstructedConfigurationObject(Midpoint, A, B);
            var E = new ConstructedConfigurationObject(Midpoint, A, C);
            var o1 = new ConstructedConfigurationObject(PerpendicularBisector, A, D);
            var o2 = new ConstructedConfigurationObject(PerpendicularBisector, D, E);
            var O = new ConstructedConfigurationObject(IntersectionOfLines, o1, o2);

            // Create the consequence configuration
            var consequenceConfiguration = Configuration.DeriveFromObjects(LooseObjectsLayout.ScaleneAcuteAngledTriangled, O);

            // Create the potential consequence theorem
            var consequenceTheorem = new Theorem(consequenceConfiguration, TheoremType.EqualAngles, new[]
            {
                new TheoremObject(TheoremObjectSignature.LineGivenByPoints, A, D, B),
                new TheoremObject(TheoremObjectSignature.LineGivenByPoints, A, O),
                new TheoremObject(TheoremObjectSignature.LineGivenByPoints, D, O),
                new TheoremObject(TheoremObjectSignature.LineGivenByPoints, A, B, D)
            });

            // Analyze
            var result = _analyzer.Analyze(originalTheorem, consequenceTheorem);

            // Assert
            result.IsSubtheorem.Should().BeTrue();
            result.UsedEqualities.Should().BeEquivalentTo(new (ConfigurationObject, ConfigurationObject)[]
            {
                // We need to discover O is the circumcircle of ABC
                (O, new ConstructedConfigurationObject(Circumcircle, A, B, C))
            });
        }

        [Test]
        public void Test_Concurrent_Medians_Theorem_With_Direct_Definitions()
        {
            // Create original configuration objects
            var A_ = new LooseConfigurationObject(Point);
            var B_ = new LooseConfigurationObject(Point);
            var C_ = new LooseConfigurationObject(Point);
            var D_ = new ConstructedConfigurationObject(Midpoint, B_, C_);
            var E_ = new ConstructedConfigurationObject(Midpoint, A_, C_);
            var F_ = new ConstructedConfigurationObject(Midpoint, C_, A_);

            // Create original configuration
            var originalConfiguration = Configuration.DeriveFromObjects(D_, E_, F_);

            // Create the original theorem
            var originalTheorem = new Theorem(originalConfiguration, TheoremType.ConcurrentObjects, new[]
            {
                new TheoremObject(TheoremObjectSignature.LineGivenByPoints, A_, D_),
                new TheoremObject(TheoremObjectSignature.LineGivenByPoints, B_, E_),
                new TheoremObject(TheoremObjectSignature.LineGivenByPoints, C_, F_)
            });

            // Create potential consequence theorem objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var D = new ConstructedConfigurationObject(Midpoint, B, C);
            var E = new ConstructedConfigurationObject(Midpoint, A, C);
            var F = new ConstructedConfigurationObject(Midpoint, A, D);
            var G = new ConstructedConfigurationObject(Midpoint, D, C);

            // Create the consequence configuration
            var consequenceConfiguration = Configuration.DeriveFromObjects(LooseObjectsLayout.ScaleneAcuteAngledTriangled, G);

            // Create the potential consequence theorem
            var consequenceTheorem = new Theorem(consequenceConfiguration, TheoremType.ConcurrentObjects, new[]
            {
                new TheoremObject(TheoremObjectSignature.LineGivenByPoints, A, G),
                new TheoremObject(TheoremObjectSignature.LineGivenByPoints, B, F),
                new TheoremObject(TheoremObjectSignature.LineGivenByPoints, C, D)
            });

            // Analyze
            var result = _analyzer.Analyze(originalTheorem, consequenceTheorem);

            // Assert
            result.IsSubtheorem.Should().BeTrue();
            result.UsedEqualities.Should().BeNullOrEmpty();
        }

        [Test]
        public void Test_Concurrent_Medians_Theorem_With_Implicit_Definitions()
        {
            // Create original configuration objects
            var A_ = new LooseConfigurationObject(Point);
            var B_ = new LooseConfigurationObject(Point);
            var C_ = new LooseConfigurationObject(Point);
            var D_ = new ConstructedConfigurationObject(Midpoint, B_, C_);
            var E_ = new ConstructedConfigurationObject(Midpoint, A_, C_);
            var F_ = new ConstructedConfigurationObject(Midpoint, C_, A_);

            // Create the original configuration
            var originalConfiguration = Configuration.DeriveFromObjects(D_, E_, F_);

            // Create the original theorem
            var originalTheorem = new Theorem(originalConfiguration, TheoremType.ConcurrentObjects, new[]
            {
                new TheoremObject(TheoremObjectSignature.LineGivenByPoints, A_, D_),
                new TheoremObject(TheoremObjectSignature.LineGivenByPoints, B_, E_),
                new TheoremObject(TheoremObjectSignature.LineGivenByPoints, C_, F_)
            });

            // Create potential consequence theorem objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var O = new ConstructedConfigurationObject(Circumcircle, A, B, C);
            var D = new ConstructedConfigurationObject(PerpendicularProjectionOnLineFromPoints, O, B, C);
            var E = new ConstructedConfigurationObject(PerpendicularProjectionOnLineFromPoints, O, C, A);
            var F = new ConstructedConfigurationObject(PerpendicularProjectionOnLineFromPoints, O, A, B);

            // Create the consequence configuration
            var consequenceConfiguration = Configuration.DeriveFromObjects(LooseObjectsLayout.ScaleneAcuteAngledTriangled, F);

            // Create the potential consequence theorem
            var consequenceTheorem = new Theorem(consequenceConfiguration, TheoremType.ConcurrentObjects, new[]
            {
                new TheoremObject(TheoremObjectSignature.LineGivenByPoints, A, D),
                new TheoremObject(TheoremObjectSignature.LineGivenByPoints, B, E),
                new TheoremObject(TheoremObjectSignature.LineGivenByPoints, C, F)
            });

            // Analyze
            var result = _analyzer.Analyze(originalTheorem, consequenceTheorem);

            // Assert
            result.IsSubtheorem.Should().BeTrue();
            result.UsedEqualities.Should().BeEquivalentTo(new (ConfigurationObject, ConfigurationObject)[]
            {
                // We need to discover the projections of O on lines of ABC are midpoints 
                (D, new ConstructedConfigurationObject(Midpoint, B, C)),
                (E, new ConstructedConfigurationObject(Midpoint, C, A)),
                (F, new ConstructedConfigurationObject(Midpoint, A, B))
            });
        }

        [Test]
        public void Test_Collinear_Points_Because_Of_Homothety()
        {
            // Create original configuration objects
            var A_ = new LooseConfigurationObject(Point);
            var B_ = new LooseConfigurationObject(Point);
            var C_ = new LooseConfigurationObject(Point);
            var D_ = new ConstructedConfigurationObject(RandomPointOnLineSegment, A_, B_);
            var l_ = new ConstructedConfigurationObject(ParallelLineToLineFromPoints, D_, B_, C_);
            var E_ = new ConstructedConfigurationObject(IntersectionOfLineAndLineFromPoints, l_, A_, C_);
            var H1_ = new ConstructedConfigurationObject(Orthocenter, A_, B_, C_);
            var H2_ = new ConstructedConfigurationObject(Orthocenter, A_, D_, E_);

            // Create the original configuration
            var originalConfiguration = Configuration.DeriveFromObjects(H2_);

            // Create the original theorem
            var originalTheorem = new Theorem(originalConfiguration, TheoremType.CollinearPoints, A_, H1_, H2_);

            // Create potential consequence theorem objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var D = new ConstructedConfigurationObject(Midpoint, A, B);
            var E = new ConstructedConfigurationObject(Midpoint, A, C);
            var H1 = new ConstructedConfigurationObject(Orthocenter, A, B, C);
            var H2 = new ConstructedConfigurationObject(Orthocenter, A, D, E);

            // Create the consequence configuration
            var consequenceConfiguration = Configuration.DeriveFromObjects(LooseObjectsLayout.ScaleneAcuteAngledTriangled, H1, H2);

            // Create the potential consequence theorem
            var consequenceTheorem = new Theorem(consequenceConfiguration, TheoremType.CollinearPoints, A, H1, H2);

            // Analyze
            var result = _analyzer.Analyze(originalTheorem, consequenceTheorem);

            // Assert
            result.IsSubtheorem.Should().BeTrue();
            result.UsedEqualities.Should().BeNullOrEmpty();
        }

        [Test]
        public void Test_Concyclic_Points_With_Second_Intersection()
        {
            // Create original configuration objects
            var A_ = new LooseConfigurationObject(Point);
            var B_ = new LooseConfigurationObject(Point);
            var C_ = new LooseConfigurationObject(Point);
            var D_ = new LooseConfigurationObject(Point);
            var E_ = new LooseConfigurationObject(Point);
            var X_ = new ConstructedConfigurationObject(SecondIntersectionOfTwoCircumcircles, A_, B_, C_, D_, E_);

            // Create the original configuration
            var originalConfiguration = Configuration.DeriveFromObjects(X_);

            // Create the original theorem
            var originalTheorem = new Theorem(originalConfiguration, TheoremType.ConcyclicPoints, A_, B_, C_, X_);

            // Create potential consequence theorem objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var D = new ConstructedConfigurationObject(Midpoint, A, B);
            var E = new ConstructedConfigurationObject(Midpoint, A, C);
            var X = new ConstructedConfigurationObject(SecondIntersectionOfTwoCircumcircles, A, E, D, C, D);

            // Create the consequence configuration
            var consequenceConfiguration = Configuration.DeriveFromObjects(LooseObjectsLayout.ScaleneAcuteAngledTriangled, X);

            // Create the potential consequence theorem
            var consequenceTheorem = new Theorem(consequenceConfiguration, TheoremType.ConcyclicPoints, A, B, E, X);

            // Analyze
            var result = _analyzer.Analyze(originalTheorem, consequenceTheorem);

            // Assert
            result.IsSubtheorem.Should().BeTrue();
            result.UsedEqualities.Should().BeNullOrEmpty();
        }

        [Test]
        public void Test_Tangent_Circles_Because_Of_Homothety()
        {
            // Create original configuration objects
            var A_ = new LooseConfigurationObject(Point);
            var B_ = new LooseConfigurationObject(Point);
            var C_ = new LooseConfigurationObject(Point);
            var D_ = new ConstructedConfigurationObject(RandomPointOnLineSegment, A_, B_);
            var l_ = new ConstructedConfigurationObject(ParallelLineToLineFromPoints, D_, B_, C_);
            var E_ = new ConstructedConfigurationObject(IntersectionOfLineAndLineFromPoints, l_, A_, C_);

            // Create the original configuration
            var originalConfiguration = Configuration.DeriveFromObjects(E_);

            // Create the original theorem
            var originalTheorem = new Theorem(originalConfiguration, TheoremType.TangentCircles, new[]
            {
                new TheoremObject(TheoremObjectSignature.CircleGivenByPoints, A_, D_, E_),
                new TheoremObject(TheoremObjectSignature.CircleGivenByPoints, A_, B_, C_)
            });

            // Create potential consequence theorem objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var D = new ConstructedConfigurationObject(Midpoint, A, B);
            var E = new ConstructedConfigurationObject(Midpoint, A, C);

            // Create the consequence configuration
            var consequenceConfiguration = Configuration.DeriveFromObjects(LooseObjectsLayout.ScaleneAcuteAngledTriangled, D, E);

            // Create the potential consequence theorem
            var consequenceTheorem = new Theorem(consequenceConfiguration, TheoremType.TangentCircles, new[]
            {
                new TheoremObject(TheoremObjectSignature.CircleGivenByPoints, A, D, E),
                new TheoremObject(TheoremObjectSignature.CircleGivenByPoints, A, B, C)
            });

            // Analyze
            var result = _analyzer.Analyze(originalTheorem, consequenceTheorem);

            // Assert
            result.IsSubtheorem.Should().BeTrue();
            result.UsedEqualities.Should().BeNullOrEmpty();
        }

        [Test]
        public void Test_Two_Parallel_Lines_Perpendicular_To_Some_Line()
        {
            // Create original configuration objects
            var A_ = new LooseConfigurationObject(Point);
            var B_ = new LooseConfigurationObject(Point);
            var C_ = new LooseConfigurationObject(Point);
            var D_ = new LooseConfigurationObject(Point);
            var l1_ = new ConstructedConfigurationObject(PerpendicularLine, A_, B_, C_);
            var l2_ = new ConstructedConfigurationObject(PerpendicularLine, D_, B_, C_);

            // Create the original configuration
            var originalConfiguration = Configuration.DeriveFromObjects(l1_, l2_);

            // Create the original theorem
            var originalTheorem = new Theorem(originalConfiguration, TheoremType.ParallelLines, l1_, l2_);

            // Create potential consequence theorem objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var G = new ConstructedConfigurationObject(Centroid, A, B, C);
            var A0 = new ConstructedConfigurationObject(PerpendicularProjection, A, B, C);
            var G0 = new ConstructedConfigurationObject(PerpendicularProjection, G, B, C);

            // Create the consequence configuration
            var consequenceConfiguration = Configuration.DeriveFromObjects(LooseObjectsLayout.ScaleneAcuteAngledTriangled, G0);

            // Create the potential consequence theorem
            var consequenceTheorem = new Theorem(consequenceConfiguration, TheoremType.ParallelLines, new[]
            {
                new TheoremObject(TheoremObjectSignature.LineGivenByPoints, A, A0),
                new TheoremObject(TheoremObjectSignature.LineGivenByPoints, G, G0)
            });

            // Analyze
            var result = _analyzer.Analyze(originalTheorem, consequenceTheorem);

            // Assert
            result.IsSubtheorem.Should().BeTrue();
            result.UsedEqualities.Should().BeNullOrEmpty();
        }

        [Test]
        public void Test_Orthocentric_Situation_Has_Perpendicular_Lines()
        {
            // Create original configuration objects
            var A_ = new LooseConfigurationObject(Point);
            var B_ = new LooseConfigurationObject(Point);
            var C_ = new LooseConfigurationObject(Point);
            var H_ = new ConstructedConfigurationObject(Orthocenter, A_, B_, C_);

            // Create the original configuration
            var originalConfiguration = Configuration.DeriveFromObjects(H_);

            // Create the original theorem
            var originalTheorem = new Theorem(originalConfiguration, TheoremType.PerpendicularLines, new[]
            {
                new TheoremObject(TheoremObjectSignature.LineGivenByPoints, A_, H_),
                new TheoremObject(TheoremObjectSignature.LineGivenByPoints, B_, C_)
            });

            // Create potential consequence theorem objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var A0 = new ConstructedConfigurationObject(PerpendicularProjection, A, B, C);
            var D = new ConstructedConfigurationObject(SecondIntersectionOfCircleAndLineFromPoints, A, A0, B, C);
            var H = new ConstructedConfigurationObject(ReflectionInLineFromPoints, D, B, C);

            // Create the consequence configuration
            var consequenceConfiguration = Configuration.DeriveFromObjects(LooseObjectsLayout.ScaleneAcuteAngledTriangled, H);

            // Create the potential consequence theorem
            var consequenceTheorem = new Theorem(consequenceConfiguration, TheoremType.ParallelLines, new[]
            {
                new TheoremObject(TheoremObjectSignature.LineGivenByPoints, B, H),
                new TheoremObject(TheoremObjectSignature.LineGivenByPoints, A, C)
            });

            // Analyze
            var result = _analyzer.Analyze(originalTheorem, consequenceTheorem);

            // Assert
            result.IsSubtheorem.Should().BeTrue();
            result.UsedEqualities.Should().BeNullOrEmpty();
        }

        [Test]
        public void Test_Miquel_Theorem()
        {
            // Create original configuration objects
            var A_ = new LooseConfigurationObject(Point);
            var B_ = new LooseConfigurationObject(Point);
            var C_ = new LooseConfigurationObject(Point);
            var D_ = new ConstructedConfigurationObject(RandomPointOnLineSegment, A_, B_);
            var E_ = new ConstructedConfigurationObject(RandomPointOnLineSegment, B_, C_);
            var F_ = new ConstructedConfigurationObject(RandomPointOnLineSegment, C_, A_);

            // Create the original configuration
            var originalConfiguration = Configuration.DeriveFromObjects(D_, E_, F_);

            // Create the original theorem
            var originalTheorem = new Theorem(originalConfiguration, TheoremType.PerpendicularLines, new[]
            {
                new TheoremObject(TheoremObjectSignature.CircleGivenByPoints, A_, E_, F_),
                new TheoremObject(TheoremObjectSignature.CircleGivenByPoints, B_, F_, D_),
                new TheoremObject(TheoremObjectSignature.CircleGivenByPoints, C_, D_, E_),
            });

            // Create potential consequence theorem objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var D = new ConstructedConfigurationObject(Midpoint, A, B);
            var E = new ConstructedConfigurationObject(Midpoint, B, C);
            var F = new ConstructedConfigurationObject(Midpoint, C, A);

            // Create the consequence configuration
            var consequenceConfiguration = Configuration.DeriveFromObjects(LooseObjectsLayout.ScaleneAcuteAngledTriangled, D, E, F);

            // Create the potential consequence theorem
            var consequenceTheorem = new Theorem(consequenceConfiguration, TheoremType.ConcurrentObjects, new[]
            {
                new TheoremObject(TheoremObjectSignature.CircleGivenByPoints, A, E, F),
                new TheoremObject(TheoremObjectSignature.CircleGivenByPoints, B, F, D),
                new TheoremObject(TheoremObjectSignature.CircleGivenByPoints, C, D, E)
            });

            // Analyze
            var result = _analyzer.Analyze(originalTheorem, consequenceTheorem);

            // Assert
            result.IsSubtheorem.Should().BeTrue();
            result.UsedEqualities.Should().BeNullOrEmpty();
        }

        [Test]
        public void Test_Reflected_Tangent_Line_Is_Still_Tangent_To_Circle()
        {
            // Create original configuration objects
            var c_ = new LooseConfigurationObject(Circle);
            var A_ = new ConstructedConfigurationObject(RandomPointOnCircle, c_);
            var O_ = new ConstructedConfigurationObject(CenterOfCircle, c_);
            var t_ = new ConstructedConfigurationObject(PerpendicularLineAtPointOfLine, A_, O_);
            var P1_ = new ConstructedConfigurationObject(RandomPointOnLine, t_);
            var P2_ = new ConstructedConfigurationObject(RandomPointOnLine, t_);
            var Q1_ = new ConstructedConfigurationObject(PointReflection, P1_, O_);
            var Q2_ = new ConstructedConfigurationObject(PointReflection, P2_, O_);

            // Create the original configuration
            var originalConfiguration = Configuration.DeriveFromObjects(Q1_, Q2_);

            // Create the original theorem
            var originalTheorem = new Theorem(originalConfiguration, TheoremType.LineTangentToCircle, new[]
            {
                new TheoremObject(TheoremObjectSignature.LineGivenByPoints, Q1_, Q2_),
                new TheoremObject(c_)
            });

            // Create potential consequence theorem objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var I = new ConstructedConfigurationObject(Incenter, A, B, C);
            var D = new ConstructedConfigurationObject(PerpendicularProjection, I, B, C);
            var E = new ConstructedConfigurationObject(PerpendicularProjection, I, C, A);
            var F = new ConstructedConfigurationObject(PerpendicularProjection, I, A, B);
            var B1 = new ConstructedConfigurationObject(PointReflection, A, I);
            var C1 = new ConstructedConfigurationObject(PointReflection, B, I);

            // Create the consequence configuration
            var consequenceConfiguration = Configuration.DeriveFromObjects(LooseObjectsLayout.ScaleneAcuteAngledTriangled, B1, C1);

            // Create the potential consequence theorem
            var consequenceTheorem = new Theorem(consequenceConfiguration, TheoremType.LineTangentToCircle, new[]
            {
                new TheoremObject(TheoremObjectSignature.LineGivenByPoints, B1, C1),
                new TheoremObject(TheoremObjectSignature.CircleGivenByPoints, D, E, F)
            });

            // Analyze
            var result = _analyzer.Analyze(originalTheorem, consequenceTheorem);

            // Assert
            result.IsSubtheorem.Should().BeTrue();
            result.UsedEqualities.Should().BeNullOrEmpty();
        }

        [Test]
        public void Test_Radical_Axis_Theorem()
        {
            // Create original configuration objects
            var c1_ = new LooseConfigurationObject(Circle);
            var c2_ = new LooseConfigurationObject(Circle);
            var A_ = new LooseConfigurationObject(Point);
            var B_ = new LooseConfigurationObject(Point);
            var P1_ = new ConstructedConfigurationObject(RandomPointOnCircle, c1_);
            var P2_ = new ConstructedConfigurationObject(RandomPointOnCircle, c1_);
            var Q1_ = new ConstructedConfigurationObject(RandomPointOnCircle, c2_);
            var Q2_ = new ConstructedConfigurationObject(SecondIntersectionOfTwoCircumcircles, Q1_, A_, B_, P1_, P2_);

            // Create the original configuration
            var originalConfiguration = new Configuration(LooseObjectsLayout.IntersectingCircles, A_, B_, P1_, P2_, Q1_, Q2_);

            // Create the original theorem
            var originalTheorem = new Theorem(originalConfiguration, TheoremType.ConcurrentObjects, new[]
            {
                new TheoremObject(TheoremObjectSignature.LineGivenByPoints, A_, B_),
                new TheoremObject(TheoremObjectSignature.LineGivenByPoints, P1_, P2_),
                new TheoremObject(TheoremObjectSignature.LineGivenByPoints, Q1_, Q2_)
            });

            // Create potential consequence theorem objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var H = new ConstructedConfigurationObject(Orthocenter, A, B, C);
            var H1 = new ConstructedConfigurationObject(ReflectionInLineFromPoints, H, A, B);
            var D = new ConstructedConfigurationObject(SecondIntersectionOfTwoCircumcircles, H, H1, A, B, C);

            // Create the consequence configuration
            var consequenceConfiguration = Configuration.DeriveFromObjects(LooseObjectsLayout.ScaleneAcuteAngledTriangled, D);

            // Create the potential consequence theorem
            var consequenceTheorem = new Theorem(consequenceConfiguration, TheoremType.LineTangentToCircle, new[]
            {
                new TheoremObject(TheoremObjectSignature.LineGivenByPoints, A, H1),
                new TheoremObject(TheoremObjectSignature.LineGivenByPoints, H, D),
                new TheoremObject(TheoremObjectSignature.LineGivenByPoints, B, C),
            });

            // Analyze
            var result = _analyzer.Analyze(originalTheorem, consequenceTheorem);

            // Assert
            result.IsSubtheorem.Should().BeTrue();
            result.UsedEqualities.Should().BeNullOrEmpty();
        }
    }
}
