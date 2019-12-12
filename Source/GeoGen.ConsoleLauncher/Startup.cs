using GeoGen.Infrastructure;
using Ninject;
using System;
using System.Threading.Tasks;
using static GeoGen.Infrastructure.Log;

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
        /// <param name="arguments">One optional argument, a path to a settings file. Its default value is "settings.json".</param>
        public static async Task Main(string[] arguments)
        {
            try
            {
                // The first argument could be the settings file
                // If it's not, fall-back to the default value
                var settingsFileName = arguments.Length > 1 ? arguments[0] : "settings.json";

                // Load the settings
                var settings = await SettingsLoader.LoadAsync<Settings>(settingsFileName, new DefaultSettings());

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