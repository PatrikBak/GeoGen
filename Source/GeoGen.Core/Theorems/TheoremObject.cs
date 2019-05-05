using System;

namespace GeoGen.Core
{
    /// <summary>
    /// Represents a geometric object of certain <see cref="ConfigurationObjectType"/> 
    /// that is be used to express a <see cref="Theorem"/>.
    /// </summary>
    public abstract class TheoremObject
    {
        #region Public properties

        /// <summary>
        /// Gets the type of the object.
        /// </summary>
        public ConfigurationObjectType Type { get; }

        /// <summary>
        /// Gets the object that directly represents this theorem object. This value
        /// can't be null for a point, but can be null for a line or a circle. 
        /// </summary>
        public ConfigurationObject ConfigurationObject { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TheoremObject"/> class.
        /// </summary>
        /// <param name="type">The type of the theorem object.</param>
        /// <param name="configurationObject">The configuration object.</param>
        protected TheoremObject(ConfigurationObjectType type, ConfigurationObject configurationObject = null)
        {
            Type = type;
            ConfigurationObject = configurationObject;

            // Make sure the types are consistent, if the configuration object is specified
            if (ConfigurationObject != null && ConfigurationObject.ObjectType != type)
                throw new ArgumentException("The type of the inner configuration object doesn't match the type of the theorem object");
        }

        #endregion
    }
}