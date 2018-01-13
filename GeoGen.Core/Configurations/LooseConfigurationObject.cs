using System;

namespace GeoGen.Core
{
    /// <summary>
    /// Represents a <see cref="ConfigurationObject"/> that is meant to be a general independent object
    /// that is passed within <see cref="ConstructionArgument"/>s to create more complex objects of the type
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
        /// Default constructor.
        /// </summary>
        /// <param name="objectType">The geometrical type of this object.</param>
        public LooseConfigurationObject(ConfigurationObjectType objectType)
        {
            ObjectType = objectType;
        }

        #endregion

        #region Overridden methods

        /// <summary>
        /// Executes an action on the configuration objects that are used to define this one
        /// (including this one). The action might get call for the same object more than once.
        /// </summary>
        /// <param name="action">The action to be performed on each object.</param>
        public override void Visit(Action<ConfigurationObject> action)
        {
            // Call the action on this object
            action(this);
        }

        #endregion
    }
}