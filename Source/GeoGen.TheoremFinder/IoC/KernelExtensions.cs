using GeoGen.Core;
using GeoGen.Utilities;
using Ninject;

namespace GeoGen.TheoremFinder
{
    /// <summary>
    /// The extension methods for <see cref="IKernel"/>.
    /// </summary>
    public static class KernelExtensions
    {
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
            kernel.Bind<ITheoremFinder>().To<TheoremFinder>();

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
                        throw new TheoremFinderException($"Unhandled value of {nameof(TheoremType)}: {type}");
                }
            });

            // Return the kernel for chaining
            return kernel;
        }
    }
}