using GeoGen.Core;

namespace GeoGen.Analyzer
{
    /// <summary>
    /// Represents a factory for creating new instances of <see cref="IContextualContainer"/>, 
    /// that should contain objects from a <see cref="Configuration"/> and get the actual 
    /// geometric version of them from a <see cref="IObjectsContainersManager"/>.
    /// <para>The implementation is supposed to be provided by the dependency injection management system.</para>
    /// </summary>
    public interface IContextualContainerFactory
    {
        /// <summary>
        /// Creates a new contextual container that represents a given configuration.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <returns>The container.</returns>
        IContextualContainer Create(Configuration configuration, IObjectsContainersManager manager);
    }
}
