using GeoGen.Utilities;
using System.Collections.Generic;

namespace GeoGen.Core
{
    /// <summary>
    /// Represents a dictionary mapping <see cref="ConfigurationObjectType"/> to lists of <see cref="ConfigurationObject"/>
    /// of that type.
    /// </summary>
    public class ConfigurationObjectMap : ObjectMap<ConfigurationObjectType, ConfigurationObject>
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationObjectMap"/> class.
        /// </summary>
        /// <param name="objects">The objects to be contained in the map.</param>
        public ConfigurationObjectMap(IEnumerable<ConfigurationObject> objects) : base(objects)
        {
        }

        #endregion

        #region GetKey implementation

        /// <inheritdoc/>
        protected override ConfigurationObjectType GetKey(ConfigurationObject value) => value.ObjectType;

        #endregion
    }
}