using GeoGen.Infrastructure;
using Ninject;
using System;
using System.Threading.Tasks;
using static GeoGen.Infrastructure.Log;

namespace GeoGen.ConsoleLauncher
{
    /// <summary>
    /// The entry class of the application.
    /// </summary>
    public static class Startup
    {
        /// <summary>
        /// The entry method of the application.
        /// </summary>
        /// <param name="arguments">One optional argument, the serialized <see cref="Settings"/> in JSON format.
        /// If they are not specified, then they are attempted to be loaded from the default 'settings.json' file.</param>
        public static async Task Main(string[] arguments)
        {
            try
            {
                // Load the settings either directly from arguments
                var settings = arguments.Length > 0 ? SettingsLoader.LoadFromString<Settings>(arguments[0])
                    // Or from the default file settings.json
                    : await SettingsLoader.LoadFromFileAsync<Settings>("settings.json");

                // Initialize the IoC system
                await IoC.InitializeAsync(settings);

                // Run the algorithm
                await IoC.Kernel.Get<IBatchRunner>().FindAllInputFilesAndRunProblemGenerationAsync();

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