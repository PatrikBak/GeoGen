using Ninject;

namespace GeoGen.TheoremSimplifier
{
    /// <summary>
    /// The extension methods for <see cref="IKernel"/>.
    /// </summary>
    public static class KernelExtensions
    {
        /// <summary>
        /// Bindings for the dependencies from the TheoremSimplifier module.
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        /// <param name="data">The data for the simplifier.</param>
        /// <returns>The kernel for chaining.</returns>
        public static IKernel AddTheoremSimplifier(this IKernel kernel, TheoremSimplifierData data)
        {
            // Bind the simplifier
            kernel.Bind<ITheoremSimplifier>().To<TheoremSimplifier>().WithConstructorArgument(data);

            // Return the kernel for chaining
            return kernel;
        }
    }
}
