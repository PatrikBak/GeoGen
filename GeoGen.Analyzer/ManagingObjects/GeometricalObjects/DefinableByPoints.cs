using System.Collections.Generic;
using GeoGen.Core;

namespace GeoGen.Analyzer
{
    /// <summary>
    /// Represents a <see cref="GeometricalObject"/> that can be defined by 
    /// <see cref="PointObject"/>s.
    /// </summary>
    internal abstract class DefinableByPoints : GeometricalObject
    {
        #region Public properties

        /// <summary>
        /// Gets the points of this object.
        /// </summary>
        public HashSet<PointObject> Points { get; }

        #endregion

        #region Public abstract properties

        /// <summary>
        /// Gets the minimal number of distinct points that are needed to define the object.
        /// </summary>
        public abstract int NumberOfNeededPoints { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor used for when we simply wrap a given configuration object.
        /// </summary>
        /// <param name="id">The id of the object.</param>
        /// <param name="configurationObject">The configuration object.</param>
        protected DefinableByPoints(int id, ConfigurationObject configurationObject)
                : base(id, configurationObject)
        {
        }

        /// <summary>
        /// Constructor used when we define object by points.
        /// </summary>
        /// <param name="id">The id of the object.</param>
        /// <param name="points">The points.</param>
        protected DefinableByPoints(int id, params PointObject[] points)
                : base(id, null)
        {
            Points = new HashSet<PointObject>(points);
        }

        #endregion
    }
}