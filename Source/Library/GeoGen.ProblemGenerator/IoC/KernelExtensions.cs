using Ninject;

namespace GeoGen.ProblemGenerator
{
    /// <summary>
    /// The extension methods for <see cref="IKernel"/>.
    /// </summary>
    public static class KernelExtensions
    {
        /// <summary>
        /// Bindings for the dependencies from the ProblemGenerator module.
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        /// <param name="settings">The settings for <see cref="ProblemGenerator"/>.</param>
        /// <returns>The kernel for chaining.</returns>
        public static IKernel AddProblemGenerator(this IKernel kernel, ProblemGeneratorSettings settings)
        {
            // Bind the generator
            kernel.Bind<IProblemGenerator>().To<ProblemGenerator>().WithConstructorArgument(settings);

            // Bind the tracer
            kernel.Bind<IGeometryFailureTracer>().To<EmptyGeometryFailureTracer>();

            // Return the kernel for chaining
            return kernel;
        }
    }
}