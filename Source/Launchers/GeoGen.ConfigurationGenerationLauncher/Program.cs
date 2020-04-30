using GeoGen.Infrastructure;
using GeoGen.MainLauncher;
using Ninject;
using System;
using System.Threading.Tasks;
using static GeoGen.Infrastructure.Log;

namespace GeoGen.ConfigurationGenerationLauncher
{
    /// <summary>
    /// The entry class of the application.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The entry method of the application.
        /// </summary>
        public static async Task Main()
        {
            try
            {
                // Load the settings from the default file
                var settings = await SettingsLoader.LoadFromFileAsync<Settings>("settings.json");

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