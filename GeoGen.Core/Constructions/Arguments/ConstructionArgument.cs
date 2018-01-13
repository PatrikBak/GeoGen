using System;

namespace GeoGen.Core
{
    /// <summary>
    /// Represents a holder of <see cref="ConfigurationObject"/>(s) that is used to 
    /// create <see cref="ConstructedConfigurationObject"/>.
    /// </summary>
    public abstract class ConstructionArgument
    {
        #region Protected abstract methods

        /// <summary>
        /// Executes an action on the configuration objects that are contained.
        /// inside the argument.
        /// </summary>
        /// <param name="action">The action to be performed on each object.</param>
        public abstract void Visit(Action<ConfigurationObject> action);

        #endregion
    }
}