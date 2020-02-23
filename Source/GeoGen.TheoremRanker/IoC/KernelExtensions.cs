using Ninject;

namespace GeoGen.TheoremRanker
{
    /// <summary>
    /// The extension methods for <see cref="IKernel"/>.
    /// </summary>
    public static class KernelExtensions
    {
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
            kernel.Bind<ITheoremRanker>().To<TheoremRanker>().WithConstructorArgument(rankerSettings);

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

                    default:
                        throw new TheoremRankerException($"Unhandled value of {nameof(RankedAspect)}: {rankedAspect}");
                }
            }

            // Return the kernel for chaining
            return kernel;
        }
    }
}
