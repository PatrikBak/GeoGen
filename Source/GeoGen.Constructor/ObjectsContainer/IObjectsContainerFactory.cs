namespace GeoGen.Constructor
{
    /// <summary>
    /// Represents a factory for creating new instances of <see cref="IObjectsContainer"/>.
    /// <para>The implementation is supposed to be provided by the dependency injection management system.</para>
    /// </summary>
    public interface IObjectsContainerFactory
    {
        /// <summary>
        /// Creates an empty objects container.
        /// </summary>
        /// <returns>An empty objects container.</returns>
        IObjectsContainer CreateContainer();
    }
}