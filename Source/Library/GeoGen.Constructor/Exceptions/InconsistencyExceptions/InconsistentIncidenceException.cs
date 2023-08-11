using GeoGen.Core;

namespace GeoGen.Constructor
{
    /// <summary>
    /// An <see cref="InconsistentPicturesException"/> thrown when the passed point does not
    /// lie on the other passed geometric or configuration line or circle in every picture.
    /// </summary>
    public class InconsistentIncidenceException : InconsistentPicturesException
    {
        #region Public properties

        /// <summary>
        /// The point whose incidence wasn't determined consistently.
        /// </summary>
        public PointObject Point { get; }

        /// <summary>
        /// The geometric line or circle where the point doesn't lie consistently.
        /// Either this value or <see cref="LineOrCircle"/> is not null.
        /// </summary>
        public DefinableByPoints GeometricLineOrCircle { get; }

        /// <summary>
        /// The line or circle where the point doesn't lie consistently.
        /// Either this value or <see cref="GeometricLineOrCircle"/> is not null.
        /// </summary>
        public ConfigurationObject LineOrCircle { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="InconsistentIncidenceException"/> class.
        /// </summary>
        /// <param name="point">The point whose incidence wasn't determined consistently.</param>
        /// <param name="geometricLineOrCircle">The geometric line or circle where the point doesn't lie consistently.</param>
        public InconsistentIncidenceException(PointObject point, DefinableByPoints geometricLineOrCircle)
        {
            Point = point ?? throw new ArgumentNullException(nameof(point));
            GeometricLineOrCircle = geometricLineOrCircle ?? throw new ArgumentNullException(nameof(geometricLineOrCircle));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InconsistentIncidenceException"/> class.
        /// </summary>
        /// <param name="point">The point whose incidence wasn't determined consistently.</param>
        /// <param name="lineOrCircle">The line or circle where the point doesn't lie consistently.</param>
        public InconsistentIncidenceException(PointObject point, ConfigurationObject lineOrCircle)
        {
            Point = point ?? throw new ArgumentNullException(nameof(point));
            LineOrCircle = lineOrCircle ?? throw new ArgumentNullException(nameof(lineOrCircle));
        }

        #endregion
    }
}