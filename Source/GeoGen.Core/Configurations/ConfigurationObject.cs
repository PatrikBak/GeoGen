using System;
using System.Diagnostics;
using System.Linq;

namespace GeoGen.Core
{
    /// <summary>
    /// Represents an object of a <see cref="Configuration"/>. 
    /// </summary>
    public abstract class ConfigurationObject
    {
        #region Debugging code

        /// <summary>
        /// The id of the last globally created object. 
        /// </summary>
        private static int _id;

        /// <summary>
        /// The lock for incrementing and setting the global id.
        /// </summary>
        private static readonly object _lock = new object();

        /// <summary>
        /// Gets the id of the configuration object.
        /// This property should be used only for diagnostic purposes.
        /// </summary>
        internal int Id { get; private set; }

        /// <summary>
        /// Sets up the id of the object in a thread-safe manner.
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

            // Setup Id only when we're debugging
            if (Debugger.IsAttached)
                SetupId();
        }

        #endregion

        #region To String

        /// <summary>
        /// Converts the configuration object to a string. 
        /// NOTE: This method is used only for debugging purposes.
        /// </summary>
        /// <returns>A human-readable string representation of the object.</returns>
        public override string ToString()
        {
            // Find the list of objects that define this one
            var definingObjects = this.GetDefiningObjects();

            // Separate the loose and the constructed ones
            var looseObjects = definingObjects.OfType<LooseConfigurationObject>().ToList();
            var constructedObjects = definingObjects.OfType<ConstructedConfigurationObject>().ToList();

            // Create a configuration simulating a definition of the object
            var configuration = new Configuration(new LooseObjectsHolder(looseObjects), constructedObjects);

            // And use the configuration converter
            return $"{Id} (Full definition: {configuration})";
        }

        #endregion
    }
}