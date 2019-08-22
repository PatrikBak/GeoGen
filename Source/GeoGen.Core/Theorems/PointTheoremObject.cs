using System.Collections.Generic;

namespace GeoGen.Core
{
    /// <summary>
    /// Represents a <see cref="TheoremObject"/> that has the type <see cref="ConfigurationObjectType.Point"/>.
    /// </summary>
    public class PointTheoremObject : BaseTheoremObject
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="PointTheoremObject"/> class.
        /// </summary>
        /// <param name="configurationObject">The configuration object whose type must be <see cref="ConfigurationObjectType.Point"/>.</param>
        public PointTheoremObject(ConfigurationObject configurationObject)
            : base(configurationObject)
        {
            // Make sure the object is not null
            if (configurationObject == null)
                throw new GeoGenException("A point theorem object must have its configuration object set.");
        }

        #endregion

        #region Public abstract methods implementation

        /// <summary>
        /// Recreates the theorem object by applying a given mapping of the inner configuration objects.
        /// Every <see cref="ConfigurationObject"/> internally contained in this theorem object must be
        /// present in the mapping. If the mapping cannot be done (for example because 2 points
        /// making a line are mapped to the same point), then null is returned.
        /// </summary>
        /// <param name="mapping">The dictionary representing the mapping.</param>
        /// <returns>The remapped theorem object, or null, if the mapping cannot be done.</returns>
        public override TheoremObject Remap(Dictionary<ConfigurationObject, ConfigurationObject> mapping) =>
            // Reuse the static helper from the base class    
            new PointTheoremObject(Map(ConfigurationObject, mapping));

        #endregion

        #region To String

        /// <summary>
        /// Converts the theorem point object to a string. 
        /// NOTE: This method is used only for debugging purposes.
        /// </summary>
        /// <returns>A human-readable string representation of the configuration.</returns>
        public override string ToString() => $"{ConfigurationObject.Id}";

        #endregion
    }
}
