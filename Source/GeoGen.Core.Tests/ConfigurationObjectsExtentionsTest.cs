using FluentAssertions;
using NUnit.Framework;
using static GeoGen.Core.ConfigurationObjectType;
using static GeoGen.Core.PredefinedConstructionType;

namespace GeoGen.Core.Tests
{
    /// <summary>
    /// The test class for <see cref="ConfigurationObject"/> and its derived types.
    /// </summary>
    [TestFixture]
    public class ConfigurationObjectsExtentionsTest
    {
        [Test]
        public void Test_Getting_Defining_Objects()
        {
            // Initialize some objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var D = new ConstructedConfigurationObject(Midpoint, A, B);
            var E = new ConstructedConfigurationObject(Midpoint, B, C);
            var F = new ConstructedConfigurationObject(Midpoint, C, A);
            var G = new ConstructedConfigurationObject(ComposedConstructions.IntersectionOfLinesFromPoints, B, F, C, D);
            var c = new ConstructedConfigurationObject(Circumcircle, E, F, G);
            var X = new ConstructedConfigurationObject(ComposedConstructions.Circumcenter, A, B, C);
            var Y = new ConstructedConfigurationObject(Midpoint, X, G);

            // Check loose object
            A.GetDefiningObjects().Should().BeEquivalentTo(A);

            // Check simple constructed object
            D.GetDefiningObjects().Should().BeEquivalentTo(A, B, D)
                .And.ContainInOrder(A, D)
                .And.HaveElementSucceeding(B, D);

            // Check complex constructed object
            c.GetDefiningObjects().Should().BeEquivalentTo(A, B, C, D, E, F, G, c)
                .And.ContainInOrder(E, c)
                .And.ContainInOrder(F, c)
                .And.ContainInOrder(G, c)
                .And.ContainInOrder(B, E)
                .And.ContainInOrder(C, E)
                .And.ContainInOrder(C, F)
                .And.ContainInOrder(A, F)
                .And.ContainInOrder(B, G)
                .And.ContainInOrder(F, G)
                .And.ContainInOrder(C, G)
                .And.ContainInOrder(D, G)
                .And.ContainInOrder(A, D)
                .And.ContainInOrder(B, D);

            // Check calling on more objects
            new[] { X, Y, c }.GetDefiningObjects().Should().BeEquivalentTo(A, B, C, D, E, F, G, c, X, Y)
                .And.ContainInOrder(E, c)
                .And.ContainInOrder(F, c)
                .And.ContainInOrder(G, c)
                .And.ContainInOrder(B, E)
                .And.ContainInOrder(C, E)
                .And.ContainInOrder(C, F)
                .And.ContainInOrder(A, F)
                .And.ContainInOrder(B, G)
                .And.ContainInOrder(F, G)
                .And.ContainInOrder(C, G)
                .And.ContainInOrder(D, G)
                .And.ContainInOrder(A, D)
                .And.ContainInOrder(B, D)
                .And.ContainInOrder(A, X)
                .And.ContainInOrder(B, X)
                .And.ContainInOrder(C, X)
                .And.ContainInOrder(X, Y)
                .And.ContainInOrder(G, Y);
        }
    }
}