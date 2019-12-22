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
        /// <param name="settings">The settings for the algorithm facade.</param>
        /// <returns>The kernel for chaining.</returns>
        public static IKernel AddAlgorithm(this IKernel kernel, AlgorithmFacadeSettings settings)
        {
            // Stateless services
            kernel.Bind<IAlgorithmFacade>().To<AlgorithmFacade>().WithConstructorArgument(settings);
            kernel.Bind<IBestTheoremsFinder>().To<BestTheoremsFinder>();

            // Return the kernel for chaining
            return kernel;
        }
    }
}