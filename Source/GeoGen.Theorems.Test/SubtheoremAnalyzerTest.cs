using FluentAssertions;
using GeoGen.ConsoleTest;
using GeoGen.Constructor;
using GeoGen.Core;
using GeoGen.Generator;
using GeoGen.Utilities;
using NUnit.Framework;
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

            // Get the factories
            var factory1 = IoC.Get<IConfigurationObjectsContainerFactory>();
            var factory2 = IoC.Get<IContextualContainerFactory>(new ContextualContainerSettings
            {
                MaximalNumberOfAttemptsToReconstruct = 100
            });

            // Create the analyzer
            _analyzer = new SubtheoremAnalyzer(constructor, factory1, factory2);
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
            result.UsedFacts.Should().BeNullOrEmpty();
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
            result.UsedFacts.Should().BeNullOrEmpty();
        }

        [Test]
        public void Test_Circumcircle_Equal_Angles_Theorem_With_Direct_Definition()
        {
            // Create original configuration objects
            var A_ = new LooseConfigurationObject(Point);
            var B_ = new LooseConfigurationObject(Point);
            var C_ = new LooseConfigurationObject(Point);
            var O_ = new ConstructedConfigurationObject(Circumcenter, A_, B_, C_);

            var x = new TheoremObjectWithPoints(Line, B_, C_);

            // Create the original configuration
            var originalConfiguration = Configuration.DeriveFromObjects(O_);

            // Create the original theorem
            var originalTheorem = new Theorem(originalConfiguration, TheoremType.EqualAngles, new[]
            {
                new TheoremObjectWithPoints(Line, B_, C_),
                new TheoremObjectWithPoints(Line, B_, O_),
                new TheoremObjectWithPoints(Line, C_, O_),
                new TheoremObjectWithPoints(Line, B_, C_)
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
                new TheoremObjectWithPoints(Line, A, D),
                new TheoremObjectWithPoints(Line, A, O),
                new TheoremObjectWithPoints(Line, D, O),
                new TheoremObjectWithPoints(Line, A, D)
            });

            // Analyze
            var result = _analyzer.Analyze(originalTheorem, consequenceTheorem);

            // Assert
            result.IsSubtheorem.Should().BeTrue();
            result.UsedEqualities.Should().BeNullOrEmpty();
            result.UsedFacts.Should().BeNullOrEmpty();
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
                new TheoremObjectWithPoints(Line, B_, C_),
                new TheoremObjectWithPoints(Line, B_, O_),
                new TheoremObjectWithPoints(Line, C_, O_),
                new TheoremObjectWithPoints(Line, B_, C_)
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
                new TheoremObjectWithPoints(Line, A, D, B),
                new TheoremObjectWithPoints(Line, A, O),
                new TheoremObjectWithPoints(Line, D, O),
                new TheoremObjectWithPoints(Line, A, B, D)
            });

            // Analyze
            var result = _analyzer.Analyze(originalTheorem, consequenceTheorem);

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
        public void Test_Concurrent_Medians_Theorem_With_Direct_Definitions()
        {
            // Create original configuration objects
            var A_ = new LooseConfigurationObject(Point);
            var B_ = new LooseConfigurationObject(Point);
            var C_ = new LooseConfigurationObject(Point);
            var D_ = new ConstructedConfigurationObject(Midpoint, B_, C_);
            var E_ = new ConstructedConfigurationObject(Midpoint, C_, A_);
            var F_ = new ConstructedConfigurationObject(Midpoint, A_, B_);

            // Create original configuration
            var originalConfiguration = Configuration.DeriveFromObjects(D_, E_, F_);

            // Create the original theorem
            var originalTheorem = new Theorem(originalConfiguration, TheoremType.ConcurrentObjects, new[]
            {
                new TheoremObjectWithPoints(Line, A_, D_),
                new TheoremObjectWithPoints(Line, B_, E_),
                new TheoremObjectWithPoints(Line, C_, F_)
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
            var consequenceConfiguration = Configuration.DeriveFromObjects(LooseObjectsLayout.ScaleneAcuteAngledTriangled, E, F, G);

            // Create the potential consequence theorem
            var consequenceTheorem = new Theorem(consequenceConfiguration, TheoremType.ConcurrentObjects, new[]
            {
                new TheoremObjectWithPoints(Line, A, G),
                new TheoremObjectWithPoints(Line, D, E),
                new TheoremObjectWithPoints(Line, C, F)
            });

            // Analyze
            var result = _analyzer.Analyze(originalTheorem, consequenceTheorem);

            // Assert
            result.IsSubtheorem.Should().BeTrue();
            result.UsedEqualities.Should().BeNullOrEmpty();
            result.UsedFacts.Should().BeNullOrEmpty();
        }

        [Test]
        public void Test_Concurrent_Medians_Theorem_With_Implicit_Definitions()
        {
            // Create original configuration objects
            var A_ = new LooseConfigurationObject(Point);
            var B_ = new LooseConfigurationObject(Point);
            var C_ = new LooseConfigurationObject(Point);
            var D_ = new ConstructedConfigurationObject(Midpoint, B_, C_);
            var E_ = new ConstructedConfigurationObject(Midpoint, C_, A_);
            var F_ = new ConstructedConfigurationObject(Midpoint, A_, B_);

            // Create the original configuration
            var originalConfiguration = Configuration.DeriveFromObjects(D_, E_, F_);

            // Create the original theorem
            var originalTheorem = new Theorem(originalConfiguration, TheoremType.ConcurrentObjects, new[]
            {
                new TheoremObjectWithPoints(Line, A_, D_),
                new TheoremObjectWithPoints(Line, B_, E_),
                new TheoremObjectWithPoints(Line, C_, F_)
            });

            // Create potential consequence theorem objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var O = new ConstructedConfigurationObject(Circumcenter, A, B, C);
            var D = new ConstructedConfigurationObject(PerpendicularProjectionOnLineFromPoints, O, B, C);
            var E = new ConstructedConfigurationObject(PerpendicularProjectionOnLineFromPoints, O, C, A);
            var F = new ConstructedConfigurationObject(PerpendicularProjectionOnLineFromPoints, O, A, B);

            // Create the consequence configuration
            var consequenceConfiguration = Configuration.DeriveFromObjects(LooseObjectsLayout.ScaleneAcuteAngledTriangled, D, E, F);

            // Create the potential consequence theorem
            var consequenceTheorem = new Theorem(consequenceConfiguration, TheoremType.ConcurrentObjects, new[]
            {
                new TheoremObjectWithPoints(Line, A, D),
                new TheoremObjectWithPoints(Line, B, E),
                new TheoremObjectWithPoints(Line, C, F)
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
            result.UsedFacts.Should().BeNullOrEmpty();
        }

        [Test]
        public void Test_Reflection_Of_H_In_Midpoint_Of_BC_Is_Point_Opposite_To_A_On_Circumcircle_ABC()
        {
            // Create original configuration objects
            var A_ = new LooseConfigurationObject(Point);
            var M_ = new LooseConfigurationObject(Point);
            var B_ = new ConstructedConfigurationObject(PointReflection, A_, M_);

            // Create the original configuration
            var originalConfiguration = Configuration.DeriveFromObjects(B_);

            // Create the original theorem
            var originalTheorem = new Theorem(originalConfiguration, TheoremType.CollinearPoints, A_, B_, M_);

            // Create potential consequence theorem objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var H = new ConstructedConfigurationObject(Orthocenter, A, B, C);
            var O = new ConstructedConfigurationObject(Circumcenter, A, B, C);
            var D = new ConstructedConfigurationObject(PointReflection, A, O);
            var M = new ConstructedConfigurationObject(Midpoint, B, C);

            // Create the consequence configuration
            var consequenceConfiguration = Configuration.DeriveFromObjects(LooseObjectsLayout.ScaleneAcuteAngledTriangled, M, D, H);

            // Create the potential consequence theorem
            var consequenceTheorem = new Theorem(consequenceConfiguration, TheoremType.CollinearPoints, H, D, M);

            // Analyze
            var result = _analyzer.Analyze(originalTheorem, consequenceTheorem);

            // Assert
            result.IsSubtheorem.Should().BeTrue();
            result.UsedFacts.Should().BeNullOrEmpty();
            result.UsedEqualities.Should().HaveCount(1, "Exactly one of the previous two potential reasons should hold true");
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
            var D = new ConstructedConfigurationObject(Midpoint, B, C);
            var E = new ConstructedConfigurationObject(Midpoint, A, B);
            var X = new ConstructedConfigurationObject(SecondIntersectionOfTwoCircumcircles, A, E, D, B, C);

            // Create the consequence configuration
            var consequenceConfiguration = Configuration.DeriveFromObjects(LooseObjectsLayout.ScaleneAcuteAngledTriangled, X);

            // Create the potential consequence theorem
            var consequenceTheorem = new Theorem(consequenceConfiguration, TheoremType.ConcyclicPoints, A, X, D, E);

            // Analyze
            var result = _analyzer.Analyze(originalTheorem, consequenceTheorem);

            // Assert
            result.IsSubtheorem.Should().BeTrue();
            result.UsedEqualities.Should().BeEmpty();
            result.UsedFacts.Should().BeNullOrEmpty();
        }

        [Test]
        public void Test_Tangent_Circles_Because_Of_Homothety()
        {
            // Create original configuration objects
            var A_ = new LooseConfigurationObject(Point);
            var B_ = new LooseConfigurationObject(Point);
            var C_ = new LooseConfigurationObject(Point);
            var D_ = new LooseConfigurationObject(Point);
            var E_ = new ConstructedConfigurationObject(IntersectionOfLinesFromPoints, A_, C_, B_, D_);

            // Create the original configuration
            var originalConfiguration = new Configuration(LooseObjectsLayout.Trapezoid, A_, B_, C_, D_, E_);

            // Create the original theorem
            var originalTheorem = new Theorem(originalConfiguration, TheoremType.TangentCircles, new[]
            {
                new TheoremObjectWithPoints(Circle, E_, A_, B_),
                new TheoremObjectWithPoints(Circle, E_, C_, D_)
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
                new TheoremObjectWithPoints(Circle, A, D, E),
                new TheoremObjectWithPoints(Circle, A, B, C)
            });

            // Analyze
            var result = _analyzer.Analyze(originalTheorem, consequenceTheorem);

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
                new Theorem(consequenceConfiguration, TheoremType.ParallelLines, new[]
                {
                    new TheoremObjectWithPoints(Line, B, C),
                    new TheoremObjectWithPoints(Line, D, E)
                })
            }).Should().BeTrue();
        }

        [Test]
        public void Test_Two_Parallel_Lines_Perpendicular_To_Some_Line()
        {
            // Create original configuration objects
            var A_ = new LooseConfigurationObject(Point);
            var B_ = new LooseConfigurationObject(Point);
            var C_ = new LooseConfigurationObject(Point);
            var D_ = new LooseConfigurationObject(Point);
            var l1_ = new ConstructedConfigurationObject(PerpendicularLineToLineFromPoints, A_, C_, D_);
            var l2_ = new ConstructedConfigurationObject(PerpendicularLineToLineFromPoints, B_, C_, D_);

            // Create the original configuration
            var originalConfiguration = Configuration.DeriveFromObjects(l1_, l2_);

            // Create the original theorem
            var originalTheorem = new Theorem(originalConfiguration, TheoremType.ParallelLines, l1_, l2_);

            // Create potential consequence theorem objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var G = new ConstructedConfigurationObject(Centroid, A, B, C);
            var l1 = new ConstructedConfigurationObject(PerpendicularLineToLineFromPoints, A, B, C);
            var l2 = new ConstructedConfigurationObject(PerpendicularLineToLineFromPoints, G, B, C);

            // Create the consequence configuration
            var consequenceConfiguration = Configuration.DeriveFromObjects(LooseObjectsLayout.ScaleneAcuteAngledTriangled, l1, l2);

            // Create the potential consequence theorem
            var consequenceTheorem = new Theorem(consequenceConfiguration, TheoremType.ParallelLines, l1, l2);

            // Analyze
            var result = _analyzer.Analyze(originalTheorem, consequenceTheorem);

            // Assert
            result.IsSubtheorem.Should().BeTrue();
            result.UsedEqualities.Should().BeNullOrEmpty();
            result.UsedFacts.Should().BeNullOrEmpty();
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
                new TheoremObjectWithPoints(Line, A_, H_),
                new TheoremObjectWithPoints(Line, B_, C_)
            });

            // Create potential consequence theorem objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var A0 = new ConstructedConfigurationObject(PerpendicularProjectionOnLineFromPoints, A, B, C);
            var D = new ConstructedConfigurationObject(SecondIntersectionOfCircleAndLineFromPoints, A, A0, B, C);
            var H = new ConstructedConfigurationObject(ReflectionInLineFromPoints, D, B, C);

            // Create the consequence configuration
            var consequenceConfiguration = Configuration.DeriveFromObjects(LooseObjectsLayout.ScaleneAcuteAngledTriangled, H);

            // Create the potential consequence theorem
            var consequenceTheorem = new Theorem(consequenceConfiguration, TheoremType.PerpendicularLines, new[]
            {
                new TheoremObjectWithPoints(Line, B, H),
                new TheoremObjectWithPoints(Line, A, C)
            });

            // Analyze
            var result = _analyzer.Analyze(originalTheorem, consequenceTheorem);

            // Assert
            result.IsSubtheorem.Should().BeTrue();
            result.UsedEqualities.Should().BeEquivalentTo(new (ConfigurationObject, ConfigurationObject)[]
            {
                // We need to discover O is the orthocenter of ABC
                (H, new ConstructedConfigurationObject(Orthocenter, A, B, C))
            });
            result.UsedFacts.Should().BeNullOrEmpty();
        }

        [Test]
        public void Test_Miquel_Theorem()
        {
            // Create original configuration objects
            var A_ = new LooseConfigurationObject(Point);
            var B_ = new LooseConfigurationObject(Point);
            var C_ = new LooseConfigurationObject(Point);
            var D_ = new ConstructedConfigurationObject(RandomPointOnLineFromPoints, A_, B_);
            var E_ = new ConstructedConfigurationObject(RandomPointOnLineFromPoints, B_, C_);
            var F_ = new ConstructedConfigurationObject(RandomPointOnLineFromPoints, C_, A_);

            // Create the original configuration
            var originalConfiguration = Configuration.DeriveFromObjects(D_, E_, F_);

            // Create the original theorem
            var originalTheorem = new Theorem(originalConfiguration, TheoremType.ConcurrentObjects, new[]
            {
                new TheoremObjectWithPoints(Circle, A_, E_, F_),
                new TheoremObjectWithPoints(Circle, B_, F_, D_),
                new TheoremObjectWithPoints(Circle, C_, D_, E_),
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
                new TheoremObjectWithPoints(Circle, A, E, F),
                new TheoremObjectWithPoints(Circle, B, F, D),
                new TheoremObjectWithPoints(Circle, C, D, E)
            });

            // Analyze
            var result = _analyzer.Analyze(originalTheorem, consequenceTheorem);

            // Assert
            result.IsSubtheorem.Should().BeTrue();
            result.UsedEqualities.Should().BeNullOrEmpty();
            result.UsedFacts.Should().BeNullOrEmpty();
        }

        [Test]
        public void Test_Reflected_Tangent_Line_Is_Still_Tangent_To_Circle()
        {
            // Create original configuration objects
            var c_ = new LooseConfigurationObject(Circle);
            var P1_ = new LooseConfigurationObject(Point);
            var P2_ = new LooseConfigurationObject(Point);
            var O_ = new ConstructedConfigurationObject(CenterOfCircle, c_);
            var Q1_ = new ConstructedConfigurationObject(PointReflection, P1_, O_);
            var Q2_ = new ConstructedConfigurationObject(PointReflection, P2_, O_);

            // Create the original configuration
            var originalConfiguration = Configuration.DeriveFromObjects(LooseObjectsLayout.CircleAndItsTangentLine, c_, P1_, P2_, Q1_, Q2_);

            // Create the original theorem
            var originalTheorem = new Theorem(originalConfiguration, TheoremType.LineTangentToCircle, new[]
            {
                new TheoremObjectWithPoints(Line, Q1_, Q2_),
                new TheoremObjectWithPoints(c_)
            });

            // Create potential consequence theorem objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var c = new ConstructedConfigurationObject(Incircle, A, B, C);
            var I = new ConstructedConfigurationObject(Incenter, A, B, C);
            var B1 = new ConstructedConfigurationObject(PointReflection, B, I);
            var C1 = new ConstructedConfigurationObject(PointReflection, C, I);

            // Create the consequence configuration
            var consequenceConfiguration = Configuration.DeriveFromObjects(LooseObjectsLayout.ScaleneAcuteAngledTriangled, B1, C1, c);

            // Create the potential consequence theorem
            var consequenceTheorem = new Theorem(consequenceConfiguration, TheoremType.LineTangentToCircle, new[]
            {
                new TheoremObjectWithPoints(Line, B1, C1),
                new TheoremObjectWithPoints(Circle, c)
            });

            // Analyze
            var result = _analyzer.Analyze(originalTheorem, consequenceTheorem);

            // Assert
            result.IsSubtheorem.Should().BeTrue();
            result.UsedEqualities.Should().BeEquivalentTo(new (ConfigurationObject, ConfigurationObject)[]
            {
                // We need to discover I is really the center of the incircle
                (I, new ConstructedConfigurationObject(CenterOfCircle, c))
            });
            result.UsedFacts.ToSet(Theorem.EquivalencyComparer).SetEquals(new[]
            {
                new Theorem(consequenceConfiguration, TheoremType.LineTangentToCircle, new[]
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
            // Create original configuration objects
            var A_ = new LooseConfigurationObject(Point);
            var B_ = new LooseConfigurationObject(Point);
            var C_ = new LooseConfigurationObject(Point);
            var D_ = new LooseConfigurationObject(Point);
            var E_ = new LooseConfigurationObject(Point);
            var F_ = new LooseConfigurationObject(Point);

            // Create the original configuration
            var originalConfiguration = new Configuration(LooseObjectsLayout.ThreeCyclicQuadrilatersOnSixPoints, A_, B_, C_, D_, E_, F_);

            // Create the original theorem
            var originalTheorem = new Theorem(originalConfiguration, TheoremType.ConcurrentObjects, new[]
            {
                new TheoremObjectWithPoints(Line, A_, B_),
                new TheoremObjectWithPoints(Line, C_, D_),
                new TheoremObjectWithPoints(Line, E_, F_)
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
            var consequenceTheorem = new Theorem(consequenceConfiguration, TheoremType.ConcurrentObjects, new[]
            {
                new TheoremObjectWithPoints(Line, A, H1),
                new TheoremObjectWithPoints(Line, H, D),
                new TheoremObjectWithPoints(Line, B, C),
            });

            // Analyze
            var result = _analyzer.Analyze(originalTheorem, consequenceTheorem);

            // Assert
            result.IsSubtheorem.Should().BeTrue();
            result.UsedEqualities.Should().BeNullOrEmpty();
            result.UsedFacts.ToSet(Theorem.EquivalencyComparer).SetEquals(new[]
            {
                new Theorem(consequenceConfiguration, TheoremType.ConcyclicPoints, A, H1, H, D),
                new Theorem(consequenceConfiguration, TheoremType.ConcyclicPoints, A, H1, B, C),
                new Theorem(consequenceConfiguration, TheoremType.ConcyclicPoints, H, D, B, C),
            })
            .Should().BeTrue();
        }

        [Test]
        public void Test_Equal_Equals_Because_Of_Parallel_Lines()
        {
            // Create original configuration objects
            var A_ = new LooseConfigurationObject(Point);
            var B_ = new LooseConfigurationObject(Point);
            var C_ = new LooseConfigurationObject(Point);
            var D_ = new LooseConfigurationObject(Point);
            var E_ = new ConstructedConfigurationObject(RandomPoint);
            var F_ = new ConstructedConfigurationObject(RandomPoint);

            // Create the original configuration
            var originalConfiguration = new Configuration(LooseObjectsLayout.Trapezoid, A_, B_, C_, D_, E_, F_);

            // Create the original theorem
            var originalTheorem = new Theorem(originalConfiguration, TheoremType.EqualAngles, new[]
            {
                new TheoremObjectWithPoints(Line, A_, B_),
                new TheoremObjectWithPoints(Line, E_, F_),
                new TheoremObjectWithPoints(Line, C_, D_),
                new TheoremObjectWithPoints(Line, E_, F_)
            });

            // Create potential consequence theorem objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var D = new ConstructedConfigurationObject(Midpoint, A, B);
            var E = new ConstructedConfigurationObject(Midpoint, C, D);
            var F = new ConstructedConfigurationObject(Midpoint, A, E);

            // Create the consequence configuration
            var consequenceConfiguration = Configuration.DeriveFromObjects(LooseObjectsLayout.ScaleneAcuteAngledTriangled, F);

            // Create the potential consequence theorem
            var consequenceTheorems = new[]
            {
                new Theorem(consequenceConfiguration, TheoremType.EqualAngles, new[]
                {
                    new TheoremObjectWithPoints(Line, B, E),
                    new TheoremObjectWithPoints(Line, C, F),
                    new TheoremObjectWithPoints(Line, D, F),
                    new TheoremObjectWithPoints(Line, C, F)
                }),
                new Theorem(consequenceConfiguration, TheoremType.EqualAngles, new[]
                {
                    new TheoremObjectWithPoints(Line, A, C),
                    new TheoremObjectWithPoints(Line, B, E),
                    new TheoremObjectWithPoints(Line, A, C),
                    new TheoremObjectWithPoints(Line, D, F)
                }),
                new Theorem(consequenceConfiguration, TheoremType.EqualAngles, new[]
                {
                    new TheoremObjectWithPoints(Line, B, C),
                    new TheoremObjectWithPoints(Line, B, E),
                    new TheoremObjectWithPoints(Line, B, C),
                    new TheoremObjectWithPoints(Line, D, F)
                })
            };

            // Analyze all theorems
            consequenceTheorems.ForEach(consequenceTheorem =>
            {
                // Analyze
                var result = _analyzer.Analyze(originalTheorem, consequenceTheorem);

                // Assert
                result.IsSubtheorem.Should().BeTrue();
                result.UsedEqualities.Should().BeNullOrEmpty();
                result.UsedFacts.ToSet(Theorem.EquivalencyComparer).SetEquals(new[]
                {
                    new Theorem(consequenceConfiguration, TheoremType.ParallelLines, new[]
                    {
                        new TheoremObjectWithPoints(Line, B, E),
                        new TheoremObjectWithPoints(Line, D, F)
                    })
                }).Should().BeTrue();
            });
        }
    }
}
