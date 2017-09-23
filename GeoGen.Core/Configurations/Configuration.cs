using System;
using System.Collections.Generic;
using GeoGen.Core.Utilities;

namespace GeoGen.Core.Configurations
{
    /// <summary>
    /// Represents a configuration of geometrical objects. It consists of a set of  
    /// <see cref="LooseConfigurationObject"/> and a list of <see cref="ConstructedConfigurationObject"/>. 
    /// The constructed objects are supposed to be ordered so that it's possible to construct them in that order.
    /// </summary>
    public class Configuration
    {
        #region Public properties

        /// <summary>
        /// Gets the set of loose configuration objects within this configuration.
        /// </summary>
        public HashSet<LooseConfigurationObject> LooseObjects { get; }

        /// <summary>
        /// Gets the list of constructed configuration objects within this configuration. 
        /// They're supposed be ordered so that it's possible to construct them in that order.
        /// </summary>
        public List<ConstructedConfigurationObject> ConstructedObjects { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructs a new configuration consisting of given configuration objects. 
        /// </summary>
        /// <param name="looseObjects">The hash set of loose configuration objects.</param>
        /// <param name="constructedObjects">The list constructed configuration objects.</param>
        public Configuration(HashSet<LooseConfigurationObject> looseObjects, List<ConstructedConfigurationObject> constructedObjects)
        {
            if (looseObjects == null)
                throw new ArgumentNullException(nameof(looseObjects));

            if (looseObjects.Empty())
                throw new ArgumentException("Loose objects can't be an empty set.", nameof(looseObjects));

            LooseObjects = looseObjects;
            ConstructedObjects = constructedObjects ?? throw new ArgumentNullException(nameof(constructedObjects));
        }

        #endregion
    }
}