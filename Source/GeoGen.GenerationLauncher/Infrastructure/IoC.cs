using GeoGen.Algorithm;
using GeoGen.ConsoleLauncher;
using GeoGen.Constructor;
using GeoGen.Generator;
using GeoGen.Infrastructure;
using GeoGen.TheoremFinder;
using GeoGen.TheoremProver;
using GeoGen.TheoremRanker;
using GeoGen.TheoremSimplifier;
using Ninject;
using System.Threading.Tasks;

namespace GeoGen.GenerationLauncher
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
        public static Task InitializeAsync(Settings settings)
        {
            // Initialize the container
            Kernel = Infrastructure.IoC.CreateKernel();

            // Add the logging system
            Kernel.AddLogging(settings.LoggingSettings);

            #region Local dependencies

            // Add local dependencies
            Kernel.Bind<IBatchRunner>().To<BatchRunner>().WithConstructorArgument(settings.InputFolderSettings);
            Kernel.Bind<IAlgorithmRunner>().To<GenerationAlgorithmRunner>().WithConstructorArgument(settings.GenerationAlgorithmRunnerSettings);
            Kernel.Bind<IAlgorithmInputProvider>().To<AlgorithmInputProvider>().WithConstructorArgument(settings.InputFolderSettings);

            #endregion           

            #region Algorithm

            // Add generator
            Kernel.AddGenerator(settings.GenerationSettings.ConfigurationFilterType)
                // With constructor
                .AddConstructor();

            // Add algorithm facade and its faked dependencies
            Kernel.Bind<IAlgorithmFacade>().To<AlgorithmFacade>().WithConstructorArgument(settings.AlgorithmFacadeSettings);
            Kernel.Bind<ITheoremProver>().To<EmptyTheoremProver>();
            Kernel.Bind<ITheoremFinder>().To<EmptyTheoremFinder>();
            Kernel.Bind<ITheoremRanker>().To<EmptyTheoremRanker>();
            Kernel.Bind<ITheoremSimplifier>().To<EmptyTheoremSimplifier>();

            #endregion

            #region Tracers

            // Rebind tracers
            Kernel.Rebind<IConstructorFailureTracer>().To<ConstructorFailureTracer>().WithConstructorArgument(settings.ConstructorFailureTracerSettings);
            Kernel.Rebind<IGeometryFailureTracer>().To<GeometryFailureTracer>().WithConstructorArgument(settings.GeometryFailureTracerSettings);

            #endregion

            // Return a finished task
            return Task.CompletedTask;
        }

        #endregion
    }
}