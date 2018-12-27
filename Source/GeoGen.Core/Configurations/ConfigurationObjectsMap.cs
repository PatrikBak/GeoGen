using System;
using System.Collections.Generic;

namespace GeoGen.Core
{
    /// <summary>
    /// Represents a dictionary mapping <see cref="ConfigurationObjectType"/> to lists of <see cref="ConfigurationObject"/>
    /// of that type.
    /// </summary>
    public class ConfigurationObjectsMap : Dictionary<ConfigurationObjectType, IReadOnlyList<ConfigurationObject>>
    {
        #region Public properties

        /// <summary>
        /// Gets the list of all configuration objects contained in this map.
        /// </summary>
        public IReadOnlyList<ConfigurationObject> AllObjects { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationObjectsMap"/> map wrapping given objects.
        /// </summary>
        /// <param name="objects">The objects to be contained in the map.</param>
        public ConfigurationObjectsMap(IEnumerable<ConfigurationObject> objects)
        {
            // Prepare the list of all objects
            var allObjects = new List<ConfigurationObject>();

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
                    // Initialize the list
                    list = new List<ConfigurationObject>();

                    // And add to the dictionary
                    Add(type, list);
                }
                else
                {
                    // Otherwise take the list from the dictionary
                    list = (List<ConfigurationObject>) base[type];
                }

                // Finally add the object to it
                list.Add(configurationObject);

                // Add it to the list of all objects as well
                allObjects.Add(configurationObject);
            }

            // Set the list of all objects
            AllObjects = allObjects;
        }

        #endregion
    }
}