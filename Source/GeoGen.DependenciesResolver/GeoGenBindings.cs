using GeoGen.Algorithm;
using GeoGen.Constructor;
using GeoGen.Core;
using GeoGen.Generator;
using GeoGen.TheoremFinder;
using GeoGen.TheoremProver;
using GeoGen.TheoremRanker;
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
            // Stateless service
            kernel.Bind<IGenerator>().To<Generator.Generator>().InSingletonScope();

            // Return the kernel for chaining
            return kernel;
        }

        /// <summary>
        /// Bindings for the dependencies from the Constructor module.
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        /// <param name="settings">The settings for <see cref="GeometryConstructor"/>.</param>
        /// <returns>The kernel for chaining.</returns>
        public static IKernel AddConstructor(this IKernel kernel, GeometryConstructorSettings settings)
        {
            // Stateless services
            kernel.Bind<IGeometryConstructor>().To<GeometryConstructor>().InSingletonScope().WithConstructorArgument(settings);
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
                        kernel.Bind<ITypedTheoremFinder>().To<CollinearPointsTheoremFinder>();
                        break;

                    case TheoremType.ConcyclicPoints:
                        kernel.Bind<ITypedTheoremFinder>().To<ConcyclicPointsTheoremFinder>();
                        break;

                    case TheoremType.ConcurrentObjects:
                        kernel.Bind<ITypedTheoremFinder>().To<ConcurrentObjectsTheoremFinder>();
                        break;

                    case TheoremType.ConcurrentLines:
                        kernel.Bind<ITypedTheoremFinder>().To<ConcurrentLinesTheoremFinder>();
                        break;

                    case TheoremType.ParallelLines:
                        kernel.Bind<ITypedTheoremFinder>().To<ParallelLinesTheoremFinder>();
                        break;

                    case TheoremType.PerpendicularLines:
                        kernel.Bind<ITypedTheoremFinder>().To<PerpendicularLinesTheoremFinder>();
                        break;

                    case TheoremType.TangentCircles:
                        kernel.Bind<ITypedTheoremFinder>().To<TangentCirclesTheoremFinder>().WithConstructorArgument(tangentCirclesFinderSettings);
                        break;

                    case TheoremType.LineTangentToCircle:
                        kernel.Bind<ITypedTheoremFinder>().To<LineTangentToCircleTheoremFinder>().WithConstructorArgument(lineTangentToCirclesFinderSettings);
                        break;

                    case TheoremType.EqualLineSegments:
                        kernel.Bind<ITypedTheoremFinder>().To<EqualLineSegmentsTheoremFinder>();
                        break;

                    case TheoremType.EqualAngles:
                        kernel.Bind<ITypedTheoremFinder>().To<EqualAnglesTheoremFinder>();
                        break;

                    case TheoremType.Incidence:
                        kernel.Bind<ITypedTheoremFinder>().To<IncidenceTheoremFinder>();
                        break;

                    default:
                        throw new GeoGenException($"Unhandled type of theorem: {type}");
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
        /// <returns>The kernel for chaining.</returns>
        public static IKernel AddAlgorithm(this IKernel kernel)
        {
            // Stateless services
            kernel.Bind<IAlgorithmFacade>().To<AlgorithmFacade>().InSingletonScope();
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
                        kernel.Bind<IAspectTheoremRanker>().To<SymmetryRanker>();
                        break;

                    case RankedAspect.Type:
                        kernel.Bind<IAspectTheoremRanker>().To<TypeRanker>().WithConstructorArgument(typeRankerSettings);
                        break;

                    case RankedAspect.TheoremsPerObject:
                        kernel.Bind<IAspectTheoremRanker>().To<TheoremsPerObjectRanker>();
                        break;

                    case RankedAspect.CirclesPerObject:
                        kernel.Bind<IAspectTheoremRanker>().To<CirclesPerObjectRanker>();
                        break;

                    case RankedAspect.NumberOfProofAttempts:
                        kernel.Bind<IAspectTheoremRanker>().To<NumberOfProofAttemptsRanker>();
                        break;

                    default:
                        throw new GeoGenException($"Unhandled type of ranked aspect: {rankedAspect}");
                }
            }

            // Return the kernel for chaining
            return kernel;
        }
    }
}
