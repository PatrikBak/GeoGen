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
            // Stateless service
            kernel.Bind<IGenerator>().To<Generator.Generator>().InSingletonScope();

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
        /// <param name="tangentCirclesFinderSettings">The settings for the tangent circles theorems finder.</param>
        /// <param name="lineTangentToCirclesFinderSettings">The settings for the line tangent to circles finder.</param>
        /// <param name="types">The types of theorems that we should be looking for.</param>
        /// <returns>The kernel for chaining.</returns>
        public static IKernel AddTheoremsFinder(this IKernel kernel,
                                                TangentCirclesTheoremsFinderSettings tangentCirclesFinderSettings,
                                                LineTangentToCircleTheoremsFinderSettings lineTangentToCirclesFinderSettings,
                                                IReadOnlyHashSet<TheoremType> types)
        {
            // Go through all requested types
            types.ForEach(type =>
            {
                // Switch based on type and do the binding
                switch (type)
                {
                    case TheoremType.CollinearPoints:
                        kernel.Bind<ITheoremsFinder>().To<CollinearPointsTheoremsFinder>();
                        break;

                    case TheoremType.ConcyclicPoints:
                        kernel.Bind<ITheoremsFinder>().To<ConcyclicPointsTheoremsFinder>();
                        break;

                    case TheoremType.ConcurrentObjects:
                        kernel.Bind<ITheoremsFinder>().To<ConcurrentObjectsTheoremsFinder>();
                        break;

                    case TheoremType.ConcurrentLines:
                        kernel.Bind<ITheoremsFinder>().To<ConcurrentLinesTheoremsFinder>();
                        break;

                    case TheoremType.ParallelLines:
                        kernel.Bind<ITheoremsFinder>().To<ParallelLinesTheoremsFinder>();
                        break;

                    case TheoremType.PerpendicularLines:
                        kernel.Bind<ITheoremsFinder>().To<PerpendicularLinesTheoremsFinder>();
                        break;

                    case TheoremType.TangentCircles:
                        kernel.Bind<ITheoremsFinder>().To<TangentCirclesTheoremsFinder>().WithConstructorArgument(tangentCirclesFinderSettings);
                        break;

                    case TheoremType.LineTangentToCircle:
                        kernel.Bind<ITheoremsFinder>().To<LineTangentToCircleTheoremsFinder>().WithConstructorArgument(lineTangentToCirclesFinderSettings);
                        break;

                    case TheoremType.EqualLineSegments:
                        kernel.Bind<ITheoremsFinder>().To<EqualLineSegmentsTheoremsFinder>();
                        break;

                    case TheoremType.EqualAngles:
                        kernel.Bind<ITheoremsFinder>().To<EqualAnglesTheoremsFinder>();
                        break;

                    default:
                        throw new GeoGenException($"Unhandled type of theorem: {type}");
                }
            });

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
