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
    /// The test class for <see cref="IncidenceTheoremFinder"/>.
    /// </summary>
    [TestFixture]
    public class IncidenceTheoremFinderTest : TypedTheoremFinderTestBase<IncidenceTheoremFinder>
    {
        [Test]
        public void Test_Line_That_Is_Already_There_Implicitly_As_Last_Object()
        {
            // Create the objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var bc = new ConstructedConfigurationObject(LineFromPoints, B, C);
            var D = new ConstructedConfigurationObject(PerpendicularProjectionOnLineFromPoints, A, B, C);
            var H = new ConstructedConfigurationObject(Orthocenter, A, B, C);
            var l = new ConstructedConfigurationObject(PerpendicularLine, H, bc);

            // Create the configuration
            var configuration = Configuration.DeriveFromObjects(ThreePoints, bc, D, H, l);

            // Run
            var (newTheorems, allTheorems) = FindTheorems(configuration);

            // Assert new theorems
            newTheorems.OrderlessEquals(new[]
            {
                new Theorem(configuration, Incidence, l, A),
                new Theorem(configuration, Incidence, l, H),
                new Theorem(configuration, Incidence, l, D)
            })
            .Should().BeTrue();

            // Assert all theorems
            allTheorems.OrderlessEquals(new[]
            {
                new Theorem(configuration, Incidence, l, A),
                new Theorem(configuration, Incidence, l, H),
                new Theorem(configuration, Incidence, l, D),
                new Theorem(configuration, Incidence, bc, B),
                new Theorem(configuration, Incidence, bc, C),
                new Theorem(configuration, Incidence, bc, D)
            })
            .Should().BeTrue();
        }

        [Test]
        public void Test_Line_That_Is_Already_There_Implicitly_With_Its_Point_As_Last_Object()
        {
            // Create the objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var bc = new ConstructedConfigurationObject(LineFromPoints, B, C);
            var l = new ConstructedConfigurationObject(PerpendicularLine, A, bc);
            var H = new ConstructedConfigurationObject(Orthocenter, A, B, C);
            var D = new ConstructedConfigurationObject(PerpendicularProjectionOnLineFromPoints, A, B, C);

            // Create the configuration
            var configuration = Configuration.DeriveFromObjects(ThreePoints, bc, l, H, D);

            // Run
            var (newTheorems, allTheorems) = FindTheorems(configuration);

            // Assert new theorems
            newTheorems.OrderlessEquals(new[]
            {
                new Theorem(configuration, Incidence, bc, D),
                new Theorem(configuration, Incidence, l, D)
            })
            .Should().BeTrue();

            // Assert all theorems
            allTheorems.OrderlessEquals(new[]
            {
                new Theorem(configuration, Incidence, l, A),
                new Theorem(configuration, Incidence, l, H),
                new Theorem(configuration, Incidence, l, D),
                new Theorem(configuration, Incidence, bc, B),
                new Theorem(configuration, Incidence, bc, C),
                new Theorem(configuration, Incidence, bc, D)
            })
            .Should().BeTrue();
        }

        [Test]
        public void Test_Line_That_Wasnt_There_Before()
        {
            // Create the objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var bc = new ConstructedConfigurationObject(LineFromPoints, B, C);
            var l = new ConstructedConfigurationObject(PerpendicularLine, A, bc);

            // Create the configuration
            var configuration = Configuration.DeriveFromObjects(ThreePoints, bc, l);

            // Run
            var (newTheorems, allTheorems) = FindTheorems(configuration);

            // Assert new theorems
            newTheorems.OrderlessEquals(new[]
            {
                new Theorem(configuration, Incidence, l, A)
            })
            .Should().BeTrue();

            // Assert all theorems
            allTheorems.OrderlessEquals(new[]
            {
                new Theorem(configuration, Incidence, l, A),
                new Theorem(configuration, Incidence, bc, B),
                new Theorem(configuration, Incidence, bc, C)
            })
            .Should().BeTrue();
        }

        [Test]
        public void Test_Circle_That_Is_Already_There_Implicitly_As_Last_Object()
        {
            // Create the objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var c1 = new ConstructedConfigurationObject(CircleWithCenterThroughPoint, B, A);
            var c2 = new ConstructedConfigurationObject(CircleWithCenterThroughPoint, C, A);
            var D = new ConstructedConfigurationObject(ReflectionInLineFromPoints, A, B, C);
            var thalesBC = new ConstructedConfigurationObject(CircleWithDiameter, B, C);

            // Create the configuration
            var configuration = Configuration.DeriveFromObjects(RightTriangle, A, c1, c2, D, thalesBC);

            // Run
            var (newTheorems, allTheorems) = FindTheorems(configuration);

            // Assert new theorems
            newTheorems.OrderlessEquals(new[]
            {
                new Theorem(configuration, Incidence, thalesBC, A),
                new Theorem(configuration, Incidence, thalesBC, B),
                new Theorem(configuration, Incidence, thalesBC, C),
                new Theorem(configuration, Incidence, thalesBC, D)
            })
            .Should().BeTrue();

            // Assert all theorems
            allTheorems.OrderlessEquals(new[]
            {
                new Theorem(configuration, Incidence, thalesBC, A),
                new Theorem(configuration, Incidence, thalesBC, B),
                new Theorem(configuration, Incidence, thalesBC, C),
                new Theorem(configuration, Incidence, thalesBC, D),
                new Theorem(configuration, Incidence, c1, A),
                new Theorem(configuration, Incidence, c1, D),
                new Theorem(configuration, Incidence, c2, A),
                new Theorem(configuration, Incidence, c2, D)
            })
            .Should().BeTrue();
        }

        [Test]
        public void Test_Circle_That_Is_Already_There_Implicitly_With_Its_Point_As_Last_Object()
        {
            // Create the objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var thalesBC = new ConstructedConfigurationObject(CircleWithDiameter, B, C);
            var D = new ConstructedConfigurationObject(ReflectionInLineFromPoints, A, B, C);

            // Create the configuration
            var configuration = Configuration.DeriveFromObjects(RightTriangle, A, thalesBC, D);

            // Run
            var (newTheorems, allTheorems) = FindTheorems(configuration);

            // Assert new theorems
            newTheorems.OrderlessEquals(new[]
            {
                new Theorem(configuration, Incidence, thalesBC, D)
            })
            .Should().BeTrue();

            // Assert all theorems
            allTheorems.OrderlessEquals(new[]
            {
                new Theorem(configuration, Incidence, thalesBC, A),
                new Theorem(configuration, Incidence, thalesBC, B),
                new Theorem(configuration, Incidence, thalesBC, C),
                new Theorem(configuration, Incidence, thalesBC, D)
            })
            .Should().BeTrue();
        }

        [Test]
        public void Test_Circle_That_Wasnt_There_Before()
        {
            // Create the objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var abc = new ConstructedConfigurationObject(Circumcircle, A, B, C);
            var thalesBC = new ConstructedConfigurationObject(CircleWithDiameter, B, C);

            // Create the configuration
            var configuration = Configuration.DeriveFromObjects(ThreePoints, abc, thalesBC);

            // Run
            var (newTheorems, allTheorems) = FindTheorems(configuration);

            // Assert new theorems
            newTheorems.OrderlessEquals(new[]
            {
                new Theorem(configuration, Incidence, thalesBC, B),
                new Theorem(configuration, Incidence, thalesBC, C)
            })
            .Should().BeTrue();

            // Assert all theorems
            allTheorems.OrderlessEquals(new[]
            {
                new Theorem(configuration, Incidence, thalesBC, B),
                new Theorem(configuration, Incidence, thalesBC, C),
                new Theorem(configuration, Incidence, abc, A),
                new Theorem(configuration, Incidence, abc, B),
                new Theorem(configuration, Incidence, abc, C)
            })
            .Should().BeTrue();
        }
    }
}
