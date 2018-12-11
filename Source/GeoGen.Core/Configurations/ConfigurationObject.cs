namespace GeoGen.Core
{
    /// <summary>
    /// Represents an object of a <see cref="Configuration"/>. 
    /// </summary>
    public abstract class ConfigurationObject
    {
        #region Private fields

        /// <summary>
        /// The backing field for the <see cref="Id"/> property.
        /// </summary>
        private int? _id;

        #endregion

        #region Public properties

        /// <summary>
        /// Gets or sets the id of this configuration object. The id should be set only once.
        /// Setting it more than once, or accesing it when it's not set, causes a <see cref="GeoGenException"/>. 
        /// </summary>
        public int Id
        {
            get => _id ?? throw new GeoGenException("The id of this object hasn't been set yet.");
            set => _id = !_id.HasValue ? value : throw new GeoGenException("The id of this object has been already set and cannot be changed.");
        }
        
        /// <summary>
        /// Indicates if this object is identified, i.e. if the <see cref="Id"/> property has been set.
        /// </summary>
        public bool HasId => _id.HasValue;

        #endregion

        #region Public abstract properties

        /// <summary>
        /// Gets the actual geometrical type of this object (such as Point, Line...)
        /// </summary>
        public abstract ConfigurationObjectType ObjectType { get; }

        #endregion
    }
}