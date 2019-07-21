using GeoGen.DependenciesResolver;
using Ninject;
using System;

namespace GeoGen.ConsoleLauncher
{
    /// <summary>
    /// Represents a static initializer of the dependency injection system.
    /// </summary>
    public static class IoC
    {
        #region Kernel

        /// <summary>
        /// Gets the dependency injection container.
        /// </summary>
        public static IKernel Kernel { get; private set; }

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes the <see cref="Kernel"/> and all the dependencies.
        /// </summary>
        public static void Initialize()
        {
            // Load the settings
            var settings = SettingsLoader.Load();

            // Initialize the container
            Kernel = IoCUtilities.CreateKernel();

            #region Bind logging system

            // Bind logging manager
            Kernel.Bind<ILoggingManager>().To<DefaultLoggingManager>();

            // Bind loggers according to the settings
            settings.Loggers.ForEach(loggersettings =>
            {
                // Switch based on the type of the settings
                switch (loggersettings)
                {
                    // If this is console settings...
                    case ConsoleLoggerSettings consoleLoggersettings:
                        // Bind it
                        Kernel.Bind<ILogger>().ToConstant(new ConsoleLogger(consoleLoggersettings));
                        // And terminate
                        break;

                    // If this is file logger settings...
                    case FileLoggerSettings fileLoggersettings:
                        // Bind it
                        Kernel.Bind<ILogger>().ToConstant(new FileLogger(fileLoggersettings));
                        // And terminate
                        break;

                    default:
                        // Otherwise we forgot something
                        throw new Exception($"Unhandled type of the settings ('{loggersettings.GetType()}') in the NInject settings.");
                }
            });

            // Setup static log service
            Log.LoggingManager = Kernel.Get<ILoggingManager>();

            #endregion

            // Add GeoGen modules
            Kernel.AddGenerator().AddConstructor().AddTheoremsFinder();
        }

        #endregion
    }
}
