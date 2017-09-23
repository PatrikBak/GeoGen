using GeoGen.Core.Configurations;

namespace GeoGen.Core.Constructions.Parameters
{
    /// <summary>
    /// Represent a <see cref="ConstructionParameter"/> that is a simple configuration object (such as Point, Line...). 
    /// It is defined by a value of <see cref="ConfigurationObjectType"/>, since it holds a definition, not a value.
    /// </summary>
    public class ObjectConstructionParameter : ConstructionParameter
    {
        #region Public properties

        /// <summary>
        /// Gets the expected type of a parameter.
        /// </summary>
        public ConfigurationObjectType ExpectedType { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Construct a new object construction parameter wrapping a configuration object type.
        /// </summary>
        /// <param name="expectedType">The configuration object type.</param>
        public ObjectConstructionParameter(ConfigurationObjectType expectedType)
        {
            ExpectedType = expectedType;
        }

        #endregion
    }
}