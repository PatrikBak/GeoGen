namespace GeoGen.Core.Configurations
{
    /// <summary>
    /// Represents an object that can be contained in a <see cref="Configuration"/>.
    /// </summary>
    public abstract class ConfigurationObject
    {
        #region Public properties

        /// <summary>
        /// Gets or sets the ID of this configuration object. 
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets the actual geometrical type of this object (such as Point, Line...)
        /// </summary>
        public ConfigurationObjectType ObjectType { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructs a new configuration object of a given type (such as Point, Line...)
        /// </summary>
        /// <param name="objectType">The configuration object type</param>
        protected ConfigurationObject(ConfigurationObjectType objectType)
        {
            ObjectType = objectType;
        }

        #endregion
    }
}