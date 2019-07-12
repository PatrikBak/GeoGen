using GeoGen.Core;

namespace GeoGen.Constructor
{
    /// <summary>
    /// Represents a factory for creating new instances of <see cref="IContextualPicture"/>, 
    /// that should represent given <see cref="Configuration"/>s and get the actual 
    /// geometric version of its objects from a <see cref="IPicturesManager"/>.
    /// <para>The implementation is supposed to be provided by the dependency injection management system.</para>
    /// </summary>
    public interface IContextualPictureFactory
    {
        /// <summary>
        /// Creates a new contextual picture that holds a given configuration and gets its
        /// analytic representations from a given pictures manager. If the construction cannot
        /// be done, then the method raises an <see cref="InconstructibleContextualPicture"/> exception.
        /// </summary>
        /// <param name="configuration">The configuration that will be drawn in the picture.</param>
        /// <param name="manager">The manager of all the pictures where our objects are drawn.</param>
        /// <returns>A contextual picture holding the given objects.</returns>
        IContextualPicture Create(Configuration configuration, IPicturesManager manager);
    }
}