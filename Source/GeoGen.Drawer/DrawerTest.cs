using GeoGen.Core;
using GeoGen.DependenciesResolver;
using GeoGen.Infrastructure;
using Ninject;
using System;
using System.Linq;
using System.Threading.Tasks;
using static GeoGen.Core.ComposedConstructions;
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

                // Bind the rules provider
                kernel.Bind<IDrawingRulesProvider>().To<DrawingRulesProvider>().WithConstructorArgument(settings.DrawingRulesProviderSettings);

                // Bind the drawer with its settings
                kernel.Bind<IDrawer>().To<MetapostDrawer>().WithConstructorArgument(settings.MetapostDrawerSettings)
                    // And its data
                    .WithConstructorArgument(new MetapostDrawerData
                    (
                        // Loaded via the drawing rules provider
                        (await kernel.Get<IDrawingRulesProvider>().GetDrawingRulesAsync()).ToDictionary(rule => rule.ObjectToDraw.Construction, rule => rule)
                    ));

                // Get the drawer and try run it on some configurations
                await kernel.Get<IDrawer>().DrawAsync(new[]
                {
                    MediansAreConcurrent(),
                    IncircleTouchesNinePointCircle()
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

        private static (Configuration, Theorem) MediansAreConcurrent()
        {
            // Create the objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var D = new ConstructedConfigurationObject(Midpoint, B, C);
            var E = new ConstructedConfigurationObject(Midpoint, C, A);
            var F = new ConstructedConfigurationObject(Midpoint, A, B);

            // Create the configuration
            var configuration = Configuration.DeriveFromObjects(Triangle, D, E, F);

            // Create the theorem
            var theorem = new Theorem(ConcurrentLines, new[]
            {
                new LineTheoremObject(A, D),
                new LineTheoremObject(B, E),
                new LineTheoremObject(C, F)
            });

            // Return them
            return (configuration, theorem);
        }

        private static (Configuration, Theorem) IncircleTouchesNinePointCircle()
        {
            // Create the objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var i = new ConstructedConfigurationObject(Incircle, A, B, C);
            var P = new ConstructedConfigurationObject(Midpoint, B, C);
            var Q = new ConstructedConfigurationObject(Midpoint, C, A);
            var R = new ConstructedConfigurationObject(Midpoint, A, B);

            // Create the configuration
            var configuration = Configuration.DeriveFromObjects(Triangle, i, P, Q, R);

            // Create the theorem
            var theorem = new Theorem(TangentCircles, new[]
            {
                new CircleTheoremObject(P, Q, R),
                new CircleTheoremObject(i),
            });

            // Return them
            return (configuration, theorem);
        }
    }
}