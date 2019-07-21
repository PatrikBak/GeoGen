using Ninject;
using Ninject.Extensions.ContextPreservation;
using Ninject.Extensions.Factory;
using Ninject.Planning.Bindings.Resolvers;

namespace GeoGen.DependenciesResolver
{
    /// <summary>
    /// Utilities for the IoC module.
    /// </summary>
    public static class IoCUtilities
    {
        /// <summary>
        /// Creates a kernel with all needed modules for GeoGen bindings.
        /// </summary>
        /// <returns>The kernel.</returns>
        public static IKernel CreateKernel()
        {
            // Create a kernel that will perform the resolution
            var kernel = new StandardKernel(new FuncModule(), new ContextPreservationModule());

            // Make sure we can bind to null 
            // This is used only for tracers, that are not compulsory
            // I like this better than the NullObject pattern, because
            // of '?' operator that can be used to prevent null-checks
            kernel.Settings.AllowNullInjection = true;

            // Make sure NInject doesn't create instances that haven't been bound
            kernel.Components.Remove<IMissingBindingResolver, SelfBindingResolver>();

            // Return it
            return kernel;
        }
    }
}
