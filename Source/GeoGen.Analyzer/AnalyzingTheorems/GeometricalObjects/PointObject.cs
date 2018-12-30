using GeoGen.Core;
using System.Collections.Generic;

namespace GeoGen.Analyzer
{
    /// <summary>
    /// Represents a geometrical point that is defined by a point <see cref="ConfigurationObject"/>.
    /// It contains all <see cref="LineObject"/>s and <see cref="CircleObject"/>s that pass through it.
    /// </summary>
    public class PointObject : GeometricalObject
    {
        #region Public properties

        /// <summary>
        /// Gets all the lines passing through this point.
        /// </summary>
        public HashSet<LineObject> Lines { get; } = new HashSet<LineObject>();

        /// <summary>
        /// Gets all the circles passing through this point.
        /// </summary>
        public HashSet<CircleObject> Circles { get; } = new HashSet<CircleObject>();

        #endregion

        #region Constructor

        /// <summary>
        /// Initialize a new instance of the <see cref="PointObject"/> class wrapping a given point <see cref="ConfigurationObject"/>.
        /// </summary>
        /// <param name="configurationObject">The point configuration object represented by this geometrical object.</param>
        public PointObject(ConfigurationObject configurationObject)
                : base(configurationObject)
        {
        }

        #endregion
    }
}