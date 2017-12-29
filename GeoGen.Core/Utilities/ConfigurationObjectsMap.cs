using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Core.Configurations;

namespace GeoGen.Core.Utilities
{
    /// <summary>
    /// Represents a dictionary mapping configuration objects types to
    /// lists of objects of that type. 
    /// </summary>
    public sealed class ConfigurationObjectsMap : Dictionary<ConfigurationObjectType, IReadOnlyList<ConfigurationObject>>
    {
        #region Private fields

        private readonly Lazy<IReadOnlyList<ConfigurationObject>> _allObjectsInitializer;

        #endregion

        #region Public properties

        public IReadOnlyList<ConfigurationObject> AllObjects => _allObjectsInitializer.Value;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructs a configuration objects map from a dictionary
        /// mapping object types to objects of that type.
        /// </summary>
        /// <param name="objects"></param>
        public ConfigurationObjectsMap(IDictionary<ConfigurationObjectType, List<ConfigurationObject>> objects)
        {
            if (objects == null)
                throw new ArgumentNullException(nameof(objects));

            foreach (var pair in objects)
            {
                Add(pair.Key, pair.Value);
            }

            _allObjectsInitializer = new Lazy<IReadOnlyList<ConfigurationObject>>(FindAllObjects);
        }

        /// <summary>
        /// Constructs a configuration objects map from an enumerable
        /// of configuration objects.
        /// </summary>
        /// <param name="objects">The objects enumerable.</param>
        public ConfigurationObjectsMap(IEnumerable<ConfigurationObject> objects)
        {
            if (objects == null)
                throw new ArgumentNullException(nameof(objects));

            Initialize(objects);

            _allObjectsInitializer = new Lazy<IReadOnlyList<ConfigurationObject>>(FindAllObjects);
        }

        /// <summary>
        /// Constructs a configuration objects map from a given configuration.
        /// </summary>
        /// <param name="configuraton">The configuration.</param>
        public ConfigurationObjectsMap(Configuration configuraton)
        {
            if (configuraton == null)
                throw new ArgumentNullException(nameof(configuraton));

            var objects = configuraton.LooseObjects
                                      .Cast<ConfigurationObject>()
                                      .Concat(configuraton.ConstructedObjects);

            Initialize(objects);

            _allObjectsInitializer = new Lazy<IReadOnlyList<ConfigurationObject>>(FindAllObjects);
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Returns the count of objects of a given type contained
        /// in this map.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The count.</returns>
        public int CountOfType(ConfigurationObjectType type)
        {
            return !ContainsKey(type) ? 0 : this[type].Count;
        }

        /// <summary>
        /// Creates a new objects map that is the result of merging
        /// this one with a given one.
        /// </summary>
        /// <param name="map">A given objects map.</param>
        /// <returns></returns>
        public ConfigurationObjectsMap Merge(ConfigurationObjectsMap map)
        {
            if (map == null)
                throw new ArgumentNullException(nameof(map));

            return new ConfigurationObjectsMap(AllObjects.Concat(map.AllObjects));
        }

        #endregion

        #region Indexer

        /// <summary>
        /// Gets all configuration objects with a given type, or
        /// an empty list if there is no such object.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The list of objects</returns>
        public new IReadOnlyList<ConfigurationObject> this[ConfigurationObjectType type]
        {
            get => !ContainsKey(type) ? new List<ConfigurationObject>() : base[type];
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Adds all objects from a given enumerable to the map.
        /// </summary>
        /// <param name="objects">The objects enumerable.</param>
        private void Initialize(IEnumerable<ConfigurationObject> objects)
        {
            if (objects == null)
                throw new ArgumentNullException(nameof(objects));

            foreach (var configurationObject in objects)
            {
                var type = configurationObject?.ObjectType ?? throw new ArgumentException("Null object");

                List<ConfigurationObject> list;

                if (!ContainsKey(type))
                {
                    list = new List<ConfigurationObject>();
                    Add(type, list);
                }

                list = (List<ConfigurationObject>) base[type];
                list.Add(configurationObject);
            }
        }

        private List<ConfigurationObject> FindAllObjects()
        {
            return this.SelectMany(pair => pair.Value).ToList();
        }

        #endregion
    }
}