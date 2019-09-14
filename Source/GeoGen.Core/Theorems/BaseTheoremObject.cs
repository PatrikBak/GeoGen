namespace GeoGen.Core
{
    /// <summary>
    /// Represents a <see cref="TheoremObject"/> that can be defined by a <see cref="Core.ConfigurationObject"/>.
    /// </summary>
    public abstract class BaseTheoremObject : TheoremObject
    {
        #region Public abstract properties

        /// <summary>
        /// The type of configuration object this theorem objects represents.
        /// </summary>
        public abstract ConfigurationObjectType Type { get; }

        #endregion

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

            // Make sure the object has the right type, if it's set
            if (ConfigurationObject != null && ConfigurationObject.ObjectType != Type)
                throw new GeoGenException($"The {GetType()} should be defined by an object of type {Type}, but the type is {ConfigurationObject.ObjectType}.");
        }

        #endregion
    }
}
