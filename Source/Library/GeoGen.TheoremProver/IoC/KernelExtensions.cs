using Ninject;

namespace GeoGen.TheoremProver
{
    /// <summary>
    /// The extension methods for <see cref="IKernel"/>.
    /// </summary>
    public static class KernelExtensions
    {
        /// <summary>
        /// Bindings for the dependencies from the TheoremProver module.
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        /// <param name="data">The data for the <see cref="InferenceRuleManager"/>.</param>
        /// <returns>The kernel for chaining.</returns>
        public static IKernel AddTheoremProver(this IKernel kernel, InferenceRuleManagerData data)
        {
            // Bind services
            kernel.Bind<ITheoremProver>().To<TheoremProver>();
            kernel.Bind<IInferenceRuleManager>().To<InferenceRuleManager>().WithConstructorArgument(data);
            kernel.Bind<IInferenceRuleApplier>().To<InferenceRuleApplier>();
            kernel.Bind<ITrivialTheoremProducer>().To<TrivialTheoremProducer>();
            kernel.Bind<IObjectIntroducer>().To<ObjectIntroducer>();

            // Bind tracer
            kernel.Bind<IInvalidInferenceTracer>().To<EmptyInvalidInferenceTracer>();

            // Return the kernel for chaining
            return kernel;
        }
    }
}