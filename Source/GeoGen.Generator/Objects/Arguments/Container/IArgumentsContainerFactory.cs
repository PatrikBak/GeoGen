using GeoGen.Core;

namespace GeoGen.Generator
{
    /// <summary>
    /// Represents a factory for creating new instances of <see cref="IContainer{T}"/>, where 'T' is <see cref="Arguments"/>. 
    /// </summary>
    /// <remarks>The implementation is supposed to be provided by the dependency injection management system.</remarks>
    public interface IArgumentsContainerFactory
    {
        /// <summary>
        /// Creates an empty arguments container.
        /// </summary>
        /// <returns>An empty arguments container.</returns>
        IContainer<Arguments> CreateContainer();
    }
}