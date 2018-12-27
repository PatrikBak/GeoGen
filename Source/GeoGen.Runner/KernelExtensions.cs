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
        /// Binds <see cref="IGeneratorFactory"/> with all its needed dependencies.
        /// </summary>
        /// <param name="kernel">The Ninject kernel.</param>
        /// <returns>The kernel for chaining.</returns>
        public static IKernel AddAlgorithm(this IKernel kernel)
        {
            // The name of the generator scope that represents a single IGenerator 
            const string GeneratorScopeName = "Generator";

            #region Generator

            // Singletons per one generation
            kernel.Bind<IConfigurationsValidator>().To<ConfigurationsValidator>().InNamedScope(GeneratorScopeName);
            kernel.Bind<IArgumentsGenerator>().To<ArgumentsGenerator>().InNamedScope(GeneratorScopeName);
            kernel.Bind<IGeneralArgumentsToStringConverter>().To<GeneralArgumentsToStringConverter>().InNamedScope(GeneratorScopeName);
            kernel.Bind<IGeneralConfigurationToStringConverter>().To<GeneralConfigurationToStringConverter>().InNamedScope(GeneratorScopeName);
            kernel.Bind<DefaultArgumentsToStringConverter>().ToSelf().InNamedScope(GeneratorScopeName);
            kernel.Bind<IFullObjectToStringConverter>().To<FullObjectToStringConverter>().InNamedScope(GeneratorScopeName);
            kernel.Bind<IContainer<ConfigurationObject>>().To<ConfigurationObjectsContainer>().InNamedScope(GeneratorScopeName);
            kernel.Bind<IContainer<GeneratedConfiguration>>().To<ConfigurationsContainer>().InNamedScope(GeneratorScopeName);
            kernel.Bind<DefaultFullObjectToStringConverter>().ToSelf().InNamedScope(GeneratorScopeName);

            // Transient objects
            kernel.Bind<IGeneratorFactory>().To<GeneratorFactory>();
            kernel.Bind<IContainer<Arguments>>().To<ArgumentsContainer>();

            // Ninject factories
            kernel.Bind<IArgumentsContainerFactory>().ToFactory().InNamedScope(GeneratorScopeName);

            // Bind Generator that needs its dynamic input
            kernel.Bind<Generator.Generator>()
                .ToSelf()
                .WithConstructorArgument("input", context => context.Kernel.Get<GeneratorInput>())
                .DefinesNamedScope(GeneratorScopeName);

            // Bind the full converter that needs loose objects from the initial configuration
            kernel.Bind<FullConfigurationToStringConverter>()
                .ToSelf()
                .InNamedScope(GeneratorScopeName)
                .WithConstructorArgument("looseObjects", context => context.Kernel.Get<GeneratorInput>().InitialConfiguration.LooseObjectsHolder.LooseObjects);

            #endregion

            #region Analyzer

            // Singletons per one generation
            kernel.Bind<ITheoremsAnalyzer>().To<TheoremsAnalyzer>().InNamedScope(GeneratorScopeName);
            kernel.Bind<IPotentialTheoremValidator>().To<PotentialTheoremValidator>().InNamedScope(GeneratorScopeName);
            kernel.Bind<INeedlessObjectsAnalyzer>().To<NeedlessObjectsAnalyzer>().InNamedScope(GeneratorScopeName);
            kernel.Bind<ILooseObjectsConstructor>().To<LooseObjectsConstructor>().InNamedScope(GeneratorScopeName);
            kernel.Bind<IConstructorsResolver>().To<ConstructorsResolver>().InNamedScope(GeneratorScopeName);            
            kernel.Bind<IContextualContainerFactory>().To<ContextualContainerFactory>().InNamedScope(GeneratorScopeName);
            kernel.Bind<IGeometryRegistrar>().To<GeometryRegistrar>().InNamedScope(GeneratorScopeName);
            kernel.Bind<IObjectContainersMapper>().To<ObjectContainersMapper>().InNamedScope(GeneratorScopeName);
            kernel.Bind<ITheoremsContainer>().To<TheoremsContainer>().InNamedScope(GeneratorScopeName);

            // Transient objects
            kernel.Bind<IComposedConstructor>().To<ComposedConstructor>();
            kernel.Bind<IObjectsContainersManager>().To<ObjectsContainersManager>();
            kernel.Bind<IObjectsContainer>().To<ObjectsContainer>();
            
            // Ninject factories
            kernel.Bind<IComposedConstructorFactory>().ToFactory().InNamedScope(GeneratorScopeName);
            kernel.Bind<IObjectsContainersManagerFactory>().ToFactory().InNamedScope(GeneratorScopeName);
            kernel.Bind<IObjectsContainerFactory>().ToFactory().InNamedScope(GeneratorScopeName);

            // Theorem verifiers
            kernel.Bind<ITheoremVerifier>().To<CollinearPointsVerifier>().InNamedScope(GeneratorScopeName);
            kernel.Bind<ITheoremVerifier>().To<ConcurrentObjectsVerifier>().InNamedScope(GeneratorScopeName);
            kernel.Bind<ITheoremVerifier>().To<ConcyclicPointsVerifier>().InNamedScope(GeneratorScopeName);
            //kernel.Bind<ITheoremVerifier>().To<EqualAnglesVerifier>().InNamedScope(GeneratorScopeName);
            kernel.Bind<ITheoremVerifier>().To<EqualLineSegmentsVerifier>().InNamedScope(GeneratorScopeName);
            kernel.Bind<ITheoremVerifier>().To<LineTangentToCircleVerifier>().InNamedScope(GeneratorScopeName);
            kernel.Bind<ITheoremVerifier>().To<ParallelLinesVerifier>().InNamedScope(GeneratorScopeName);
            kernel.Bind<ITheoremVerifier>().To<PerpendicularLinesVerifier>().InNamedScope(GeneratorScopeName);
            kernel.Bind<ITheoremVerifier>().To<TangentCirclesVerifier>().InNamedScope(GeneratorScopeName);

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

            // Return the kernel for chaining
            return kernel;
        }
    }
}