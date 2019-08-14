using GeoGen.Core;

namespace GeoGen.Constructor
{
    /// <summary>
    /// Represents a factory for creating new instances of <see cref="Pictures"/>.
    /// <para>The implementation is supposed to be provided by the dependency injection management system.</para>
    /// </summary>
    public interface IPicturesFactory
    {
        /// <summary>
        /// Creates new empty <see cref="Pictures"/> that should represent a given <see cref="Configuration"/>.
        /// </summary>
        /// <param name="configuration">The configuration to be represented by the pictures.</param>
        /// <returns>The pictures.</returns>
        Pictures CreatePictures(Configuration configuration);
    }
}
