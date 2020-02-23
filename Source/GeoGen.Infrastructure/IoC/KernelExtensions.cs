using GeoGen.Utilities;
using Ninject;

namespace GeoGen.Infrastructure
{
    /// <summary>
    /// The extension methods for <see cref="IKernel"/>.
    /// </summary>
    public static class KernelExtensions
    {
        /// <summary>
        /// Bindings for the logging system.
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        /// <param name="settings">The settings for the logging system.</param>
        /// <returns>The kernel for chaining.</returns>
        public static IKernel AddLogging(this IKernel kernel, LoggingSettings settings)
        {
            // Bind logging manager
            kernel.Bind<ILoggingManager>().To<CustomLoggingManager>();

            // Bind loggers according to the settings
            settings.Loggers.ForEach(loggersettings =>
            {
                // Switch based on the type of the settings
                switch (loggersettings)
                {
                    // If this is console settings...
                    case ConsoleLoggerSettings consoleLoggersettings:

                        // Bind it
                        kernel.Bind<ILogger>().ToConstant(new ConsoleLogger(consoleLoggersettings));

                        break;

                    // If this is file logger settings...
                    case FileLoggerSettings fileLoggersettings:

                        // Bind it
                        kernel.Bind<ILogger>().ToConstant(new FileLogger(fileLoggersettings));

                        break;

                    // Unhandled cases
                    default:
                        throw new SettingsException($"Unhandled type of {nameof(BaseLoggerSettings)}: ({loggersettings.GetType()}");
                }
            });

            // Setup static log service
            Log.LoggingManager = kernel.Get<ILoggingManager>();

            // Return the kernel for chaining
            return kernel;
        }
    }
}