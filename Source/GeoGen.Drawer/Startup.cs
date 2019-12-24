using GeoGen.Algorithm;
using GeoGen.ConsoleLauncher;
using GeoGen.Infrastructure;
using GeoGen.Utilities;
using Ninject;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GeoGen.Drawer
{
    /// <summary>
    /// The class that runs tests of drawing.
    /// </summary>
    public static class Startup
    {
        /// <summary>
        /// The main method.
        /// </summary>
        private static async Task Main()
        {
            try
            {
                // Load the settings
                var settings = await SettingsLoader.LoadFromFileAsync<DrawerSettings>("settings.json", new DefaultDrawerSettings());

                // Initialize the IoC system
                await IoC.InitializeAsync(settings);

                // Run the UI loop
                await UILoopAsync();
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
        private static async Task UILoopAsync()
        {
            // Empty line
            Console.WriteLine();

            // Prepare the read content of the current file
            TheoremWithRanking[] content = null;

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
                        content = IoC.Kernel.Get<ITheoremsWithRankingJsonLazyReader>().Read(path).ToArray();

                        // Make sure there is any theorem
                        if (content.Length == 0)
                            throw new DrawerException("No theorems to draw in the file.");

                        // Log their count
                        Console.WriteLine($"Number of theorems in the file: {content.Length}.\n");
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
                            throw new DrawerException($"The number must be in the interval [1, {content.Length}]");
                    }
                    // Otherwise try to parse it as an interval
                    else
                    {
                        // Otherwise we have an interval. Try to parse it
                        var matches = Regex.Match(command, "^(\\d+)-(\\d+)$");

                        // If we didn't have a match, make aware
                        if (!matches.Success)
                            throw new DrawerException($"Cannot parse the interval: {command}");

                        // Otherwise try to parse the start
                        if (!int.TryParse(matches.Groups[1].Value, out start))
                            throw new DrawerException($"Cannot parse the number: {matches.Groups[1].Value}");

                        // As well as the end
                        if (!int.TryParse(matches.Groups[2].Value, out end))
                            throw new DrawerException($"Cannot parse the number: {matches.Groups[2].Value}");

                        // Make sure they are correct
                        if (!(1 <= start && start <= end && end <= content.Length))
                            throw new DrawerException($"It must hold 1 <= start <= end <= {content.Length}, but start = {start} and end = {end}.");
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
                    var drawerInput = content.ItemsBetween(start - 1, end).Select(theoremWithRanking => (theoremWithRanking.Configuration, theoremWithRanking.Theorem));

                    // Perform the drawing for the desired input
                    await IoC.Kernel.Get<IDrawer>().DrawAsync(drawerInput);
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
                    Log.LoggingManager.LogFatal(message);

                    break;

                // The default case is a generic message
                default:

                    // Log it
                    Log.LoggingManager.LogFatal($"An unexpected exception has occurred: \n\n{exception}");

                    break;
            }
        }
    }
}