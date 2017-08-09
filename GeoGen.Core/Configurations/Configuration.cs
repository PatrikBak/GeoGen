using System.Collections.Generic;

namespace GeoGen.Core.Configurations
{
    /// <summary>
    /// Represents a configuration of geometrical object. It consists of two sets of configuration objects, 
    /// <see cref="LooseConfigurationObject"/> and <see cref="ConstructedConfigurationObject"/>. 
    /// </summary>
    public class Configuration
    {
        #region Public properties

        /// <summary>
        /// Gets or sets the ID of this configuration. 
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets the set of loose configuration objects within this configuration.
        /// </summary>
        public HashSet<LooseConfigurationObject> LooseObjects { get; }

        /// <summary>
        /// Gets the set of constructed configuration objects within this configuration.
        /// </summary>
        public HashSet<ConstructedConfigurationObject> ConstructedObjects { get; }

        #endregion

        #region Constructor
        
        /// <summary>
        /// Constructs a new configuration consisting of given configuration objects. 
        /// </summary>
        /// <param name="looseObjects">The hash set of loose configuration objects.</param>
        /// <param name="constructedObjects">The hash set of constructed configuration objects.</param>
        public Configuration(HashSet<LooseConfigurationObject> looseObjects, HashSet<ConstructedConfigurationObject> constructedObjects)
        {
            LooseObjects = looseObjects;
            ConstructedObjects = constructedObjects;
        } 

        #endregion
    }
}