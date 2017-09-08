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
        public virtual int Id { get; set; }

        #endregion

        #region Abstract properties

        /// <summary>
        /// Gets the actual geometrical type of this object (such as Point, Line...)
        /// </summary>
        public abstract ConfigurationObjectType ObjectType { get; }

        #endregion
    }
}