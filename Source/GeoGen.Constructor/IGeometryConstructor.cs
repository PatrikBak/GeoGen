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
        /// Performs geometric examination of a given constructed object with respect to a given containers manager
        /// that represents a given configuration.
        /// The object will be constructed, but won't be added to the manager's containers.
        /// </summary>
        /// <param name="configuration">The configuration that is drawn in the manager's containers.</param>
        /// <param name="manager">The manager of objects containers where all the needed objects for the constructed object should be drawn.</param>
        /// <param name="constructedObject">The constructed configuration object to be examined.</param>
        /// <returns>The geometry data of the object.</returns>
        GeometryData Examine(Configuration configuration, IObjectsContainersManager manager, ConstructedConfigurationObject constructedObject);
    }
}