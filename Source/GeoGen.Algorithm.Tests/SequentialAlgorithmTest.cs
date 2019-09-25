using FluentAssertions;
using GeoGen.Constructor;
using GeoGen.Core;
using GeoGen.DependenciesResolver;
using GeoGen.Generator;
using GeoGen.TheoremFinder;
using GeoGen.TheoremProver;
using GeoGen.Utilities;
using Ninject;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.Algorithm.Tests
{
    /// <summary>
    /// The test class for <see cref="SequentialAlgorithm"/>.
    /// </summary>
    [TestFixture]
    public class SequentialAlgorithmTest
    {
        #region SequentialAlgorithm instance

        /// <summary>
        /// Gets the instance of the sequential algorithm.
        /// </summary>
        private static SequentialAlgorithm Algorithm
        {
            get
            {
                // Prepare the kernel
                var kernel = IoC.CreateKernel().AddGenerator()
                    // Add theorem finder with no restrictions
                    .AddTheoremFinder(new TangentCirclesTheoremFinderSettings
                    {
                        ExcludeTangencyInsidePicture = false
                    },
                    new LineTangentToCircleTheoremFinderSettings
                    {
                        ExcludeTangencyInsidePicture = false
                    },
                    typeof(TheoremType).GetEnumValues().Cast<TheoremType>().Except(new[] { TheoremType.EqualObjects, TheoremType.Incidence }).ToReadOnlyHashSet())
                    // Prover with no theorems 
                    // TODO: Get some theorems 
                    .AddTheoremProver(new TheoremProverData(new List<(Configuration, TheoremMap)>()))
                    // Constructor that ignores inconsistencies
                    .AddConstructor(new PicturesSettings
                    {
                        NumberOfPictures = 5,
                        MaximalAttemptsToReconstructAllPictures = 0,
                        MaximalAttemptsToReconstructOnePicture = 0
                    });

                // Bind the algorithm
                kernel.Bind<SequentialAlgorithm>().ToSelf();

                // Get it
                return kernel.Get<SequentialAlgorithm>();
            }
        }

        #endregion

        [Test]
        public void Test_That_It_Runs_Without_Exception()
        {
            // Prepare the objects
            var A = new LooseConfigurationObject(ConfigurationObjectType.Point);
            var B = new LooseConfigurationObject(ConfigurationObjectType.Point);
            var C = new LooseConfigurationObject(ConfigurationObjectType.Point);

            // Prepare the configuration
            var configuration = Configuration.DeriveFromObjects(LooseObjectsLayout.ThreePoints, A, B, C);

            // Prepare the input
            var input = new GeneratorInput
            {
                InitialConfiguration = configuration,
                Constructions = new[] { PredefinedConstructions.Midpoint },
                NumberOfIterations = 3
            };

            // Prepare the action running the algorithm
            void Execute()
            {
                // Execute it
                var (theorems, outputs) = Algorithm.Run(input);

                // Make sure theorems are there
                theorems.Should().NotBeNull();

                // Try to run the outputs enumerable
                outputs.ToList();
            };

            // Make sure it doesn't throw anything
            ((Action)Execute).Should().NotThrow();
        }
    }
}
