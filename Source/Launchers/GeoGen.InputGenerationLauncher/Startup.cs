using GeoGen.ConfigurationGenerationLauncher;
using GeoGen.ConfigurationGenerator;
using GeoGen.Constructor;
using GeoGen.Core;
using GeoGen.MainLauncher;
using GeoGen.ProblemGenerator;
using GeoGen.TheoremFinder;
using GeoGen.Utilities;
using Ninject;
using Ninject.Parameters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static GeoGen.Core.ConfigurationObjectType;
using static GeoGen.Core.LooseObjectLayout;

namespace GeoGen.InputGenerationLauncher
{
    /// <summary>
    /// The entry class of the application.
    /// </summary>
    public static class Startup
    {
        #region Private properties

        /// <summary>
        /// The kernel that can resolve <see cref="IProblemGenerator"/>.
        /// </summary>
        private static IKernel _kernel;

        /// <summary>
        /// The constructions to be used in every input file as well as to generate them.
        /// </summary>
        private static IReadOnlyHashSet<Construction> _constructions;

        /// <summary>
        /// The content of the template input file that contains replaceable parts in curly brackets.
        /// </summary>
        private static string _templateInputFile;

        #endregion

        #region Main method

        /// <summary>
        /// The entry method of the application.
        /// </summary>
        private static void Main()
        {
            // Initialize the kernel
            _kernel = Infrastructure.IoC.CreateKernel()
                // Add the constructor
                .AddConstructor()
                // Add the configuration generator that uses fast generation (it does not really matter here)
                .AddConfigurationGenerator(new GenerationSettings(ConfigurationFilterType.Fast));

            // Bind the generator
            _kernel.Bind<IProblemGenerator>().To<ProblemGenerator.ProblemGenerator>();

            // Add an empty failure tracer
            _kernel.Bind<IGeometryFailureTracer>().To<EmptyGeometryFailureTracer>();

            // Add an empty theorem finder
            _kernel.Bind<ITheoremFinder>().To<EmptyTheoremFinder>();

            // Load the construction file
            _constructions = File.ReadAllLines("constructions.txt")
                // Each line should be a construction
                .Select(Parser.ParseConstruction)
                // Enumerate
                .ToReadOnlyHashSet();

            // Load the template input file
            _templateInputFile = File.ReadAllText("input_template.txt");

            // Go through the all types of generated input files
            new[]
            {
                Experiment_Example()
            }
            // For each create input files within Results folder
            .ForEach(pair =>
            {
                // Deconstruct
                var (folderName, generatorInputs) = pair;

                #region Preparing the folder

                // Create the path to the folder with inputs
                var inputFolder = Path.Combine("Results", pair.folderName);

                // If the folder exists, clear it
                if (Directory.Exists(inputFolder))
                    Directory.EnumerateFiles(inputFolder).ForEach(File.Delete);

                // Otherwise create it
                else
                    Directory.CreateDirectory(inputFolder);

                #endregion

                #region Generating individual files

                // Prepare the counter
                var counter = 0;

                // Go through the particular inputs
                foreach (var input in generatorInputs)
                {
                    // Count the input in
                    counter++;

                    // Prepare the constructions as a single string
                    var constructionString = input.Constructions
                        // For each take the name
                        .Select(construction => construction.Name)
                        // Each on a separate line
                        .ToJoinedString("\n");

                    // Prepare the formatted configuration by creating a formatter for it
                    var configurationString = new OutputFormatter(input.InitialConfiguration.AllObjects)
                        // Formatting it
                        .FormatConfiguration(input.InitialConfiguration)
                        // Replacing any curly braces in the definitions
                        .Replace("{", "").Replace("}", "");

                    // Prepare the content by taking the template file
                    var content = _templateInputFile
                        // Replace the constructions
                        .Replace("{Constructions}", constructionString)
                        // Replace the configuration
                        .Replace("{InitialConfiguration}", configurationString)
                        // Replace the iterations
                        .Replace("{Iterations}", input.NumberOfIterations.ToString())
                        // Replace maximal points
                        .Replace("{MaximalPoints}", input.MaximalNumbersOfObjectsToAdd[Point].ToString())
                        // Replace maximal lines
                        .Replace("{MaximalLines}", input.MaximalNumbersOfObjectsToAdd[Line].ToString())
                        // Replace maximal circles
                        .Replace("{MaximalCircles}", input.MaximalNumbersOfObjectsToAdd[Circle].ToString());

                    // Prepare the path to the input file that will be created
                    var filePath = Path.Combine(inputFolder, $"input_{counter}.txt");

                    // Prepare the writer of the input file
                    using var writer = new StreamWriter(new FileStream(filePath, FileMode.Create));

                    // Write the content
                    writer.Write(content);
                }

                // Log how many files have been created
                Console.WriteLine($"[{Path.GetFileName(inputFolder)}] {counter} file(s)");

                #endregion
            });
        }

        #endregion

        #region Experiments

        /// <summary>
        /// An example of an experiment.
        /// </summary>
        /// <returns>The tuple of the name of the folder identifying experiments and the input enumerable.</returns>
        private static (string folderName, IEnumerable<ProblemGeneratorInput> inputs) Experiment_Example()
        {
            // Create the loose objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);

            // Create the initial configuration
            var configuration = Configuration.DeriveFromObjects(Triangle, A, B, C);

            // Create the dictionary with the counts of objects to be added
            var maximalNumbersOfObjectsObjectsToAdd = new Dictionary<ConfigurationObjectType, int>
            {
                { Point, 1 },
                { Line, 0 },
                { Circle, 0 }
            };

            // Prepare the generator input
            var problemGeneratorInput = new ProblemGeneratorInput(configuration, _constructions, numberOfIterations: 1, maximalNumbersOfObjectsObjectsToAdd);

            // Prepare the generator settings for the generator that doesn't exclude asymmetric configurations
            var settings = new ProblemGeneratorSettings(numberOfPictures: 5, excludeAsymmetricConfigurations: false);

            // Prepare the generation enumerable by taking the generator 
            var generatorInputs = _kernel.Get<IProblemGenerator>(new ConstructorArgument("settings", settings))
                // Pass the input to it
                .Generate(problemGeneratorInput)
                // Unwrap the enumerable
                .generationOutputs
                // Take the configuration
                .Select(output => output.Configuration)
                // Every generated configuration makes an input file
                .Select(configuration => new ProblemGeneratorInput(configuration, _constructions,
                    // We will want 3 iterations
                    numberOfIterations: 3,
                    // And 2 points at most
                    new Dictionary<ConfigurationObjectType, int>
                    {
                        { Point, 2 },
                        { Line, 0 },
                        { Circle,0 }
                    }));

            // Return the final result
            return (nameof(Experiment_Example), generatorInputs);
        }

        #endregion
    }
}