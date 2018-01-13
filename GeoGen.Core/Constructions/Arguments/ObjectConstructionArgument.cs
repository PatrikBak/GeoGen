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
        /// Gets the object that this argument wraps.
        /// </summary>
        public ConfigurationObject PassedObject { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="passedObject">The passed configuration object.</param>
        public ObjectConstructionArgument(ConfigurationObject passedObject)
        {
            PassedObject = passedObject ?? throw new ArgumentNullException(nameof(passedObject));
        }

        #endregion

        #region Overridden methods

        /// <summary>
        /// Executes an action on the configuration objects that are contained.
        /// inside the argument.
        /// </summary>
        /// <param name="action">The action to be performed on each object.</param>
        public override void Visit(Action<ConfigurationObject> action)
        {
            action(PassedObject);
        }

        #endregion
    }
}