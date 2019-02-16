using GeoGen.Core;

namespace GeoGen.Constructor
{
    /// <summary>
    /// Represents a service that cares care of geometrical construction of <see cref="Configuration"/>s and
    /// <see cref="ConfigurationObject"/>s.
    /// </summary>
    public interface IGeometryConstructor
    {
        /// <summary>
        /// Constructs a given configuration.
        /// </summary>
        /// <param name="configuration">The configuration to be constructed.</param>
        /// <returns>The geometry data of the configuration.</returns>
        GeometryData Construct(Configuration configuration);

        /// <summary>
        /// Performs geometric examination of a given constructed object with respect to a given containers manager.
        /// The object will be constructed, but won't be added to manager's containers.
        /// </summary>
        /// <param name="constructedObject">The constructed configuration object to be examined.</param>
        /// <param name="manager">The manager of objects containers.</param>
        /// <returns>The geometry data of the object.</returns>
        GeometryData Examine(ConstructedConfigurationObject constructedObject, IObjectsContainersManager manager);
    }
}