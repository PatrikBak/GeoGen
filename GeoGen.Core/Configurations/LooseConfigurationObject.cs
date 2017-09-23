using GeoGen.Core.Constructions;

namespace GeoGen.Core.Configurations
{
    /// <summary>
    /// Represents a <see cref="ConfigurationObject"/> that is meant to be a general independent object
    /// that is passed as an argument to <see cref="Construction"/> to create more complex objects of type
    /// <see cref="ConstructedConfigurationObject"/>. It is defined by a <see cref="ConfigurationObjectType"/>.
    /// </summary>
    public class LooseConfigurationObject : ConfigurationObject
    {
        #region Configuration object properties

        /// <summary>
        /// Gets the actual geometrical type of this object (such as Point, Line...)
        /// </summary>
        public override ConfigurationObjectType ObjectType { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructs a new loose configuration object of a given type (such as Point, Line...)
        /// </summary>
        /// <param name="objectType">The configuration object type.</param>
        public LooseConfigurationObject(ConfigurationObjectType objectType)
        {
            ObjectType = objectType;
        }

        #endregion
    }
}