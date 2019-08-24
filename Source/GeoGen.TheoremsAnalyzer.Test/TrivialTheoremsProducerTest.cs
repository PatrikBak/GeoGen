using FluentAssertions;
using GeoGen.Constructor;
using GeoGen.Core;
using GeoGen.DependenciesResolver;
using GeoGen.TheoremsFinder;
using GeoGen.Utilities;
using Ninject;
using NUnit.Framework;
using System;
using System.Linq;
using static GeoGen.Core.ComposedConstructions;
using static GeoGen.Core.ConfigurationObjectType;
using static GeoGen.Core.LooseObjectsLayout;
using static GeoGen.Core.TheoremType;

namespace GeoGen.TheoremsAnalyzer.Test
{
    /// <summary>
    /// The test class for <see cref="TrivialTheoremsProducer"/>.
    /// </summary>
    [TestFixture]
    public class TrivialTheoremsProducerTest
    {
        #region TrivialTheoremsProducer instance

        /// <summary>
        /// Gets the trivial theorems producer.
        /// </summary>
        private static ITrivialTheoremsProducer Producer
        {
            get
            {
                // Initialize IoC
                var kernel = IoC.CreateKernel();

                // Add the theorems finder with no restrictions
                kernel.AddTheoremsFinder(new TangentCirclesTheoremsFinderSettings
                {
                    ExcludeTangencyInsidePicture = false
                },
                    new LineTangentToCircleTheoremsFinderSettings
                    {
                        ExcludeTangencyInsidePicture = false
                    },
                    typeof(TheoremType).GetEnumValues().Cast<TheoremType>().ToReadOnlyHashSet())
                    // Add constructor ignoring inconsistencies
                    .AddConstructor(new PicturesSettings
                    {
                        NumberOfPictures = 5,
                        MaximalAttemptsToReconstructOnePicture = 0,
                        MaximalAttemptsToReconstructAllPictures = 0
                    });

                // Bind producer
                kernel.Bind<ITrivialTheoremsProducer>().To<TrivialTheoremsProducer>();

                // Get it
                return kernel.Get<ITrivialTheoremsProducer>();
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

            // Create the configuration
            var configuration = Configuration.DeriveFromObjects(ThreePoints, I);

            // Find the trivial theorems
            Producer.DeriveTrivialTheoremsFromLastObject(configuration)
                // There should be only equal angles
                .ToSet().SetEquals(new[]
                {
                    new Theorem(configuration, EqualAngles, new[]
                    {
                        new AngleTheoremObject(new LineTheoremObject(B, A), new LineTheoremObject(A, I)),
                        new AngleTheoremObject(new LineTheoremObject(C, A), new LineTheoremObject(A, I)),
                    }),

                    new Theorem(configuration, EqualAngles, new[]
                    {
                        new AngleTheoremObject(new LineTheoremObject(C, B), new LineTheoremObject(B, I)),
                        new AngleTheoremObject(new LineTheoremObject(A, B), new LineTheoremObject(B, I)),
                    }),

                    new Theorem(configuration, EqualAngles, new[]
                    {
                        new AngleTheoremObject(new LineTheoremObject(A, C), new LineTheoremObject(C, I)),
                        new AngleTheoremObject(new LineTheoremObject(B, C), new LineTheoremObject(C, I)),
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

            // Create the configuration
            var configuration = Configuration.DeriveFromObjects(ThreePoints, M);

            // Prepare angles that are indeed equal to <A/2
            var halfAlfa = new[]
            {
                new AngleTheoremObject(new LineTheoremObject(A, B), new LineTheoremObject(A, M)),
                new AngleTheoremObject(new LineTheoremObject(A, C), new LineTheoremObject(A, M)),
                new AngleTheoremObject(new LineTheoremObject(B, C), new LineTheoremObject(B, M)),
                new AngleTheoremObject(new LineTheoremObject(B, C), new LineTheoremObject(C, M))
            };

            // Prepare angles that are equal to the angle of the chord AM
            var chordAM = new[]
            {
                new AngleTheoremObject(new LineTheoremObject(A, B), new LineTheoremObject(B, M)),
                new AngleTheoremObject(new LineTheoremObject(A, C), new LineTheoremObject(C, M)),
                new AngleTheoremObject(new LineTheoremObject(A, M), new LineTheoremObject(B, C))
            };

            // Find the trivial theorems
            Producer.DeriveTrivialTheoremsFromLastObject(configuration)
                // There should be these theorems
                .ToSet().SetEquals(new[]
                {
                    // Half alpha angles
                    new Theorem(configuration, EqualAngles, new[] {halfAlfa[0], halfAlfa[1] }),
                    new Theorem(configuration, EqualAngles, new[] {halfAlfa[0], halfAlfa[2] }),
                    new Theorem(configuration, EqualAngles, new[] {halfAlfa[0], halfAlfa[3] }),
                    new Theorem(configuration, EqualAngles, new[] {halfAlfa[1], halfAlfa[2] }),
                    new Theorem(configuration, EqualAngles, new[] {halfAlfa[1], halfAlfa[3] }),
                    new Theorem(configuration, EqualAngles, new[] {halfAlfa[2], halfAlfa[3] }),

                    // Chord AM angles
                    new Theorem(configuration, EqualAngles, new[] {chordAM[0], chordAM[1] }),
                    new Theorem(configuration, EqualAngles, new[] {chordAM[0], chordAM[2] }),
                    new Theorem(configuration, EqualAngles, new[] {chordAM[1], chordAM[2] }),

                    // Alpha angles
                    new Theorem(configuration, EqualAngles, new[]
                    {
                        new AngleTheoremObject(new LineTheoremObject(A, B), new LineTheoremObject(A, C)),
                        new AngleTheoremObject(new LineTheoremObject(M, B), new LineTheoremObject(M, C))
                    }),

                    // Beta angles
                    new Theorem(configuration, EqualAngles, new[]
                    {
                        new AngleTheoremObject(new LineTheoremObject(A, B), new LineTheoremObject(B, C)),
                        new AngleTheoremObject(new LineTheoremObject(A, M), new LineTheoremObject(M, C))
                    }),

                    // Gamma angles
                    new Theorem(configuration, EqualAngles, new[]
                    {
                        new AngleTheoremObject(new LineTheoremObject(A, C), new LineTheoremObject(B, C)),
                        new AngleTheoremObject(new LineTheoremObject(A, M), new LineTheoremObject(M, B))
                    }),

                    // Equal line segments
                    new Theorem(configuration, EqualLineSegments, new[]
                    {
                        new LineSegmentTheoremObject(B, M),
                        new LineSegmentTheoremObject(C, M)
                    }),

                    // Concyclic points
                    new Theorem(configuration, ConcyclicPoints, A, B, C, M),
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

                    // Get the constructed object
                    var constructedObject = new ConstructedConfigurationObject(construction, looseObjects.LooseObjects.ToArray());

                    // Create the configuration
                    return new Configuration(looseObjects, new[] { constructedObject });
                })
                // For each execute the algorithm
                .ForEach(configuration =>
                {
                    // Prepare the action executing the algorithm
                    Action action = () => Producer.DeriveTrivialTheoremsFromLastObject(configuration);

                    // Make sure there is no exception
                    action.Should().NotThrow($"The construction {configuration.ConstructedObjects[0].Construction} has a problem.");
                });
        }
    }
}