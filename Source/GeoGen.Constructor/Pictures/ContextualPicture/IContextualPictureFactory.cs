using GeoGen.Core;

namespace GeoGen.Constructor
{
    /// <summary>
    /// Represents a factory for creating new instances of <see cref="ContextualPicture"/>, 
    /// that should represent already drawn <see cref="ConfigurationObject"/>s within <see cref="Pictures"/>.
    /// <para>The implementation is supposed to be provided by the dependency injection management system.</para>
    /// </summary>
    public interface IContextualPictureFactory
    {
        /// <summary>
        /// Creates a new contextual picture that displays given <see cref="Pictures"/>.
        /// If the construction cannot be done, then the method raises an 
        /// <see cref="InconstructibleContextualPicture"/> exception.
        /// </summary>
        /// <param name="pictures">The pictures that should be drawn in the contextual picture.</param>
        /// <returns>The contextual picture.</returns>
        ContextualPicture Create(Pictures pictures);
    }
}