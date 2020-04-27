using Ninject;

namespace GeoGen.TheoremProver.InferenceRuleProvider
{
    /// <summary>
    /// The extension methods for <see cref="IKernel"/>.
    /// </summary>
    public static class KernelExtensions
    {
        /// <summary>
        /// Bindings for the dependencies for the TheoremProver.InferenceRuleProvider module.
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        /// <param name="settings">The settings for <see cref="InferenceRuleProvider"/>.</param>
        /// <returns>The kernel for chaining.</returns>
        public static IKernel AddInferenceRuleProvider(this IKernel kernel, InferenceRuleProviderSettings settings)
        {
            // Bind the rule provider
            kernel.Bind<IInferenceRuleProvider>().To<InferenceRuleProvider>().WithConstructorArgument(settings);

            // Return the kernel for chaining
            return kernel;
        }
    }
}