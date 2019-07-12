namespace GeoGen.Constructor
{
    /// <summary>
    /// Represents a factory for creating new instances of <see cref="IPicturesManager"/>.
    /// <para>The implementation is supposed to be provided by the dependency injection management system.</para>
    /// </summary>
    public interface IPicturesManagerFactory
    {
        /// <summary>
        /// Creates a new object pictures manager with empty pictures.
        /// </summary>
        /// <returns>A new object pictures manager with empty pictures.</returns>
        IPicturesManager CreatePicturesManager();
    }
}
