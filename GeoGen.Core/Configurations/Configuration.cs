using System;
using System.Collections.Generic;
using GeoGen.Core.Utilities;

namespace GeoGen.Core.Configurations
{
    /// <summary>
    /// Represents a configuration of geometrical objects. It consists of 
    /// a list of LooseConfigurationObjects and a list of ConstructedConfigurationObjects.
    /// The constructed objects are supposed to be ordered so that it's possible to construct them in that order.
    /// The order of the loose objects is important when the configuration defines composed constructions.
    /// </summary>
    public sealed class Configuration
    {
        #region Private fields

        /// <summary>
        /// The lazy configuration objects map initializer.
        /// </summary>
        private readonly Lazy<ConfigurationObjectsMap> _objectsMapInitialzer;

        #endregion

        #region Public properties

        /// <summary>
        /// Gets the list of loose configuration objects within this configuration.
        /// </summary>
        public List<LooseConfigurationObject> LooseObjects { get; }

        /// <summary>
        /// Gets the list of constructed configuration objects within this configuration. 
        /// They're supposed be ordered so that it's possible to construct them in that order.
        /// </summary>
        public List<ConstructedConfigurationObject> ConstructedObjects { get; }

        /// <summary>
        /// Gets the configuration objects map of this configuration.
        /// </summary>
        public ConfigurationObjectsMap ObjectsMap => _objectsMapInitialzer.Value;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructs a new configuration consisting of given configuration objects. 
        /// </summary>
        /// <param name="looseObjects">The list of loose configuration objects.</param>
        /// <param name="constructedObjects">The list constructed configuration objects.</param>
        public Configuration(List<LooseConfigurationObject> looseObjects, List<ConstructedConfigurationObject> constructedObjects)
        {
            LooseObjects = looseObjects ?? throw new ArgumentNullException(nameof(looseObjects));
            ConstructedObjects = constructedObjects ?? throw new ArgumentNullException(nameof(constructedObjects));
            _objectsMapInitialzer = new Lazy<ConfigurationObjectsMap>(() => new ConfigurationObjectsMap(this));
        }

        #endregion
        
        public IEnumerable<List<ConstructedConfigurationObject>> GroupConstructedObjects()
        {
            var currentObjects = new List<ConstructedConfigurationObject>();

            bool ShouldBeYielded() => currentObjects.Count == currentObjects[0].Construction.OutputTypes.Count;

            foreach (var constructedObject in ConstructedObjects)
            {
                currentObjects.Add(constructedObject);

                if (!ShouldBeYielded())
                    continue;

                yield return currentObjects;

                currentObjects.Clear();
            }
        }
    }
}