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
        /// <param name="settings">The settings for the module.</param>
        /// <returns>The kernel for chaining.</returns>
        public static IKernel AddTheoremProver(this IKernel kernel, TheoremProvingSettings settings)
        {
            // Bind the services
            kernel.Bind<ITheoremProver>().To<TheoremProver>().WithConstructorArgument(settings.TheoremProverSettings);
            kernel.Bind<IInferenceRuleManager>().To<InferenceRuleManager>().WithConstructorArgument(settings.InferenceRuleManagerData);
            kernel.Bind<IObjectIntroducer>().To<ObjectIntroducer>().WithConstructorArgument(settings.ObjectIntroducerData);
            kernel.Bind<IInferenceRuleApplier>().To<InferenceRuleApplier>();
            kernel.Bind<ITrivialTheoremProducer>().To<TrivialTheoremProducer>();

            // Bind the tracer
            kernel.Bind<IInvalidInferenceTracer>().To<EmptyInvalidInferenceTracer>();

            // Return the kernel for chaining
            return kernel;
        }
    }
}