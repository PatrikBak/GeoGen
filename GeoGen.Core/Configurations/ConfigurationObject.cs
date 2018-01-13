using System;

namespace GeoGen.Core
{
    /// <summary>
    /// Represents an object of a <see cref="Configuration"/>. 
    /// </summary>
    public abstract class ConfigurationObject
    {
        #region Public properties

        /// <summary>
        /// Gets or sets the id of this configuration object. The id should be
        /// unique solely during the generation process. It will be reseted every time
        /// the process starts over.
        /// </summary>
        public int? Id { get; set; }

        #endregion

        #region Public abstract properties

        /// <summary>
        /// Gets the actual geometrical type of this object (such as Point, Line...)
        /// </summary>
        public abstract ConfigurationObjectType ObjectType { get; }

        #endregion

        #region Abstract methods

        /// <summary>
        /// Executes an action on the configuration objects that are used to define this one
        /// (including this one). The action might get call for the same object more than once.
        /// </summary>
        /// <param name="action">The action to be performed on each object.</param>
        public abstract void Visit(Action<ConfigurationObject> action);

        #endregion
    }
}