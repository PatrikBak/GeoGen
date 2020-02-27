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
        /// <param name="settings">The settings for <see cref="ProblemGenerator"/>.</param>
        /// <returns>The kernel for chaining.</returns>
        public static IKernel AddAlgorithm(this IKernel kernel, ProblemGeneratorSettings settings)
        {
            // Stateless services
            kernel.Bind<IProblemGenerator>().To<ProblemGenerator>().WithConstructorArgument(settings);

            // Tracer
            kernel.Bind<IGeometryFailureTracer>().To<EmptyGeometryFailureTracer>();

            // Return the kernel for chaining
            return kernel;
        }
    }
}