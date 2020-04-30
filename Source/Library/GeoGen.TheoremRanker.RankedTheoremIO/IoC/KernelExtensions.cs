using Ninject;
using Ninject.Extensions.Factory;

namespace GeoGen.TheoremRanker.RankedTheoremIO
{
    /// <summary>
    /// The extension methods for <see cref="IKernel"/>.
    /// </summary>
    public static class KernelExtensions
    {
        /// <summary>
        /// Bindings for the dependencies for the ranked theorem IO module.
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        /// <returns>The kernel for chaining.</returns>
        public static IKernel AddRankedTheoremIO(this IKernel kernel)
        {
            // Bind the writer and factory
            kernel.Bind<IRankedTheoremJsonLazyWriter>().To<RankedTheoremJsonLazyWriter>();
            kernel.Bind<IRankedTheoremJsonLazyWriterFactory>().ToFactory();

            // Bind the reader
            kernel.Bind<IRankedTheoremJsonLazyReader>().To<RankedTheoremJsonLazyReader>();

            // Return the kernel for chaining
            return kernel;
        }
    }
}