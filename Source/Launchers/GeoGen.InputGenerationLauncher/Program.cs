﻿using GeoGen.ConfigurationGenerator;
using GeoGen.Constructor;
using GeoGen.Core;
using GeoGen.Infrastructure;
using GeoGen.ProblemGenerator;
using GeoGen.TheoremFinder;
using GeoGen.Utilities;
using Ninject;
using Ninject.Parameters;
using Serilog;
using System.Diagnostics;
using static GeoGen.Core.ConfigurationObjectType;
using static GeoGen.Core.LooseObjectLayout;

namespace GeoGen.InputGenerationLauncher
{
    /// <summary>
    /// The entry class of the application.
    /// </summary>
    public static class Program
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
            // Setup simple logger
            Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();

            // Initialize the kernel
            _kernel = NinjectUtilities.CreateKernel()
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
            var resultsFolder = "Results";

            // If the folder exists, clear it
            if (Directory.Exists(resultsFolder))
                Directory.Delete(resultsFolder, recursive: true);

            // Otherwise create it
            else
                Directory.CreateDirectory(resultsFolder);

            #endregion

            // Prepare the stopwatch
            var stopwatch = Stopwatch.StartNew();

            // Prepare the counter of generated inputs
            var counter = 0;

            // Go through the all types of generated input files
            new[]
            {
                Triangle_TwoObjects_PlusThreeObjects(),
                Triangle_FourObjects_OnlyFullySymmetric_PlusTwoObjects(),
                Triangle_FourObjects_TwoLinesAndTwoPoints_OnlySymmetric_OnlyUsedLines_PlusTwoObjects(),
                Triangle_FourObjects_ThreeLinesAndOnePoint_OnlySymmetric_OnlyUsedLines_PlusTwoObjects(),
                Quadrilateral_TwoObjects_PlusTwoObjects()
            }
            // Merge the inputs
            .Flatten()
            // Handle each generated file
            .ForEach(input =>
            {
                // Count the input in
                counter++;

                // Log the count, but not too often
                if (counter % 100 == 0)
                    Log.Information("{count} files after {time} ms.", counter, stopwatch.ElapsedMilliseconds);

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
                    // Replace the symmetry generation mode
                    .Replace("{SymmetryGenerationMode}", input.SymmetryGenerationMode.ToString());

                // Create the directory where the file goes
                Directory.CreateDirectory(Path.Combine(resultsFolder, $"input_{counter}"));

                // Write the content
                File.WriteAllText(Path.Combine(resultsFolder, $"input_{counter}/input_{counter}.txt"), content);
            });

            // Log how many files have been created
            Log.Information("Generated {counter} file(s) after {time} ms.", counter, stopwatch.ElapsedMilliseconds);
        }

        #endregion

        #region Experiments

        private static IEnumerable<ProblemGeneratorInput> Triangle_TwoObjects_PlusThreeObjects()
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
            var problemGeneratorInput = new ProblemGeneratorInput(configuration, _constructions, numberOfIterations: 2, maximalNumbersOfObjectsObjectsToAdd, SymmetryGenerationMode.GenerateBothSymmetricAndAsymmetric);

            // Prepare the generator settings
            var settings = new ProblemGeneratorSettings(numberOfPictures: 2);

            // Return the generation enumerable by taking the generator 
            return _kernel.Get<IProblemGenerator>(new ConstructorArgument("settings", settings))
                // Pass the input to it
                .Generate(problemGeneratorInput)
                // Unwrap the enumerable
                .generationOutputs
                // Take the configuration
                .Select(output => output.Configuration)
                // That is on the last iteration
                .Where(configuration => configuration.IterationIndex == 2)
                // Every generated configuration makes an input file
                .Select(configuration => new ProblemGeneratorInput(configuration, _constructions,
                    // We will want plus 3 objects
                    numberOfIterations: 3,
                    // Set maximal numbers of objects to be added
                    new Dictionary<ConfigurationObjectType, int>
                    {
                        // Points and lines are not limited
                        { Point, 3 },
                        { Line, 3 },

                        // We want at most 2 circles in total. More are not necessary, since circles
                        // never appear as construction arguments, only in theorems itself, and at most
                        // two of them (when we have two tangent circles)
                        { Circle, 2 - configuration.ObjectMap.GetObjectsForKeys(Circle).Count() }
                    },
                    // We will want only symmetric results
                    symmetryGenerationMode: SymmetryGenerationMode.GenerateOnlySymmetric));
        }

        private static IEnumerable<ProblemGeneratorInput> Quadrilateral_TwoObjects_PlusTwoObjects()
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
            var problemGeneratorInput = new ProblemGeneratorInput(configuration, _constructions, numberOfIterations: 2, maximalNumbersOfObjectsObjectsToAdd, SymmetryGenerationMode.GenerateBothSymmetricAndAsymmetric);

            // Prepare the generator settings
            var settings = new ProblemGeneratorSettings(numberOfPictures: 2);

            // Return the generation enumerable by taking the generator 
            return _kernel.Get<IProblemGenerator>(new ConstructorArgument("settings", settings))
                // Pass the input to it
                .Generate(problemGeneratorInput)
                // Unwrap the enumerable
                .generationOutputs
                // Take the configuration
                .Select(output => output.Configuration)
                // That is on the last iteration
                .Where(configuration => configuration.IterationIndex == 2)
                // Every generated configuration makes an input file
                .Select(configuration => new ProblemGeneratorInput(configuration, _constructions,
                    // We will want plus 2 objects
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
                    symmetryGenerationMode: SymmetryGenerationMode.GenerateOnlySymmetric));
        }

        private static IEnumerable<ProblemGeneratorInput> Triangle_FourObjects_OnlyFullySymmetric_PlusTwoObjects()
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
                { Point, 4 },
                { Line, 4 },
                { Circle, 2 }
            };

            // Prepare the generator input that takes only fully symmetric configurations
            var problemGeneratorInput = new ProblemGeneratorInput(configuration, _constructions, numberOfIterations: 4, maximalNumbersOfObjectsObjectsToAdd, SymmetryGenerationMode.GenerateOnlyFullySymmetric);

            // Prepare the generator settings
            var settings = new ProblemGeneratorSettings(numberOfPictures: 2);

            // Return the generation enumerable by taking the generator 
            return _kernel.Get<IProblemGenerator>(new ConstructorArgument("settings", settings))
                // Pass the input to it
                .Generate(problemGeneratorInput)
                // Unwrap the enumerable
                .generationOutputs
                // Take the configuration
                .Select(output => output.Configuration)
                // That is on the last iteration
                .Where(configuration => configuration.IterationIndex == 3)
                // Every generated configuration makes an input file
                .Select(configuration => new ProblemGeneratorInput(configuration, _constructions,
                    // We will want plus 2 objects
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
                    symmetryGenerationMode: SymmetryGenerationMode.GenerateOnlySymmetric));
        }

        private static IEnumerable<ProblemGeneratorInput> Triangle_FourObjects_TwoLinesAndTwoPoints_OnlySymmetric_OnlyUsedLines_PlusTwoObjects()
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
                { Circle, 0 }
            };

            // Prepare the generator input that takes only fully symmetric configurations
            var problemGeneratorInput = new ProblemGeneratorInput(configuration, _constructions, numberOfIterations: 4, maximalNumbersOfObjectsObjectsToAdd, SymmetryGenerationMode.GenerateOnlySymmetric);

            // Prepare the generator settings
            var settings = new ProblemGeneratorSettings(numberOfPictures: 2);

            // Return the generation enumerable by taking the generator 
            return _kernel.Get<IProblemGenerator>(new ConstructorArgument("settings", settings))
                // Pass the input to it
                .Generate(problemGeneratorInput)
                // Unwrap the enumerable
                .generationOutputs
                // Take the configuration
                .Select(output => output.Configuration)
                // That is on the last iteration
                .Where(configuration => configuration.IterationIndex == 4)
                // Exclude those where there is a hanging line
                .Where(configuration => !configuration.ObjectMap.GetObjectsForKeys(Line).Any(line
                    // We don't want a line that is not used in a construction    
                    => configuration.ConstructedObjects.All(constructedObject => !constructedObject.PassedArguments.FlattenedList.Contains(line))))
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
                    symmetryGenerationMode: SymmetryGenerationMode.GenerateOnlySymmetric));
        }

        private static IEnumerable<ProblemGeneratorInput> Triangle_FourObjects_ThreeLinesAndOnePoint_OnlySymmetric_OnlyUsedLines_PlusTwoObjects()
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
                { Line, 3 },
                { Circle, 0 }
            };

            // Prepare the generator input that takes only fully symmetric configurations
            var problemGeneratorInput = new ProblemGeneratorInput(configuration, _constructions, numberOfIterations: 4, maximalNumbersOfObjectsObjectsToAdd, SymmetryGenerationMode.GenerateOnlySymmetric);

            // Prepare the generator settings
            var settings = new ProblemGeneratorSettings(numberOfPictures: 2);

            // Return the generation enumerable by taking the generator 
            return _kernel.Get<IProblemGenerator>(new ConstructorArgument("settings", settings))
                // Pass the input to it
                .Generate(problemGeneratorInput)
                // Unwrap the enumerable
                .generationOutputs
                // Take the configuration
                .Select(output => output.Configuration)
                // That is on the last iteration
                .Where(configuration => configuration.IterationIndex == 4)
                // Exclude those where there is a hanging line
                .Where(configuration => !configuration.ObjectMap.GetObjectsForKeys(Line).Any(line
                    // We don't want a line that is not used in a construction    
                    => configuration.ConstructedObjects.All(constructedObject => !constructedObject.PassedArguments.FlattenedList.Contains(line))))
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
                    symmetryGenerationMode: SymmetryGenerationMode.GenerateOnlySymmetric));
        }

        #endregion
    }
}