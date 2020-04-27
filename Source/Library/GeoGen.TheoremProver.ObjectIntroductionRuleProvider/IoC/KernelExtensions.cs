using Ninject;

namespace GeoGen.TheoremProver.ObjectIntroductionRuleProvider
{
    /// <summary>
    /// The extension methods for <see cref="IKernel"/>.
    /// </summary>
    public static class KernelExtensions
    {
        /// <summary>
        /// Bindings for the dependencies for the TheoremProver.ObjectIntroductionRuleProvider module.
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        /// <param name="settings">The settings for <see cref="ObjectIntroductionRuleProvider"/>.</param>
        /// <returns>The kernel for chaining.</returns>
        public static IKernel AddObjectIntroductionRuleProvider(this IKernel kernel, ObjectIntroductionRuleProviderSettings settings)
        {
            // Bind the rule provider
            kernel.Bind<IObjectIntroductionRuleProvider>().To<ObjectIntroductionRuleProvider>().WithConstructorArgument(settings);

            // Return the kernel for chaining
            return kernel;
        }
    }
}