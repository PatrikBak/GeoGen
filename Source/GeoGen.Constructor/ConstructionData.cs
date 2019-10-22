using GeoGen.Core;

namespace GeoGen.Constructor
{
    /// <summary>
    /// Represents data of geometric construction of <see cref="ConfigurationObject"/>s provided by <see cref="IGeometryConstructor"/>.
    /// </summary>
    public class ConstructionData
    {
        #region Public properties

        /// <summary>
        /// The first found pair of objects that turned out to be geometrically the same one.
        /// If there is none, the value should be (null, null).
        /// </summary>
        public (ConfigurationObject olderObject, ConfigurationObject newerObject) Duplicates { get; }

        /// <summary>
        /// Gets or sets the object that turned out to be geometrically inconstructible. 
        /// If there is none, the value should be null.
        /// </summary>
        public ConfigurationObject InconstructibleObject { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ConstructionData"/> class.
        /// </summary>
        /// <param name="duplicates">The first found pair of objects that turned out to be geometrically the same one. If there is none, the value should be (null, null).</param>
        /// <param name="inconstructibleObject">Gets or sets the object that turned out to be geometrically inconstructible. If there is none, the value should be null.</param>
        public ConstructionData((ConfigurationObject olderObject, ConfigurationObject newerObject) duplicates, ConfigurationObject inconstructibleObject)
        {
            Duplicates = duplicates;
            InconstructibleObject = inconstructibleObject;
        }

        #endregion
    }
}
