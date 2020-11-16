using GeoGen.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.Core
{
    /// <summary>
    /// Represents a line segment defined by two <see cref="PointTheoremObject"/>s.
    /// </summary>
    public class LineSegmentTheoremObject : TheoremObject
    {
        #region Public properties

        /// <summary>
        /// The first point of this pair.
        /// </summary>
        public PointTheoremObject Point1 { get; }

        /// <summary>
        /// The second point of this pair.
        /// </summary>
        public PointTheoremObject Point2 { get; }

        /// <summary>
        /// The points of the segment as a set.
        /// </summary>
        public IReadOnlyHashSet<PointTheoremObject> PointSet { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LineSegmentTheoremObject"/> class.
        /// </summary>
        /// <param name="point1">The first point of the line segment.</param>
        /// <param name="point2">The second line of the line segment.</param>
        public LineSegmentTheoremObject(PointTheoremObject point1, PointTheoremObject point2)
        {
            // Set the points
            Point1 = point1 ?? throw new ArgumentNullException(nameof(point1));
            Point2 = point2 ?? throw new ArgumentNullException(nameof(point2));

            // Create the point set
            PointSet = new HashSet<PointTheoremObject> { Point1, Point2 }.AsReadOnly();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LineSegmentTheoremObject"/> class.
        /// </summary>
        /// <param name="point1">The first point of the line segment.</param>
        /// <param name="point2">The second line of the line segment.</param>
        public LineSegmentTheoremObject(ConfigurationObject point1, ConfigurationObject point2)
            : this(new PointTheoremObject(point1), new PointTheoremObject(point2))
        {
        }

        #endregion

        #region Public abstract methods implementation

        /// <inheritdoc/>
        public override IEnumerable<ConfigurationObject> GetInnerConfigurationObjects()
            // Return the merged inner objects
            => Point1.GetInnerConfigurationObjects().Concat(Point2.GetInnerConfigurationObjects());

        /// <inheritdoc/>
        public override LineSegmentTheoremObject Remap(IReadOnlyDictionary<ConfigurationObject, ConfigurationObject> mapping, bool flattenObjectsFromPoints = false)
        {
            // Map particular objects
            var mappedPoint1 = Point1.Remap(mapping, flattenObjectsFromPoints);
            var mappedPoint2 = Point2.Remap(mapping, flattenObjectsFromPoints);

            // Reconstruct based on the fact whether remapping could be done
            return mappedPoint1 != null && mappedPoint2 != null
                // If yes, do the reconstruction
                ? new LineSegmentTheoremObject(mappedPoint1, mappedPoint2)
                // If no, return null
                : null;
        }

        #endregion

        #region HashCode and Equals

        /// <inheritdoc/>
        public override int GetHashCode() => PointSet.GetHashCode();

        /// <inheritdoc/>
        public override bool Equals(object otherObject)
            // Either the references are equal
            => this == otherObject
                // Or the object is not null
                || otherObject != null
                // And it is a line segment
                && otherObject is LineSegmentTheoremObject otherSegment
                // And their point sets are equal
                && PointSet.Equals(otherSegment.PointSet);

        #endregion

        #region Debug-only to string

#if DEBUG

        /// <inheritdoc/>
        public override string ToString() => PointSet.Select(point => point.ToString()).Ordered().ToJoinedString();

#endif

        #endregion
    }
}
