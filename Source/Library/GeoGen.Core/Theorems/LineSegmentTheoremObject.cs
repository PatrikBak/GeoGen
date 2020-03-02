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

        /// <inheritdoc/>
        public override TheoremObject Remap(IReadOnlyDictionary<ConfigurationObject, ConfigurationObject> mapping, bool flattenObjectsFromPoints = false)
        {
            // Remap the objects
            var objects = RemapObjects(mapping, flattenObjectsFromPoints);

            // Reconstruct based on the fact whether remapping could be done
            return objects != default ? new LineSegmentTheoremObject((PointTheoremObject)objects.Item1, (PointTheoremObject)objects.Item2) : null;
        }

        #endregion
    }
}
