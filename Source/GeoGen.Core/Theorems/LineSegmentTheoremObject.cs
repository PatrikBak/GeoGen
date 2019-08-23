using System.Collections.Generic;

namespace GeoGen.Core
{
    /// <summary>
    /// Represents a line segment defined by two <see cref="PointTheoremObject"/>s.
    /// </summary>
    public class LineSegmentTheoremObject : PairTheoremObject
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LineSegmentTheoremObject"/> class.
        /// </summary>
        /// <param name="object1">The first point of the line segment.</param>
        /// <param name="object2">The second line of the line segment.</param>
        public LineSegmentTheoremObject(PointTheoremObject object1, PointTheoremObject object2)
            : base(object1, object2)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LineSegmentTheoremObject"/> class.
        /// </summary>
        /// <param name="object1">The first point of the line segment.</param>
        /// <param name="object2">The second line of the line segment.</param>
        public LineSegmentTheoremObject(ConfigurationObject point1, ConfigurationObject point2)
            : this(new PointTheoremObject(point1), new PointTheoremObject(point2))
        {
        }

        #endregion

        #region Remap implementation

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
            // Remap the objects
            var objects = RemapObjects(mapping);

            // Reconstruct based on the fact whether remapping could be done
            return objects != default ? new LineSegmentTheoremObject((PointTheoremObject)objects.Item1, (PointTheoremObject)objects.Item2) : null;
        }

        #endregion

        #region HashCode and Equals

        /// <summary>
        /// Gets the hash code of this object.
        /// </summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode()
        {
            // I don't think there is a better way to define it, see the comment to the 
            // GetHashCode method in the LineTheoremObject or CircleTheoremObject classes
            return "LineSegment".GetHashCode();
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
                // And it is a line segment object
                && otherObject is LineSegmentTheoremObject lineSegment
                // And either the first and second object are equal
                && ((Object1.Equals(lineSegment.Object1) && Object2.Equals(lineSegment.Object2))
                // Or the first one is equal to the second and vice versa
                || (Object1.Equals(lineSegment.Object2) && Object2.Equals(lineSegment.Object1)));
        }

        #endregion
    }
}
