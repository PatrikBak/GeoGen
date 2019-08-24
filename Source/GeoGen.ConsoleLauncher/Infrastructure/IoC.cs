using GeoGen.DependenciesResolver;
using GeoGen.TheoremsAnalyzer;
using GeoGen.Utilities;
using Ninject;
using System;
using System.Threading.Tasks;

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
        public static async Task InitializeAsync()
        {
            // Load the settings
            var settings = SettingsLoader.Load();

            // Initialize the container
            Kernel = DependenciesResolver.IoC.CreateKernel();

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

                        break;

                    // If this is file logger settings...
                    case FileLoggerSettings fileLoggersettings:

                        // Bind it
                        Kernel.Bind<ILogger>().ToConstant(new FileLogger(fileLoggersettings));

                        break;

                    default:

                        // Otherwise we forgot something
                        throw new Exception($"Unhandled type of the settings ('{loggersettings.GetType()}') in the NInject settings.");
                }
            });

            // Setup static log service
            Log.LoggingManager = Kernel.Get<ILoggingManager>();

            #endregion

            // Add local dependencies
            Kernel.Bind<IParser>().To<Parser>();
            Kernel.Bind<IBatchRunner>().To<BatchRunner>().WithConstructorArgument(settings.InputFolderSettings);
            Kernel.Bind<IAlgorithmRunner>().To<AlgorithmRunner>().WithConstructorArgument(settings.AlgorithmRunnerSettings);
            Kernel.Bind<IGeneratorInputsProvider>().To<GeneratorInputsProvider>().WithConstructorArgument(settings.InputFolderSettings);
            Kernel.Bind<ITemplateTheoremProvider>().To<TemplateTheoremProvider>().WithConstructorArgument(settings.TemplateTheoremsFolderSettings);

            // Add generator
            Kernel.AddGenerator()
                // With constructor that uses loaded settings
                .AddConstructor(settings.PicturesManagerSettings)
                // With theorems finder and its settings
                .AddTheoremsFinder(settings.TangentCirclesTheoremsFinderSettings,
                                   settings.LineTangentToCircleTheoremsFinderSettings,
                                   settings.SeekedTheoremTypes.ToReadOnlyHashSet())
                // With analyzer and its data
                .AddTheoremsAnalyzer(new TheoremsAnalyzerData
                {
                    // Template theorems are loaded at the beginning
                    TemplateTheorems = await Kernel.Get<ITemplateTheoremProvider>().GetTemplateTheoremsAsync()
                })
                // And finally the algorithm
                .AddAlgorithm();
        }

        #endregion
    }
}
