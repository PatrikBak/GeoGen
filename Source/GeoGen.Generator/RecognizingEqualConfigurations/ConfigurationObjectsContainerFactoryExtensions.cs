using GeoGen.Core;
using GeoGen.Utilities;

namespace GeoGen.Generator
{
    /// <summary>
    /// Extension methods for <see cref="IConfigurationObjectsContainerFactory"/>.
    /// </summary>
    public static class ConfigurationObjectsContainerFactoryExtensions
    {
        /// <summary>
        /// Creates a container containing objects of a given configuration.
        /// </summary>
        /// <param name="factory">The factory.</param>
        /// <param name="configuration">The configuration whose objects will be added to the container.</param>
        /// <returns>The container.</returns>
        public static IContainer<ConfigurationObject> CreateContainer(this IConfigurationObjectsContainerFactory factory, Configuration configuration)
        {
            // Create an empty container
            var container = factory.CreateContainer();

            // Add all the objects to it
            configuration.ObjectsMap.AllObjects.ForEach(container.Add);

            // Return it
            return container;
        }
    }
}
