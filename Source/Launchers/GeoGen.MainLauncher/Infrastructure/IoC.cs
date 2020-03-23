using GeoGen.ConfigurationGenerator;
using GeoGen.Constructor;
using GeoGen.Infrastructure;
using GeoGen.ProblemAnalyzer;
using GeoGen.ProblemGenerator;
using GeoGen.TheoremFinder;
using GeoGen.TheoremProver;
using GeoGen.TheoremRanker;
using GeoGen.TheoremSimplifier;
using GeoGen.TheoremSorter;
using Ninject;
using Ninject.Extensions.Factory;
using System.Threading.Tasks;

namespace GeoGen.MainLauncher
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
            Kernel.Bind<IBatchRunner>().To<BatchRunner>();
            Kernel.Bind<IProblemGenerationRunner>().To<ProblemGenerationRunner>().WithConstructorArgument(settings.ProblemGenerationRunnerSettings);
            Kernel.Bind<IProblemGeneratorInputProvider>().To<ProblemGeneratorInputProvider>().WithConstructorArgument(settings.ProblemGeneratorInputProviderSettings);
            Kernel.Bind<IInferenceRuleProvider>().To<InferenceRuleProvider>().WithConstructorArgument(settings.InferenceRuleProviderSettings);
            Kernel.Bind<ISimplificationRuleProvider>().To<SimplificationRuleProvider>().WithConstructorArgument(settings.SimplificationRuleProviderSettings);
            Kernel.Bind<IObjectIntroductionRuleProvider>().To<ObjectIntroductionRuleProvider>().WithConstructorArgument(settings.ObjectIntroductionRuleProviderSettings);
            Kernel.Bind<ITheoremSorterTypeResolver>().To<TheoremSorterTypeResolver>().WithConstructorArgument(settings.TheoremSorterTypeResolverSettings);
            Kernel.Bind<IRankedTheoremJsonLazyWriter>().To<RankedTheoremJsonLazyWriter>();
            Kernel.Bind<IRankedTheoremJsonLazyWriterFactory>().ToFactory();

            #endregion

            // Load the inference rules
            var managerData = new InferenceRuleManagerData(await Kernel.Get<IInferenceRuleProvider>().GetInferenceRulesAsync());

            // Use them to bind the tracker
            Kernel.Bind<IInferenceRuleUsageTracker>().To<InferenceRuleUsageTracker>().WithConstructorArgument(managerData);

            #region Algorithm

            // Add the configuration generator with its settings
            Kernel.AddConfigurationGenerator(settings.GenerationSettings)
                // And the constructor
                .AddConstructor()
                // And the theorem finder
                .AddTheoremFinder(settings.TheoremFindingSettings)
                // And the theorem ranker
                .AddTheoremRanker(settings.TheoremRankingSettings)
                // And the theorem prover and with its settings
                .AddTheoremProver(new TheoremProvingSettings
                (
                    // Set the loaded manager data
                    inferenceRuleManagerData: managerData,

                    // Load the object introduction data as well
                    objectIntroducerData: new ObjectIntroducerData(await Kernel.Get<IObjectIntroductionRuleProvider>().GetObjectIntroductionRulesAsync())
                ))
                // And the theorem simplifier with the loaded simplification rules
                .AddTheoremSimplifier(new TheoremSimplifierData(await Kernel.Get<ISimplificationRuleProvider>().GetSimplificationRulesAsync()))
                // And the problem generator and with its settings
                .AddProblemGenerator(settings.ProblemGeneratorSettings)
                // And the problem analyzer
                .AddProblemAnalyzer()
                // And the sorter
                .AddTheoremSorter();

            #endregion

            #region Tracers

            // Rebind Constructor Failure Tracer only if we're supposed be tracing
            if (settings.TraceConstructorFailures)
                Kernel.Rebind<IConstructorFailureTracer>().To<ConstructorFailureTracer>().WithConstructorArgument(settings.ConstructorFailureTracerSettings);

            // Rebind Geometry Failure Tracer only if we're supposed be tracing
            if (settings.TraceGeometryFailures)
                Kernel.Rebind<IGeometryFailureTracer>().To<GeometryFailureTracer>().WithConstructorArgument(settings.GeometryFailureTracerSettings);

            // Rebind Invalid Inference Tracer only if we're supposed be tracing
            if (settings.TraceInvalidInferences)
                Kernel.Rebind<IInvalidInferenceTracer>().To<InvalidInferenceTracer>().WithConstructorArgument(settings.InvalidInferenceTracerSettings);

            // Rebind Sorting Geometry Failure Tracer only if we're supposed be tracing
            if (settings.TraceSortingGeometryFailures)
                Kernel.Rebind<ISortingGeometryFailureTracer>().To<SortingGeometryFailureTracer>().WithConstructorArgument(settings.SortingGeometryFailureTracerSettings);

            #endregion
        }

        #endregion
    }
}