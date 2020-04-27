using GeoGen.Constructor;
using GeoGen.Core;
using GeoGen.TheoremFinder;
using GeoGen.TheoremProver.InferenceRuleProvider;
using GeoGen.TheoremProver.ObjectIntroductionRuleProvider;
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

namespace GeoGen.TheoremProver.IntegrationTest
{
    /// <summary>
    /// The entry class of the application.
    /// </summary>
    public static class Startup
    {
        /// <summary>
        /// The entry method of the application.
        /// </summary>
        /// <param name="arguments">The three arguments:
        /// <list type="number">
        /// <item>Path to the inference rule folder.</item>
        /// <item>The extension of the inference rule files.</item>
        /// <item>Path to the object introduction rule file.</item>
        /// </list> 
        /// </param>
        private static async Task Main(string[] arguments)
        {
            #region Kernel preparation

            // Prepare the settings for the inference rule provider
            var inferenceRuleProviderSettings = new InferenceRuleProviderSettings(ruleFolderPath: arguments[0], fileExtension: arguments[1]);

            // Prepare the settings for the object introduction rule provider
            var objectIntroductionRuleProviderSettings = new ObjectIntroductionRuleProviderSettings(filePath: arguments[2]);

            // Prepare the kernel
            var kernel = Infrastructure.IoC.CreateKernel()
                // That constructors configurations
                .AddConstructor()
                // That can find theorems
                .AddTheoremFinder(new TheoremFindingSettings
                                  (
                                      // Look for theorems of any type
                                      soughtTheoremTypes: Enum.GetValues(typeof(TheoremType)).Cast<TheoremType>()
                                          // Except for the EqualObjects that don't have a finder
                                          .Except(TheoremType.EqualObjects.ToEnumerable())
                                          // Enumerate
                                          .ToArray(),

                                      // Exclude in-picture tangencies
                                      new TangentCirclesTheoremFinderSettings(excludeTangencyInsidePicture: true),
                                      new LineTangentToCircleTheoremFinderSettings(excludeTangencyInsidePicture: true)
                                  ))
                // That can prove theorems
                .AddTheoremProver(new TheoremProvingSettings
                (
                    // Use the provider to find the inference rules
                    new InferenceRuleManagerData(await new InferenceRuleProvider.InferenceRuleProvider(inferenceRuleProviderSettings).GetInferenceRulesAsync()),

                    // Use the provider to find the object introduction rules
                    new ObjectIntroducerData(await new ObjectIntroductionRuleProvider.ObjectIntroductionRuleProvider(objectIntroductionRuleProviderSettings).GetObjectIntroductionRulesAsync())
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
                LineTangentToCircle(),
                ConcurrencyViaObjectIntroduction(),
                SimpleLineSegments(),
            }
            // Perform each
            .ForEach(configuration =>
            {
                #region Finding theorems

                // Prepare 3 pictures in which the configuration is drawn
                var pictures = kernel.Get<IGeometryConstructor>().ConstructWithUniformLayout(configuration, numberOfPictures: 3).pictures;

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
                Console.WriteLine($"\nConfiguration:\n\n{formatter.FormatConfiguration(configuration).Indent(2)}\n");
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

        private static Configuration LineTangentToCircle()
        {
            // Create objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var D = new ConstructedConfigurationObject(Incenter, A, B, C);
            var E = new ConstructedConfigurationObject(PerpendicularProjectionOnLineFromPoints, D, B, C);
            var F = new ConstructedConfigurationObject(PointReflection, E, D);
            var G = new ConstructedConfigurationObject(ReflectionInLineFromPoints, F, A, D);

            // Return the configuration
            return Configuration.DeriveFromObjects(Triangle, A, B, C, D, E, F, G);
        }

        private static Configuration ConcurrencyViaObjectIntroduction()
        {
            // Create objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var D = new ConstructedConfigurationObject(Orthocenter, A, B, C);
            var E = new ConstructedConfigurationObject(Circumcenter, B, C, D);
            var F = new ConstructedConfigurationObject(ParallelogramPoint, A, B, C);
            var G = new ConstructedConfigurationObject(Incenter, B, C, E);

            // Return the configuration
            return Configuration.DeriveFromObjects(Triangle, A, B, C, D, E, F, G);
        }

        private static Configuration SimpleLineSegments()
        {
            // Create objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var l = new ConstructedConfigurationObject(TangentLine, A, B, C);
            var D = new ConstructedConfigurationObject(PerpendicularProjection, B, l);
            var E = new ConstructedConfigurationObject(PerpendicularProjection, C, l);
            var F = new ConstructedConfigurationObject(Midpoint, B, D);
            var G = new ConstructedConfigurationObject(Midpoint, C, E);

            // Return the configuration
            return Configuration.DeriveFromObjects(Triangle, A, B, C, D, E, F, G);
        }

        #endregion
    }
}