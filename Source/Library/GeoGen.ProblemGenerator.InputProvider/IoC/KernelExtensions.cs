using Ninject;

namespace GeoGen.ProblemGenerator.InputProvider
{
    /// <summary>
    /// The extension methods for <see cref="IKernel"/>.
    /// </summary>
    public static class KernelExtensions
    {
        /// <summary>
        /// Bindings for the dependencies for the ProblemGenerator.InputProvider module.
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        /// <param name="settings">The settings for <see cref="ProblemGeneratorInputProvider"/>.</param>
        /// <returns>The kernel for chaining.</returns>
        public static IKernel AddProblemGeneratorInputProvider(this IKernel kernel, ProblemGeneratorInputProviderSettings settings)
        {
            // Bind the input provider
            kernel.Bind<IProblemGeneratorInputProvider>().To<ProblemGeneratorInputProvider>().WithConstructorArgument(settings);

            // Return the kernel for chaining
            return kernel;
        }
    }
}