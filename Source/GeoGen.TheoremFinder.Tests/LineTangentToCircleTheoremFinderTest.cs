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
    /// The test class for <see cref="LineTangentToCircleTheoremFinder"/>.
    /// </summary>
    [TestFixture]
    public class LineTangentToCircleTheoremFinderTest : TypedTheoremFinderTestBase<LineTangentToCircleTheoremFinder>
    {
        #region Instance

        /// <summary>
        /// Gets the instance of the finder that doesn't exclude tangencies inside the picture.
        /// </summary>
        private static LineTangentToCircleTheoremFinder WithoutExcludingTangenciesInsidePicture => new LineTangentToCircleTheoremFinder(
            // Create the right settings
            new LineTangentToCircleTheoremFinderSettings(excludeTangencyInsidePicture: false));

        /// <summary>
        /// Gets the instance of the finder that excludes tangencies inside the picture.
        /// </summary>
        private static LineTangentToCircleTheoremFinder WithExcludingTangenciesInsidePicture => new LineTangentToCircleTheoremFinder(
            // Create the right settings
            new LineTangentToCircleTheoremFinderSettings(excludeTangencyInsidePicture: true));

        #endregion

        [Test]
        public void Test_Right_Triangle_With_Perpendicular_Projections_Without_Excluding_Tangencies()
        {
            // Create the objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var D = new ConstructedConfigurationObject(PerpendicularProjectionOnLineFromPoints, A, B, C);
            var E = new ConstructedConfigurationObject(PerpendicularProjectionOnLineFromPoints, D, A, B);

            // Create the configuration
            var configuration = Configuration.DeriveFromObjects(RightTriangle, A, E);

            // Run
            var (newTheorems, allTheorems) = FindTheorems(configuration, () => WithoutExcludingTangenciesInsidePicture);

            // Assert new theorems
            newTheorems.OrderlessEquals(new[]
            {
                new Theorem(LineTangentToCircle, new TheoremObject[]
                {
                    new LineTheoremObject(D, C),
                    new CircleTheoremObject(A, E, D)
                }),
                new Theorem(LineTangentToCircle, new TheoremObject[]
                {
                    new LineTheoremObject(B, D),
                    new CircleTheoremObject(A, E, D)
                }),
                new Theorem(LineTangentToCircle, new TheoremObject[]
                {
                    new LineTheoremObject(B, C),
                    new CircleTheoremObject(A, E, D)
                }),
                new Theorem(LineTangentToCircle, new TheoremObject[]
                {
                    new LineTheoremObject(A, D),
                    new CircleTheoremObject(B, E, D)
                })
            })
            .Should().BeTrue();

            // Assert all theorems
            allTheorems.OrderlessEquals(new[]
            {
                new Theorem(LineTangentToCircle, new TheoremObject[]
                {
                    new LineTheoremObject(D, C),
                    new CircleTheoremObject(A, E, D)
                }),
                new Theorem(LineTangentToCircle, new TheoremObject[]
                {
                    new LineTheoremObject(B, D),
                    new CircleTheoremObject(A, E, D)
                }),
                new Theorem(LineTangentToCircle, new TheoremObject[]
                {
                    new LineTheoremObject(B, C),
                    new CircleTheoremObject(A, E, D)
                }),
                new Theorem(LineTangentToCircle, new TheoremObject[]
                {
                    new LineTheoremObject(A, D),
                    new CircleTheoremObject(B, E, D)
                }),
                new Theorem(LineTangentToCircle, new TheoremObject[]
                {
                    new LineTheoremObject(A, C),
                    new CircleTheoremObject(A, B, D)
                }),
                new Theorem(LineTangentToCircle, new TheoremObject[]
                {
                    new LineTheoremObject(A, B),
                    new CircleTheoremObject(A, C, D)
                }),
                new Theorem(LineTangentToCircle, new TheoremObject[]
                {
                    new LineTheoremObject(A, E),
                    new CircleTheoremObject(A, C, D)
                }),
                new Theorem(LineTangentToCircle, new TheoremObject[]
                {
                    new LineTheoremObject(B, E),
                    new CircleTheoremObject(A, C, D)
                })
            })
            .Should().BeTrue();
        }

        [Test]
        public void Test_Excircles_With_Excluding_Tangencies()
        {
            // Create the objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var eC = new ConstructedConfigurationObject(Excircle, C, A, B);
            var Ib = new ConstructedConfigurationObject(Excenter, B, A, C);
            var P = new ConstructedConfigurationObject(PerpendicularProjectionOnLineFromPoints, Ib, A, C);
            var eB = new ConstructedConfigurationObject(Excircle, B, A, C);

            // Create the configuration
            var configuration = Configuration.DeriveFromObjects(Triangle, Ib, P, eC, eB);

            // Run
            var (newTheorems, allTheorems) = FindTheorems(configuration, () => WithExcludingTangenciesInsidePicture);

            // Assert new theorems
            newTheorems.OrderlessEquals(new[]
            {
                new Theorem(LineTangentToCircle, new TheoremObject[]
                {
                    new LineTheoremObject(A, B),
                    new CircleTheoremObject(eB)
                }),
                new Theorem(LineTangentToCircle, new TheoremObject[]
                {
                    new LineTheoremObject(B, C),
                    new CircleTheoremObject(eB)
                })
            })
            .Should().BeTrue();

            // Assert all theorems
            allTheorems.OrderlessEquals(new[]
            {
                new Theorem(LineTangentToCircle, new TheoremObject[]
                {
                    new LineTheoremObject(A, B),
                    new CircleTheoremObject(eB)
                }),
                new Theorem(LineTangentToCircle, new TheoremObject[]
                {
                    new LineTheoremObject(B, C),
                    new CircleTheoremObject(eB)
                }),
                new Theorem(LineTangentToCircle, new TheoremObject[]
                {
                    new LineTheoremObject(A, B),
                    new CircleTheoremObject(eC)
                }),
                new Theorem(LineTangentToCircle, new TheoremObject[]
                {
                    new LineTheoremObject(B, C),
                    new CircleTheoremObject(eC)
                }),
                new Theorem(LineTangentToCircle, new TheoremObject[]
                {
                    new LineTheoremObject(A, C),
                    new CircleTheoremObject(eC)
                }),
                new Theorem(LineTangentToCircle, new TheoremObject[]
                {
                    new LineTheoremObject(P, C),
                    new CircleTheoremObject(eC)
                }),
                new Theorem(LineTangentToCircle, new TheoremObject[]
                {
                    new LineTheoremObject(A, P),
                    new CircleTheoremObject(eC)
                })
            })
            .Should().BeTrue();
        }
    }
}
