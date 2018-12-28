using GeoGen.Core;

namespace GeoGen.Analyzer
{
    /// <summary>
    /// Represents a service that takes care of constructing a configuration.
    /// It finds out if the configuration is constructible and if it doesn't
    /// contain duplicate objects.
    /// </summary>
    public interface IGeometryRegistrar
    {
        /// <summary>
        /// Registers a given configuration to the actual geometrical system
        /// and returns if it is contructible and if it doesn't contain
        /// duplicate objects.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <returns>The registration result.</returns>
        RegistrationResult Register(Configuration configuration);
    }
}