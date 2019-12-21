using GeoGen.Algorithm;
using GeoGen.Constructor;
using GeoGen.Core;
using GeoGen.Generator;
using GeoGen.TheoremFinder;
using GeoGen.TheoremProver;
using GeoGen.TheoremRanker;
using GeoGen.TheoremSimplifier;
using GeoGen.Utilities;
using Ninject;
using Ninject.Extensions.Factory;

namespace GeoGen.Infrastructure
{
    /// <summary>
    /// The extension methods for <see cref="IKernel"/>.
    /// </summary>
    public static class KernelExtensions
    {
        /// <summary>
        /// Bindings for the logging system.
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        /// <param name="settings">The settings for the logging system.</param>
        /// <returns>The kernel for chaining.</returns>
        public static IKernel AddLogging(this IKernel kernel, LoggingSettings settings)
        {
            // Bind logging manager
            kernel.Bind<ILoggingManager>().To<CustomLoggingManager>();

            // Bind loggers according to the settings
            settings.Loggers.ForEach(loggersettings =>
            {
                // Switch based on the type of the settings
                switch (loggersettings)
                {
                    // If this is console settings...
                    case ConsoleLoggerSettings consoleLoggersettings:

                        // Bind it
                        kernel.Bind<ILogger>().ToConstant(new ConsoleLogger(consoleLoggersettings));

                        break;

                    // If this is file logger settings...
                    case FileLoggerSettings fileLoggersettings:

                        // Bind it
                        kernel.Bind<ILogger>().ToConstant(new FileLogger(fileLoggersettings));

                        break;

                    default:

                        // Otherwise we forgot something
                        throw new SettingsException($"Unhandled type of the settings ('{loggersettings.GetType()}') in the NInject bindings.");
                }
            });

            // Setup static log service
            Log.LoggingManager = kernel.Get<ILoggingManager>();

            // Return the kernel for chaining
            return kernel;
        }

        /// <summary>
        /// Bindings for the dependencies from the Generator module.
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        /// <param name="filterType">The type of configuration filter to be used.</param>
        /// <returns>The kernel for chaining.</returns>
        public static IKernel AddGenerator(this IKernel kernel, ConfigurationFilterType filterType)
        {
            // Stateless service
            kernel.Bind<IGenerator>().To<Generator.Generator>().InSingletonScope();

            // Factory
            kernel.Bind<IConfigurationFilterFactory>().ToFactory();

            // Bind the filter according to the type
            switch (filterType)
            {
                case ConfigurationFilterType.MemoryEfficient:
                    kernel.Bind<IConfigurationFilter>().To<MemoryEfficientConfigurationFilter>().InSingletonScope();
                    break;

                case ConfigurationFilterType.Fast:
                    kernel.Bind<IConfigurationFilter>().To<FastConfigurationFilter>().InSingletonScope();
                    break;

                default:
                    throw new GeoGenException($"Unhandled value of {nameof(ConfigurationFilterType)}: {filterType}");
            }

            // Return the kernel for chaining
            return kernel;
        }

        /// <summary>
        /// Bindings for the dependencies from the Constructor module.
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        /// <returns>The kernel for chaining.</returns>
        public static IKernel AddConstructor(this IKernel kernel)
        {
            // Stateless services
            kernel.Bind<IGeometryConstructor>().To<GeometryConstructor>().InSingletonScope();
            kernel.Bind<IConstructorsResolver>().To<ConstructorsResolver>().InSingletonScope();

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
            kernel.Bind<IPredefinedConstructor>().To<SecondIntersectionOfTwoCircumcirclesConstructor>().InSingletonScope();

            // Factories
            kernel.Bind<IComposedConstructorFactory>().ToFactory().InSingletonScope();

            // Factory outputs
            kernel.Bind<IComposedConstructor>().To<ComposedConstructor>();

            // Return the kernel for chaining
            return kernel;
        }

        /// <summary>
        /// Bindings for the dependencies from the Theorem Finder module.
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        /// <param name="tangentCirclesFinderSettings">The settings for the tangent circles theorem finder.</param>
        /// <param name="lineTangentToCirclesFinderSettings">The settings for the line tangent to circles theorem finder.</param>
        /// <param name="types">The types of theorems that we should be looking for.</param>
        /// <returns>The kernel for chaining.</returns>
        public static IKernel AddTheoremFinder(this IKernel kernel,
                                               TangentCirclesTheoremFinderSettings tangentCirclesFinderSettings,
                                               LineTangentToCircleTheoremFinderSettings lineTangentToCirclesFinderSettings,
                                               IReadOnlyHashSet<TheoremType> types)
        {
            // Bind theorem finder
            kernel.Bind<ITheoremFinder>().To<TheoremFinder.TheoremFinder>().InSingletonScope();

            // Go through all requested types
            types.ForEach(type =>
            {
                // Switch based on type and do the binding
                switch (type)
                {
                    case TheoremType.CollinearPoints:
                        kernel.Bind<ITypedTheoremFinder>().To<CollinearPointsTheoremFinder>().InSingletonScope();
                        break;

                    case TheoremType.ConcyclicPoints:
                        kernel.Bind<ITypedTheoremFinder>().To<ConcyclicPointsTheoremFinder>().InSingletonScope();
                        break;

                    case TheoremType.ConcurrentObjects:
                        kernel.Bind<ITypedTheoremFinder>().To<ConcurrentObjectsTheoremFinder>().InSingletonScope();
                        break;

                    case TheoremType.ConcurrentLines:
                        kernel.Bind<ITypedTheoremFinder>().To<ConcurrentLinesTheoremFinder>().InSingletonScope();
                        break;

                    case TheoremType.ParallelLines:
                        kernel.Bind<ITypedTheoremFinder>().To<ParallelLinesTheoremFinder>().InSingletonScope();
                        break;

                    case TheoremType.PerpendicularLines:
                        kernel.Bind<ITypedTheoremFinder>().To<PerpendicularLinesTheoremFinder>().InSingletonScope();
                        break;

                    case TheoremType.TangentCircles:
                        kernel.Bind<ITypedTheoremFinder>().To<TangentCirclesTheoremFinder>().InSingletonScope().WithConstructorArgument(tangentCirclesFinderSettings);
                        break;

                    case TheoremType.LineTangentToCircle:
                        kernel.Bind<ITypedTheoremFinder>().To<LineTangentToCircleTheoremFinder>().InSingletonScope().WithConstructorArgument(lineTangentToCirclesFinderSettings);
                        break;

                    case TheoremType.EqualLineSegments:
                        kernel.Bind<ITypedTheoremFinder>().To<EqualLineSegmentsTheoremFinder>().InSingletonScope();
                        break;

                    case TheoremType.EqualAngles:
                        kernel.Bind<ITypedTheoremFinder>().To<EqualAnglesTheoremFinder>().InSingletonScope();
                        break;

                    case TheoremType.Incidence:
                        kernel.Bind<ITypedTheoremFinder>().To<IncidenceTheoremFinder>().InSingletonScope();
                        break;

                    default:
                        throw new GeoGenException($"Unhandled value of {nameof(TheoremType)}: {type}");
                }
            });

            // Return the kernel for chaining
            return kernel;
        }

        /// <summary>
        /// Bindings for the dependencies from the TheoremProver module.
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        /// <param name="proverData">The data for the theorem prover.</param>
        /// <returns>The kernel for chaining.</returns>
        public static IKernel AddTheoremProver(this IKernel kernel, TheoremProverData proverData)
        {
            // Stateless services
            kernel.Bind<ITheoremProver>().To<TheoremProver.TheoremProver>().InSingletonScope().WithConstructorArgument(proverData);
            kernel.Bind<ISubtheoremDeriver>().To<SubtheoremDeriver>().InSingletonScope();
            kernel.Bind<ITrivialTheoremProducer>().To<TrivialTheoremProducer>().InSingletonScope();

            // Derivers
            kernel.Bind<ITheoremDeriver>().To<RectangleDeriver>().InSingletonScope();
            kernel.Bind<ITheoremDeriver>().To<IncidencesAndCollinearityDeriver>().InSingletonScope();
            kernel.Bind<ITheoremDeriver>().To<IncidencesAndConcyclityDeriver>().InSingletonScope();
            kernel.Bind<ITheoremDeriver>().To<ThalesTheoremDeriver>().InSingletonScope();
            kernel.Bind<ITheoremDeriver>().To<TransitivityDeriver>().InSingletonScope();
            kernel.Bind<ITheoremDeriver>().To<ParallelogramDeriver>().InSingletonScope();
            kernel.Bind<ITheoremDeriver>().To<RadicalAxisDeriver>().InSingletonScope();
            kernel.Bind<ITheoremDeriver>().To<CollinearityWithLinesFromPointsDeriver>().InSingletonScope();
            kernel.Bind<ITheoremDeriver>().To<PerpendicularLineToParallelLinesDeriver>().InSingletonScope();
            kernel.Bind<ITheoremDeriver>().To<ExplicitLineWithIncidencesDeriver>().InSingletonScope();
            kernel.Bind<ITheoremDeriver>().To<ConcyclicPointsWithExplicitCenterDeriver>().InSingletonScope();
            kernel.Bind<ITheoremDeriver>().To<ConcyclityWithCirclesFromPointsDeriver>().InSingletonScope();
            kernel.Bind<ITheoremDeriver>().To<ExplicitCircleWithIncidencesDeriver>().InSingletonScope();
            kernel.Bind<ITheoremDeriver>().To<IsoscelesTrianglesPerpendicularityDeriver>().InSingletonScope();

            // Return the kernel for chaining
            return kernel;
        }

        /// <summary>
        /// Bindings for the dependencies from the Algorithm module.
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        /// <param name="settings">The settings for the algorithm facade.</param>
        /// <returns>The kernel for chaining.</returns>
        public static IKernel AddAlgorithm(this IKernel kernel, AlgorithmFacadeSettings settings)
        {
            // Stateless services
            kernel.Bind<IAlgorithmFacade>().To<AlgorithmFacade>().InSingletonScope().WithConstructorArgument(settings);
            kernel.Bind<IBestTheoremsFinder>().To<BestTheoremsFinder>().InSingletonScope();

            // Return the kernel for chaining
            return kernel;
        }

        /// <summary>
        /// Bindings for the dependencies from the TheoremRanker module.
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        /// <param name="rankerSettings">The settings for <see cref="TheoremRanker.TheoremRanker"/></param>
        /// <param name="typeRankerSettings">The settings for <see cref="TypeRanker"/>.</param>
        /// <returns>The kernel for chaining.</returns>
        public static IKernel AddTheoremRanker(this IKernel kernel, TheoremRankerSettings rankerSettings, TypeRankerSettings typeRankerSettings)
        {
            // Bind ranker
            kernel.Bind<ITheoremRanker>().To<TheoremRanker.TheoremRanker>().WithConstructorArgument(rankerSettings);

            // Bind requested rankers
            foreach (var rankedAspect in rankerSettings.RankingCoefficients.Keys)
            {
                // Switch on the aspect
                switch (rankedAspect)
                {
                    case RankedAspect.Symmetry:
                        kernel.Bind<IAspectTheoremRanker>().To<SymmetryRanker>().InSingletonScope();
                        break;

                    case RankedAspect.Type:
                        kernel.Bind<IAspectTheoremRanker>().To<TypeRanker>().InSingletonScope().WithConstructorArgument(typeRankerSettings);
                        break;

                    case RankedAspect.TheoremsPerObject:
                        kernel.Bind<IAspectTheoremRanker>().To<TheoremsPerObjectRanker>().InSingletonScope();
                        break;

                    case RankedAspect.CirclesPerObject:
                        kernel.Bind<IAspectTheoremRanker>().To<CirclesPerObjectRanker>().InSingletonScope();
                        break;

                    case RankedAspect.NumberOfProofAttempts:
                        kernel.Bind<IAspectTheoremRanker>().To<NumberOfProofAttemptsRanker>().InSingletonScope();
                        break;

                    default:
                        throw new GeoGenException($"Unhandled value of {nameof(RankedAspect)}: {rankedAspect}");
                }
            }

            // Return the kernel for chaining
            return kernel;
        }

        /// <summary>
        /// Bindings for the dependencies from the TheoremSimplifier module.
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        /// <param name="data">The data for the simplifier.</param>
        /// <returns>The kernel for chaining.</returns>
        public static IKernel AddTheoremSimplifier(this IKernel kernel, TheoremSimplifierData data)
        {
            // Bind the simplifier
            kernel.Bind<ITheoremSimplifier>().To<TheoremSimplifier.TheoremSimplifier>().InSingletonScope().WithConstructorArgument(data);

            // Return the kernel for chaining
            return kernel;
        }
    }
}
