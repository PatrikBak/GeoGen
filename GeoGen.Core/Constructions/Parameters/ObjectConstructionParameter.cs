namespace GeoGen.Core
{
    /// <summary>
    /// Represent a <see cref="ConstructionParameter"/> that is a simple configuration object (such as Point, Line...). 
    /// It is defined by a value of <see cref="ConfigurationObjectType"/>.
    /// </summary>
    public class ObjectConstructionParameter : ConstructionParameter
    {
        #region Public properties

        /// <summary>
        /// Gets the expected type of the parameter.
        /// </summary>
        public ConfigurationObjectType ExpectedType { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="expectedType">The object type represented by this parameter.</param>
        public ObjectConstructionParameter(ConfigurationObjectType expectedType)
        {
            ExpectedType = expectedType;
        }

        #endregion
    }
}