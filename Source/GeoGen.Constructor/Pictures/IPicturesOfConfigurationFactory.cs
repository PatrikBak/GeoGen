using GeoGen.Core;

namespace GeoGen.Constructor
{
    /// <summary>
    /// Represents a factory for creating new instances of <see cref="PicturesOfConfiguration"/>.
    /// <para>The implementation is supposed to be provided by the dependency injection management system.</para>
    /// </summary>
    public interface IPicturesOfConfigurationFactory
    {
        /// <summary>
        /// Creates new empty <see cref="PicturesOfConfiguration"/> that should represent a given <see cref="Configuration"/>.
        /// </summary>
        /// <param name="configuration">The configuration to be represented by the pictures.</param>
        /// <returns>The pictures.</returns>
        PicturesOfConfiguration CreatePictures(Configuration configuration);
    }
}
