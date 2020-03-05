using Ninject;

namespace GeoGen.ProblemAnalyzer
{
    /// <summary>
    /// The extension methods for <see cref="IKernel"/>.
    /// </summary>
    public static class KernelExtensions
    {
        /// <summary>
        /// Bindings for the dependencies from the ProblemAnalyzer module.
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        /// <returns>The kernel for chaining.</returns>
        public static IKernel AddAnalyzer(this IKernel kernel)
        {
            // Bind the analyzer
            kernel.Bind<IGeneratedProblemAnalyzer>().To<GeneratedProblemAnalyzer>();

            // Return the kernel for chaining
            return kernel;
        }
    }
}