using GeoGen.Algorithm;
using GeoGen.Constructor;
using GeoGen.Infrastructure;
using GeoGen.TheoremProver;
using GeoGen.TheoremSimplifier;
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
            Kernel = Infrastructure.IoC.CreateKernel();

            // Add the logging system
            Kernel.AddLogging(settings.LoggingSettings);

            #region Local dependencies

            // Add local dependencies
            Kernel.Bind<IBatchRunner>().To<BatchRunner>().WithConstructorArgument(settings.InputFolderSettings);
            Kernel.Bind<IAlgorithmRunner>().To<AlgorithmRunner>().WithConstructorArgument(settings.AlgorithmRunnerSettings);
            Kernel.Bind<IBestTheoremTracker>().To<BestTheoremTracker>().WithConstructorArgument(settings.BestTheoremsTrackerSettings);
            Kernel.Bind<ITheoremDataWriter>().To<FileTheoremDataWriter>().WithConstructorArgument(settings.FileTheoremDataWriterSettings);
            Kernel.Bind<IAlgorithmInputProvider>().To<AlgorithmInputProvider>().WithConstructorArgument(settings.InputFolderSettings);
            Kernel.Bind<ITemplateTheoremProvider>().To<TemplateTheoremProvider>().WithConstructorArgument(settings.TemplateTheoremsFolderSettings);
            Kernel.Bind<ISimplificationRulesProvider>().To<SimplificationRulesProvider>().WithConstructorArgument(settings.SimplificationRulesProviderSettings);

            #endregion

            #region Tracers

            // Bind Subtheorem Deriver Tracer only if we're supposed be tracing
            if (settings.TracersSettings.SubtheoremDeriverGeometryFailureTracerSettings != null)
                Kernel.Bind<ISubtheoremDeriverGeometryFailureTracer>().To<SubtheoremDeriverGeometryFailureTracer>().WithConstructorArgument(settings.TracersSettings.SubtheoremDeriverGeometryFailureTracerSettings);

            // Bind Constructor Failure Tracer only if we're supposed be tracing
            if (settings.TracersSettings.ConstructorFailureTracerSettings != null)
                Kernel.Bind<IConstructorFailureTracer>().To<ConstructorFailureTracer>().WithConstructorArgument(settings.TracersSettings.ConstructorFailureTracerSettings);

            // Bind Geometry Failure Tracer only if we're supposed be tracing
            if (settings.TracersSettings.GeometryFailureTracerSettings != null)
                Kernel.Bind<IGeometryFailureTracer>().To<GeometryFailureTracer>().WithConstructorArgument(settings.TracersSettings.GeometryFailureTracerSettings);

            #endregion

            #region Algorithm

            // Add generator
            Kernel.AddGenerator(settings.AlgorithmSettings.GenerationSettings.ConfigurationFilterType)
                // With constructor
                .AddConstructor()
                // With theorem finder and its settings
                .AddTheoremFinder(settings.AlgorithmSettings.TheoremFindingSettings.TangentCirclesTheoremFinderSettings,
                                  settings.AlgorithmSettings.TheoremFindingSettings.LineTangentToCircleTheoremFinderSettings,
                                  settings.AlgorithmSettings.TheoremFindingSettings.SoughtTheoremTypes.ToReadOnlyHashSet())
                // With theorem prover and its data
                .AddTheoremProver(new TheoremProverData
                (
                    // Template theorems are loaded at the beginning
                    templateTheorems: await Kernel.Get<ITemplateTheoremProvider>().GetTemplateTheoremsAsync()
                ))
                // With theorem ranker and its settings
                .AddTheoremRanker(settings.AlgorithmSettings.TheoremRankingSettings.TheoremRankerSettings,
                                  settings.AlgorithmSettings.TheoremRankingSettings.TypeRankerSettings)
                // With theorem simplifier and its data
                .AddTheoremSimplifier(new TheoremSimplifierData
                (
                    // Simplification rules are loaded at the beginning
                    rules: (await Kernel.Get<ISimplificationRulesProvider>().GetSimplificationRulesAsync()).ToReadOnlyHashSet()
                ))
                // And finally the algorithm with its settings
                .AddAlgorithm(settings.AlgorithmSettings.AlgorithmFacadeSettings);

            #endregion
        }

        #endregion
    }
}