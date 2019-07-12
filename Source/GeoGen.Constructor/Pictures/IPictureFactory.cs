namespace GeoGen.Constructor
{
    /// <summary>
    /// Represents a factory for creating new instances of <see cref="IPicture"/>.
    /// <para>The implementation is supposed to be provided by the dependency injection management system.</para>
    /// </summary>
    public interface IPictureFactory
    {
        /// <summary>
        /// Creates an empty picture.
        /// </summary>
        /// <returns>An empty picture.</returns>
        IPicture CreatePicture();
    }
}