using System;
using GeoGen.Core.Configurations;

namespace GeoGen.Analyzer.Objects
{
    /// <summary>
    /// A base class for all geometrical objects represented in a real 
    /// coordinate system. 
    /// </summary>
    internal abstract class GeometricalObject : IEquatable<GeometricalObject>
    {
        #region Public properties

        /// <summary>
        /// Gets the configuration object associated with this geometrical object.
        /// </summary>
        public ConfigurationObject ConfigurationObject { get; set; }

        #endregion

        #region IEquatable methods

        /// <summary>
        /// Checks if a given other is equal to this one.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public abstract bool Equals(GeometricalObject other);

        #endregion
    }
}