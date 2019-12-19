using System.Collections.Generic;

namespace GeoGen.Core
{
    /// <summary>
    /// Represents a <see cref="TheoremObject"/> that has the type <see cref="ConfigurationObjectType.Point"/>.
    /// </summary>
    public class PointTheoremObject : BaseTheoremObject
    {
        #region Public abstract properties implementation

        /// <summary>
        /// The type of configuration object this theorem objects represents.
        /// </summary>
        public override ConfigurationObjectType Type => ConfigurationObjectType.Point;

        #endregion

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
        public override TheoremObject Remap(IReadOnlyDictionary<ConfigurationObject, ConfigurationObject> mapping)
        {
            // Reuse the static helper from the base class    
            var mappedPoint = Map(ConfigurationObject, mapping);

            // If it's incorrect, return null
            if (mappedPoint == null)
                return null;

            // Otherwise construct the object
            return new PointTheoremObject(mappedPoint);
        }

        /// <summary>
        /// Gets the configuration objects that internally define this theorem object.
        /// </summary>
        /// <returns>The enumerable of the internal configuration objects.</returns>
        public override IEnumerable<ConfigurationObject> GetInnerConfigurationObjects() => new[] { ConfigurationObject };

        #endregion

        #region HashCode and Equals

        /// <summary>
        /// Gets the hash code of this object.
        /// </summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode() => ConfigurationObject.GetHashCode();

        /// <summary>
        /// Finds out if a passed object is equal to this one.
        /// </summary>
        /// <param name="otherObject">The passed object.</param>
        /// <returns>true, if they are equal; false otherwise.</returns>
        public override bool Equals(object otherObject)
        {
            // Either the references are equals
            return this == otherObject
                // Or the object is not null
                || otherObject != null
                // And is a point object
                && otherObject is PointTheoremObject point
                // And their objects are equal
                && point.ConfigurationObject.Equals(ConfigurationObject);
        }

        #endregion

        #region Debug-only to string

#if DEBUG

        /// <summary>
        /// Converts the theorem point object to a string. 
        /// </summary>
        /// <returns>A human-readable string representation of the configuration.</returns>
        public override string ToString() => ConfigurationObject.Id.ToString();

#endif

        #endregion
    }
}
