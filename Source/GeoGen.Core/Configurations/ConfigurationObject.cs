using System.Collections.Generic;

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

        #region Public abstract methods

        /// <summary>
        /// Enumerates the objects that are internally used to define this configuration object.
        /// </summary>
        /// <returns>The enumeration of the internal objects.</returns>
        public abstract IEnumerable<ConfigurationObject> GetInternalObjects();

        #endregion

        #region To String

        /// <summary>
        /// Converts a given object to a string. 
        /// NOTE: This method id used only for debugging purposes.
        /// </summary>
        /// <returns>A human-readable string representation of the object.</returns>
        public override string ToString() => ToStringHelper.ObjectToString(this);

        #endregion
    }
}