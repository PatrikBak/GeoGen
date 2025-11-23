using Ninject;
using Ninject.Extensions.Factory;
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

            // Manually load factories, needed for with PublishSingleFile
            // It is in a try-caatch because when running from a non-published state,
            // it will throw an exception that it has already been loaded.
            try { kernel.Load(new FuncModule()); } catch { }

            // Return it
            return kernel;
        }
    }
}