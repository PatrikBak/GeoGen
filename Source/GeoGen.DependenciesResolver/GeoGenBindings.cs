using GeoGen.Algorithm;
using GeoGen.Constructor;
using GeoGen.Core;
using GeoGen.Generator;
using GeoGen.TheoremsAnalyzer;
using GeoGen.TheoremsFinder;
using GeoGen.Utilities;
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
        /// Bindings for the dependencies from the Generator module.
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        /// <returns>The kernel for chaining.</returns>
        public static IKernel AddGenerator(this IKernel kernel)
        {
            // Stateless services
            kernel.Bind<IGenerator>().To<Generator.Generator>().InSingletonScope();
            kernel.Bind<IArgumentsGenerator>().To<ArgumentsGenerator>().InSingletonScope();
            kernel.Bind<IGeneralArgumentsToStringConverter>().To<GeneralArgumentsToStringConverter>().InSingletonScope();
            kernel.Bind<IGeneralConfigurationToStringConverter>().To<GeneralConfigurationToStringConverter>().InSingletonScope();
            kernel.Bind<IFullObjectToStringConverter>().To<FullObjectToStringConverter>().InSingletonScope();

            // Factories
            kernel.Bind<IArgumentsContainerFactory>().ToFactory().InSingletonScope();
            kernel.Bind<IConfigurationObjectsContainerFactory>().ToFactory().InSingletonScope();
            kernel.Bind<IConfigurationsContainerFactory>().ToFactory().InSingletonScope();

            // Factory outputs
            kernel.Bind<IContainer<Arguments>>().To<ArgumentsContainer>();
            kernel.Bind<IContainer<ConfigurationObject>>().To<ConfigurationObjectsContainer>();
            kernel.Bind<IContainer<GeneratedConfiguration>>().To<ConfigurationsContainer>();

            // Converters used by factory outputs
            kernel.Bind<DefaultFullObjectToStringConverter>().ToSelf();
            kernel.Bind<FullConfigurationToStringConverter>().ToSelf();
            kernel.Bind<DefaultArgumentsToStringConverter>().ToSelf();

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
            // Stateless services
            kernel.Bind<IConstructorsResolver>().To<ConstructorsResolver>().InSingletonScope();
            kernel.Bind<IGeometryConstructor>().To<GeometryConstructor>().InSingletonScope();

            // Stateless predefined constructors
            kernel.Bind<IPredefinedConstructor>().To<CenterOfCircleConstructor>().InSingletonScope();
            kernel.Bind<IPredefinedConstructor>().To<CircleWithCenterThroughPointConstructor>().InSingletonScope();
            kernel.Bind<IPredefinedConstructor>().To<CircumcircleConstructor>().InSingletonScope();
            kernel.Bind<IPredefinedConstructor>().To<InternalAngleBisectorConstructor>().InSingletonScope();
            kernel.Bind<IPredefinedConstructor>().To<IntersectionOfLinesConstructor>().InSingletonScope();
            kernel.Bind<IPredefinedConstructor>().To<LineFromPointsConstructor>().InSingletonScope();
            kernel.Bind<IPredefinedConstructor>().To<MidpointConstructor>().InSingletonScope();
            kernel.Bind<IPredefinedConstructor>().To<PerpendicularLineConstructor>().InSingletonScope();
            kernel.Bind<IPredefinedConstructor>().To<ParallelLineConstructor>().InSingletonScope();
            kernel.Bind<IPredefinedConstructor>().To<PerpendicularProjectionConstructor>().InSingletonScope();
            kernel.Bind<IPredefinedConstructor>().To<PointReflectionConstructor>().InSingletonScope();
            kernel.Bind<IPredefinedConstructor>().To<SecondIntersectionOfCircleAndLineFromPointsConstructor>().InSingletonScope();
            kernel.Bind<IPredefinedConstructor>().To<SecondIntersectionOfCircleWithCenterAndLineFromPointsConstructor>().InSingletonScope();
            kernel.Bind<IPredefinedConstructor>().To<SecondIntersectionOfTwoCircumcirclesConstructor>().InSingletonScope();

            // Factories
            kernel.Bind<IComposedConstructorFactory>().ToFactory().InSingletonScope();
            kernel.Bind<IContextualPictureFactory>().ToFactory().InSingletonScope();
            kernel.Bind<IPicturesFactory>().ToFactory().InSingletonScope();

            // Factory outputs
            kernel.Bind<IComposedConstructor>().To<ComposedConstructor>();
            kernel.Bind<ContextualPicture>().ToSelf();
            kernel.Bind<Pictures>().ToSelf().WithConstructorArgument(settings);

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
            kernel.Bind<ITheoremsFinder>().To<CollinearPointsTheoremsFinder>().InSingletonScope();
            kernel.Bind<ITheoremsFinder>().To<ConcurrentObjectsTheoremsFinder>().InSingletonScope();
            kernel.Bind<ITheoremsFinder>().To<ConcyclicPointsTheoremsFinder>().InSingletonScope();
            kernel.Bind<ITheoremsFinder>().To<EqualAnglesTheoremsFinder>().InSingletonScope();
            kernel.Bind<ITheoremsFinder>().To<EqualLineSegmentsTheoremsFinder>().InSingletonScope();
            kernel.Bind<ITheoremsFinder>().To<LineTangentToCircleTheoremsFinder>().InSingletonScope();
            kernel.Bind<ITheoremsFinder>().To<ParallelLinesTheoremsFinder>().InSingletonScope();
            kernel.Bind<ITheoremsFinder>().To<PerpendicularLinesTheoremsFinder>().InSingletonScope();
            kernel.Bind<ITheoremsFinder>().To<TangentCirclesTheoremsFinder>().InSingletonScope();

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
            // Stateless services
            kernel.Bind<ITheoremsAnalyzer>().To<TheoremsAnalyzer.TheoremsAnalyzer>().InSingletonScope().WithConstructorArgument(analyzerData);
            kernel.Bind<ITrivialTheoremsProducer>().To<TrivialTheoremsProducer>().InSingletonScope();
            kernel.Bind<ITransitivityDeriver>().To<TransitivityDeriver>().InSingletonScope();
            kernel.Bind<ISubtheoremsDeriver>().To<SubtheoremsDeriver>().InSingletonScope();

            // Return the kernel for chaining
            return kernel;
        }

        /// <summary>
        /// Bindings for the dependencies from the Algorithm module.
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        /// <returns>The kernel for chaining.</returns>
        public static IKernel AddAlgorithm(this IKernel kernel)
        {
            // Stateless services
            kernel.Bind<IAlgorithm>().To<SequentialAlgorithm>().InSingletonScope();

            // Return the kernel for chaining
            return kernel;
        }
    }
}
