using Ninject;
using Ninject.Extensions.Factory;
using System;

namespace GeoGen.ConfigurationGenerator
{
    /// <summary>
    /// The extension methods for <see cref="IKernel"/>.
    /// </summary>
    public static class KernelExtensions
    {
        /// <summary>
        /// Bindings for the dependencies from the ConfigurationGenerator module.
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        /// <param name="settings">The settings for the module.</param>
        /// <returns>The kernel for chaining.</returns>
        public static IKernel AddConfigurationGenerator(this IKernel kernel, GenerationSettings settings)
        {
            // Stateless service
            kernel.Bind<IConfigurationGenerator>().To<ConfigurationGenerator>();

            // Factory
            kernel.Bind<IConfigurationFilterFactory>().ToFactory();

            #region Bind configuration filter

            // Find the expected name of the class with the corresponding namespace
            var classNameWithNamespace = $"{typeof(IConfigurationFilter).Namespace}.{settings.ConfigurationFilterType}ConfigurationFilter";

            // Find the type of the finder from the name
            var configurationFilterType = Type.GetType(classNameWithNamespace);

            // Handle if it couldn't be found
            if (configurationFilterType == null)
                throw new ConfigurationGeneratorException($"Couldn't find an implementation of {nameof(IConfigurationFilter)} for type '{settings.ConfigurationFilterType}', expected class name with namespace '{classNameWithNamespace}'");

            // Otherwise do the binding
            kernel.Bind(typeof(IConfigurationFilter)).To(configurationFilterType);

            #endregion

            // Return the kernel for chaining
            return kernel;
        }
    }
}