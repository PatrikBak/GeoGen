using Ninject;
using Ninject.Syntax;
using System.Linq;

namespace GeoGen.ConsoleTest
{
    /// <summary>
    /// The extension methods for Ninject syntax.
    /// </summary>
    public static class NinjectExtensions
    {
        /// <summary>
        /// Binds given <typeparamref name="TService"/> to given <typeparamref name="TImplementation"/> 
        /// with constructed argument of type <typeparamref name="TSettings"/> which is pulled from 
        /// the context parameters as a <see cref="DynamicSettingsParameter{TSettings}"/>.
        /// </summary>
        /// <typeparam name="TService">The type of service being bound.</typeparam>
        /// <typeparam name="TImplementation">The of implementation to which we bound.</typeparam>
        /// <typeparam name="TSettings">The type of the settings object.</typeparam>
        /// <param name="kernel">The kernel.</param>
        /// <returns>Fluent API syntax.</returns>
        public static IBindingWithOrOnSyntax<TImplementation> BindsWithDynamicSettings<TService, TImplementation, TSettings>(this IKernel kernel)
             where TImplementation : TService
        {
            // Bind to the service with the constructor argument created from the corresponding dynamic settings
            return kernel.Bind<TService>()
                         .To<TImplementation>()
                         .WithConstructorArgument(context => (TSettings)context.Parameters.OfType<DynamicSettingsParameter>().Single(p => p.Value.GetType() == typeof(TSettings)).Value);
        }
    }
}