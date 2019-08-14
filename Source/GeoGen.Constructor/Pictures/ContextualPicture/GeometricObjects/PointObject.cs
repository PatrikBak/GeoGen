using GeoGen.Core;
using System.Collections.Generic;

namespace GeoGen.Constructor
{
    /// <summary>
    /// Represents a geometric point that is defined by a point <see cref="ConfigurationObject"/>.
    /// It contains all <see cref="LineObject"/>s and <see cref="CircleObject"/>s that pass through it.
    /// </summary>
    public class PointObject : GeometricObject
    {
        #region Private fields

        /// <summary>
        /// The lines passing through this point.
        /// </summary>
        private readonly HashSet<LineObject> _lines = new HashSet<LineObject>();

        /// <summary>
        /// The circles passing through this point.
        /// </summary>
        private readonly HashSet<CircleObject> _circles = new HashSet<CircleObject>();

        #endregion

        #region Public properties

        /// <summary>
        /// Gets all the lines passing through this point.
        /// </summary>
        public IReadOnlyCollection<LineObject> Lines => _lines;

        /// <summary>
        /// Gets all the circles passing through this point.
        /// </summary>
        public IReadOnlyCollection<CircleObject> Circles => _circles;

        #endregion

        #region Constructor

        /// <summary>
        /// Initialize a new instance of the <see cref="PointObject"/> class wrapping a given point <see cref="ConfigurationObject"/>.
        /// </summary>
        /// <param name="configurationObject">The point configuration object represented by this geometric object.</param>
        public PointObject(ConfigurationObject configurationObject)
                : base(configurationObject)
        {
        }

        #endregion

        #region Internal methods

        /// <summary>
        /// Adds a line to the collection of lines passing through this point.
        /// </summary>
        /// <param name="line">The passing line</param>
        internal void AddLine(LineObject line) => _lines.Add(line);

        /// <summary>
        /// Adds a circle to the collection of circles passing through this point.
        /// </summary>
        /// <param name="circle">The passing circle</param>
        internal void AddCircle(CircleObject circle) => _circles.Add(circle);

        #endregion

        #region To String

        /// <summary>
        /// Converts a given point to a string. 
        /// NOTE: This method is used only for debugging purposes.
        /// </summary>
        /// <returns>A human-readable string representation of the point.</returns>
        public override string ToString() => ConfigurationObject.ToString();

        #endregion
    }
}