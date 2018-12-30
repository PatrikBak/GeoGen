using GeoGen.Core;
using System;

namespace GeoGen.Analyzer
{
    /// <summary>
    /// Represents a base class for a geometric object that can directly correspond to a
    /// <see cref="Core.ConfigurationObject"/>, but doesn't necessary have to and also can be 
    /// defined implicitly by <see cref="PointObject"/>s (see <see cref="DefinableByPoints"/>).
    /// </summary>
    public abstract class GeometricalObject
    {
        #region Public properties

        /// <summary>
        /// Gets the configuration object that represents this object. It might
        /// be null, if this is a line or a circle and its defined by points.
        /// </summary>
        public ConfigurationObject ConfigurationObject { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initialize a new instance of the <see cref="GeometricalObject"/> class wrapping a given <see cref="Core.ConfigurationObject"/>.
        /// </summary>
        /// <param name="configurationObject">The configuration object represented by this geometrical object.</param>
        protected GeometricalObject(ConfigurationObject configurationObject)
        {
            ConfigurationObject = configurationObject;
        }

        #endregion
    }
}