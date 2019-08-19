using GeoGen.Utilities;

namespace GeoGen.Generator
{
    /// <summary>
    /// Represents a factory for creating new instances of <see cref="IContainer{T}"/>, where 'T' is <see cref="GeneratedConfiguration"/>. 
    /// <para>The implementation is supposed to be provided by the dependency injection management system.</para>
    /// </summary>
    public interface IConfigurationsContainerFactory
    {
        /// <summary>
        /// Creates an empty configurations container.
        /// </summary>
        /// <returns>An empty configurations container.</returns>
        IContainer<GeneratedConfiguration> CreateContainer();
    }
}