using Ninject;
using Ninject.Planning.Bindings.Resolvers;

namespace GeoGen.Infrastructure
{
    /// <summary>
    /// Static NInject utilities.
    /// </summary>
    public static class NinjectUtilities
    {
        /// <summary>
        /// Creates and sets up a <see cref="StandardKernel"/>.
        /// </summary>
        /// <returns>The kernel.</returns>
        public static StandardKernel CreateKernel()
        {
            // Create a kernel that will perform the resolution
            var kernel = new StandardKernel();

            // Make sure we can bind to null
            // I don't think that not using NullObjectPattern is such a big deal
            kernel.Settings.AllowNullInjection = true;

            // Make sure NInject doesn't create instances that haven't been bound
            kernel.Components.Remove<IMissingBindingResolver, SelfBindingResolver>();

            // Return it
            return kernel;
        }
    }
}