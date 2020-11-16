using GeoGen.ConfigurationGenerator;
using GeoGen.Constructor;
using GeoGen.Infrastructure;
using GeoGen.MainLauncher;
using GeoGen.ProblemGenerator;
using GeoGen.ProblemGenerator.InputProvider;
using GeoGen.TheoremFinder;
using Ninject;
using System;
using System.Threading.Tasks;

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
        /// <param name="customConfigurationFilePaths"><inheritdoc cref="Application.Run(string[], Func{IKernel, JsonConfiguration, Task})"/></param>
        public static async Task Main(string[] customConfigurationFilePaths) => await Application.Run(customConfigurationFilePaths, async (kernel, settings) =>
        {
            // Initialize the container
            await PrepareDIContainer(kernel, settings);

            // Run the algorithm
            await kernel.Get<IBatchRunner>().FindAllInputFilesAndRunProblemGenerationAsync();
        });

        /// <summary>
        /// Initializes the NInject kernel.
        /// </summary>
        /// <param name="kernel">The dependency injection container.</param>
        /// <param name="settings">The settings of the application.</param>
        private static Task PrepareDIContainer(IKernel kernel, JsonConfiguration settings)
        {
            #region Local dependencies

            // Add local dependencies
            kernel.Bind<IBatchRunner>().To<BatchRunner>();
            kernel.Bind<IProblemGenerationRunner>().To<GenerationOnlyProblemGenerationRunner>().WithConstructorArgument(settings.GetSettings<GenerationOnlyProblemGenerationRunnerSettings>());

            #endregion           

            #region Problem generator

            // Add the configuration generator with its settings
            kernel.AddConfigurationGenerator(settings.GetSettings<GenerationSettings>())
                // Add the constructor
                .AddConstructor()
                // Add the problem generator with its settings
                .AddProblemGenerator(settings.GetSettings<ProblemGeneratorSettings>())
                // Add problem generator input provider
                .AddProblemGeneratorInputProvider(settings.GetSettings<ProblemGeneratorInputProviderSettings>());

            // Add an empty theorem finder
            kernel.Bind<ITheoremFinder>().To<EmptyTheoremFinder>();

            #endregion

            #region Tracers

            // Rebind Constructor Failure Tracer only if we're supposed be tracing
            if (settings.GetSettings<bool>("TraceConstructorFailures"))
                kernel.Rebind<IConstructorFailureTracer>().To<ConstructorFailureTracer>().WithConstructorArgument(settings.GetSettings<ConstructorFailureTracerSettings>());

            // Rebind Geometry Failure Tracer only if we're supposed be tracing
            if (settings.GetSettings<bool>("TraceGeometryFailures"))
                kernel.Rebind<IGeometryFailureTracer>().To<GeometryFailureTracer>().WithConstructorArgument(settings.GetSettings<GeometryFailureTracerSettings>());

            #endregion

            // Return a finished task
            return Task.CompletedTask;
        }
    }
}