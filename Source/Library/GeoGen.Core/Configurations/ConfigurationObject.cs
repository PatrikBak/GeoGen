using System.Collections.Generic;

namespace GeoGen.Core
{
    /// <summary>
    /// Represents an object of a <see cref="Configuration"/>. 
    /// </summary>
    public abstract class ConfigurationObject
    {
        #region Debug-only code

#if DEBUG

        /// <summary>
        /// The id of the last globally created object. 
        /// </summary>
        private static int _id;

        /// <summary>
        /// The lock for setting the global id.
        /// </summary>
        private static readonly object _lock = new object();

        /// <summary>
        /// The global id of the configuration object.
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// Sets up the global id of the object in a thread-safe manner.
        /// </summary>
        private void SetupId()
        {
            // Lock so that two threads won't set the same id.
            lock (_lock)
            {
                _id++;
                Id = _id;
            }
        }

#endif
        #endregion

        #region Public properties

        /// <summary>
        /// Gets the type of this object.
        /// </summary>
        public ConfigurationObjectType ObjectType { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationObject"/> with a given type.
        /// </summary>
        /// <param name="objectType">The type of the object.</param>
        protected ConfigurationObject(ConfigurationObjectType objectType)
        {
            ObjectType = objectType;

#if DEBUG
            // Setup the id in the debug mode
            SetupId();
#endif
        }

        #endregion

        #region Public abstract methods

        /// <summary>
        /// Recreates the object using a given mapping of loose objects.
        /// </summary>
        /// <param name="mapping">The mapping of the loose objects.</param>
        /// <returns>The remapped object.</returns>
        public abstract ConfigurationObject Remap(IReadOnlyDictionary<LooseConfigurationObject, LooseConfigurationObject> mapping);

        #endregion
    }
}