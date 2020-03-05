using Ninject;

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
        /// <param name="settings">The settings for <see cref="TheoremSorter"/>.</param>
        /// <returns>The kernel for chaining.</returns>
        public static IKernel AddTheoremSorter(this IKernel kernel, TheoremSorterSettings settings)
        {
            // Bind the sorter
            kernel.Bind<ITheoremSorter>().To<TheoremSorter>().WithConstructorArgument(settings);

            // Tracer
            kernel.Bind<ISortingGeometryFailureTracer>().To<EmptySortingGeometryFailureTracer>();

            // Return the kernel for chaining
            return kernel;
        }
    }
}
