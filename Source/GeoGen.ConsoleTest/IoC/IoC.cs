using GeoGen.TheoremsFinder;
using GeoGen.Constructor;
using GeoGen.Core;
using GeoGen.Generator;
using Ninject;
using Ninject.Extensions.ContextPreservation;
using Ninject.Extensions.Factory;
using System.Linq;

namespace GeoGen.ConsoleTest
{
    /// <summary>
    /// The IoC container.
    /// </summary>
    public static class IoC
    {
        #region Kernel

        /// <summary>
        /// The NInject kernel.
        /// </summary>
        public static IKernel Kernel { get; private set; }

        #endregion

        #region Get method

        /// <summary>
        /// Gets the given service from the kernel, with possible dynamic configuration objects.
        /// </summary>
        /// <typeparam name="T">The type of the retrieved service.</typeparam>
        /// <param name="dynamicConfiguration">The dynamic configuration.</param>
        /// <returns>The retrieved service.</returns>
        public static T Get<T>(params object[] dynamicConfiguration)
        {
            // Delegate the call to the kernel with the passed objects cast to dynamic parameters
            return Kernel.Get<T>(dynamicConfiguration.Select(obj => new DynamicSettingsParameter(obj)).ToArray());
        }

        #endregion

        #region Bootstrapping

        /// <summary>
        /// Initializes the Ninject IoC system.
        /// </summary>
        public static void Bootstrap()
        {
            // Create a kernel that will perform the resolution
            Kernel = new StandardKernel(new FuncModule(), new ContextPreservationModule());

            // Make sure we can bind to null 
            // This is used only for tracers, that are not compulsory
            // I like this better than the NullObject pattern, because
            // of '?' operator that can be used to prevent null-checks
            Kernel.Settings.AllowNullInjection = true;

            // Add bindings
            AddGenerator();
            AddConstructor();
            AddAnalyzer();
            AddLocalDependencies();
        }

        /// <summary>
        /// Bindings the dependencies from the Generator module.
        /// </summary>
        private static void AddGenerator()
        {
            Kernel.Bind<IGenerator>().To<Generator.Generator>();
            Kernel.Bind<IArgumentsGenerator>().To<ArgumentsGenerator>();
            Kernel.Bind<IGeneralArgumentsToStringConverter>().To<GeneralArgumentsToStringConverter>();
            Kernel.Bind<IGeneralConfigurationToStringConverter>().To<GeneralConfigurationToStringConverter>();
            Kernel.Bind<IFullObjectToStringConverter>().To<FullObjectToStringConverter>();
            Kernel.Bind<DefaultFullObjectToStringConverter>().ToSelf();
            Kernel.Bind<DefaultArgumentsToStringConverter>().ToSelf();
            Kernel.Bind<IContainer<Arguments>>().To<ArgumentsContainer>();
            Kernel.Bind<IContainer<ConfigurationObject>>().To<ConfigurationObjectsContainer>();
            Kernel.Bind<IContainer<GeneratedConfiguration>>().To<ConfigurationsContainer>();

            // Factories
            Kernel.Bind<IArgumentsContainerFactory>().ToFactory();
            Kernel.Bind<IConfigurationObjectsContainerFactory>().ToFactory();
            Kernel.Bind<IConfigurationsContainerFactory>().ToFactory();

            // Tracers
            Kernel.Bind<IInconstructibleObjectsTracer>().ToConstant((IInconstructibleObjectsTracer) null);
            Kernel.Bind<IEqualObjectsTracer>().ToConstant((IEqualObjectsTracer) null);
        }

        /// <summary>
        /// Bindings the dependencies from the Constructor module.
        /// </summary>
        private static void AddConstructor()
        {
            Kernel.Bind<IGeometryConstructor>().To<GeometryConstructor>();
            Kernel.Bind<ILooseObjectsConstructor>().To<LooseObjectsConstructor>();
            Kernel.Bind<IConstructorsResolver>().To<ConstructorsResolver>();
            Kernel.Bind<IComposedConstructor>().To<ComposedConstructor>();
            Kernel.Bind<IPicture>().To<Picture>();

            // Bindings with dynamic settings
            Kernel.BindsWithDynamicSettings<IContextualPicture, ContextualPicture, ContextualPictureSettings>();
            Kernel.BindsWithDynamicSettings<IPicturesManager, PicturesManager, PicturesManagerSettings>();

            // Predefined constructors
            Kernel.Bind<IPredefinedConstructor>().To<CenterOfCircleConstructor>();
            Kernel.Bind<IPredefinedConstructor>().To<CircleWithCenterThroughPointConstructor>();
            Kernel.Bind<IPredefinedConstructor>().To<CircumcircleConstructor>();
            Kernel.Bind<IPredefinedConstructor>().To<InternalAngleBisectorConstructor>();
            Kernel.Bind<IPredefinedConstructor>().To<IntersectionOfLinesConstructor>();
            Kernel.Bind<IPredefinedConstructor>().To<LineFromPointsConstructor>();
            Kernel.Bind<IPredefinedConstructor>().To<MidpointConstructor>();
            Kernel.Bind<IPredefinedConstructor>().To<PerpendicularLineConstructor>();
            Kernel.Bind<IPredefinedConstructor>().To<ParallelLineConstructor>();
            Kernel.Bind<IPredefinedConstructor>().To<PerpendicularProjectionConstructor>();
            Kernel.Bind<IPredefinedConstructor>().To<PointReflectionConstructor>();
            Kernel.Bind<IPredefinedConstructor>().To<SecondIntersectionOfCircleAndLineFromPointsConstructor>();
            Kernel.Bind<IPredefinedConstructor>().To<SecondIntersectionOfCircleWithCenterAndLineFromPointsConstructor>();
            Kernel.Bind<IPredefinedConstructor>().To<SecondIntersectionOfTwoCircumcirclesConstructor>();

            // Factories
            Kernel.Bind<IComposedConstructorFactory>().ToFactory();
            Kernel.Bind<IPicturesManagerFactory>().ToFactory();
            Kernel.Bind<IPictureFactory>().ToFactory();
            Kernel.Bind<IContextualPictureFactory>().ToFactory();

            // Tracers
            Kernel.Bind<IGeometryConstructionFailureTracer>().ToConstant((IGeometryConstructionFailureTracer) null);
            Kernel.Bind<IContexualPictureConstructionFailureTracer>().ToConstant((IContexualPictureConstructionFailureTracer) null);
        }

        /// <summary>
        /// Bindings the dependencies from the Analyzer module.
        /// </summary>
        private static void AddAnalyzer()
        {
            Kernel.BindsWithDynamicSettings<IRelevantTheoremsAnalyzer, RelevantTheoremsAnalyzer, TheoremAnalysisSettings>();

            // Potential theorem analyzers
            Kernel.Bind<IPotentialTheoremsAnalyzer>().To<CollinearPointsAnalyzer>();
            Kernel.Bind<IPotentialTheoremsAnalyzer>().To<ConcurrentObjectsAnalyzer>();
            Kernel.Bind<IPotentialTheoremsAnalyzer>().To<ConcyclicPointsAnalyzer>();
            Kernel.Bind<IPotentialTheoremsAnalyzer>().To<EqualAnglesAnalyzer>();
            Kernel.Bind<IPotentialTheoremsAnalyzer>().To<EqualLineSegmentsAnalyzer>();
            Kernel.Bind<IPotentialTheoremsAnalyzer>().To<LineTangentToCircleAnalyzer>();
            Kernel.Bind<IPotentialTheoremsAnalyzer>().To<ParallelLinesAnalyzer>();
            Kernel.Bind<IPotentialTheoremsAnalyzer>().To<PerpendicularLinesAnalyzer>();
            Kernel.Bind<IPotentialTheoremsAnalyzer>().To<TangentCirclesAnalyzer>();
        }

        /// <summary>
        /// Bindings the dependencies from this module.
        /// </summary>
        private static void AddLocalDependencies()
        {
            Kernel.Rebind<IEqualObjectsTracer, DefaultEqualObjectsTracer>().To<DefaultEqualObjectsTracer>().InSingletonScope();
            Kernel.Rebind<IInconstructibleObjectsTracer, DefaultInconstructibleObjectsTracer>().To<DefaultInconstructibleObjectsTracer>().InSingletonScope();
            Kernel.Rebind<IGeometryConstructionFailureTracer, DefaultGeometryConstructionFailureTracer>().To<DefaultGeometryConstructionFailureTracer>().InSingletonScope();
            Kernel.Rebind<IContexualPictureConstructionFailureTracer, DefaultContexualPictureConstructionFailureTracer>().To<DefaultContexualPictureConstructionFailureTracer>().InSingletonScope();
            Kernel.Bind<IAlgorithm>().To<SequentialAlgorithm>();
            Kernel.Bind<SimpleCompleteTheoremAnalyzer>().ToSelf();
        }

        #endregion
    }
}
