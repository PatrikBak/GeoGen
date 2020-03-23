using Ninject;
using Ninject.Extensions.Factory;

namespace GeoGen.TheoremSorter
{
    /// <summary>
    /// The extension methods for <see cref="IKernel"/>.
    /// </summary>
    public static class KernelExtensions
    {
        /// <summary>
        /// Bindings for the dependencies from the TheoremSorter module.
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        /// <returns>The kernel for chaining.</returns>
        public static IKernel AddTheoremSorter(this IKernel kernel)
        {
            // Bind the sorter
            kernel.Bind<ITheoremSorter>().To<TheoremSorter>();

            // Bind the tracer
            kernel.Bind<ISortingGeometryFailureTracer>().To<EmptySortingGeometryFailureTracer>();

            // Bind the factory
            kernel.Bind<ITheoremSorterFactory>().ToFactory();

            // Return the kernel for chaining
            return kernel;
        }
    }
}
