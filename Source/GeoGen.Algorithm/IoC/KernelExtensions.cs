using Ninject;

namespace GeoGen.Algorithm
{
    /// <summary>
    /// The extension methods for <see cref="IKernel"/>.
    /// </summary>
    public static class KernelExtensions
    {
        /// <summary>
        /// Bindings for the dependencies from the Algorithm module.
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        /// <param name="facadeSettings">The settings for the algorithm facade.</param>
        /// <param name="finderSettings">The settings for the best theorems finder.</param>
        /// <returns>The kernel for chaining.</returns>
        public static IKernel AddAlgorithm(this IKernel kernel, AlgorithmFacadeSettings facadeSettings, BestTheoremsFinderSettings finderSettings)
        {
            // Stateless services
            kernel.Bind<IAlgorithmFacade>().To<AlgorithmFacade>().WithConstructorArgument(facadeSettings);
            kernel.Bind<IBestTheoremsFinder>().To<BestTheoremsFinder>().WithConstructorArgument(finderSettings);

            // Return the kernel for chaining
            return kernel;
        }
    }
}