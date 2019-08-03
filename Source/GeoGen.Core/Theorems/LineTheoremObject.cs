using System.Collections.Generic;

namespace GeoGen.Core
{
    /// <summary>
    /// Represents a <see cref="TheoremObjectWithPoints"/> that is a line.
    /// </summary>
    public class LineTheoremObject : TheoremObjectWithPoints
    {
        #region Public abstract properties implementation

        /// <summary>
        /// Gets the minimal number of distinct points that are needed to define this type of object.
        /// </summary>
        public override int NumberOfNeededPoints => 2;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="LineTheoremObject"/> class.
        /// </summary>
        /// <param name="configurationObject">The optional configuration object representing this theorem object.</param>
        /// <param name="points">The optional points that define / lie on this object.</param>
        public LineTheoremObject(ConfigurationObject configurationObject, IEnumerable<ConfigurationObject> points = null)
            : base(configurationObject, points)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LineTheoremObject"/> class defined by points.
        /// </summary>
        /// <param name="points">The points that define this object.</param>
        public LineTheoremObject(params ConfigurationObject[] points) : this(null, points)
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
            return objectPoints != default ? new LineTheoremObject(objectPoints.Item1, objectPoints.Item2) : null;
        }

        #endregion

        #region To String

        /// <summary>
        /// Converts the circle to a string. 
        /// NOTE: This method is used only for debugging purposes.
        /// </summary>
        /// <returns>A human-readable string representation of the configuration.</returns>
        public override string ToString() => $"Line {base.ToString()}";

        #endregion
    }
}
