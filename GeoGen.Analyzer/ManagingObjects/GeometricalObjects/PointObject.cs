using System;
using System.Collections.Generic;
using GeoGen.AnalyticalGeometry;
using GeoGen.Core;

namespace GeoGen.Analyzer
{
    /// <summary>
    /// Represents a geometrical point that is aware of all lines and circles
    /// that it belongs to.
    /// </summary>
    internal class PointObject : GeometricalObject
    {
        #region Public properties

        public HashSet<LineObject> Lines { get; } = new HashSet<LineObject>();

        public HashSet<CircleObject> Circles { get; } = new HashSet<CircleObject>();

        #endregion

        #region Constructor

        public PointObject(int id, ConfigurationObject configurationObject)
                : base(id, configurationObject)
        {
        }

        #endregion

        #region Public methods

        public IEnumerable<GeometricalObject> ObjectsThatContainThisPoint(Type type)
        {
            if (type == typeof(LineObject))
                return Lines;

            if (type == typeof(CircleObject))
                return Circles;

            throw new AnalyzerException($"Can't return objects for type: {type}");
        } 

        #endregion
    }
}