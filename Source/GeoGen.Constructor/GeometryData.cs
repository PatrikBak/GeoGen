using GeoGen.Core;

namespace GeoGen.Constructor
{
    /// <summary>
    /// Represents the result of geometric examination of <see cref="ConfigurationObject"/>s.
    /// </summary>
    public class GeometryData
    {
        /// <summary>
        /// Gets or sets if the configuration was successfully examined. 
        /// </summary>
        public bool SuccessfullyExamined { get; set; }

        /// <summary>
        /// Gets or sets the first found pair of objects that turned out to be geometrically the same one.
        /// If there is none, the value should be (null, null).
        /// </summary>
        public (ConfigurationObject olderObject, ConfigurationObject newerObject) Duplicates { get; set; }

        /// <summary>
        /// Gets or sets the first object that turned out to be geometrically inconstructible. 
        /// If there is none, the value should be null.
        /// </summary>
        public ConfigurationObject InconstructibleObject { get; set; }

        /// <summary>
        /// Gets or sets the manager of all objects containers where the configuration was drawn to.
        /// </summary>
        public IObjectsContainersManager Manager { get; set; }
    }
}
