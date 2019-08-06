using FluentAssertions;
using GeoGen.Constructor;
using GeoGen.Core;
using GeoGen.DependenciesResolver;
using GeoGen.Utilities;
using NUnit.Framework;
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
        #region Service instance

        /// <summary>
        /// The instance of the producer.
        /// </summary>
        private ITrivialTheoremsProducer _producer;

        #endregion

        #region SetUp

        [OneTimeSetUp]
        public void Initialize()
        {
            // Initialize IoC
            var kernl = IoCUtilities.CreateKernel().AddGenerator().AddConstructor().AddTheoremsFinder().AddTheoremsAnalyzer();

            // Create the producer
            _producer = kernl.Get<ITrivialTheoremsProducer>(new PicturesManagerSettings
            {
                NumberOfPictures = 8,
                MaximalAttemptsToReconstructOnePicture = 100,
                MaximalAttemptsToReconstructAllPictures = 1000
            });
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
            _producer.DeriveTrivialTheoremsFromLastObject(configuration)
                // There should be only equal angles
                .ToSet(Theorem.EquivalencyComparer).SetEquals(new[]
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
            _producer.DeriveTrivialTheoremsFromLastObject(configuration)
                // There should be these theorems
                .ToSet(Theorem.EquivalencyComparer).SetEquals(new[]
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
    }
}