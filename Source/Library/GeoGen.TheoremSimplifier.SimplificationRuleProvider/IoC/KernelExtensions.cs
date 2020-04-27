using Ninject;

namespace GeoGen.TheoremSimplifier.SimplificationRuleProvider
{
    /// <summary>
    /// The extension methods for <see cref="IKernel"/>.
    /// </summary>
    public static class KernelExtensions
    {
        /// <summary>
        /// Bindings for the dependencies for the TheoremSimplifier.RuleProvider module.
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        /// <param name="settings">The settings for <see cref="SimplificationRuleProvider"/>.</param>
        /// <returns>The kernel for chaining.</returns>
        public static IKernel AddSimplificationRuleProvider(this IKernel kernel, SimplificationRuleProviderSettings settings)
        {
            // Bind the rule provider
            kernel.Bind<ISimplificationRuleProvider>().To<SimplificationRuleProvider>().WithConstructorArgument(settings);

            // Return the kernel for chaining
            return kernel;
        }
    }
}