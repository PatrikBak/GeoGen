using GeoGen.AnalyticGeometry;
using GeoGen.Analyzer;
using GeoGen.Core;
using GeoGen.Generator;
using Ninject;
using Ninject.Extensions.Factory;
using Ninject.Extensions.NamedScope;

namespace GeoGen.Runner
{
    /// <summary>
    /// The extension methods for <see cref="IKernel"/>.
    /// </summary>
    public static class KernelExtensions
    {
        /// <summary>
        /// Binds <see cref="IGenerator"/> with all its needed dependencies having a given input.
        /// </summary>
        /// <param name="kernel">The Ninject kernel.</param>
        /// <param name="generatorInput">The input for the generator.</param>
        public static void AddAlgorithm(this IKernel kernel, GeneratorInput generatorInput)
        {
            // The name of the generator scope that represents a single IGenerator 
            const string GeneratorScopeName = "Generator";

            #region Generator

            // Singletons per one generation
            kernel.Bind<IConfigurationsValidator>().To<ConfigurationsValidator>().InNamedScope(GeneratorScopeName);
            kernel.Bind<IArgumentsGenerator>().To<ArgumentsGenerator>().InNamedScope(GeneratorScopeName);
            kernel.Bind<IGeneralArgumentsToStringConverter>().To<GeneralArgumentsToStringConverter>().InNamedScope(GeneratorScopeName);
            kernel.Bind<IGeneralConfigurationToStringConverter>().To<GeneralConfigurationToStringConverter>().InNamedScope(GeneratorScopeName);
            kernel.Bind<IFullObjectToStringConverter>().To<FullObjectToStringConverter>().InNamedScope(GeneratorScopeName);
            kernel.Bind<IContainer<ConfigurationObject>>().To<ConfigurationObjectsContainer>().InNamedScope(GeneratorScopeName);
            kernel.Bind<IContainer<GeneratedConfiguration>>().To<ConfigurationsContainer>().InNamedScope(GeneratorScopeName);
            kernel.Bind<DefaultFullObjectToStringConverter>().ToSelf().InNamedScope(GeneratorScopeName);
            kernel.Bind<DefaultArgumentsToStringConverter>().ToSelf().InNamedScope(GeneratorScopeName);
            
            // Transient objects
            kernel.Bind<IGeneratorFactory>().To<GeneratorFactory>();
            kernel.Bind<IContainer<Arguments>>().To<ArgumentsContainer>();

            // Tracers
            kernel.Bind<IInconstructibleObjectsTracer>().ToConstant((IInconstructibleObjectsTracer) null);
            kernel.Bind<IEqualObjectsTracer>().ToConstant((IEqualObjectsTracer) null);

            // Ninject factories
            kernel.Bind<IArgumentsContainerFactory>().ToFactory().InNamedScope(GeneratorScopeName);

            // Bind Generator that needs its dynamic input
            kernel.Bind<IGenerator>()
                .To<Generator.Generator>()
                .WithConstructorArgument("input", generatorInput)
                .DefinesNamedScope(GeneratorScopeName);

            // Bind the full converter that needs loose objects from the initial configuration
            kernel.Bind<FullConfigurationToStringConverter>()
                .ToSelf()
                .InNamedScope(GeneratorScopeName)
                .WithConstructorArgument("looseObjects", generatorInput.InitialConfiguration.LooseObjectsHolder.LooseObjects);

            #endregion

            #region Analyzer

            // Singletons per one generation
            kernel.Bind<ITheoremsAnalyzer>().To<TheoremsAnalyzer>().InNamedScope(GeneratorScopeName);
            kernel.Bind<INeedlessObjectsAnalyzer>().To<NeedlessObjectsAnalyzer>().InNamedScope(GeneratorScopeName);
            kernel.Bind<ILooseObjectsConstructor>().To<LooseObjectsConstructor>().InNamedScope(GeneratorScopeName);
            kernel.Bind<IConstructorsResolver>().To<ConstructorsResolver>().InNamedScope(GeneratorScopeName);            
            kernel.Bind<IGeometryRegistrar>().To<GeometryRegistrar>().InNamedScope(GeneratorScopeName);

            // Transient objects
            kernel.Bind<IComposedConstructor>().To<ComposedConstructor>();
            kernel.Bind<IObjectsContainer>().To<ObjectsContainer>();
            kernel.Bind<IContextualContainer>().To<ContextualContainer>();

            // Tracer
            kernel.Bind<IInconsistentContainersTracer>().ToConstant((IInconsistentContainersTracer) null);
            
            // An object with constructor arguments
            kernel.Bind<IObjectsContainersManager>()
                .To<ObjectsContainersManager>()
                .WithConstructorArgument("numberOfContainers", generatorInput.NumberOfContainers)
                .WithConstructorArgument("maximalAttemptsToReconstructOneContainer", generatorInput.MaximalAttemptsToReconstructOneContainer)
                .WithConstructorArgument("maximalAttemptsToReconstructAllContainers", generatorInput.MaximalAttemptsToReconstructAllContainers);

            // Ninject factories
            kernel.Bind<IComposedConstructorFactory>().ToFactory().InNamedScope(GeneratorScopeName);
            kernel.Bind<IObjectsContainersManagerFactory>().ToFactory().InNamedScope(GeneratorScopeName);
            kernel.Bind<IObjectsContainerFactory>().ToFactory().InNamedScope(GeneratorScopeName);
            kernel.Bind<IContextualContainerFactory>().ToFactory().InNamedScope(GeneratorScopeName);
            
            // Potential theorem analyzers
            kernel.Bind<IPotentialTheoremsAnalyzer>().To<CollinearPointsAnalyzer>().InNamedScope(GeneratorScopeName);
            kernel.Bind<IPotentialTheoremsAnalyzer>().To<ConcurrentObjectsAnalyzer>().InNamedScope(GeneratorScopeName);
            kernel.Bind<IPotentialTheoremsAnalyzer>().To<ConcyclicPointsAnalyzer>().InNamedScope(GeneratorScopeName);
            kernel.Bind<IPotentialTheoremsAnalyzer>().To<EqualAnglesAnalyzer>().InNamedScope(GeneratorScopeName);
            kernel.Bind<IPotentialTheoremsAnalyzer>().To<EqualLineSegmentsAnalyzer>().InNamedScope(GeneratorScopeName);
            kernel.Bind<IPotentialTheoremsAnalyzer>().To<LineTangentToCircleAnalyzer>().InNamedScope(GeneratorScopeName);
            kernel.Bind<IPotentialTheoremsAnalyzer>().To<ParallelLinesAnalyzer>().InNamedScope(GeneratorScopeName);
            kernel.Bind<IPotentialTheoremsAnalyzer>().To<PerpendicularLinesAnalyzer>().InNamedScope(GeneratorScopeName);
            kernel.Bind<IPotentialTheoremsAnalyzer>().To<TangentCirclesAnalyzer>().InNamedScope(GeneratorScopeName);

            // Predefined constructors
            kernel.Bind<IPredefinedConstructor>().To<CircumcenterFromPointsConstructor>().InNamedScope(GeneratorScopeName);
            kernel.Bind<IPredefinedConstructor>().To<CircumcircleFromPointsConstructor>().InNamedScope(GeneratorScopeName);
            kernel.Bind<IPredefinedConstructor>().To<IntersectionOfLinesConstructor>().InNamedScope(GeneratorScopeName);
            kernel.Bind<IPredefinedConstructor>().To<IntersectionOfLinesFromLineAndPointsConstructor>().InNamedScope(GeneratorScopeName);
            kernel.Bind<IPredefinedConstructor>().To<IntersectionOfLinesFromPointsConstructor>().InNamedScope(GeneratorScopeName);
            kernel.Bind<IPredefinedConstructor>().To<InternalAngleBisectorFromPointsConstructor>().InNamedScope(GeneratorScopeName);
            kernel.Bind<IPredefinedConstructor>().To<RandomPointOnLineFromPointsConstructor>().InNamedScope(GeneratorScopeName);
            kernel.Bind<IPredefinedConstructor>().To<MidpointFromPointsConstructor>().InNamedScope(GeneratorScopeName);
            kernel.Bind<IPredefinedConstructor>().To<PerpendicularLineFromPointsConstructor>().InNamedScope(GeneratorScopeName);

            #endregion

            #region Analytic geometry

            kernel.Bind<IRandomObjectsProvider>().To<RandomObjectsProvider>().InNamedScope(GeneratorScopeName);
            kernel.Bind<IRandomnessProvider>().To<RandomnessProvider>().InNamedScope(GeneratorScopeName);
            kernel.Bind<ITriangleConstructor>().To<TriangleConstructor>().InNamedScope(GeneratorScopeName);

            #endregion
        }
    }
}