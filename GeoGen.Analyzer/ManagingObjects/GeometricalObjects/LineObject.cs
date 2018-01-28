using System;
using System.Collections.Generic;
using GeoGen.Core;

namespace GeoGen.Analyzer
{
    /// <summary>
    /// Represents a geometrical line that is aware of the points on it.
    /// </summary>
    internal class LineObject : DefinableByPoints
    {
        #region DefineableByPoints properties

        /// <summary>
        /// Gets the minimal number of distinct points that are needed to define the object.
        /// </summary>
        public override int NumberOfNeededPoints { get; } = 2;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor used for when we simply wrap a given configuration object.
        /// </summary>
        /// <param name="id">The id of the object.</param>
        /// <param name="configurationObject">The configuration object.</param>
        public LineObject(int id, ConfigurationObject configurationObject)
                : base(id, configurationObject)
        {
        }

        /// <summary>
        /// Constructor used when we define object by points.
        /// </summary>
        /// <param name="id">The id of the object.</param>
        /// <param name="points">The points.</param>
        public LineObject(int id, params PointObject[] points)
                : base(id, points)
        {
        } 

        #endregion
    }
}