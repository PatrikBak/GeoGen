using GeoGen.Core;

namespace GeoGen.Analyzer
{
    /// <summary>
    /// Represents a service that cares care of constructing and storing actual geometrical 
    /// representations of <see cref="Configuration"/>s. 
    /// </summary>
    public interface IGeometryRegistrar
    {
        /// <summary>
        /// Registers a given configuration to the geometry system. The configuration must be constructible 
        /// and must not contain duplicate objects. If it does, the registration won't be successful, i.e. 
        /// the geometrical representation of the configuration won't be stored.
        /// </summary>
        /// <param name="configuration">The configuration to be registered.</param>
        /// <returns>A registration result containing information about inconstructible and duplicate objects.</returns>
        RegistrationResult Register(Configuration configuration);

        /// <summary>
        /// Gets the manager corresponding to a given configuration. The configuration must
        /// have been registered before.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <returns>The manager corresponding to the given configuration.</returns>
        IObjectsContainersManager GetContainersManager(Configuration configuration);
    }
}