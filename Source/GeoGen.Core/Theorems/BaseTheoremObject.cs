namespace GeoGen.Core
{
    /// <summary>
    /// Represents a <see cref="TheoremObject"/> that can be defined by a <see cref="Core.ConfigurationObject"/>.
    /// </summary>
    public abstract class BaseTheoremObject : TheoremObject
    {
        #region Public properties

        /// <summary>
        /// Gets the configuration object corresponding to this object. 
        /// </summary>
        public ConfigurationObject ConfigurationObject { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="TheoremObject"/> class.
        /// </summary>
        /// <param name="configurationObject">The configuration object corresponding to this object.</param>
        protected BaseTheoremObject(ConfigurationObject configurationObject = null)
        {
            ConfigurationObject = configurationObject;
        }

        #endregion
    }
}
