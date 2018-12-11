using GeoGen.Utilities;
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
        #region Public properties

        /// <summary>
        /// Gets or sets the id of this configuration. The id is used by
        /// some generator and analyzer services to perform caching. 
        /// </summary>
        public int? Id { get; set; }

        #endregion

        #region Private fields

        /// <summary>
        /// The lazy objects map initializer.
        /// </summary>
        private readonly Lazy<ConfigurationObjectsMap> _objectsMapInitialzer;

        #endregion

        #region Public properties

        /// <summary>
        /// Gets the loose configuration objects wrapped in a holder.
        /// </summary>
        public LooseObjectsHolder LooseObjectsHolder { get; }

        /// <summary>
        /// Gets the list of loose configuration objects within this configuration.
        /// </summary>
        public IReadOnlyList<LooseConfigurationObject> LooseObjects => LooseObjectsHolder.LooseObjects;

        /// <summary>
        /// Gets or sets the layout of the loose objects of the configuration. This value doesn't have
        /// to be set, then loose objects are constructed randomly. But if it's set, it
        /// must logically correspond to the loose objects property. 
        /// </summary>
        public LooseObjectsLayout? LooseObjectsLayout
        {
            get => LooseObjectsHolder.Layout;
            set => LooseObjectsHolder.Layout = value;
        }

        /// <summary>
        /// Gets the list of constructed configuration objects within this configuration. 
        /// They're supposed be ordered so that it's possible to construct them in that order.
        /// </summary>
        public IReadOnlyList<ConstructedConfigurationObject> ConstructedObjects { get; }

        /// <summary>
        /// Gets the list of the last added objects to the configuration.
        /// </summary>
        public IReadOnlyList<ConstructedConfigurationObject> LastAddedObjects { get; }

        /// <summary>
        /// Gets the configuration objects map of this configuration.
        /// </summary>
        public ConfigurationObjectsMap ObjectsMap => _objectsMapInitialzer.Value;

        /// <summary>
        /// Gets the number of objects of this configuration
        /// </summary>
        public int NumberOfObjects => LooseObjects.Count + ConstructedObjects.Count;

        /// <summary>
        /// Creates a set of the ids of the objects of this configuration.
        /// </summary>
        /// <returns></returns>
        public HashSet<int> ObjectsIds()
        {
            // Cast all objects to the set of their ids
            return ObjectsMap.AllObjects.Select(obj => obj.Id).ToSet();
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="looseObjects">The list of loose configuration objects.</param>
        /// <param name="constructedObjects">The list of constructed configuration objects.</param>
        /// <param name="layout">The layout of loose objects.</param>
        public Configuration(IReadOnlyList<LooseConfigurationObject> looseObjects, IReadOnlyList<ConstructedConfigurationObject> constructedObjects, LooseObjectsLayout? layout = null)
                : this(new LooseObjectsHolder(looseObjects, layout), constructedObjects)
        {
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="looseObjectsHolder">The loose objects holder.</param>
        /// <param name="constructedObjects">The constructed objects.</param>
        public Configuration(LooseObjectsHolder looseObjectsHolder, IReadOnlyList<ConstructedConfigurationObject> constructedObjects)
        {
            LooseObjectsHolder = looseObjectsHolder;
            ConstructedObjects = constructedObjects ?? throw new ArgumentNullException(nameof(constructedObjects));

            // Prepare the initializer
            _objectsMapInitialzer = new Lazy<ConfigurationObjectsMap>(() =>
            {
                var allObjects = looseObjectsHolder.LooseObjects.Cast<ConfigurationObject>().Concat(constructedObjects);

                return new ConfigurationObjectsMap(allObjects);
            });

            // Set the last added objects
            LastAddedObjects = GroupConstructedObjects().LastOrDefault();
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Groups constructed objects into lists so that each list represents
        /// the output of a single construction. If we use only constructions with
        /// one output, then each list will have the size 1. 
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
                yield return new List<ConstructedConfigurationObject>(currentObjects);

                // Clean it and therefore prepare for new objects
                currentObjects.Clear();
            }
        }

        /// <summary>
        /// Creates a configuration with the same loose objects and its layout, and the
        /// constructed object that consist from the current constructed objects and
        /// given new objects.
        /// </summary>
        /// <param name="newObjects">The new objects list.</param>
        /// <returns>The new configuration.</returns>
        public Configuration Derive(List<ConstructedConfigurationObject> newObjects)
        {
            // Prepare new constructed objects
            var constructedObjects = ConstructedObjects.ToList().Concat(newObjects).ToList();

            // Return a new configuration
            return new Configuration(LooseObjectsHolder, constructedObjects);
        }

        #endregion
    }
}