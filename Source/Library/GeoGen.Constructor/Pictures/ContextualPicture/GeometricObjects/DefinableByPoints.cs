using GeoGen.Core;
using GeoGen.Utilities;

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
        public IReadOnlyHashSet<PointObject> Points { get; }

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
            Points = _points.AsReadOnly();
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Returns the common points that this object has in common with a given one.
        /// </summary>
        /// <param name="other">The given object.</param>
        /// <returns>A lazy enumerable of common points.</returns>
        public IEnumerable<PointObject> CommonPointsWith(DefinableByPoints other) => Points.Intersect(other.Points);

        /// <summary>
        /// Finds out if this object contains all passed points.
        /// </summary>
        /// <param name="points">The points to be checked.</param>
        /// <returns>true, if it contains all the points; false otherwise.</returns>
        public bool ContainsAll(IEnumerable<PointObject> points) => _points.IsSupersetOf(points);

        #endregion
    }
}