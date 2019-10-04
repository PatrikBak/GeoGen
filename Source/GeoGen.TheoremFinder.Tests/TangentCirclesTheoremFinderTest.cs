using FluentAssertions;
using GeoGen.Core;
using GeoGen.Utilities;
using NUnit.Framework;
using static GeoGen.Core.ComposedConstructions;
using static GeoGen.Core.ConfigurationObjectType;
using static GeoGen.Core.LooseObjectsLayout;
using static GeoGen.Core.PredefinedConstructionType;
using static GeoGen.Core.TheoremType;

namespace GeoGen.TheoremFinder.Tests
{
    /// <summary>
    /// The test class for <see cref="TangentCirclesTheoremFinder"/>
    /// </summary>
    [TestFixture]
    public class TangentCirclesTheoremFinderTest : TypedTheoremFinderTestBase<TangentCirclesTheoremFinder>
    {
        #region Instance

        /// <summary>
        /// Gets the instance of the finder that doesn't exclude tangencies inside the picture.
        /// </summary>
        private static TangentCirclesTheoremFinder WithoutExcludingTangenciesInsidePicture => new TangentCirclesTheoremFinder(new TangentCirclesTheoremFinderSettings
        {
            ExcludeTangencyInsidePicture = false
        });

        /// <summary>
        /// Gets the instance of the finder that excludes tangencies inside the picture.
        /// </summary>
        private static TangentCirclesTheoremFinder WithExcludingTangenciesInsidePicture => new TangentCirclesTheoremFinder(new TangentCirclesTheoremFinderSettings
        {
            ExcludeTangencyInsidePicture = true
        });

        #endregion

        [Test]
        public void Test_Triangle_With_Feurbach_Circle_And_Excircles_Without_Excluding_Tangencies()
        {
            // Create the objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var k = new ConstructedConfigurationObject(Excircle, B, A, C);
            var l = new ConstructedConfigurationObject(Excircle, C, A, B);
            var i = new ConstructedConfigurationObject(Incircle, A, B, C);
            var P = new ConstructedConfigurationObject(Midpoint, B, C);
            var Q = new ConstructedConfigurationObject(Midpoint, C, A);
            var R = new ConstructedConfigurationObject(Midpoint, A, B);

            // Create the configuration
            var configuration = Configuration.DeriveFromObjects(ThreePoints, k, l, i, P, Q, R);

            // Run
            var (newTheorems, allTheorems) = FindTheorems(configuration, () => WithoutExcludingTangenciesInsidePicture);

            // Assert new theorems
            newTheorems.OrderlessEquals(new[]
            {
                new Theorem(TangentCircles, new[]
                {
                    new CircleTheoremObject(P, Q, R),
                    new CircleTheoremObject(i),
                }),
                new Theorem(TangentCircles, new[]
                {
                    new CircleTheoremObject(P, Q, R),
                    new CircleTheoremObject(k),
                }),
                new Theorem(TangentCircles, new[]
                {
                    new CircleTheoremObject(P, Q, R),
                    new CircleTheoremObject(l)
                }),
                new Theorem(TangentCircles, new[]
                {
                    new CircleTheoremObject(A, B, C),
                    new CircleTheoremObject(A, Q, R)
                }),
                new Theorem(TangentCircles, new[]
                {
                    new CircleTheoremObject(A, B, C),
                    new CircleTheoremObject(B, R, P)
                }),
            })
            .Should().BeTrue();

            // Assert all theorems
            allTheorems.OrderlessEquals(new[]
            {
                new Theorem(TangentCircles, new[]
                {
                    new CircleTheoremObject(P, Q, R),
                    new CircleTheoremObject(i),
                }),
                new Theorem(TangentCircles, new[]
                {
                    new CircleTheoremObject(P, Q, R),
                    new CircleTheoremObject(k),
                }),
                new Theorem(TangentCircles, new[]
                {
                    new CircleTheoremObject(P, Q, R),
                    new CircleTheoremObject(l)
                }),
                new Theorem(TangentCircles, new[]
                {
                    new CircleTheoremObject(A, B, C),
                    new CircleTheoremObject(A, Q, R)
                }),
                new Theorem(TangentCircles, new[]
                {
                    new CircleTheoremObject(A, B, C),
                    new CircleTheoremObject(B, R, P)
                }),
                new Theorem(TangentCircles, new[]
                {
                    new CircleTheoremObject(A, B, C),
                    new CircleTheoremObject(C, P, Q)
                }),
            })
            .Should().BeTrue();
        }

        [Test]
        public void Test_Triangle_With_Feurbach_Circle_And_Excircles_With_Excluding_Tangencies()
        {
            // Create the objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var k = new ConstructedConfigurationObject(Excircle, B, A, C);
            var l = new ConstructedConfigurationObject(Excircle, C, A, B);
            var i = new ConstructedConfigurationObject(Incircle, A, B, C);
            var P = new ConstructedConfigurationObject(Midpoint, B, C);
            var Q = new ConstructedConfigurationObject(Midpoint, C, A);
            var R = new ConstructedConfigurationObject(Midpoint, A, B);

            // Create the configuration
            var configuration = Configuration.DeriveFromObjects(ThreePoints, k, l, i, P, Q, R);

            // Run
            var (newTheorems, allTheorems) = FindTheorems(configuration, () => WithExcludingTangenciesInsidePicture);

            // Assert new theorems
            newTheorems.OrderlessEquals(new[]
            {
                new Theorem(TangentCircles, new[]
                {
                    new CircleTheoremObject(P, Q, R),
                    new CircleTheoremObject(i),
                }),
                new Theorem(TangentCircles, new[]
                {
                    new CircleTheoremObject(P, Q, R),
                    new CircleTheoremObject(k),
                }),
                new Theorem(TangentCircles, new[]
                {
                    new CircleTheoremObject(P, Q, R),
                    new CircleTheoremObject(l)
                })
            })
            .Should().BeTrue();

            // Assert all theorems
            allTheorems.OrderlessEquals(new[]
            {
                new Theorem(TangentCircles, new[]
                {
                    new CircleTheoremObject(P, Q, R),
                    new CircleTheoremObject(i),
                }),
                new Theorem(TangentCircles, new[]
                {
                    new CircleTheoremObject(P, Q, R),
                    new CircleTheoremObject(k),
                }),
                new Theorem(TangentCircles, new[]
                {
                    new CircleTheoremObject(P, Q, R),
                    new CircleTheoremObject(l)
                })
            })
            .Should().BeTrue();
        }
    }
}
