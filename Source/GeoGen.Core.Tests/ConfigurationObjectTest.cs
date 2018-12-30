using FluentAssertions;
using NUnit.Framework;
using static GeoGen.Core.ConfigurationObjectType;
using static GeoGen.Core.PredefinedConstructionType;
using static GeoGen.Utilities.TestHelpers.ConfigurationObjects;
using static GeoGen.Utilities.TestHelpers.IdentifiedObjects;

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
            var D = Construct(MidpointFromPoints, A, B);
            var E = Construct(MidpointFromPoints, B, C);
            var F = Construct(MidpointFromPoints, C, A);
            var G = Construct(IntersectionOfLinesFromPoints, B, F, C, D);
            var c = Construct(CircumcircleFromPoints, E, F, G);
            var l = Construct(PerpendicularLineFromPoints, A, E, D);

            // Identify
            Identify(A, B, C, D, E, F, G, c, l);

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