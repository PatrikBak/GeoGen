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
        /// <param name="flattenObjectsFromPoints">Indicates whether explicit objects LineFromPoints or Circumcircle should be made implicit.</param>
        /// <returns>The remapped theorem object, or null, if the mapping cannot be done.</returns>
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
