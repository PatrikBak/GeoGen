using GeoGen.ConfigurationGenerator;
using GeoGen.Constructor;
using GeoGen.Infrastructure;
using GeoGen.ProblemAnalyzer;
using GeoGen.ProblemGenerator;
using GeoGen.ProblemGenerator.InputProvider;
using GeoGen.TheoremFinder;
using GeoGen.TheoremProver;
using GeoGen.TheoremProver.InferenceRuleProvider;
using GeoGen.TheoremProver.ObjectIntroductionRuleProvider;
using GeoGen.TheoremRanker;
using GeoGen.TheoremRanker.RankedTheoremIO;
using GeoGen.TheoremSorter;
using GeoGen.Utilities;
using Ninject;

namespace GeoGen.MainLauncher
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

            #region Clear best theorem folders

            // Clear JSON best theorem folder
            IOUtilities.ClearDirectoryIfItExists(settings.GetSettings<ProblemGenerationRunnerSettings>().JsonBestTheoremFolder);

            // Clear Readable best theorem folder
            IOUtilities.ClearDirectoryIfItExists(settings.GetSettings<ProblemGenerationRunnerSettings>().ReadableBestTheoremFolder);

            #endregion

            // Run the algorithm
            await kernel.Get<IBatchRunner>().FindAllInputFilesAndRunProblemGenerationAsync();
        });

        /// <summary>
        /// Initializes the NInject kernel.
        /// </summary>
        /// <param name="kernel">The dependency injection container.</param>
        /// <param name="settings">The settings of the application.</param>
        private static async Task PrepareDIContainer(IKernel kernel, JsonConfiguration settings)
        {
            // Add ranked theorem writing and reading
            kernel.AddRankedTheoremIO();

            #region Local dependencies

            // Add local dependencies
            kernel.Bind<IBatchRunner>().To<BatchRunner>();
            kernel.Bind<IProblemGenerationRunner>().To<ProblemGenerationRunner>().WithConstructorArgument(settings.GetSettings<ProblemGenerationRunnerSettings>());
            kernel.Bind<ITheoremSorterTypeResolver>().To<TheoremSorterTypeResolver>().WithConstructorArgument(settings.GetSettings<TheoremSorterTypeResolverSettings>());

            #endregion

            #region Providers

            // Add inference rule provider
            kernel.AddInferenceRuleProvider(settings.GetSettings<InferenceRuleProviderSettings>())
                // Add object introduction rule provider
                .AddObjectIntroductionRuleProvider(settings.GetSettings<ObjectIntroductionRuleProviderSettings>())
                // Add problem generator input provider
                .AddProblemGeneratorInputProvider(settings.GetSettings<ProblemGeneratorInputProviderSettings>());

            #endregion

            // Load the inference rules
            var managerData = new InferenceRuleManagerData(await kernel.Get<IInferenceRuleProvider>().GetInferenceRulesAsync());

            // Use them to bind the tracker
            kernel.Bind<IInferenceRuleUsageTracker>().To<InferenceRuleUsageTracker>().WithConstructorArgument(managerData);

            #region Algorithm

            // Add the configuration generator with its settings
            kernel.AddConfigurationGenerator(settings.GetSettings<GenerationSettings>())
                // And the constructor
                .AddConstructor()
                // And the theorem finder
                .AddTheoremFinder(settings.GetSettings<TheoremFindingSettings>())
                // And the theorem ranker
                .AddTheoremRanker(settings.GetSettings<TheoremRankerSettings>())
                // And the theorem prover and with its settings
                .AddTheoremProver(new TheoremProvingSettings
                (
                    // Set the loaded manager data
                    inferenceRuleManagerData: managerData,

                    // Load the object introduction data as well
                    objectIntroducerData: new ObjectIntroducerData(await kernel.Get<IObjectIntroductionRuleProvider>().GetObjectIntroductionRulesAsync()),

                    // Set the prover's settings
                    theoremProverSettings: settings.GetSettings<TheoremProverSettings>()
                ))
                // And the problem generator and with its settings
                .AddProblemGenerator(settings.GetSettings<ProblemGeneratorSettings>())
                // And the problem analyzer
                .AddProblemAnalyzer()
                // And the sorter
                .AddTheoremSorter();

            #endregion

            #region Tracers

            // Rebind Constructor Failure Tracer only if we're supposed be tracing
            if (settings.GetSettings<bool>("TraceConstructorFailures"))
                kernel.Rebind<IConstructorFailureTracer>().To<ConstructorFailureTracer>().WithConstructorArgument(settings.GetSettings<ConstructorFailureTracerSettings>());

            // Rebind Geometry Failure Tracer only if we're supposed be tracing
            if (settings.GetSettings<bool>("TraceGeometryFailures"))
                kernel.Rebind<IGeometryFailureTracer>().To<GeometryFailureTracer>().WithConstructorArgument(settings.GetSettings<GeometryFailureTracerSettings>());

            // Rebind Invalid Inference Tracer only if we're supposed be tracing
            if (settings.GetSettings<bool>("TraceInvalidInferences"))
                kernel.Rebind<IInvalidInferenceTracer>().To<InvalidInferenceTracer>().WithConstructorArgument(settings.GetSettings<InvalidInferenceTracerSettings>());

            // Rebind Sorting Geometry Failure Tracer only if we're supposed be tracing
            if (settings.GetSettings<bool>("TraceSortingGeometryFailures"))
                kernel.Rebind<ISortingGeometryFailureTracer>().To<SortingGeometryFailureTracer>().WithConstructorArgument(settings.GetSettings<SortingGeometryFailureTracerSettings>());

            #endregion
        }
    }
}