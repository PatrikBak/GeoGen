using GeoGen.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.Core
{
    /// <summary>
    /// Represents a <see cref="TheoremObjectWithPoints"/> that is a circle.
    /// </summary>
    public class CircleTheoremObject : TheoremObjectWithPoints
    {
        #region Public abstract properties implementation

        /// <summary>
        /// Gets the minimal number of distinct points that are needed to define this type of object.
        /// </summary>
        public override int NumberOfNeededPoints => 3;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="CircleTheoremObject"/> class.
        /// </summary>
        /// <param name="configurationObject">The optional configuration object representing this theorem object.</param>
        /// <param name="points">The optional points that define / lie on this object.</param>
        public CircleTheoremObject(ConfigurationObject configurationObject, IEnumerable<ConfigurationObject> points = null)
            : base(configurationObject, points)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CircleTheoremObject"/> class defined by points.
        /// </summary>
        /// <param name="points">The points that define this object.</param>
        public CircleTheoremObject(params ConfigurationObject[] points) : this(null, points)
        {
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
        public override TheoremObject Remap(Dictionary<ConfigurationObject, ConfigurationObject> mapping)
        {
            // Remap object and points
            var objectPoints = RemapObjectAndPoints(mapping);

            // Remap if those could be remapped correctly
            return objectPoints != default ? new CircleTheoremObject(objectPoints.Item1, objectPoints.Item2) : null;
        }

        #endregion

        #region HashCode and Equals

        /// <summary>
        /// Gets the hash code of this object.
        /// </summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode()
        {
            // I don't think there is a better way to define it, because even if we have 
            // 2 circles with completely different points, it might still be the same circle.
            // We're working with large numbers of theorems at once, so it should fine like this.
            return "Circle".GetHashCode();
        }

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
                // And it is a circle object
                && otherObject is CircleTheoremObject circle
                // And either their configuration objects are defined and equal
                && ((ConfigurationObject != null && ConfigurationObject.Equals(circle.ConfigurationObject))
                // Or they have enough common points
                || Points.Intersect(circle.Points).Count() >= NumberOfNeededPoints);
        }

        #endregion

        #region To String

        /// <summary>
        /// Converts the circle theorem object to a string. 
        /// NOTE: This method is used only for debugging purposes.
        /// </summary>
        /// <returns>A human-readable string representation of the configuration.</returns>
        public override string ToString()
        {
            // If there is a specific configuration object, we include it 
            var objectPart = ConfigurationObject == null ? "" : $"{ConfigurationObject.Id}";

            // If there are points, include them
            var pointsPart = Points.Any() ? $"({Points.Select(p => p.Id).Ordered().ToJoinedString()})" : "";

            // Construct the final string including the points
            return $"{objectPart}{pointsPart}";
        }

        #endregion    
    }
}