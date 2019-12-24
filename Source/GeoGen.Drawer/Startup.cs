using GeoGen.Core;
using GeoGen.Infrastructure;
using GeoGen.Utilities;
using Ninject;
using System;
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

                // Get the drawer and try run it on some configurations
                await IoC.Kernel.Get<IDrawer>().DrawAsync(new[]
                {
                    MediansAreConcurrent(),
                    IncircleTouchesNinePointCircle(),
                    HomeRound()
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

                        // Prepare the message containing the command
                        var message = $"An exception when performing the command '{commandException.CommandWithArguments}', " +
                            // And the exit code
                            $"exit code {commandException.ExitCode}";

                        // If the message isn't empty, append it
                        if (!commandException.Message.IsEmpty())
                            message += $" message: {commandException.Message}";

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

        private static (Configuration, Theorem) HomeRound()
        {
            // Create the objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var l1 = new ConstructedConfigurationObject(PerpendicularBisector, A, B);
            var l2 = new ConstructedConfigurationObject(PerpendicularBisector, A, C);
            var D = new ConstructedConfigurationObject(IntersectionOfLineAndLineFromPoints, l1, B, C);
            var E = new ConstructedConfigurationObject(IntersectionOfLineAndLineFromPoints, l2, B, C);
            var p1 = new ConstructedConfigurationObject(ParallelLineToLineFromPoints, D, A, C);
            var p2 = new ConstructedConfigurationObject(ParallelLineToLineFromPoints, E, A, B);
            var F = new ConstructedConfigurationObject(IntersectionOfLines, p1, p2);

            // Create the configuration
            var configuration = Configuration.DeriveFromObjects(Triangle, F);

            // Create the theorem
            var theorem = new Theorem(EqualLineSegments, new[]
            {
                new LineSegmentTheoremObject(F, B),
                new LineSegmentTheoremObject(F, C)
            });

            // Return them
            return (configuration, theorem);
        }
    }
}