using GeoGen.Core;
using GeoGen.Utilities;

namespace GeoGen.Constructor
{
    /// <summary>
    /// Represents a geometric line that can be defined by two <see cref="PointObject"/>. 
    /// It contains all the <see cref="PointObject"/>s that lie on it.
    /// </summary>
    public class LineObject : DefinableByPoints
    {
        #region DefineableByPoints properties

        /// <inheritdoc/>
        public override int NumberOfNeededPoints => 2;

        #endregion

        #region Constructors

        /// <summary>
        /// Initialize a new instance of the <see cref="LineObject"/> class.
        /// </summary>
        /// <param name="points">The points that define this line.</param>
        public LineObject(params PointObject[] points)
                : base(configurationObject: null, points)
        {
        }

        /// <summary>
        /// Initialize a new instance of the <see cref="LineObject"/> class.
        /// </summary>
        /// <param name="configurationObject">The configuration object represented by this line.</param>
        /// <param name="points">The points that define this line.</param>
        public LineObject(ConfigurationObject configurationObject, IEnumerable<PointObject> points)
                : base(configurationObject, points)
        {
        }

        /// <summary>
        /// Initialize a new instance of the <see cref="LineObject"/> class.
        /// </summary>
        /// <param name="configurationObject">The configuration object represented by this line.</param>
        public LineObject(ConfigurationObject configurationObject)
                : base(configurationObject, Array.Empty<PointObject>())
        {
        }

        #endregion

        #region Debug-only to string

#if DEBUG

        /// <inheritdoc/>
        public override string ToString()
        {
            // If there is a specific configuration object, we include it 
            var objectPart = ConfigurationObject == null ? "" : $"{ConfigurationObject.Id}";

            // If there are points, include them
            var pointsPart = Points.Any() ? $"[{Points.Select(point => point.ToString()).Ordered().ToJoinedString()}]" : "";

            // Construct the final string including the points
            return $"{objectPart}{pointsPart}";
        }

#endif

        #endregion
    }
}