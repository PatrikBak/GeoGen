using GeoGen.Core;

namespace GeoGen.Generator
{
    /// <summary>
    /// Represents a factory for creating new instances of <see cref="IContainer{T}"/>, where 'T' is <see cref="ConfigurationObject"/>. 
    /// <para>The implementation is supposed to be provided by the dependency injection management system.</para>
    /// </summary>
    public interface IConfigurationObjectsContainerFactory
    {
        /// <summary>
        /// Creates an empty configuration objects container.
        /// </summary>
        /// <returns>An empty configuration objects container.</returns>
        IContainer<ConfigurationObject> CreateContainer();
    }
}