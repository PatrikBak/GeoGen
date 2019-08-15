using GeoGen.Core;
using GeoGen.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.Constructor
{
    /// <summary>
    /// Represents a <see cref="GeometricObject"/> that can be defined by <see cref="PointObject"/>s.
    /// </summary>
    public abstract class DefinableByPoints : GeometricObject
    {
        #region Private fields

        /// <summary>
        /// The points that lie on this object.
        /// </summary>
        private readonly HashSet<PointObject> _points;

        #endregion

        #region Public properties

        /// <summary>
        /// Gets the points that lie on this object.
        /// </summary>
        public IReadOnlyCollection<PointObject> Points => _points;

        #endregion

        #region Internal methods

        /// <summary>
        /// Adds the points to the object.
        /// </summary>
        /// <param name="point">The point.</param>
        internal void AddPoint(PointObject point) => _points.Add(point);

        #endregion

        #region Public abstract properties

        /// <summary>
        /// Gets the minimal number of distinct points that are needed to define this type of object.
        /// </summary>
        public abstract int NumberOfNeededPoints { get; }

        #endregion

        #region Public methods

        /// <summary>
        /// Returns the common points that this object has in common with a given one.
        /// </summary>
        /// <param name="other">The given object.</param>
        /// <returns>A lazy enumerable of common points.</returns>
        public IEnumerable<PointObject> CommonPointsWith(DefinableByPoints other) => Points.Intersect(other.Points);

        #endregion

        #region Constructor

        /// <summary>
        /// Initialize a new instance of the <see cref="DefinableByPoints"/> class.
        /// </summary>
        /// <param name="configurationObject">The configuration object represented by this geometric object.</param>
        /// <param name="points">The points that define this object.</param>
        protected DefinableByPoints(ConfigurationObject configurationObject, IEnumerable<PointObject> points)
                : base(configurationObject)
        {
            _points = new HashSet<PointObject>(points);
        }

        #endregion

        #region To String

        /// <summary>
        /// Converts a given object to a string. 
        /// NOTE: This method is used only for debugging purposes.
        /// </summary>
        /// <returns>A human-readable string representation of the object.</returns>
        public override string ToString()
        {
            // Create the configuration object part, if there is any
            var configurationObject = ConfigurationObject == null ? "" : $"{ConfigurationObject} ";

            // Concatenate it with points
            return $"{configurationObject}Points: {Points.ToJoinedString("; ")}";
        }

        #endregion
    }
}