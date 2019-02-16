using GeoGen.Core;

namespace GeoGen.Constructor
{
    /// <summary>
    /// Represents a factory for creating new instances of <see cref="IComposedConstructor"/>, for a given <see cref="ComposedConstruction"/>.
    /// <para>The implementation is supposed to be provided by the dependency injection management system.</para>
    /// </summary>
    public interface IComposedConstructorFactory
    {
        /// <summary>
        /// Creates a composed constructor that performs a given composed construction.
        /// </summary>
        /// <param name="construction">The composed construction to be performed.</param>
        /// <returns>The constructor that performs the construction.</returns>
        IComposedConstructor Create(ComposedConstruction construction);
    }
}