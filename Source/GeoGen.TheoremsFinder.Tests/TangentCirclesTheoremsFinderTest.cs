using FluentAssertions;
using GeoGen.Core;
using GeoGen.Utilities;
using NUnit.Framework;
using static GeoGen.Core.ComposedConstructions;
using static GeoGen.Core.ConfigurationObjectType;
using static GeoGen.Core.LooseObjectsLayout;
using static GeoGen.Core.PredefinedConstructionType;
using static GeoGen.Core.TheoremType;

namespace GeoGen.TheoremsFinder.Tests
{
    /// <summary>
    /// The test class for <see cref="TangentCirclesTheoremsFinder"/>
    /// </summary>
    [TestFixture]
    public class TangentCirclesTheoremsFinderTest : TheoremsFinderTestBase<TangentCirclesTheoremsFinder>
    {
        [Test]
        public void Test_Triangle_With_Feurbach_Circle_And_Excircles()
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
            var (newTheorems, allTheorems) = FindTheorems(configuration);

            // Assert new theorems
            newTheorems.ToSet().SetEquals(new[]
            {
                new Theorem(configuration, TangentCircles, new[]
                {
                    new CircleTheoremObject(P, Q, R),
                    new CircleTheoremObject(i),
                }),
                new Theorem(configuration, TangentCircles, new[]
                {
                    new CircleTheoremObject(P, Q, R),
                    new CircleTheoremObject(k),
                }),
                new Theorem(configuration, TangentCircles, new[]
                {
                    new CircleTheoremObject(P, Q, R),
                    new CircleTheoremObject(l)
                }),
                new Theorem(configuration, TangentCircles, new[]
                {
                    new CircleTheoremObject(A, B, C),
                    new CircleTheoremObject(A, Q, R)
                }),
                new Theorem(configuration, TangentCircles, new[]
                {
                    new CircleTheoremObject(A, B, C),
                    new CircleTheoremObject(B, R, P)
                }),
            })
            .Should().BeTrue();

            // Assert all theorems
            allTheorems.ToSet().SetEquals(new[]
            {
                new Theorem(configuration, TangentCircles, new[]
                {
                    new CircleTheoremObject(P, Q, R),
                    new CircleTheoremObject(i),
                }),
                new Theorem(configuration, TangentCircles, new[]
                {
                    new CircleTheoremObject(P, Q, R),
                    new CircleTheoremObject(k),
                }),
                new Theorem(configuration, TangentCircles, new[]
                {
                    new CircleTheoremObject(P, Q, R),
                    new CircleTheoremObject(l)
                }),
                new Theorem(configuration, TangentCircles, new[]
                {
                    new CircleTheoremObject(A, B, C),
                    new CircleTheoremObject(A, Q, R)
                }),
                new Theorem(configuration, TangentCircles, new[]
                {
                    new CircleTheoremObject(A, B, C),
                    new CircleTheoremObject(B, R, P)
                }),
                new Theorem(configuration, TangentCircles, new[]
                {
                    new CircleTheoremObject(A, B, C),
                    new CircleTheoremObject(C, P, Q)
                }),
            })
            .Should().BeTrue();
        }
    }
}
