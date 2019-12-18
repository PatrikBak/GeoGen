using Ninject;
using Ninject.Extensions.ContextPreservation;
using Ninject.Extensions.Factory;
using Ninject.Planning.Bindings.Resolvers;

namespace GeoGen.Infrastructure
{
    /// <summary>
    /// Utilities for the IoC module.
    /// </summary>
    public static class IoC
    {
        /// <summary>
        /// Creates a kernel with all needed modules for <see cref="GeoGenBindings"/>.
        /// </summary>
        /// <returns>The kernel.</returns>
        public static IKernel CreateKernel()
        {
            // Create the function module (that is disposable)
            using var funcModule = new FuncModule();

            // Create a kernel that will perform the resolution
            var kernel = new StandardKernel(funcModule, new ContextPreservationModule());

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
