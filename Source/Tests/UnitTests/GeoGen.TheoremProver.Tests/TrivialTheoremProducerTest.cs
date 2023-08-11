using FluentAssertions;
using GeoGen.Constructor;
using GeoGen.Core;
using GeoGen.Infrastructure;
using GeoGen.TheoremFinder;
using GeoGen.Utilities;
using Ninject;
using NUnit.Framework;
using static GeoGen.Core.ComposedConstructions;
using static GeoGen.Core.ConfigurationObjectType;
using static GeoGen.Core.TheoremType;

namespace GeoGen.TheoremProver.Tests
{
    /// <summary>
    /// The test class for <see cref="TrivialTheoremProducer"/>.
    /// </summary>
    [TestFixture]
    public class TrivialTheoremProducerTest
    {
        #region TrivialTheoremProducer instance

        /// <summary>
        /// Gets the trivial theorems producer.
        /// </summary>
        private static ITrivialTheoremProducer Producer
        {
            get
            {
                // Initialize IoC
                var kernel = NinjectUtilities.CreateKernel()
                    // Add the theorem finder with no restrictions
                    .AddTheoremFinder(new TheoremFindingSettings
                                        (
                                           // Look for any type except for EqualObjects
                                           soughtTheoremTypes: typeof(TheoremType).GetEnumValues().Cast<TheoremType>().Except(new[] { EqualObjects }).ToArray(),

                                           // Don't exclude tangencies
                                           new TangentCirclesTheoremFinderSettings(excludeTangencyInsidePicture: false),
                                           new LineTangentToCircleTheoremFinderSettings(excludeTangencyInsidePicture: false)
                                        ))
                    // And constructor
                    .AddConstructor();

                // Bind producer
                kernel.Bind<ITrivialTheoremProducer>().To<TrivialTheoremProducer>();

                // Get it
                return kernel.Get<ITrivialTheoremProducer>();
            }
        }

        #endregion

        [Test]
        public void Test_With_Midpoint_Of_Opposite_Arc()
        {
            // Create objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var M = new ConstructedConfigurationObject(MidpointOfOppositeArc, A, B, C);

            // Find the trivial theorems
            Producer.InferTrivialTheoremsFromObject(M)
                // There should be these theorems
                .OrderlessEquals(new[]
                {
                    // Equal line segments
                    new Theorem(EqualLineSegments, new[]
                    {
                        new LineSegmentTheoremObject(B, M),
                        new LineSegmentTheoremObject(C, M)
                    }),

                    // Concyclic points
                    new Theorem(ConcyclicPoints, A, B, C, M),
                })
                .Should().BeTrue();
        }

        [Test]
        public void Test_That_All_Constructed_Objects_Can_Be_Examined()
        {
            // Get all composed construction
            Constructions.GetComposedConstructions()
                // For each prepare a configuration with the last object simulating it
                .Select(construction =>
                {
                    // Get the loose objects
                    var looseObjects = construction.Configuration.LooseObjectsHolder;

                    // Return the constructed object
                    return new ConstructedConfigurationObject(construction, looseObjects.LooseObjects.ToArray());
                })
                // For each execute the algorithm
                .ForEach(constructedObject =>
                {
                    // Prepare the action executing the algorithm
                    Action action = () => Producer.InferTrivialTheoremsFromObject(constructedObject);

                    // Make sure there is no exception
                    action.Should().NotThrow($"The construction {constructedObject} has a problem.");
                });
        }
    }
}