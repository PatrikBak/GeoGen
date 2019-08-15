using GeoGen.Utilities;
using System.Collections.Generic;

namespace GeoGen.Core
{
    /// <summary>
    /// Represents a dictionary mapping <see cref="ConfigurationObjectType"/> to lists of <see cref="ConfigurationObject"/>
    /// of that type.
    /// </summary>
    public class ConfigurationObjectsMap : ObjectsMap<ConfigurationObjectType, ConfigurationObject>
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationObjectsMap"/> class.
        /// </summary>
        /// <param name="objects">The objects to be contained in the map.</param>
        public ConfigurationObjectsMap(IEnumerable<ConfigurationObject> objects) : base(objects)
        {
        }

        #endregion

        #region GetKey implementation

        /// <summary>
        /// Gets the key for a given value.
        /// </summary>
        /// <param name="value">The value</param>
        /// <returns>The key.</returns>
        protected override ConfigurationObjectType GetKey(ConfigurationObject value) => value.ObjectType;

        #endregion
    }
}