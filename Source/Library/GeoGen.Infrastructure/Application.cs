using GeoGen.Infrastructure;
using GeoGen.Utilities;
using Ninject;
using Serilog;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace GeoGen.MainLauncher
{
    /// <summary>
    /// A helper class that contains a convenient method for settings up an application with a configuration loaded
    /// from JSON files, logging using Serilog, and NInject dependency injection container.
    /// </summary>
    public static class Application
    {
        #region Public constants

        /// <summary>
        /// The path to the configuration that is attempted to be loaded if no explicit files via the command line 
        /// arguments are provided. This file is overwritten during each build, so it is not supposed to be edited 
        /// for customization. Instead, create and edit the file <paramref name="DevConfigurationFilePath"/>.
        /// </summary>
        public const string DefaultConfigurationFilePath = "settings.json";

        /// <summary>
        /// The path to the file where developer-specific configuration can be specified. This file is loaded
        /// only if no paths are specified via the command line arguments. 
        /// <para>One possible work-flow is that a developer creates a copy of <see cref="DefaultConfigurationFilePath"/>, 
        /// renames it to this file, and edits only this file for playing around with the values. 
        /// </para>
        /// </summary>
        public const string DevConfigurationFilePath = "settings.dev.json";

        #endregion

        #region Public methods

        /// <summary>
        /// Assembles all needed components for the application and performs the <paramref name="applicationCode"/> function.
        /// <para>
        /// <list type="number">
        /// <item>It builds <see cref="JsonConfiguration"/> using either <paramref name="customConfigurationFilePaths"/>, if they
        /// are not empty, or files <see cref="DefaultConfigurationFilePath"/> and <see cref="DevConfigurationFilePath"/>.</item>
        /// <item>Sets up Serilog logging using <see cref="LoggingUtilities.SetupSerilog(JsonConfiguration)"/>.</item>
        /// <item>Constructs an empty Ninject kernel using <see cref="NinjectUtilities.CreateKernel"/>.</item>
        /// <item>Runs the <paramref name="applicationCode"/> and logs its exceptions.</item>
        /// </list>
        /// </para>
        /// </summary>
        /// <param name="customConfigurationFilePaths">The paths to custom configuration files loaded in this order. If no files are 
        /// specified, then the files <see cref="DefaultConfigurationFilePath"/> and <see cref="DevConfigurationFilePath"/> are used.</param>
        /// <param name="applicationCode">The function performing the actual application code.</param>
        public static async Task Run(string[] customConfigurationFilePaths, Func<IKernel, JsonConfiguration, Task> applicationCode)
        {
            #region Load JsonConfiguration

            // Prepare the variable for settings
            JsonConfiguration jsonConfiguration;

            try
            {
                // If the custom files are specified...
                jsonConfiguration = customConfigurationFilePaths.Length != 0
                    // Load the configuration from them
                    ? new JsonConfiguration(customConfigurationFilePaths)
                    // Otherwise use the default files
                    : new JsonConfiguration(DefaultConfigurationFilePath, DevConfigurationFilePath);
            }
            catch (Exception e)
            {
                // Re-throw
                throw new ConfigurationException($"Cannot set up a JsonConfiguration from arguments: {customConfigurationFilePaths.ToJoinedString()}", e);
            }

            #endregion

            #region Logging

            try
            {

                // Ensure we have logging
                LoggingUtilities.SetupSerilog(jsonConfiguration);
            }
            catch (Exception e)
            {
                // Re-throw
                throw new LoggingException($"Cannot set up Serilog using the configuration loaded from {customConfigurationFilePaths.ToJoinedString()}", e);
            }

            #endregion

            #region Application Code

            try
            {
                // Log start
                Log.Information("The application has started.");

                // Log from where we have the configuration from
                Log.Information("Configuration loaded from files: {files}", jsonConfiguration.LoadedConfigurationFilePaths.ToJoinedString());

                // Prepare a stopwatch
                var stopwatch = Stopwatch.StartNew();

                // Execute the application code with a new empty kernel
                await applicationCode(NinjectUtilities.CreateKernel(), jsonConfiguration);

                // Log end
                Log.Information("The application has finished correctly in {time:F2} sec.", stopwatch.ElapsedMilliseconds / 1000d);
            }
            // Catch for any unhandled exception
            catch (Exception e)
            {
                // Log if there is any
                Log.Fatal(e, $"An unexpected exception has occurred.");

                // This is a sad end
                throw;
            }
            finally
            {
                // Wait for a click if we should do so...
                if (ShouldPauseBeforeExit())
                {
                    Console.WriteLine("\nPress any key to exit...");
                    Console.ReadKey();
                }

                // A function to check if we should pause before exit
                static bool ShouldPauseBeforeExit()
                {
                    // Linux/Mac: terminals set TERM environment variable
                    // Windows: check if we own the console

                    if (Environment.OSVersion.Platform == PlatformID.Unix)
                    {
                        // On Linux/Mac, if TERM is set, we're in a real terminal
                        // If not set, might be double-clicked (supposedly rare on Linux)
                        return string.IsNullOrEmpty(Environment.GetEnvironmentVariable("TERM"));
                    }

                    try
                    {
                        // Windows: check if console was created just for us
                        return GetConsoleProcessList(new uint[2], 2) <= 1;
                    }
                    catch
                    {
                        // If we fail, we'll pause just in case
                        return true;
                    }

                    // Retrieves a list of the processes attached to the current console.
                    [DllImport("kernel32.dll")]
                    static extern uint GetConsoleProcessList(uint[] processList, uint processCount);
                }
            }

            #endregion
        }

        #endregion
    }
}