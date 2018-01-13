using System;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.Core
{
    /// <summary>
    /// Represents a configuration of geometrical objects. It consists of 
    /// a list of <see cref="LooseConfigurationObject"/> and a list of 
    /// <see cref="ConstructedConfigurationObject"/>. The loose objects are 
    /// the first objects to be drawn (for example:in a triangle, they would be 
    /// 3 points). The constructed objects are supposed to be ordered so that it's
    /// possible to construct them in that order. The order of the loose objects 
    /// is important when the configuration defines a <see cref="ComposedConstruction"/>. 
    /// All objects are supposed to be mutually distinct.
    /// </summary>
    public class Configuration
    {
        #region Private fields

        /// <summary>
        /// The lazy objects map initializer.
        /// </summary>
        private readonly Lazy<ConfigurationObjectsMap> _objectsMapInitialzer;

        #endregion

        #region Public properties

        /// <summary>
        /// Gets the list of loose configuration objects within this configuration.
        /// </summary>
        public IReadOnlyList<LooseConfigurationObject> LooseObjects { get; }

        /// <summary>
        /// Gets the list of constructed configuration objects within this configuration. 
        /// They're supposed be ordered so that it's possible to construct them in that order.
        /// </summary>
        public IReadOnlyList<ConstructedConfigurationObject> ConstructedObjects { get; }

        /// <summary>
        /// Gets the configuration objects map of this configuration.
        /// </summary>
        public ConfigurationObjectsMap ObjectsMap => _objectsMapInitialzer.Value;

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="looseObjects">The list of loose configuration objects.</param>
        /// <param name="constructedObjects">The list of constructed configuration objects.</param>
        public Configuration(IReadOnlyList<LooseConfigurationObject> looseObjects, IReadOnlyList<ConstructedConfigurationObject> constructedObjects)
        {
            LooseObjects = looseObjects ?? throw new ArgumentNullException(nameof(looseObjects));
            ConstructedObjects = constructedObjects ?? throw new ArgumentNullException(nameof(constructedObjects));

            // Prepare the initializer
            _objectsMapInitialzer = new Lazy<ConfigurationObjectsMap>(() =>
            {
                var allObjects = looseObjects.Cast<ConfigurationObject>().Concat(constructedObjects);

                return new ConfigurationObjectsMap(allObjects);
            });
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Groups constructed objects into lists so that each list represents
        /// the output of a single construction. If we use only constructions with
        /// one output, then each list will have the size 1. The enumeration is lazy and
        /// returns always the same list with different objects (so in order to use the list
        /// after enumeration it's necessary to copy it).
        /// </summary>
        /// <returns>The enumerable of grouped constructed objects.</returns>
        public IEnumerable<List<ConstructedConfigurationObject>> GroupConstructedObjects()
        {
            // Prepare the result
            var currentObjects = new List<ConstructedConfigurationObject>();

            // Local function to find out if the current list has the needed number
            // of objects (according to the expected number of construction outputs).
            bool ShouldBeYielded() => currentObjects.Count == currentObjects[0].Construction.OutputTypes.Count;

            // Group objects
            foreach (var constructedObject in ConstructedObjects)
            {
                // Add the object to the list
                currentObjects.Add(constructedObject);

                // Call local function to find out if we have enough objects
                if (!ShouldBeYielded())
                    continue;

                // Yield the result
                yield return currentObjects;

                // Clean it and therefore prepare for new objects
                currentObjects.Clear();
            }
        }

        #endregion
    }
}