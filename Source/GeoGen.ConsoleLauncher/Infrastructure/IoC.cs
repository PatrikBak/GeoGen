using GeoGen.Algorithm;
using GeoGen.Constructor;
using GeoGen.Generator;
using GeoGen.Infrastructure;
using GeoGen.TheoremFinder;
using GeoGen.TheoremProver;
using GeoGen.TheoremRanker;
using GeoGen.TheoremSimplifier;
using GeoGen.Utilities;
using Ninject;
using Ninject.Extensions.Factory;
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
            Kernel = Infrastructure.IoC.CreateKernel();

            // Add the logging system
            Kernel.AddLogging(settings.LoggingSettings);

            #region Local dependencies

            // Add local dependencies
            Kernel.Bind<IBatchRunner>().To<BatchRunner>().WithConstructorArgument(settings.InputFolderSettings);
            Kernel.Bind<IAlgorithmRunner>().To<DebugAlgorithmRunner>().WithConstructorArgument(settings.DebugAlgorithmRunnerSettings);
            Kernel.Bind<IAlgorithmInputProvider>().To<AlgorithmInputProvider>().WithConstructorArgument(settings.InputFolderSettings);
            Kernel.Bind<IInferenceRuleProvider>().To<InferenceRuleProvider>().WithConstructorArgument(settings.InferenceRuleFolderSettings);
            Kernel.Bind<ISimplificationRuleProvider>().To<SimplificationRuleProvider>().WithConstructorArgument(settings.SimplificationRuleProviderSettings);
            Kernel.Bind<IRankedTheoremWriter>().To<RankedTheoremWriter>();
            Kernel.Bind<ITheoremWithRankingJsonLazyWriterFactory>().ToFactory();
            Kernel.Bind<ITheoremWithRankingJsonLazyWriter>().To<TheoremWithRankingJsonLazyWriter>();

            #endregion           

            // Load the inference rules
            var managerData = new InferenceRuleManagerData(await Kernel.Get<IInferenceRuleProvider>().GetInferenceRulesAsync());

            // Use them to find the tracker
            Kernel.Bind<IInferenceRuleUsageTracker>().To<InferenceRuleUsageTracker>().WithConstructorArgument(managerData);

            #region Algorithm

            // Add generator
            Kernel.AddGenerator(settings.AlgorithmSettings.GenerationSettings.ConfigurationFilterType)
                // With constructor
                .AddConstructor()
                // With theorem finder and its settings
                .AddTheoremFinder(settings.AlgorithmSettings.TheoremFindingSettings.TangentCirclesTheoremFinderSettings,
                                  settings.AlgorithmSettings.TheoremFindingSettings.LineTangentToCircleTheoremFinderSettings,
                                  settings.AlgorithmSettings.TheoremFindingSettings.SoughtTheoremTypes.ToReadOnlyHashSet())
                // With theorem ranker and its settings
                .AddTheoremRanker(settings.AlgorithmSettings.TheoremRankingSettings.TheoremRankerSettings,
                                  settings.AlgorithmSettings.TheoremRankingSettings.TypeRankerSettings)
                // With theorem prover and its data
                .AddTheoremProver(managerData)
                // With theorem simplifier and its data
                .AddTheoremSimplifier(new TheoremSimplifierData
                (
                    // Simplification rules are loaded at the beginning
                    rules: (await Kernel.Get<ISimplificationRuleProvider>().GetSimplificationRulesAsync()).ToReadOnlyHashSet()
                ))
                // And finally the algorithm with its settings
                .AddAlgorithm(settings.AlgorithmSettings.AlgorithmFacadeSettings, settings.AlgorithmSettings.BestTheoremFinderSettings);

            #endregion

            #region Tracers

            #region ConstructorFailureTracer

            // Rebind Constructor Failure Tracer only if we're supposed be tracing
            if (settings.TracingSettings.ConstructorFailureTracerSettings != null)
                Kernel.Rebind<IConstructorFailureTracer>().To<ConstructorFailureTracer>().WithConstructorArgument(settings.TracingSettings.ConstructorFailureTracerSettings);

            #endregion

            #region GeometryFailureTracer

            // Rebind Geometry Failure Tracer only if we're supposed be tracing
            if (settings.TracingSettings.GeometryFailureTracerSettings != null)
                Kernel.Rebind<IGeometryFailureTracer>().To<GeometryFailureTracer>().WithConstructorArgument(settings.TracingSettings.GeometryFailureTracerSettings);

            #endregion

            #endregion
        }

        #endregion
    }
}