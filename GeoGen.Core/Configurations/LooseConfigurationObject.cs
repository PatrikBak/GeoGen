namespace GeoGen.Core.Configurations
{
    /// <summary>
    /// Represents a <see cref="ConfigurationObject"/> that is meant to be a general independent object
    /// that is passed as an argument to <see cref="Constructions.Construction"/> to create more complex objects of type
    /// <see cref="ConstructedConfigurationObject"/>. It is defined by a <see cref="ConfigurationObjectType"/>.
    /// </summary>
    public class LooseConfigurationObject : ConfigurationObject
    {
        #region Constructor

        /// <summary>
        /// Constructs a new loose configuration object of a given type (such as Point, Line...)
        /// </summary>
        /// <param name="objectType">The configuration object type</param>
        public LooseConfigurationObject(ConfigurationObjectType objectType) : base(objectType)
        {
        }

        #endregion
    }
}