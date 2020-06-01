using FluentAssertions;
using GeoGen.Core;
using GeoGen.Utilities;
using NUnit.Framework;
using static GeoGen.Core.ComposedConstructions;
using static GeoGen.Core.ConfigurationObjectType;
using static GeoGen.Core.LooseObjectLayout;
using static GeoGen.Core.PredefinedConstructions;
using static GeoGen.Core.TheoremType;

namespace GeoGen.TheoremRanker.Tests
{
    /// <summary>
    /// The test class for <see cref="SymmetryRanker"/>.
    /// </summary>
    [TestFixture]
    public class SymmetryRankerTest
    {
        [Test]
        public void Test_Triangle_With_No_Symmetry()
        {
            // Prepare the objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var D = new ConstructedConfigurationObject(PointReflection, A, B);

            // Prepare the configuration
            var configuration = Configuration.DeriveFromObjects(Triangle, A, B, C, D);

            // Prepare the theorem
            var theorem = new Theorem(EqualLineSegments, new[]
            {
                new LineSegmentTheoremObject(D, A),
                new LineSegmentTheoremObject(D, B)
            });

            // Rank
            var rank = new SymmetryRanker().Rank(theorem, configuration, allTheorems: null);

            // Assert
            rank.Should().Be(0);
        }

        [Test]
        public void Test_Triangle_With_Partial_Symmetry()
        {
            // Prepare the objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var M = new ConstructedConfigurationObject(Midpoint, A, B);
            var N = new ConstructedConfigurationObject(Midpoint, A, C);

            // Prepare the configuration
            var configuration = Configuration.DeriveFromObjects(Triangle, A, B, C, M, N);

            // Prepare the theorem
            var theorem = new Theorem(ParallelLines, new[]
            {
                new LineTheoremObject(B, C),
                new LineTheoremObject(M, N)
            });

            // Rank
            var rank = new SymmetryRanker().Rank(theorem, configuration, allTheorems: null);

            // Assert
            rank.Rounded().Should().Be((1d / 3).Rounded());
        }

        [Test]
        public void Test_Triangle_With_Full_Symmetry()
        {
            // Prepare the objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var P = new ConstructedConfigurationObject(Midpoint, B, C);
            var Q = new ConstructedConfigurationObject(Midpoint, C, A);
            var R = new ConstructedConfigurationObject(Midpoint, A, B);

            // Prepare the configuration
            var configuration = Configuration.DeriveFromObjects(Triangle, A, B, C, P, Q, R);

            // Prepare the theorem
            var theorem = new Theorem(ConcurrentLines, new[]
            {
                new LineTheoremObject(A, P),
                new LineTheoremObject(B, Q),
                new LineTheoremObject(C, R)
            });

            // Rank
            var rank = new SymmetryRanker().Rank(theorem, configuration, allTheorems: null);

            // Assert
            rank.Should().Be(1);
        }

        [Test]
        public void Test_Quadrilateral_With_No_Symmetry()
        {
            // Prepare the objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var D = new LooseConfigurationObject(Point);
            var P = new ConstructedConfigurationObject(Incenter, A, B, D);
            var Q = new ConstructedConfigurationObject(Centroid, C, B, D);
            var R = new ConstructedConfigurationObject(Incenter, B, C, Q);

            // Prepare the configuration
            var configuration = Configuration.DeriveFromObjects(Quadrilateral, P, Q, R);

            // Prepare the theorem
            // NOTE: I did not really try to make it be true in general
            var theorem = new Theorem(PerpendicularLines, new[]
            {
                new LineTheoremObject(A,R),
                new LineTheoremObject(B, D)
            });

            // Rank
            var rank = new SymmetryRanker().Rank(theorem, configuration, allTheorems: null);

            // Assert
            rank.Should().Be(0);
        }

        [Test]
        public void Test_Quadrilateral_With_Weak_Partial_Symmetry()
        {
            // Prepare the objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var D = new LooseConfigurationObject(Point);
            var P = new ConstructedConfigurationObject(Incenter, A, B, D);
            var Q = new ConstructedConfigurationObject(Centroid, C, B, D);

            // Prepare the configuration
            var configuration = Configuration.DeriveFromObjects(Quadrilateral, P, Q);

            // Prepare the theorem
            // NOTE: I did not really try to make it be true in general
            var theorem = new Theorem(PerpendicularLines, new[]
            {
                new LineTheoremObject(P, Q),
                new LineTheoremObject(B, D)
            });

            // Rank
            var rank = new SymmetryRanker().Rank(theorem, configuration, allTheorems: null);

            // Assert
            rank.Rounded().Should().Be((1d / 9).Rounded());
        }

        [Test]
        public void Test_Quadrilateral_With_Medium_Partial_Symmetry()
        {
            // Prepare the objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var D = new LooseConfigurationObject(Point);
            var P = new ConstructedConfigurationObject(Incenter, A, B, D);
            var Q = new ConstructedConfigurationObject(Incenter, C, B, D);

            // Prepare the configuration
            var configuration = Configuration.DeriveFromObjects(Quadrilateral, P, Q);

            // Prepare the theorem
            // NOTE: I did not really try to make it be true in general
            var theorem = new Theorem(PerpendicularLines, new[]
            {
                new LineTheoremObject(P, Q),
                new LineTheoremObject(B, D)
            });

            // Rank
            var rank = new SymmetryRanker().Rank(theorem, configuration, allTheorems: null);

            // Assert
            rank.Rounded().Should().Be((3d / 9).Rounded());
        }

        [Test]
        public void Test_Quadrilateral_With_Very_Strong_Partial_Symmetry()
        {
            // Prepare the objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var D = new LooseConfigurationObject(Point);
            var P = new ConstructedConfigurationObject(Midpoint, A, B);
            var Q = new ConstructedConfigurationObject(Midpoint, B, C);
            var R = new ConstructedConfigurationObject(Midpoint, C, D);
            var S = new ConstructedConfigurationObject(Midpoint, D, A);

            // Prepare the configuration
            var configuration = Configuration.DeriveFromObjects(Quadrilateral, P, Q, R, S);

            // Prepare the theorem
            // NOTE: I did not really try to make it be true in general
            var theorem = new Theorem(PerpendicularLines, new[]
            {
                new LineTheoremObject(P, R),
                new LineTheoremObject(Q, S)
            });

            // Rank
            var rank = new SymmetryRanker().Rank(theorem, configuration, allTheorems: null);

            // Assert
            rank.Rounded().Should().Be((5d / 9).Rounded());
        }

        [Test]
        public void Test_Quadrilateral_With_Full_Symmetry()
        {
            // Prepare the objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var D = new LooseConfigurationObject(Point);
            var P = new ConstructedConfigurationObject(Incenter, A, B, C);
            var Q = new ConstructedConfigurationObject(Incenter, B, C, D);
            var R = new ConstructedConfigurationObject(Incenter, C, D, A);
            var S = new ConstructedConfigurationObject(Incenter, D, A, B);

            // Prepare the configuration
            var configuration = Configuration.DeriveFromObjects(Quadrilateral, P, Q, R, S);

            // Prepare the theorem
            // NOTE: I did not really try to make it be true in general
            var theorem = new Theorem(ConcyclicPoints, P, Q, R, S);

            // Rank
            var rank = new SymmetryRanker().Rank(theorem, configuration, allTheorems: null);

            // Assert
            rank.Should().Be(1);
        }
    }
}