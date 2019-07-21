using GeoGen.Constructor;
using GeoGen.Core;
using GeoGen.Generator;
using GeoGen.TheoremsFinder;
using Ninject;
using Ninject.Extensions.Factory;

namespace GeoGen.DependenciesResolver
{
    /// <summary>
    /// The class containing bindings for all GeoGen modules.
    /// </summary>
    public static class GeoGenBindings
    {
        /// <summary>
        /// Bindings the dependencies from the Generator module.
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        /// <returns>The kernel for chaining.</returns>
        public static IKernel AddGenerator(this IKernel kernel)
        {
            kernel.Bind<IGenerator>().To<Generator.Generator>();
            kernel.Bind<IArgumentsGenerator>().To<ArgumentsGenerator>();
            kernel.Bind<IGeneralArgumentsToStringConverter>().To<GeneralArgumentsToStringConverter>();
            kernel.Bind<IGeneralConfigurationToStringConverter>().To<GeneralConfigurationToStringConverter>();
            kernel.Bind<IFullObjectToStringConverter>().To<FullObjectToStringConverter>();
            kernel.Bind<DefaultFullObjectToStringConverter>().ToSelf();
            kernel.Bind<FullConfigurationToStringConverter>().ToSelf();
            kernel.Bind<DefaultArgumentsToStringConverter>().ToSelf();
            kernel.Bind<IContainer<Arguments>>().To<ArgumentsContainer>();
            kernel.Bind<IContainer<ConfigurationObject>>().To<ConfigurationObjectsContainer>();
            kernel.Bind<IContainer<GeneratedConfiguration>>().To<ConfigurationsContainer>();

            // Factories
            kernel.Bind<IArgumentsContainerFactory>().ToFactory();
            kernel.Bind<IConfigurationObjectsContainerFactory>().ToFactory();
            kernel.Bind<IConfigurationsContainerFactory>().ToFactory();

            // Tracers
            kernel.Bind<IInconstructibleObjectsTracer>().ToConstant((IInconstructibleObjectsTracer) null);
            kernel.Bind<IEqualObjectsTracer>().ToConstant((IEqualObjectsTracer) null);

            // Return the kernel for chaining
            return kernel;
        }

        /// <summary>
        /// Bindings the dependencies from the Constructor module.
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        /// <returns>The kernel for chaining.</returns>
        public static IKernel AddConstructor(this IKernel kernel)
        {
            kernel.Bind<IGeometryConstructor>().To<GeometryConstructor>();
            kernel.Bind<ILooseObjectsConstructor>().To<LooseObjectsConstructor>();
            kernel.Bind<IConstructorsResolver>().To<ConstructorsResolver>();
            kernel.Bind<IComposedConstructor>().To<ComposedConstructor>();
            kernel.Bind<IPicture>().To<Picture>();
            kernel.Bind<IContextualPicture>().To<ContextualPicture>();

            // Bindings with dynamic settings
            kernel.BindsWithDynamicSettings<IPicturesManager, PicturesManager, PicturesManagerSettings>();

            // Predefined constructors
            kernel.Bind<IPredefinedConstructor>().To<CenterOfCircleConstructor>();
            kernel.Bind<IPredefinedConstructor>().To<CircleWithCenterThroughPointConstructor>();
            kernel.Bind<IPredefinedConstructor>().To<CircumcircleConstructor>();
            kernel.Bind<IPredefinedConstructor>().To<InternalAngleBisectorConstructor>();
            kernel.Bind<IPredefinedConstructor>().To<IntersectionOfLinesConstructor>();
            kernel.Bind<IPredefinedConstructor>().To<LineFromPointsConstructor>();
            kernel.Bind<IPredefinedConstructor>().To<MidpointConstructor>();
            kernel.Bind<IPredefinedConstructor>().To<PerpendicularLineConstructor>();
            kernel.Bind<IPredefinedConstructor>().To<ParallelLineConstructor>();
            kernel.Bind<IPredefinedConstructor>().To<PerpendicularProjectionConstructor>();
            kernel.Bind<IPredefinedConstructor>().To<PointReflectionConstructor>();
            kernel.Bind<IPredefinedConstructor>().To<SecondIntersectionOfCircleAndLineFromPointsConstructor>();
            kernel.Bind<IPredefinedConstructor>().To<SecondIntersectionOfCircleWithCenterAndLineFromPointsConstructor>();
            kernel.Bind<IPredefinedConstructor>().To<SecondIntersectionOfTwoCircumcirclesConstructor>();

            // Factories
            kernel.Bind<IComposedConstructorFactory>().ToFactory();
            kernel.Bind<IPicturesManagerFactory>().ToFactory();
            kernel.Bind<IPictureFactory>().ToFactory();
            kernel.Bind<IContextualPictureFactory>().ToFactory();

            // Tracers
            kernel.Bind<IGeometryConstructionFailureTracer>().ToConstant((IGeometryConstructionFailureTracer) null);
            kernel.Bind<IContexualPictureConstructionFailureTracer>().ToConstant((IContexualPictureConstructionFailureTracer) null);

            // Return the kernel for chaining
            return kernel;
        }

        /// <summary>
        /// Bindings the dependencies from the Theorems Finder module.
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        /// <returns>The kernel for chaining.</returns>
        public static IKernel AddTheoremsFinder(this IKernel kernel)
        {
            kernel.Bind<IRelevantTheoremsAnalyzer>().To<RelevantTheoremsAnalyzer>();

            // Potential theorem analyzers
            kernel.Bind<IPotentialTheoremsAnalyzer>().To<CollinearPointsAnalyzer>();
            kernel.Bind<IPotentialTheoremsAnalyzer>().To<ConcurrentObjectsAnalyzer>();
            kernel.Bind<IPotentialTheoremsAnalyzer>().To<ConcyclicPointsAnalyzer>();
            kernel.Bind<IPotentialTheoremsAnalyzer>().To<EqualAnglesAnalyzer>();
            kernel.Bind<IPotentialTheoremsAnalyzer>().To<EqualLineSegmentsAnalyzer>();
            kernel.Bind<IPotentialTheoremsAnalyzer>().To<LineTangentToCircleAnalyzer>();
            kernel.Bind<IPotentialTheoremsAnalyzer>().To<ParallelLinesAnalyzer>();
            kernel.Bind<IPotentialTheoremsAnalyzer>().To<PerpendicularLinesAnalyzer>();
            kernel.Bind<IPotentialTheoremsAnalyzer>().To<TangentCirclesAnalyzer>();

            // Return the kernel for chaining
            return kernel;
        }
    }
}
