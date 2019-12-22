using Ninject;
using Ninject.Extensions.Factory;

namespace GeoGen.Generator
{
    /// <summary>
    /// The extension methods for <see cref="IKernel"/>.
    /// </summary>
    public static class KernelExtensions
    {
        /// <summary>
        /// Bindings for the dependencies from the Generator module.
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        /// <param name="filterType">The type of configuration filter to be used.</param>
        /// <returns>The kernel for chaining.</returns>
        public static IKernel AddGenerator(this IKernel kernel, ConfigurationFilterType filterType)
        {
            // Stateless service
            kernel.Bind<IGenerator>().To<Generator>();

            // Factory
            kernel.Bind<IConfigurationFilterFactory>().ToFactory();

            // Bind the filter according to the type
            switch (filterType)
            {
                case ConfigurationFilterType.MemoryEfficient:
                    kernel.Bind<IConfigurationFilter>().To<MemoryEfficientConfigurationFilter>();
                    break;

                case ConfigurationFilterType.Fast:
                    kernel.Bind<IConfigurationFilter>().To<FastConfigurationFilter>();
                    break;

                default:
                    throw new GeneratorException($"Unhandled value of {nameof(ConfigurationFilterType)}: {filterType}");
            }

            // Return the kernel for chaining
            return kernel;
        }
    }
}