using FluentAssertions;
using GeoGen.Constructor;
using GeoGen.Core;
using GeoGen.DependenciesResolver;
using GeoGen.Infrastructure;
using GeoGen.TheoremFinder;
using GeoGen.Utilities;
using Ninject;
using NUnit.Framework;
using System;
using System.Linq;
using static GeoGen.Core.ComposedConstructions;
using static GeoGen.Core.ConfigurationObjectType;
using static GeoGen.Core.TheoremType;

namespace GeoGen.TheoremProver.Test
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
                var kernel = IoC.CreateKernel()
                    // Add the theorem finder with no restrictions
                    .AddTheoremFinder(new TangentCirclesTheoremFinderSettings(excludeTangencyInsidePicture: true),
                                      new LineTangentToCircleTheoremFinderSettings(excludeTangencyInsidePicture: false),
                                      // Take all types except for equal objects for which there is no finder
                                      typeof(TheoremType).GetEnumValues().Cast<TheoremType>().Except(new[] { EqualObjects }).ToReadOnlyHashSet())
                    // Add constructor with 5 pictures
                    .AddConstructor(new GeometryConstructorSettings(numberOfPictures: 5));

                // Bind producer
                kernel.Bind<ITrivialTheoremProducer>().To<TrivialTheoremProducer>();

                // Get it
                return kernel.Get<ITrivialTheoremProducer>();
            }
        }

        #endregion

        [Test]
        public void Test_With_Incenter()
        {
            // Create objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var I = new ConstructedConfigurationObject(Incenter, A, B, C);

            // Find the trivial theorems
            Producer.DeriveTrivialTheoremsFromObject(I)
                // There should be only equal angles
                .OrderlessEquals(new[]
                {
                    new Theorem(EqualAngles, new[]
                    {
                        new AngleTheoremObject(B, A, A, I),
                        new AngleTheoremObject(C, A, A, I),
                    }),

                    new Theorem(EqualAngles, new[]
                    {
                        new AngleTheoremObject(C, B, B, I),
                        new AngleTheoremObject(A, B, B, I),
                    }),

                    new Theorem(EqualAngles, new[]
                    {
                        new AngleTheoremObject(A, C, C, I),
                        new AngleTheoremObject(B, C, C, I),
                    })
                })
                .Should().BeTrue();
        }

        [Test]
        public void Test_With_Midpoint_Of_Opposite_Arc()
        {
            // Create objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var M = new ConstructedConfigurationObject(MidpointOfOppositeArc, A, B, C);

            // Prepare angles that are indeed equal to <A/2
            var halfAlfa = new[]
            {
                new AngleTheoremObject(A, B, A, M),
                new AngleTheoremObject(A, C, A, M),
                new AngleTheoremObject(B, C, B, M),
                new AngleTheoremObject(B, C, C, M)
            };

            // Prepare angles that are equal to the angle of the chord AM
            var chordAM = new[]
            {
                new AngleTheoremObject(A, B, B, M),
                new AngleTheoremObject(A, C, C, M),
                new AngleTheoremObject(A, M, B, C)
            };

            // Find the trivial theorems
            Producer.DeriveTrivialTheoremsFromObject(M)
                // There should be these theorems
                .OrderlessEquals(new[]
                {
                    // Half alpha angles
                    new Theorem(EqualAngles, new[] {halfAlfa[0], halfAlfa[1] }),
                    new Theorem(EqualAngles, new[] {halfAlfa[0], halfAlfa[2] }),
                    new Theorem(EqualAngles, new[] {halfAlfa[0], halfAlfa[3] }),
                    new Theorem(EqualAngles, new[] {halfAlfa[1], halfAlfa[2] }),
                    new Theorem(EqualAngles, new[] {halfAlfa[1], halfAlfa[3] }),
                    new Theorem(EqualAngles, new[] {halfAlfa[2], halfAlfa[3] }),

                    // Chord AM angles
                    new Theorem(EqualAngles, new[] {chordAM[0], chordAM[1] }),
                    new Theorem(EqualAngles, new[] {chordAM[0], chordAM[2] }),
                    new Theorem(EqualAngles, new[] {chordAM[1], chordAM[2] }),

                    // Alpha angles
                    new Theorem(EqualAngles, new[]
                    {
                        new AngleTheoremObject(A, B, A, C),
                        new AngleTheoremObject(M, B, M, C)
                    }),

                    // Beta angles
                    new Theorem(EqualAngles, new[]
                    {
                        new AngleTheoremObject(A, B, B, C),
                        new AngleTheoremObject(A, M, M, C)
                    }),

                    // Gamma angles
                    new Theorem(EqualAngles, new[]
                    {
                        new AngleTheoremObject(A, C, B, C),
                        new AngleTheoremObject(A, M, M, B)
                    }),

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
                    Action action = () => Producer.DeriveTrivialTheoremsFromObject(constructedObject);

                    // Make sure there is no exception
                    action.Should().NotThrow($"The construction {constructedObject} has a problem.");
                });
        }
    }
}