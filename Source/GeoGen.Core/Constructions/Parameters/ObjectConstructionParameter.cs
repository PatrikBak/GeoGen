namespace GeoGen.Core
{
    /// <summary>
    /// Represent a <see cref="ConstructionParameter"/> that represents a configuration 
    /// object of some type, defined by a <see cref="ConfigurationObjectType"/>. This parameter
    /// corresponds to an <see cref="ObjectConstructionArgument"/>.
    /// </summary>
    public class ObjectConstructionParameter : ConstructionParameter
    {
        #region Public properties

        /// <summary>
        /// Gets the object type that this parameter represents.
        /// </summary>
        public ConfigurationObjectType ObjectType { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectConstructionParameter"/> class.
        /// </summary>
        /// <param name="expectedType">The object type represented by this parameter.</param>
        public ObjectConstructionParameter(ConfigurationObjectType expectedType)
        {
            ObjectType = expectedType;
        }

        #endregion
    }
}