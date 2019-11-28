using GeoGen.Algorithm;
using GeoGen.Constructor;
using GeoGen.DependenciesResolver;
using GeoGen.TheoremProver;
using GeoGen.Utilities;
using Ninject;
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
        /// <param name="settings">The settings of the application.</param>
        public static async Task InitializeAsync(Settings settings)
        {
            // Initialize the container
            Kernel = DependenciesResolver.IoC.CreateKernel();

            #region Logging system

            // Bind logging manager
            Kernel.Bind<ILoggingManager>().To<CustomLoggingManager>();

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
                        throw new SettingsException($"Unhandled type of the settings ('{loggersettings.GetType()}') in the NInject bindings.");
                }
            });

            // Setup static log service
            Log.LoggingManager = Kernel.Get<ILoggingManager>();

            #endregion

            #region Local dependencies

            // Add local dependencies
            Kernel.Bind<IParser>().To<Parser>();
            Kernel.Bind<IBatchRunner>().To<BatchRunner>().WithConstructorArgument(settings.InputFolderSettings);
            Kernel.Bind<IAlgorithmRunner>().To<AlgorithmRunner>().WithConstructorArgument(settings.AlgorithmRunnerSettings);
            Kernel.Bind<IAlgorithmInputProvider>().To<AlgorithmInputProvider>().WithConstructorArgument(settings.InputFolderSettings);
            Kernel.Bind<ITemplateTheoremProvider>().To<TemplateTheoremProvider>().WithConstructorArgument(settings.TemplateTheoremsFolderSettings);
            Kernel.Bind<IBestTheoremsTracker>().To<BestTheoremsTracker>().WithConstructorArgument(settings.BestTheoremsTrackerSettings);

            // Add tracers
            Kernel.Bind<ISubtheoremDeriverGeometryFailureTracer>().To<SubtheoremDeriverGeometryFailureTracer>().WithConstructorArgument(settings.TracersSettings.SubtheoremDeriverGeometryFailureTracerSettings);
            Kernel.Bind<IConstructorFailureTracer>().To<ConstructorFailureTracer>().WithConstructorArgument(settings.TracersSettings.ConstructorFailureTracerSettings);
            Kernel.Bind<IGeometryFailureTracer>().To<GeometryFailureTracer>().WithConstructorArgument(settings.TracersSettings.GeometryFailureTracerSettings);

            #endregion

            #region Algorithm

            // Add generator
            Kernel.AddGenerator()
                // With constructor that uses loaded settings
                .AddConstructor(settings.AlgorithmSettings.GeometryConstructorSettings)
                // With theorem finder and its settings
                .AddTheoremFinder(settings.AlgorithmSettings.TangentCirclesTheoremFinderSettings,
                                  settings.AlgorithmSettings.LineTangentToCircleTheoremFinderSettings,
                                  settings.AlgorithmSettings.SoughtTheoremTypes.ToReadOnlyHashSet())
                // With prover and its data
                .AddTheoremProver(new TheoremProverData
                (
                    // Template theorems are loaded at the beginning
                    templateTheorems: await Kernel.Get<ITemplateTheoremProvider>().GetTemplateTheoremsAsync()
                ))
                // With theorem ranker and its settings
                .AddTheoremRanker(settings.AlgorithmSettings.TheoremRankerSettings)
                // And finally the algorithm
                .AddAlgorithm();

            #endregion
        }

        #endregion
    }
}