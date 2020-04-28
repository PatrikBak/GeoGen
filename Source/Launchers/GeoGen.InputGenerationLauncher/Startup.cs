using GeoGen.ConfigurationGenerator;
using GeoGen.Constructor;
using GeoGen.Core;
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

            #region Preparing the results folder

            // Create the path to the folder with inputs
            var inputFolder = "Results";

            // If the folder exists, clear it
            if (Directory.Exists(inputFolder))
                Directory.EnumerateFiles(inputFolder).ForEach(File.Delete);

            // Otherwise create it
            else
                Directory.CreateDirectory(inputFolder);

            #endregion

            // Go through the all types of generated input files
            new[]
            {
                TriangleTwoObjectsPlusTwoObjects(),
                QuadrilateralAndTwoObjectsPlusTwoObjects()
            }
            // For each create input files within Results folder
            .ForEach(generatorInputs =>
            {
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
                        .Replace("{MaximalCircles}", input.MaximalNumbersOfObjectsToAdd[Circle].ToString())
                        // Replace the symmetry generation flag
                        .Replace("{GenerateOnlySymmetricConfigurations}", input.ExcludeAsymmetricConfigurations.ToString().ToLower());

                    // Create the directory where the file goes
                    Directory.CreateDirectory(Path.Combine(inputFolder, $"input_{counter}"));

                    // Write the content
                    File.WriteAllText(Path.Combine(inputFolder, $"input_{counter}/input_{counter}.txt"), content);
                }

                // Log how many files have been created
                Console.WriteLine($"Generated {counter} file(s)");

                #endregion
            });
        }

        #endregion

        #region Experiments

        /// <summary>
        /// An experiment where any triangle with two objects is extended by two more objects.
        /// </summary>
        /// <returns>The enumerable of inputs.</returns>
        private static IEnumerable<ProblemGeneratorInput> TriangleTwoObjectsPlusTwoObjects()
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
                { Point, 2 },
                { Line, 2 },
                { Circle, 2 }
            };

            // Prepare the generator input that doesn't exclude asymmetric configurations
            var problemGeneratorInput = new ProblemGeneratorInput(configuration, _constructions, numberOfIterations: 2, maximalNumbersOfObjectsObjectsToAdd, excludeAsymmetricConfigurations: false);

            // Prepare the generator settings
            var settings = new ProblemGeneratorSettings(numberOfPictures: 5);

            // Return the generation enumerable by taking the generator 
            return _kernel.Get<IProblemGenerator>(new ConstructorArgument("settings", settings))
                // Pass the input to it
                .Generate(problemGeneratorInput)
                // Unwrap the enumerable
                .generationOutputs
                // Take the configuration
                .Select(output => output.Configuration)
                // That is on the last, i.e. second iteration
                .Where(configuration => configuration.IterationIndex == 2)
                // Every generated configuration makes an input file
                .Select(configuration => new ProblemGeneratorInput(configuration, _constructions,
                    // We will want 2 iterations
                    numberOfIterations: 2,
                    // Set maximal numbers of objects to be added
                    new Dictionary<ConfigurationObjectType, int>
                    {
                        // Points and lines are not limited
                        { Point, 2 },
                        { Line, 2 },

                        // We want at most 2 circles in total. More are not necessary, since circles
                        // never appear as construction arguments, only in theorems itself, and at most
                        // two of them (when we have two tangent circles)
                        { Circle, 2 - configuration.ObjectMap.GetObjectsForKeys(Circle).Count() }
                    },
                    // We will want only symmetric results
                    excludeAsymmetricConfigurations: true));
        }

        /// <summary>
        /// An experiment where any quadrilateral with two objects is extended by two more objects.
        /// </summary>
        /// <returns>The enumerable of inputs.</returns>
        private static IEnumerable<ProblemGeneratorInput> QuadrilateralAndTwoObjectsPlusTwoObjects()
        {
            // Create the loose objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var D = new LooseConfigurationObject(Point);

            // Create the initial configuration
            var configuration = Configuration.DeriveFromObjects(Quadrilateral, A, B, C, D);

            // Create the dictionary with the counts of objects to be added
            var maximalNumbersOfObjectsObjectsToAdd = new Dictionary<ConfigurationObjectType, int>
            {
                { Point, 2 },
                { Line, 2 },
                { Circle, 2 }
            };

            // Prepare the generator input that doesn't exclude asymmetric configurations
            var problemGeneratorInput = new ProblemGeneratorInput(configuration, _constructions, numberOfIterations: 2, maximalNumbersOfObjectsObjectsToAdd, excludeAsymmetricConfigurations: false);

            // Prepare the generator settings
            var settings = new ProblemGeneratorSettings(numberOfPictures: 5);

            // Return the generation enumerable by taking the generator 
            return _kernel.Get<IProblemGenerator>(new ConstructorArgument("settings", settings))
                // Pass the input to it
                .Generate(problemGeneratorInput)
                // Unwrap the enumerable
                .generationOutputs
                // Take the configuration
                .Select(output => output.Configuration)
                // That is on the last, i.e. second iteration
                .Where(configuration => configuration.IterationIndex == 2)
                // Every generated configuration makes an input file
                .Select(configuration => new ProblemGeneratorInput(configuration, _constructions,
                    // We will want 2 iterations
                    numberOfIterations: 2,
                    // Set maximal numbers of objects to be added
                    new Dictionary<ConfigurationObjectType, int>
                    {
                        // Points and lines are not limited
                        { Point, 2 },
                        { Line, 2 },

                        // We want at most 2 circles in total. More are not necessary, since circles
                        // never appear as construction arguments, only in theorems itself, and at most
                        // two of them (when we have two tangent circles)
                        { Circle, 2 - configuration.ObjectMap.GetObjectsForKeys(Circle).Count() }
                    },
                    // We will want only symmetric results
                    excludeAsymmetricConfigurations: true));
        }

        #endregion
    }
}