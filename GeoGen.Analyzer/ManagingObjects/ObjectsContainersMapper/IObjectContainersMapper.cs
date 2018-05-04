using GeoGen.Core;

namespace GeoGen.Analyzer
{
    /// <summary>
    /// Represents a service that takes care of mapping <see cref="Configuration"/>s
    /// to their actual geometrical representations wrapped inside <see cref="IObjectsContainersManager"/>s.
    /// </summary>
    internal interface IObjectContainersMapper
    {
        /// <summary>
        /// Gets the manager corresponding to a given configuration.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <returns>The manager.</returns>
        IObjectsContainersManager Get(Configuration configuration);

        /// <summary>
        /// Creates a new manager for a given configuration and returns it.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <returns>The manager.</returns>
        IObjectsContainersManager Create(Configuration configuration);
    }
}
