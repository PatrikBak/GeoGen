using GeoGen.ConsoleLauncher;
using GeoGen.Constructor;
using GeoGen.Core;
using GeoGen.Generator;
using GeoGen.TheoremFinder;
using GeoGen.TheoremProver;
using GeoGen.Utilities;
using Ninject;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using static GeoGen.Core.ComposedConstructions;
using static GeoGen.Core.ConfigurationObjectType;
using static GeoGen.Core.LooseObjectLayout;
using static GeoGen.Core.PredefinedConstructions;

namespace GeoGen.TheoremProverLauncher
{
    /// <summary>
    /// The entry class of the application.
    /// </summary>
    public static class Startup
    {
        /// <summary>
        /// The entry method of the application.
        /// </summary>
        /// <param name="arguments">The two arguments: path to the inference rule folder; the extension of the inference rule files.</param>
        private static async Task Main(string[] arguments)
        {
            #region Kernel preparation

            // Prepare the kernel
            var kernel = Infrastructure.IoC.CreateKernel()
                // That constructors configurations
                .AddConstructor()
                // That can find theorems
                .AddTheoremFinder(new TheoremFindingSettings
                                  (
                                      // Look for theorems of any type
                                      soughtTheoremTypes: Enum.GetValues(typeof(TheoremType)).Cast<TheoremType>()
                                          // Except the following ones
                                          .Except(new[]
                                          {
                                              TheoremType.ConcurrentObjects,
                                              TheoremType.EqualAngles,
                                              TheoremType.EqualObjects
                                          })
                                          // Enumerate
                                          .ToArray(),

                                      // Exclude in-picture tangencies
                                      new TangentCirclesTheoremFinderSettings(excludeTangencyInsidePicture: true),
                                      new LineTangentToCircleTheoremFinderSettings(excludeTangencyInsidePicture: true)
                                  ))
                // That can prove theorems
                .AddTheoremProver(new InferenceRuleManagerData
                (
                    // In order to find rules use the rule provider that uses the folder specified in the arguments
                    rules: await new InferenceRuleProvider(new InferenceRuleProviderSettings(ruleFolderPath: arguments[0], fileExtension: arguments[1])).GetInferenceRulesAsync()
                ));

            #endregion

            #region Tests

            // Take the tests
            new[]
            {
                IncenterAndTangentLine(),
                Midpoints(),
                Parallelogram(),
                HiddenExcenter(),
                HiddenMidpoint(),
                SlowOne(),
            }
            // Perform each
            .ForEach(configuration =>
            {
                #region Finding theorems

                // Prepare 3 pictures in which the configuration is drawn
                var pictures = kernel.Get<IGeometryConstructor>().Construct(configuration, numberOfPictures: 3, LooseObjectDrawingStyle.GenerationFriendly).pictures;

                // Prepare a contextual picture
                var contextualPicture = new ContextualPicture(pictures);

                // Find all theorems
                var theorems = kernel.Get<ITheoremFinder>().FindAllTheorems(contextualPicture);

                // Separate the old theorems by taking all the theorems
                var oldTheorems = new TheoremMap(theorems.AllObjects
                    // That doesn't contain the last object in their definition
                    .Where(theorem => !theorem.GetInnerConfigurationObjects().Contains(configuration.LastConstructedObject)));

                // Separate the new theorems by taking all the theorem
                var newTheorems = new TheoremMap(theorems.AllObjects
                    // That do contain the last object in their definition
                    .Where(theorem => theorem.GetInnerConfigurationObjects().Contains(configuration.LastConstructedObject)));

                #endregion

                #region Writing theorems

                // Prepare the formatter of all the output
                var formatter = new OutputFormatter(configuration.AllObjects);

                // Prepare a local function that converts given theorems to a string
                string TheoremString(IEnumerable<Theorem> theorems) =>
                    // If there are no theorems
                    theorems.IsEmpty()
                        // Then return an indication of it 
                        ? "nothing"
                        // Otherwise format each theorem
                        : theorems.Select(formatter.FormatTheorem)
                            // Order alphabetically
                            .Ordered()
                            // Add the index
                            .Select((theoremString, index) => $"[{index + 1}] {theoremString}")
                            // Make each on a separate line
                            .ToJoinedString("\n");

                // Write the configuration and theorems
                Console.WriteLine($"Configuration:\n\n{formatter.FormatConfiguration(configuration).Indent(2)}\n");
                Console.WriteLine($"OldTheorems:\n\n{TheoremString(oldTheorems.AllObjects).Indent(2)}\n");
                Console.WriteLine($"NewTheorems:\n\n{TheoremString(newTheorems.AllObjects).Indent(2)}\n");

                #endregion

                #region Proving theorems

                // Prepare a timer
                var totalTime = new Stopwatch();

                // Start it
                totalTime.Start();

                // Perform the theorem finding with proofs
                var proverOutput = kernel.Get<ITheoremProver>().ProveTheoremsAndConstructProofs(oldTheorems, newTheorems, contextualPicture);

                // Stop the timer
                totalTime.Stop();

                #endregion

                #region Writing results

                // Get the proofs
                var proofString = proverOutput
                    // Sort by the statement
                    .OrderBy(pair => formatter.FormatTheorem(pair.Key))
                    // Format each
                    .Select(pair => formatter.FormatTheoremProof(pair.Value))
                    // Trim
                    .Select(proofString => proofString.Trim())
                    // Make an empty line between each
                    .ToJoinedString("\n\n");

                // Write it
                Console.WriteLine(proofString);

                // Write the unproven theorems too
                Console.WriteLine($"\nUnproved:\n\n{TheoremString(newTheorems.AllObjects.Except(proverOutput.Keys)).Indent(2)}\n");

                // Report time
                Console.WriteLine($"Total time: {totalTime.ElapsedMilliseconds}");
                Console.WriteLine("----------------------------------------------");
                Console.WriteLine();

                #endregion
            });

            #endregion
        }

        #region Test configurations

        private static Configuration IncenterAndTangentLine()
        {
            // Create objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var l1 = new ConstructedConfigurationObject(InternalAngleBisector, B, A, C);
            var l2 = new ConstructedConfigurationObject(InternalAngleBisector, C, A, B);
            var D = new ConstructedConfigurationObject(IntersectionOfLines, l1, l2);
            var E = new ConstructedConfigurationObject(TangentLine, D, B, C);

            // Return the configuration
            return Configuration.DeriveFromObjects(Triangle, A, B, C, D, E);
        }

        private static Configuration Midpoints()
        {
            // Create objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var D = new ConstructedConfigurationObject(Midpoint, A, B);
            var E = new ConstructedConfigurationObject(Midpoint, B, C);
            var F = new ConstructedConfigurationObject(Midpoint, C, A);

            // Return the configuration
            return Configuration.DeriveFromObjects(Triangle, D, E, F);
        }

        private static Configuration Parallelogram()
        {
            // Create objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var D = new ConstructedConfigurationObject(PointReflection, B, A);
            var E = new ConstructedConfigurationObject(PointReflection, C, A);

            // Return the configuration
            return Configuration.DeriveFromObjects(Triangle, D, E);
        }

        private static Configuration HiddenExcenter()
        {
            // Create objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var D = new ConstructedConfigurationObject(Incenter, A, B, C);
            var E = new ConstructedConfigurationObject(OppositePointOnCircumcircle, D, B, C);

            // Return the configuration
            return Configuration.DeriveFromObjects(Triangle, E);
        }

        private static Configuration HiddenMidpoint()
        {
            // Create objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var D = new ConstructedConfigurationObject(Median, A, B, C);
            var E = new ConstructedConfigurationObject(IntersectionOfLineAndLineFromPoints, D, B, C);

            // Return the configuration
            return Configuration.DeriveFromObjects(Triangle, D, E);
        }

        private static Configuration SlowOne()
        {
            // Create objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var l = new ConstructedConfigurationObject(ExternalAngleBisector, A, B, C);
            var D = new ConstructedConfigurationObject(PerpendicularProjection, B, l);
            var E = new ConstructedConfigurationObject(PerpendicularProjection, C, l);
            var F = new ConstructedConfigurationObject(ReflectionInLineFromPoints, B, C, E);
            var G = new ConstructedConfigurationObject(ReflectionInLineFromPoints, C, B, D);

            // Return the configuration
            return Configuration.DeriveFromObjects(Triangle, A, B, C, l, D, E, F, G);
        }

        #endregion
    }
}