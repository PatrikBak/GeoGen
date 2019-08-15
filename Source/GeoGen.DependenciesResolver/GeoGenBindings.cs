using GeoGen.Constructor;
using GeoGen.Core;
using GeoGen.Generator;
using GeoGen.TheoremsAnalyzer;
using GeoGen.TheoremsFinder;
using GeoGen.TheoremsFinder.new_stuff;
using Ninject;
using Ninject.Extensions.Factory;
using System.Linq;

namespace GeoGen.DependenciesResolver
{
    /// <summary>
    /// The class containing bindings for all GeoGen modules.
    /// </summary>
    public static class GeoGenBindings
    {
        /// <summary>
        /// Bindings for the dependencies from the Generator module.
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

            // Return the kernel for chaining
            return kernel;
        }

        /// <summary>
        /// Bindings for the dependencies from the Constructor module.
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        /// <param name="settings">The settings for <see cref="Pictures"/>.</param>
        /// <returns>The kernel for chaining.</returns>
        public static IKernel AddConstructor(this IKernel kernel, PicturesSettings settings)
        {
            kernel.Bind<IConstructorsResolver>().To<ConstructorsResolver>();
            kernel.Bind<IComposedConstructor>().To<ComposedConstructor>();
            kernel.Bind<IGeometryConstructor>().To<GeometryConstructor>();
            kernel.Bind<ContextualPicture>().ToSelf();
            kernel.Bind<Pictures>().ToSelf().WithConstructorArgument(settings);

            // Factories
            kernel.Bind<IComposedConstructorFactory>().ToFactory();
            kernel.Bind<IContextualPictureFactory>().ToFactory();
            kernel.Bind<IPicturesFactory>().ToFactory();

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

            // Return the kernel for chaining
            return kernel;
        }

        /// <summary>
        /// Bindings for the dependencies from the Theorems Finder module.
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        /// <returns>The kernel for chaining.</returns>
        public static IKernel AddTheoremsFinder(this IKernel kernel)
        {
            kernel.Bind<ITheoremsFinder>().To<T>();

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

        /// <summary>
        /// Bindings for the dependencies from the Analyzer module.
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        /// <param name="analyzerData">The data for the theorems analyzer.</param>
        /// <returns>The kernel for chaining.</returns>
        public static IKernel AddTheoremsAnalyzer(this IKernel kernel, TheoremsAnalyzerData analyzerData)
        {
            kernel.Bind<ITheoremsAnalyzer>().To<TheoremsAnalyzer.TheoremsAnalyzer>().WithConstructorArgument(analyzerData);
            kernel.Bind<ITrivialTheoremsProducer>().To<TrivialTheoremsProducer>();
            kernel.Bind<ISubtheoremAnalyzer>().To<SubtheoremAnalyzer>();
            kernel.Bind<ITransitivityDeriver>().To<TransitivityDeriver>();

            // Return the kernel for chaining
            return kernel;
        }


    }

    public class T : ITheoremsFinder
    {
        public TheoremsMap FindAllTheorems(ContextualPicture picture)
        {
            return new TheoremsMap(Enumerable.Empty<Theorem>());
        }

        public TheoremsMap FindNewTheorems(HierarchicalContextualPicture picture)
        {
            return new TheoremsMap(Enumerable.Empty<Theorem>());
        }
    }
}
