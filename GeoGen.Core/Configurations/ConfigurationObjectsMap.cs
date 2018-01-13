using System;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.Core
{
    /// <summary>
    /// Represents a dictionary mapping configuration objects types to lists of objects of that type. 
    /// </summary>
    public class ConfigurationObjectsMap : Dictionary<ConfigurationObjectType, IReadOnlyList<ConfigurationObject>>
    {
        #region Private fields

        /// <summary>
        /// The lazy initializer of the list of all contained configuration objects.
        /// </summary>
        private readonly Lazy<IReadOnlyList<ConfigurationObject>> _allObjectsInitializer;

        #endregion

        #region Public properties

        /// <summary>
        /// Gets the list of all configuration objects contained in this map.
        /// </summary>
        public IReadOnlyList<ConfigurationObject> AllObjects => _allObjectsInitializer.Value;

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="objects">The objects enumerable.</param>
        public ConfigurationObjectsMap(IEnumerable<ConfigurationObject> objects)
        {
            // Add all objects
            foreach (var configurationObject in objects)
            {
                // Find the type of the current object
                var type = configurationObject?.ObjectType ?? throw new ArgumentException("Null object");

                // Prepare the list of objects of this type
                List<ConfigurationObject> list;

                // If we don't have this type yet
                if (!ContainsKey(type))
                {
                    // Initialize it
                    list = new List<ConfigurationObject>();

                    // And add to the dictionary
                    Add(type, list);
                }
                else
                {
                    // Otherwise take the list from the dictionary
                    list = (List<ConfigurationObject>) base[type];
                }

                // And finally add the object to it
                list.Add(configurationObject);
            }

            // Initialize the initializer
            _allObjectsInitializer = new Lazy<IReadOnlyList<ConfigurationObject>>(FindAllObjects);
        }

        /// <summary>
        /// Constructor that takes a dictionary mapping types to objects.
        /// </summary>
        /// <param name="objects">The objects dictionary.</param>
        public ConfigurationObjectsMap(IDictionary<ConfigurationObjectType, List<ConfigurationObject>> objects)
        {
            // Add all pairs directly
            foreach (var pair in objects)
            {
                Add(pair.Key, pair.Value);
            }

            // Initialize the initializer
            _allObjectsInitializer = new Lazy<IReadOnlyList<ConfigurationObject>>(FindAllObjects);
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Returns the count of objects of a given type contained in this map.
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

        #region Private helpers

        /// <summary>
        /// Finds the list of all objects contained in this map.
        /// </summary>
        /// <returns>The list of all objects</returns>
        private List<ConfigurationObject> FindAllObjects()
        {
            // Merge the objects of all types
            return this.SelectMany(pair => pair.Value).ToList();
        }

        #endregion
    }
}