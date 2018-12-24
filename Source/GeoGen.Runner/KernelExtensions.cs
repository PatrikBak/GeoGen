using Ninject;
using GeoGen.Generator;
using GeoGen.Core;
using Ninject.Extensions.Factory;
using Ninject.Extensions.NamedScope;
using GeoGen.AnalyticGeometry;
using GeoGen.Analyzer;

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
            kernel.Bind<IArgumentsGenerator>().To<ArgumentsGenerator>().InNamedScope(GeneratorScopeName);
            kernel.Bind<IConstructionSignatureMatcher>().To<ConstructionSignatureMatcher>().InNamedScope(GeneratorScopeName);
            kernel.Bind<IObjectsGenerator>().To<ObjectsGenerator>().InNamedScope(GeneratorScopeName);
            kernel.Bind<IGeneralConfigurationToStringProvider>().To<GeneralConfigurationToStringProvider>().InNamedScope(GeneratorScopeName);
            kernel.Bind<IGeneralArgumentsToStringConverter>().To<GeneralArgumentsToStringConverter>().InNamedScope(GeneratorScopeName);
            kernel.Bind<IConfigurationsValidator>().To<ConfigurationsValidator>().InNamedScope(GeneratorScopeName);
            kernel.Bind<IContainer<GeneratedConfiguration>>().To<ConfigurationsContainer>().InNamedScope(GeneratorScopeName);
            kernel.Bind<DefaultArgumentsToStringConverter>().ToSelf().InNamedScope(GeneratorScopeName);
            kernel.Bind<IFullObjectToStringConverter, FullObjectToStringConverter>().To<FullObjectToStringConverter>().InNamedScope(GeneratorScopeName);

            // Transient objects
            kernel.Bind<IGeneratorFactory>().To<GeneratorFactory>();
            kernel.Bind<IContainer<Arguments>>().To<ArgumentsContainer>();

            // Ninject factories
            kernel.Bind<IArgumentsContainerFactory>().ToFactory().InNamedScope(GeneratorScopeName);

            // Bindings with dynamic constructors arguments
            kernel.Bind<IGenerator>()
                .To<Generator.Generator>()
                .WithConstructorArgument("input", context => context.Kernel.Get<GeneratorInput>())
                .DefinesNamedScope(GeneratorScopeName);

            kernel.Bind<FullConfigurationToStringConverter>()
                .ToSelf()
                .InNamedScope(GeneratorScopeName)
                .WithConstructorArgument("looseObjectsHolder", context => context.Kernel.Get<GeneratorInput>().InitialConfiguration.LooseObjectsHolder);

            kernel.Bind<IConstructionsContainer>()
                .To<ConstructionsContainer>()
                .InNamedScope(GeneratorScopeName)
                .WithConstructorArgument("constructions", context => context.Kernel.Get<GeneratorInput>().Constructions);                

            kernel.Bind<IContainer<ConfigurationObject>>()
                .To<ConfigurationObjectsContainer>()
                .InNamedScope(GeneratorScopeName)
                .WithConstructorArgument("initialConfiguration", context => context.Kernel.Get<GeneratorInput>().InitialConfiguration);

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

            // Predefines constructors
            kernel.Bind<IPredefinedConstructor>().To<CircumcenterFromPointsConstructor>().InNamedScope(GeneratorScopeName);
            kernel.Bind<IPredefinedConstructor>().To<CircumcircleFromPointsConstructor>().InNamedScope(GeneratorScopeName);
            kernel.Bind<IPredefinedConstructor>().To<IntersectionOfLinesConstructor>().InNamedScope(GeneratorScopeName);
            kernel.Bind<IPredefinedConstructor>().To<IntersectionOfLinesFromLineAndPointsConstructor>().InNamedScope(GeneratorScopeName);
            kernel.Bind<IPredefinedConstructor>().To<IntersectionOfLinesFromPointsConstructor>().InNamedScope(GeneratorScopeName);
            kernel.Bind<IPredefinedConstructor>().To<InternalAngleBisectorFromPointsConstructor>().InNamedScope(GeneratorScopeName);
            kernel.Bind<IPredefinedConstructor>().To<LoosePointOnLineFromPointsConstructor>().InNamedScope(GeneratorScopeName);
            kernel.Bind<IPredefinedConstructor>().To<MidpointFromPointsConstructor>().InNamedScope(GeneratorScopeName);
            kernel.Bind<IPredefinedConstructor>().To<PerpendicularLineFromPointsConstructor>().InNamedScope(GeneratorScopeName);

            #endregion

            #region Analytic geometry

            kernel.Bind<IRandomObjectsProvider>().To<RandomObjectsProvider>().InNamedScope(GeneratorScopeName);
            kernel.Bind<IRandomnessProvider>().To<RandomnessProvider>().InNamedScope(GeneratorScopeName);
            kernel.Bind<ITriangleConstructor>().To<TriangleConstructor>().InNamedScope(GeneratorScopeName);

            #endregion

            #region Core

            kernel.Bind<ICombinator>().To<Combinator>().InNamedScope(GeneratorScopeName);
            kernel.Bind<IVariationsProvider>().To<VariationsProvider>().InNamedScope(GeneratorScopeName);
            kernel.Bind<ISubsetsProvider>().To<SubsetsProvider>().InNamedScope(GeneratorScopeName);

            #endregion

            // Return the kernel for chaining
            return kernel;
        }
    }
}