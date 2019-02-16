using GeoGen.Core;
using System.Collections.Generic;

namespace GeoGen.Constructor
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

        #region To String

        /// <summary>
        /// Converts a given point to a string. 
        /// NOTE: This method id used only for debugging purposes.
        /// </summary>
        /// <returns>A human-readable string representation of the point.</returns>
        public override string ToString() => ConfigurationObject.ToString();

        #endregion
    }
}