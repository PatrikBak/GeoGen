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
    public class ConfigurationObjectTest
    {
        [Test]
        public void Test_Getting_Internal_Objects()
        {
            // Initialize some objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var D = new ConstructedConfigurationObject(Midpoint, A, B);
            var E = new ConstructedConfigurationObject(Midpoint, B, C);
            var F = new ConstructedConfigurationObject(Midpoint, C, A);
            var G = new ConstructedConfigurationObject(ComposedConstructions.IntersectionOfLinesFromPoints(), B, F, C, D);
            var c = new ConstructedConfigurationObject(Circumcircle, E, F, G);
            var l = new ConstructedConfigurationObject(ComposedConstructions.PerpendicularLineToLineFromPoints(), A, E, D);

            // Identify
            IdentifiedObject.Identify(A, B, C, D, E, F, G, c, l);

            // Check loose objects
            A.GetInternalObjects().Should().BeEmpty();
            B.GetInternalObjects().Should().BeEmpty();
            C.GetInternalObjects().Should().BeEmpty();

            // Check constructed objects
            D.GetInternalObjects().Should().BeEquivalentTo(A, B);
            E.GetInternalObjects().Should().BeEquivalentTo(B, C);
            F.GetInternalObjects().Should().BeEquivalentTo(A, C);
            G.GetInternalObjects().Should().BeEquivalentTo(A, B, C, D, F);
            c.GetInternalObjects().Should().BeEquivalentTo(A, B, C, D, E, F, G);
            l.GetInternalObjects().Should().BeEquivalentTo(A, B, C, D, E);
        }
    }
}