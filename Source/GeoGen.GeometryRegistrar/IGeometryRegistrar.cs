using GeoGen.Core;

namespace GeoGen.GeometryRegistrar
{
    /// <summary>
    /// Represents a service that cares care of geometrical construction of <see cref="Configuration"/>s.
    /// </summary>
    public interface IGeometryRegistrar
    {
        /// <summary>
        /// Registers a given configuration to the geometry system. 
        /// </summary>
        /// <param name="configuration">The configuration to be registered.</param>
        /// <returns>A registration result containing information about inconstructible and duplicate objects.</returns>
        RegistrationResult Register(Configuration configuration);
    }
}