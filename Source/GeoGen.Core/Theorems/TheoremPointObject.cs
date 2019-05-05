namespace GeoGen.Core
{
    /// <summary>
    /// Represents a <see cref="TheoremObject"/> that has the type <see cref="ConfigurationObjectType.Point"/>.
    /// </summary>
    public class TheoremPointObject : TheoremObject
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="TheoremPointObject"/> class.
        /// </summary>
        /// <param name="configurationObject">The configuration object whose type must be <see cref="ConfigurationObjectType.Point"/>.</param>
        public TheoremPointObject(ConfigurationObject configurationObject)
            : base(configurationObject.ObjectType, configurationObject)
        {
        }

        #endregion

        #region To String

        /// <summary>
        /// Converts the theorem point object to a string. 
        /// NOTE: This method is used only for debugging purposes.
        /// </summary>
        /// <returns>A human-readable string representation of the configuration.</returns>
        public override string ToString() => $"{Type} {ConfigurationObject.Id}";

        #endregion
    }
}
