using GeoGen.Core;

namespace GeoGen.Constructor
{
    /// <summary>
    /// Represents a base class for a geometric object that can directly correspond to a
    /// <see cref="Core.ConfigurationObject"/>.
    /// </summary>
    public abstract class GeometricObject
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
        /// Initialize a new instance of the <see cref="GeometricObject"/> class wrapping a given <see cref="Core.ConfigurationObject"/>.
        /// </summary>
        /// <param name="configurationObject">The configuration object represented by this geometric object.</param>
        protected GeometricObject(ConfigurationObject configurationObject)
        {
            ConfigurationObject = configurationObject;
        }

        #endregion
    }
}