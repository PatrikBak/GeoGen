using GeoGen.Utilities;
using System.Collections.Generic;

namespace GeoGen.Core
{
    /// <summary>
    /// Represents a <see cref="TheoremObject"/> that has the type <see cref="ConfigurationObjectType.Point"/>.
    /// </summary>
    public class PointTheoremObject : BaseTheoremObject
    {
        #region Public abstract properties implementation

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public override TheoremObject Remap(IReadOnlyDictionary<ConfigurationObject, ConfigurationObject> mapping, bool flattenObjectsFromPoints = false)
        {
            // Reuse the static helper from the base class    
            var mappedPoint = Map(ConfigurationObject, mapping);

            // If it's incorrect, return null
            if (mappedPoint == null)
                return null;

            // Otherwise construct the object
            return new PointTheoremObject(mappedPoint);
        }
        /// <inheritdoc/>
        public override IEnumerable<ConfigurationObject> GetInnerConfigurationObjects() => ConfigurationObject.ToEnumerable();

        #endregion

        #region HashCode and Equals

        /// <inheritdoc/>
        public override int GetHashCode() => ConfigurationObject.GetHashCode();

        /// <inheritdoc/>
        public override bool Equals(object otherObject)
            // Either the references are equals
            => this == otherObject
                // Or the object is not null
                || otherObject != null
                // And is a point object
                && otherObject is PointTheoremObject point
                // And their objects are equal
                && point.ConfigurationObject.Equals(ConfigurationObject);

        #endregion

        #region Debug-only to string

#if DEBUG

        /// <inheritdoc/>
        public override string ToString() => ConfigurationObject.Id.ToString();

#endif

        #endregion
    }
}
