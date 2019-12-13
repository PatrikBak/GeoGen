using GeoGen.Core;
using GeoGen.DependenciesResolver;
using GeoGen.Infrastructure;
using Ninject;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static GeoGen.Core.ConfigurationObjectType;
using static GeoGen.Core.LooseObjectsLayout;
using static GeoGen.Core.PredefinedConstructions;
using static GeoGen.Core.TheoremType;

namespace GeoGen.Drawer
{
    /// <summary>
    /// The class that runs tests of drawing.
    /// </summary>
    public static class DrawerTest
    {
        /// <summary>
        /// The main method.
        /// </summary>
        private static async Task Main()
        {
            try
            {
                // Load the settings
                var settings = await SettingsLoader.LoadAsync<DrawerSettings>("settings.json", new DefaultDrawerSettings());

                // Create the kernel 
                var kernel = IoC.CreateKernel()
                    // With logging
                    .AddLogging(settings.LoggingSettings)
                    // And the constructor module
                    .AddConstructor();

                // Bind the drawer with its settings
                kernel.Bind<IDrawer>().To<MetapostDrawer>().WithConstructorArgument(settings.MetapostDrawerSettings)
                    // And its data
                    // TODO: Parse and load them from a file
                    .WithConstructorArgument(new MetapostDrawerData(DrawingRules()));

                // Get the drawer and try run it on some configurations
                await kernel.Get<IDrawer>().DrawAsync(new[]
                {
                    MediansAreConcurrent()
                });
            }
            // Catch for any unhandled exception
            catch (Exception e)
            {
                // Handle known cases holding additional data
                switch (e)
                {
                    // Exception when compiling / post-compiling
                    case CommandException commandException:

                        // Write out the command
                        Log.LoggingManager.LogFatal($"An exception when performing the command '{commandException.CommandWithArguments}', " +
                            // And the code
                            $"exit code {commandException.ExitCode}, message: {commandException.Message}\n\n" +
                            // And the standard output
                            $"Standard output: {commandException.StandardOutput}\n\n" +
                            // And the error output
                            $"Error output: {commandException.ErrorOutput}\n\n");

                        break;

                    // The default case is a generic message
                    default:

                        // Log it
                        Log.LoggingManager.LogFatal($"An unexpected exception has occurred: \n\n{e}\n");

                        break;
                }

                // This is a sad end
                Environment.Exit(-1);
            }
        }

        private static IReadOnlyDictionary<Construction, DrawingRule> DrawingRules()
        {
            // Prepare the points
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var M = new ConstructedConfigurationObject(Midpoint, A, B);

            // Prepare the rule
            var midpointRule = new DrawingRule(M, Array.Empty<ConstructedConfigurationObject>(), new List<DrawingCommand>
            {
                // We want to mark the midpoint
                new DrawingCommand(DrawingCommandType.Point, ObjectDrawingStyle.NormalObject, new[] { M }),

                // As well as the segment 
                new DrawingCommand(DrawingCommandType.Segment, ObjectDrawingStyle.NormalObject, new[] { A, B })
            });

            // Return it in a dictionary
            return new Dictionary<Construction, DrawingRule> { { Midpoint, midpointRule } };
        }

        private static (Configuration, Theorem) MediansAreConcurrent()
        {
            // Prepare the objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var D = new ConstructedConfigurationObject(Midpoint, B, C);
            var E = new ConstructedConfigurationObject(Midpoint, C, A);
            var F = new ConstructedConfigurationObject(Midpoint, A, B);

            // Prepare the configuration
            var configuration = Configuration.DeriveFromObjects(Triangle, D, E, F);

            // Prepare the theorem
            var theorem = new Theorem(ConcurrentLines, new[]
            {
                new LineTheoremObject(A, D),
                new LineTheoremObject(B, E),
                new LineTheoremObject(C, F)
            });

            // Return them
            return (configuration, theorem);
        }
    }
}