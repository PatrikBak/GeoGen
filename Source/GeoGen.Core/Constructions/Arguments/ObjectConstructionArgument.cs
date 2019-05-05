using System;

namespace GeoGen.Core
{
    /// <summary>
    /// Represents a <see cref="ConstructionArgument"/> that wraps a single <see cref="ConfigurationObject"/>.
    /// </summary>
    public class ObjectConstructionArgument : ConstructionArgument
    {
        #region Public properties

        /// <summary>
        /// Gets the configuration object that is passed as an argument.
        /// </summary>
        public ConfigurationObject PassedObject { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectConstructionArgument"/> class.
        /// </summary>
        /// <param name="passedObject">The configuration object that is passed as an argument.</param>
        public ObjectConstructionArgument(ConfigurationObject passedObject)
        {
            PassedObject = passedObject ?? throw new ArgumentNullException(nameof(passedObject));
        }

        #endregion

        #region To String

        /// <summary>
        /// Converts the object construction argument to a string. 
        /// NOTE: This method is used only for debObjecugging purposes.
        /// </summary>
        /// <returns>A human-readable string representation of the configuration.</returns>
        public override string ToString() => PassedObject.Id.ToString();

        #endregion
    }
}