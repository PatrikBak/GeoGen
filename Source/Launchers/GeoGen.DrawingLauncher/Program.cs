using GeoGen.Core;
using GeoGen.Infrastructure;
using GeoGen.TheoremRanker;
using GeoGen.TheoremRanker.RankedTheoremIO;
using GeoGen.Utilities;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static GeoGen.Infrastructure.Log;

namespace GeoGen.DrawingLauncher
{
    /// <summary>
    /// The entry class of the application.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The entry method of the application.
        /// </summary>
        private static async Task Main()
        {
            try
            {
                // Load the settings
                var settings = await SettingsLoader.LoadFromFileAsync<Settings>("settings.json");

                // Initialize the IoC system
                await IoC.InitializeAsync(settings);

                // Run the UI loop
                await UILoopAsync(reorderObjects: settings.ReorderObjects);
            }
            // Catch for any unhandled exception
            catch (Exception e)
            {
                // Let the helper method log it
                LogException(e);

                // This is a sad end
                Environment.Exit(-1);
            }
        }

        /// <summary>
        /// The main loop of a simple UI. 
        /// </summary>
        /// <returns>The task representing the asynchronous operation.</returns>
        private static async Task UILoopAsync(bool reorderObjects)
        {
            // Empty line
            Console.WriteLine();

            // Prepare the read content of the current file
            RankedTheorem[] content = null;

            // Loop until break
            while (true)
            {
                #region Getting input file

                // If we don't have a file...
                if (content == null)
                {
                    // Ask for the file
                    Console.Write("Enter the name of the file: ");

                    // Get the file
                    var path = Console.ReadLine().Trim();

                    // Empty line
                    Console.WriteLine();

                    // Remove quotes from it (the result of a drag-and-drop)
                    path = path.Replace("\"", "");

                    #region Reading the file

                    try
                    {
                        // Read the content
                        content = IoC.Kernel.Get<IRankedTheoremJsonLazyReader>().Read(path).ToArray();

                        // Make sure there is any theorem
                        if (content.Length == 0)
                            throw new DrawingLauncherException("No theorems to draw in the file.");

                        // Log their count
                        Console.WriteLine($"Number of theorems in the file: {content.Length}.");
                    }
                    catch (Exception e)
                    {
                        // If there a problem, log it
                        LogException(e);

                        // Empty line
                        Console.WriteLine();

                        // Reset the current file
                        content = null;

                        // Move on
                        continue;
                    }

                    // Empty line
                    Console.WriteLine();

                    #endregion
                }

                #endregion               

                #region Getting interval or another file command

                // Ask for it
                Console.Write("Enter the interval start-end (indexing from 1), or a single number, or 'q' for another file: ");

                // Get the command
                var command = Console.ReadLine().Trim();

                // Empty line
                Console.WriteLine();

                // If we have a 'q'...
                if (command == "q")
                {
                    // Then we want to quit this file
                    content = null;

                    // Move on
                    continue;
                }

                // Prepare the start and end of the interval of images that we're drawing
                int start;
                int end;

                try
                {
                    // Try to parse it as a single number
                    if (int.TryParse(command, out var number))
                    {
                        // Then the start is the same as the end
                        start = number;
                        end = number;

                        // Make sure it's within the accepted range
                        if (number > content.Length || number < 0)
                            throw new DrawingLauncherException($"The number must be in the interval [1, {content.Length}]");
                    }
                    // Otherwise try to parse it as an interval
                    else
                    {
                        // Otherwise we have an interval. Try to parse it
                        var matches = Regex.Match(command, "^(\\d+)-(\\d+)$");

                        // If we didn't have a match, make aware
                        if (!matches.Success)
                            throw new DrawingLauncherException($"Cannot parse the interval: {command}");

                        // Otherwise try to parse the start
                        if (!int.TryParse(matches.Groups[1].Value, out start))
                            throw new DrawingLauncherException($"Cannot parse the number: {matches.Groups[1].Value}");

                        // As well as the end
                        if (!int.TryParse(matches.Groups[2].Value, out end))
                            throw new DrawingLauncherException($"Cannot parse the number: {matches.Groups[2].Value}");

                        // Make sure they are correct
                        if (!(1 <= start && start <= end && end <= content.Length))
                            throw new DrawingLauncherException($"It must hold 1 <= start <= end <= {content.Length}, but start = {start} and end = {end}.");
                    }
                }
                catch (Exception e)
                {
                    // If there a problem, log it
                    LogException(e);

                    // Empty line
                    Console.WriteLine();

                    // Move on 
                    continue;
                }

                #endregion

                #region Reading and drawing

                try
                {
                    // Get the needed configurations and theorems for drawing
                    var drawerInput = content.ItemsBetween(start - 1, end)
                        // Make them symmetric if we are supposed to
                        .Select(rankedTheorem =>
                        {
                            // If not, we're done
                            if (!reorderObjects)
                                return rankedTheorem;

                            // If yes, make the configuration symmetric
                            var configuration = NormalizeOrderOfLooseObjectsBasedOnSymmetry((rankedTheorem.Configuration, rankedTheorem.Theorem));

                            // And return the altered ranked theorem object
                            return new RankedTheorem(rankedTheorem.Theorem, rankedTheorem.Ranking, configuration);
                        });

                    // Perform the drawing for the desired input
                    await IoC.Kernel.Get<IDrawer>().DrawAsync(drawerInput, startingId: start);
                }
                catch (Exception e)
                {
                    // If there a problem, log it
                    LogException(e);

                    // Empty line
                    Console.WriteLine();
                }

                #endregion
            }
        }

        /// <summary>
        /// Reorders the loose object of a configuration that have a triangle layout in such a way that 
        /// the first will be the one that is fixed in the symmetry remapping (the drawing will cause
        /// it to be 'above').
        /// </summary>
        /// <param name="pair">The configuration and the theorem that determine the symmetry.</param>
        /// <returns>The configuration with changed loose objects, or the same configuration if the change cannot be done.</returns>
        private static Configuration NormalizeOrderOfLooseObjectsBasedOnSymmetry((Configuration, Theorem) pair)
        {
            // Deconstruct
            var (configuration, theorem) = pair;

            // We will make a use of a simple function that converts an object mapping to a loose object mapping
            static IReadOnlyDictionary<LooseConfigurationObject, LooseConfigurationObject> ExtractLooseObjects(IReadOnlyDictionary<ConfigurationObject, ConfigurationObject> mapping)
                // Take pairs of loose objects
                => mapping.Where(pair => pair.Key is LooseConfigurationObject)
                        // Recreate the dictionary
                        .ToDictionary(pair => (LooseConfigurationObject)pair.Key, pair => (LooseConfigurationObject)pair.Value);

            // Another useful function extracts a pair of exchanged objects from a mapping
            static (LooseConfigurationObject, LooseConfigurationObject) ExtractExchangedObjects(IReadOnlyDictionary<LooseConfigurationObject, LooseConfigurationObject> mapping)
                // First make tuples
                => mapping.Select(pair => (pair.Key, pair.Value))
                    // Exclude identities
                    .Where(pair => !pair.Key.Equals(pair.Value))
                    // Now take the first needed one, or default
                    .FirstOrDefault(pair => mapping[pair.Value].Equals(pair.Key));

            // Switch based on the layout
            switch (configuration.LooseObjectsHolder.Layout)
            {
                // In triangle cases symmetry means to put A above
                case LooseObjectLayout.Triangle:
                {
                    // Take a symmetry mapping of loose objects
                    var symmetryMapping = theorem.GetSymmetryMappings(configuration).Select(ExtractLooseObjects).FirstOrDefault();

                    // If there is no symmetry, no change
                    if (symmetryMapping == null)
                        return configuration;

                    // Otherwise there must be two exchanged points
                    var (point1, point2) = ExtractExchangedObjects(symmetryMapping);

                    // Find the last one as well
                    var point3 = symmetryMapping.Keys.Except(new[] { point1, point2 }).Single();

                    // Get the final order in which the fixed point goes first
                    var newLooseObjects = new LooseObjectHolder(new[] { point3, point1, point2 }, LooseObjectLayout.Triangle);

                    // Return the final result
                    return new Configuration(newLooseObjects, configuration.ConstructedObjects);
                }

                // In quadrilateral cases there are two mains to make symmetry
                case LooseObjectLayout.Quadrilateral:
                {
                    // Go through the symmetry mappings 
                    foreach (var symmetryMapping in theorem.GetSymmetryMappings(configuration).Select(ExtractLooseObjects))
                    {
                        // There must be two exchanged points
                        var (point1, point2) = ExtractExchangedObjects(symmetryMapping);

                        // Get the other two points as well
                        var otherPoints = symmetryMapping.Keys.Except(new[] { point1, point2 }).ToArray();

                        // In the ideal case we want to place a side horizontally
                        // But that will look symmetric only if the other points are exchangeable as well
                        // That can be tested by testing just one point
                        if (symmetryMapping[otherPoints[0]].Equals(otherPoints[1]))
                        {
                            // If this is the case, then we will not get a better order
                            // We can even choose what will be a side, let's say point1, point2
                            var newLooseObjects = new LooseObjectHolder(new[] { otherPoints[0], point1, point2, otherPoints[1] }, LooseObjectLayout.Quadrilateral);

                            // Return the final result
                            return new Configuration(newLooseObjects, configuration.ConstructedObjects);
                        }
                        // Otherwise this means we cannot place a side horizontally, but we may try it with a diagonal
                        else
                        {
                            // The diagonal must be the second and the last point
                            var newLooseObjects = new LooseObjectHolder(new[] { otherPoints[0], point1, otherPoints[1], point2 }, LooseObjectLayout.Quadrilateral);

                            // Reset the configuration. We will not break because there still might be an order that allows
                            // us to place a side horizontally
                            configuration = new Configuration(newLooseObjects, configuration.ConstructedObjects);
                        }
                    }

                    // Return the configuration, no matter what happened to it
                    return configuration;
                }

                // In other cases we will not do anything
                default:
                    return configuration;
            }
        }

        /// <summary>
        /// Logs a given exception.
        /// </summary>
        /// <param name="exception">The exception to be logged.</param>
        private static void LogException(Exception exception)
        {
            // Handle known cases holding additional data
            switch (exception)
            {
                // Exception when compiling / post-compiling
                case CommandException commandException:

                    // Prepare the message containing the command
                    var message = $"An exception when performing the command '{commandException.CommandWithArguments}', " +
                        // And the exit code
                        $"exit code {commandException.ExitCode}";

                    // If the message isn't empty, append it
                    if (!commandException.Message.IsEmpty())
                        message += $", message: {commandException.Message}";

                    // If the standard output isn't empty, append it
                    if (!commandException.StandardOutput.IsEmpty())
                        message += $"\n\nStandard output:\n\n{commandException.StandardOutput}";

                    // If the standard output isn't empty, append it
                    if (!commandException.ErrorOutput.IsEmpty())
                        message = $"{message.Trim()}\n\nError output:\n\n{commandException.ErrorOutput}";

                    // Write out the exception
                    LoggingManager.LogFatal(message);

                    break;

                // The default case is a generic message
                default:

                    // Log it
                    LoggingManager.LogFatal($"An unexpected exception has occurred: \n\n{exception}");

                    break;
            }
        }
    }
}