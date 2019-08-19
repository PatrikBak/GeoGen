using GeoGen.Core;
using GeoGen.Utilities;

namespace GeoGen.Generator
{
    /// <summary>
    /// Represents a factory for creating new instances of <see cref="IContainer{T}"/>, where 'T' is <see cref="Arguments"/>. 
    /// <para>The implementation is supposed to be provided by the dependency injection management system.</para>
    /// </summary>
    public interface IArgumentsContainerFactory
    {
        /// <summary>
        /// Creates an empty arguments container.
        /// </summary>
        /// <returns>An empty arguments container.</returns>
        IContainer<Arguments> CreateContainer();
    }
}