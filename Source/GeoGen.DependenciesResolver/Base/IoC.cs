using Ninject;
using Ninject.Extensions.ContextPreservation;
using Ninject.Extensions.Factory;
using Ninject.Planning.Bindings.Resolvers;
using System.Linq;

namespace GeoGen.DependenciesResolver
{
    /// <summary>
    /// The IoC container.
    /// </summary>
    public static class IoC
    {
        #region Kernel

        /// <summary>
        /// The NInject kernel.
        /// </summary>
        public static IKernel Kernel { get; private set; }

        #endregion

        #region Static constructor

        /// <summary>
        /// The static constructor that initializes <see cref="Kernel"/>.
        /// </summary>
        static IoC()
        {
            // Create a kernel that will perform the resolution
            Kernel = new StandardKernel(new FuncModule(), new ContextPreservationModule());

            // Make sure we can bind to null 
            // This is used only for tracers, that are not compulsory
            // I like this better than the NullObject pattern, because
            // of '?' operator that can be used to prevent null-checks
            Kernel.Settings.AllowNullInjection = true;

            // Make sure NInject doesn't create instances that haven't been bound
            Kernel.Components.Remove<IMissingBindingResolver, SelfBindingResolver>();
        }

        #endregion

        #region Get method

        /// <summary>
        /// Gets the given service from the kernel, with possible dynamic configuration objects.
        /// </summary>
        /// <typeparam name="T">The type of the retrieved service.</typeparam>
        /// <param name="dynamicConfiguration">The dynamic configuration.</param>
        /// <returns>The retrieved service.</returns>
        public static T Get<T>(params object[] dynamicConfiguration)
        {
            // Delegate the call to the kernel with the passed objects cast to dynamic parameters
            return Kernel.Get<T>(dynamicConfiguration.Select(obj => new DynamicSettingsParameter(obj)).ToArray());
        }

        #endregion
    }
}
