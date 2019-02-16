namespace GeoGen.Constructor
{
    /// <summary>
    /// Represents a factory for creating new instances of <see cref="IObjectsContainersManager"/>.
    /// <para>The implementation is supposed to be provided by the dependency injection management system.</para>
    /// </summary>
    public interface IObjectsContainersManagerFactory
    {
        /// <summary>
        /// Creates a new object containers manager with empty containers.
        /// </summary>
        /// <returns>A new object containers manager with empty containers.</returns>
        IObjectsContainersManager CreateContainersManager();
    }
}
