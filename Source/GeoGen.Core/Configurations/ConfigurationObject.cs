namespace GeoGen.Core
{
    /// <summary>
    /// Represents an object of a <see cref="Configuration"/>. It inherits from <see cref="IdentifiedObject"/>, 
    /// since some GeoGen algorithms require to work with an integer id. 
    /// </summary>
    public abstract class ConfigurationObject : IdentifiedObject
    {
        #region Public properties

        /// <summary>
        /// Gets the type of this object.
        /// </summary>
        public ConfigurationObjectType ObjectType { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationObject"/> with a given type.
        /// </summary>
        /// <param name="objectType">The type of the object.</param>
        protected ConfigurationObject(ConfigurationObjectType objectType)
        {
            ObjectType = objectType;
        }

        #endregion
    }
}