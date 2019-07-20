using Ninject;
using Ninject.Syntax;
using System.Linq;

namespace GeoGen.IoC
{
    /// <summary>
    /// The extension methods for Ninject syntax.
    /// </summary>
    public static class NinjectExtensions
    {
        /// <summary>
        /// Binds given <typeparamref name="TService"/> to given <typeparamref name="TImplementation"/> 
        /// with constructed argument of type <typeparamref name="TSettings"/> which is pulled from 
        /// the context parameters as a <see cref="DynamicSettingsParameter"/>.
        /// </summary>
        /// <typeparam name="TService">The type of service being bound.</typeparam>
        /// <typeparam name="TImplementation">The of implementation to which we bound.</typeparam>
        /// <typeparam name="TSettings">The type of the settings object.</typeparam>
        /// <param name="kernel">The kernel.</param>
        /// <returns>Fluent API syntax.</returns>
        public static IBindingWithOrOnSyntax<TImplementation> BindsWithDynamicSettings<TService, TImplementation, TSettings>(this IKernel kernel)
             where TImplementation : TService
        {
            // Bind the type of service
            return kernel.Bind<TService>()
                         // To the requested implementation
                         .To<TImplementation>()
                         // With a constructor argument found in the preserved context...
                         .WithConstructorArgument(context =>
                         {
                             // Get the dynamic settings parameters
                             var parameters = context.Parameters.OfType<DynamicSettingsParameter>()
                                    // Of the requested type
                                    .Where(p => p.Value.GetType() == typeof(TSettings))
                                    // Enumerate them
                                    .ToArray();

                             // Make sure there is some
                             if (parameters.Length == 0)
                                 throw new IoCException($"The service {typeof(TImplementation)} requires a dynamic parameter of type {typeof(TSettings)}. Pass it to the Kernel.Get method.");

                             // Make sure there is exactly one
                             if (parameters.Length > 1)
                                 throw new IoCException($"There cannot be more than one dynamic parameter of type {typeof(TSettings)}.");

                             // Get the value from the single suitable parameter
                             return parameters[0].Value;
                         });
        }
    }
}