using Ninject;
using System;
using System.Threading.Tasks;
using static GeoGen.ConsoleLauncher.Log;

namespace GeoGen.ConsoleLauncher
{
    /// <summary>
    /// The entry point of the application.
    /// </summary>
    public static class Startup
    {
        /// <summary>
        /// The main function.
        /// </summary>
        /// <param name="arguments">The arguments of the application.</param>
        public static async Task Main(string[] arguments)
        {
            try
            {
                // Load the settings
                // The first argument could be the settings file
                // If it's not, fall-back to the default value
                var settings = await SettingsLoader.LoadAsync(arguments.Length > 1 ? arguments[0] : "settings.json");

                // Initialize the IoC system
                await IoC.InitializeAsync(settings);

                // Run the algorithm
                await IoC.Kernel.Get<IBatchRunner>().FindAllInputFilesAndRunAlgorithmsAsync();

                // Log that we're done
                LoggingManager.LogInfo("The application has finished.\n");
            }
            // Catch for any unhandled exception
            catch (Exception e)
            {
                // Log if there is any
                LoggingManager.LogFatal($"An unexpected exception has occurred: \n\n{e}\n");

                // This is a sad end
                Environment.Exit(-1);
            }
        }
    }
}