using System.Collections.Generic;

namespace GeoGen.Core
{
    /// <summary>
    /// Represents an object of a <see cref="Configuration"/>. 
    /// </summary>
    public abstract class ConfigurationObject 
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

        #region To String

        /// <summary>
        /// Converts a given object to a string. 
        /// NOTE: This method is used only for debugging purposes.
        /// </summary>
        /// <returns>A human-readable string representation of the object.</returns>
        public override string ToString() => ToStringHelper.ObjectToString(this);

        #endregion
    }
}