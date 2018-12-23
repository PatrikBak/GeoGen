using System;
using System.Collections.Generic;
using GeoGen.Core;

namespace GeoGen.Analyzer
{
    /// <summary>
    /// Represents a geometrical point that is aware of all lines and circles
    /// that it belongs to.
    /// </summary>
    public class PointObject : GeometricalObject
    {
        #region Public properties

        /// <summary>
        /// Gets the set of lines passing through this point.
        /// </summary>
        public HashSet<LineObject> Lines { get; } = new HashSet<LineObject>();

        /// <summary>
        /// Gets the set of circles passing through this point.
        /// </summary>
        public HashSet<CircleObject> Circles { get; } = new HashSet<CircleObject>();

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="id">The id of this point.</param>
        /// <param name="configurationObject">The configuration object wrapped by this point.</param>
        public PointObject(int id, ConfigurationObject configurationObject)
                : base(id, configurationObject)
        {
        }

        #endregion
    }
}