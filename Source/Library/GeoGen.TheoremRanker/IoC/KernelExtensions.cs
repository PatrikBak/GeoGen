using Ninject;
using System;

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
        /// <param name="settings">The settings for the module.</param>
        /// <returns>The kernel for chaining.</returns>
        public static IKernel AddTheoremRanker(this IKernel kernel, TheoremRankingSettings settings)
        {
            // Bind ranker
            kernel.Bind<ITheoremRanker>().To<TheoremRanker>().WithConstructorArgument(settings.TheoremRankerSettings);

            #region Bind theorem rankers for individual ranked aspects

            // Bind aspect theorem rankers based on the aspects we're ranking
            foreach (var rankedAspect in settings.TheoremRankerSettings.RankingCoefficients.Keys)
            {
                // Find the expected name of the class with the corresponding namespace
                var classNameWithNamespace = $"{typeof(IAspectTheoremRanker).Namespace}.{rankedAspect}Ranker";

                // Find the type of the ranker from the name
                var theoremRankerType = Type.GetType(classNameWithNamespace);

                // Handle if it couldn't be found
                if (theoremRankerType == null)
                    throw new TheoremRankerException($"Couldn't find an implementation of {nameof(IAspectTheoremRanker)} for type '{rankedAspect}', expected class name with namespace '{classNameWithNamespace}'");

                // Otherwise do the binding
                var binding = kernel.Bind(typeof(IAspectTheoremRanker)).To(theoremRankerType);

                // In some cases there is a constructor argument
                switch (rankedAspect)
                {
                    // Theorem type
                    case RankedAspect.TheoremType:
                        binding.WithConstructorArgument(settings.TheoremTypeRankerSettings);
                        break;

                    // Constructions
                    case RankedAspect.Constructions:
                        binding.WithConstructorArgument(settings.ConstructionsRankerSettings);
                        break;
                }
            }

            #endregion

            // Return the kernel for chaining
            return kernel;
        }
    }
}
