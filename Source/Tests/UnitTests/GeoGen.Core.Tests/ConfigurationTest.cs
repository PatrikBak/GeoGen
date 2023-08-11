using FluentAssertions;
using NUnit.Framework;
using static GeoGen.Core.ComposedConstructions;
using static GeoGen.Core.ConfigurationObjectType;
using static GeoGen.Core.LooseObjectLayout;
using static GeoGen.Core.PredefinedConstructionType;

namespace GeoGen.Core.Tests
{
    /// <summary>
    /// The test class for <see cref="Configuration"/>.
    /// </summary>
    [TestFixture]
    public class ConfigurationTest
    {
        [Test]
        public void Test_GetObjectsThatWouldMakeThisConfigurationFullySymmetric_Midpoint()
        {
            // Create objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var D = new ConstructedConfigurationObject(Midpoint, B, C);
            var I = new ConstructedConfigurationObject(Incenter, A, B, C);

            // Create the configuration and call the method
            Configuration.DeriveFromObjects(Triangle, D, I).GetObjectsThatWouldMakeThisConfigurationFullySymmetric()
                // We need to add other two points
                .Should().BeEquivalentTo(new[]
                {
                    new ConstructedConfigurationObject(Midpoint, A, C),
                    new ConstructedConfigurationObject(Midpoint, A, B)
                });
        }

        [Test]
        public void Test_GetObjectsThatWouldMakeThisConfigurationFullySymmetric_PointReflection()
        {
            // Create objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var D = new ConstructedConfigurationObject(PointReflection, B, C);
            var H = new ConstructedConfigurationObject(Orthocenter, A, B, C);

            // Create the configuration and call the method
            Configuration.DeriveFromObjects(Triangle, D, H).GetObjectsThatWouldMakeThisConfigurationFullySymmetric()
                // We need 5 more reflections
                .Should().BeEquivalentTo(new[]
                {
                    new ConstructedConfigurationObject(PointReflection, C, B),
                    new ConstructedConfigurationObject(PointReflection, A, B),
                    new ConstructedConfigurationObject(PointReflection, B, A),
                    new ConstructedConfigurationObject(PointReflection, A, C),
                    new ConstructedConfigurationObject(PointReflection, C, A)
                });
        }

        [Test]
        public void Test_GetObjectsThatWouldMakeThisConfigurationFullySymmetric_Mix()
        {
            // Create objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var D = new ConstructedConfigurationObject(PointReflection, B, C);
            var E = new ConstructedConfigurationObject(Midpoint, B, C);
            var H = new ConstructedConfigurationObject(Orthocenter, A, B, C);
            var I = new ConstructedConfigurationObject(Incenter, A, B, C);
            var G = new ConstructedConfigurationObject(Centroid, E, I, H);
            var K = new ConstructedConfigurationObject(PointReflection, H, I);

            // Create the configuration and call the method
            Configuration.DeriveFromObjects(Triangle, D, E, H, I, G, K).GetObjectsThatWouldMakeThisConfigurationFullySymmetric()
                // We need lots of objects
                .Should().BeEquivalentTo(new[]
                {
                    new ConstructedConfigurationObject(PointReflection, C, B),
                    new ConstructedConfigurationObject(PointReflection, A, B),
                    new ConstructedConfigurationObject(PointReflection, B, A),
                    new ConstructedConfigurationObject(PointReflection, A, C),
                    new ConstructedConfigurationObject(PointReflection, C, A),
                    new ConstructedConfigurationObject(Midpoint, A, C),
                    new ConstructedConfigurationObject(Midpoint, A, B),
                    new ConstructedConfigurationObject(Centroid, new ConstructedConfigurationObject(Midpoint, A, C), I, H),
                    new ConstructedConfigurationObject(Centroid, new ConstructedConfigurationObject(Midpoint, A, B), I, H),
                });
        }

        [Test]
        public void Test_GetObjectsThatWouldMakeThisConfigurationSymmetric_Midpoint()
        {
            // Create objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var D = new ConstructedConfigurationObject(Midpoint, B, C);
            var I = new ConstructedConfigurationObject(Incenter, A, B, C);

            // Create the configuration and call the method
            Configuration.DeriveFromObjects(Triangle, D, I).GetObjectsThatWouldMakeThisConfigurationSymmetric()
                // We can add one midpoint everywhere
                .Should().BeEquivalentTo(new[]
                {
                    // It already is symmetric
                    Array.Empty<ConstructedConfigurationObject>(),

                    // Or still would be if we added a midpoint
                    new[] { new ConstructedConfigurationObject(Midpoint, A, C) },
                    new[] { new ConstructedConfigurationObject(Midpoint, A, B) }
                });
        }

        [Test]
        public void Test_GetObjectsThatWouldMakeThisConfigurationSymmetric_PointReflection()
        {
            // Create objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var D = new ConstructedConfigurationObject(PointReflection, B, C);
            var H = new ConstructedConfigurationObject(Orthocenter, A, B, C);

            // Create the configuration and call the method
            Configuration.DeriveFromObjects(Triangle, D, H).GetObjectsThatWouldMakeThisConfigurationSymmetric()
                // We need one more reflection in every case                
                .Should().BeEquivalentTo(new[]
                {
                    new[] { new ConstructedConfigurationObject(PointReflection, C, B) },
                    new[] { new ConstructedConfigurationObject(PointReflection, A, C) },
                    new[] { new ConstructedConfigurationObject(PointReflection, B, A) }
                });
        }

        [Test]
        public void Test_GetObjectsThatWouldMakeThisConfigurationSymmetric_Mix()
        {
            // Create objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var D = new ConstructedConfigurationObject(PointReflection, B, C);
            var E = new ConstructedConfigurationObject(Midpoint, B, C);
            var H = new ConstructedConfigurationObject(Orthocenter, A, B, C);
            var I = new ConstructedConfigurationObject(Incenter, A, B, C);
            var G = new ConstructedConfigurationObject(Centroid, E, I, H);
            var K = new ConstructedConfigurationObject(PointReflection, H, I);

            // Create the configuration and call the method
            Configuration.DeriveFromObjects(Triangle, D, E, H, I, G, K).GetObjectsThatWouldMakeThisConfigurationSymmetric()
                // We need lots of objects
                .Should().BeEquivalentTo(new[]
                {
                    new[]
                    {
                        new ConstructedConfigurationObject(PointReflection, C, B)
                    },
                    new[]
                    {
                        new ConstructedConfigurationObject(PointReflection, A, C),
                        new ConstructedConfigurationObject(Midpoint, A, C),
                        new ConstructedConfigurationObject(Centroid, new ConstructedConfigurationObject(Midpoint, A, C), I, H)
                    },
                    new[]
                    {
                        new ConstructedConfigurationObject(PointReflection, B, A),
                        new ConstructedConfigurationObject(Midpoint, A, B),
                        new ConstructedConfigurationObject(Centroid, new ConstructedConfigurationObject(Midpoint, A, B), I, H)
                    }
                });
        }
    }
}