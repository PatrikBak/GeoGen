using FluentAssertions;
using GeoGen.Core;
using GeoGen.Utilities;
using NUnit.Framework;
using static GeoGen.Core.ComposedConstructions;
using static GeoGen.Core.ConfigurationObjectType;
using static GeoGen.Core.LooseObjectsLayout;
using static GeoGen.Core.TheoremType;

namespace GeoGen.TheoremsFinder.Tests
{
    /// <summary>
    /// The test class for <see cref="EqualAnglesTheoremsFinder"/>.
    /// </summary>
    [TestFixture]
    public class EqualAnglesTheoremsFinderTest : TheoremsFinderTestBase<EqualAnglesTheoremsFinder>
    {
        [Test]
        public void Test_Triangle_With_Many_Midpoints()
        {
            // Create the objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var I = new ConstructedConfigurationObject(Incenter, A, B, C);
            var J = new ConstructedConfigurationObject(Incenter, I, B, C);

            // Create the configuration
            var configuration = Configuration.DeriveFromObjects(ThreePoints, J);

            // Run
            var (newTheorems, allTheorems) = FindTheorems(configuration);

            // Assert new theorems
            newTheorems.OrderlessEquals(new[]
            {
                new Theorem(configuration, EqualAngles, new[]
                {
                    new AngleTheoremObject(new LineTheoremObject(I, J), new LineTheoremObject(B, I)),
                    new AngleTheoremObject(new LineTheoremObject(I, J), new LineTheoremObject(C, I))
                }),
                new Theorem(configuration, EqualAngles, new[]
                {
                    new AngleTheoremObject(new LineTheoremObject(B, J), new LineTheoremObject(B, C)),
                    new AngleTheoremObject(new LineTheoremObject(B, J), new LineTheoremObject(B, I))
                }),
                new Theorem(configuration, EqualAngles, new[]
                {
                    new AngleTheoremObject(new LineTheoremObject(C, J), new LineTheoremObject(B, C)),
                    new AngleTheoremObject(new LineTheoremObject(C, J), new LineTheoremObject(C, I))
                })
            })
            .Should().BeTrue();

            // Assert all theorems
            allTheorems.OrderlessEquals(new[]
            {
                new Theorem(configuration, EqualAngles, new[]
                {
                    new AngleTheoremObject(new LineTheoremObject(I, J), new LineTheoremObject(B, I)),
                    new AngleTheoremObject(new LineTheoremObject(I, J), new LineTheoremObject(C, I))
                }),
                new Theorem(configuration, EqualAngles, new[]
                {
                    new AngleTheoremObject(new LineTheoremObject(B, J), new LineTheoremObject(B, C)),
                    new AngleTheoremObject(new LineTheoremObject(B, J), new LineTheoremObject(B, I))
                }),
                new Theorem(configuration, EqualAngles, new[]
                {
                    new AngleTheoremObject(new LineTheoremObject(C, J), new LineTheoremObject(B, C)),
                    new AngleTheoremObject(new LineTheoremObject(C, J), new LineTheoremObject(C, I))
                }),
                new Theorem(configuration, EqualAngles, new[]
                {
                    new AngleTheoremObject(new LineTheoremObject(A, I), new LineTheoremObject(A, B)),
                    new AngleTheoremObject(new LineTheoremObject(A, I), new LineTheoremObject(A, C))
                }),
                new Theorem(configuration, EqualAngles, new[]
                {
                    new AngleTheoremObject(new LineTheoremObject(B, I), new LineTheoremObject(B, C)),
                    new AngleTheoremObject(new LineTheoremObject(B, I), new LineTheoremObject(B, A))
                }),
                new Theorem(configuration, EqualAngles, new[]
                {
                    new AngleTheoremObject(new LineTheoremObject(C, I), new LineTheoremObject(C, A)),
                    new AngleTheoremObject(new LineTheoremObject(C, I), new LineTheoremObject(C, B))
                })
            })
            .Should().BeTrue();
        }
    }
}
