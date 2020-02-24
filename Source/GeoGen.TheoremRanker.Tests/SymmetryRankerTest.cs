using FluentAssertions;
using GeoGen.Core;
using NUnit.Framework;
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
        public void Test_With_No_Symmetry()
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
            var rank = new SymmetryRanker().Rank(theorem, configuration, allTheorems: null).ranking;

            // Assert
            rank.Should().Be(0);
        }

        [Test]
        public void Test_With_Partial_Symmetry()
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
            var rank = new SymmetryRanker().Rank(theorem, configuration, allTheorems: null).ranking;

            // Assert
            rank.Should().Be(0.2);
        }

        [Test]
        public void Test_With_Full_Symmetry()
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
            var rank = new SymmetryRanker().Rank(theorem, configuration, allTheorems: null).ranking;

            // Assert
            rank.Should().Be(1);
        }
    }
}
