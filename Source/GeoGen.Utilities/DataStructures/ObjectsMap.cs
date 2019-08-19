using System.Collections.Generic;

namespace GeoGen.Utilities
{
    /// <summary>
    /// Represents a dictionary mapping <see cref="TKey"/>s to lists of <see cref="TValue"/>s.
    /// </summary>
    public abstract class ObjectsMap<TKey, TValue> : Dictionary<TKey, IReadOnlyList<TValue>>
    {
        #region Public properties

        /// <summary>
        /// Gets the list of all objects contained in this map.
        /// </summary>
        public IReadOnlyList<TValue> AllObjects { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectsMap"/> class wrapping given objects.
        /// </summary>
        /// <param name="objects">The objects to be contained in the map.</param>
        public ObjectsMap(IEnumerable<TValue> objects)
        {
            // Prepare the list of all objects
            var allObjects = new List<TValue>();

            // Add all objects
            foreach (var @object in objects)
            {
                // Find the key for the current object
                var type = GetKey(@object);

                // Prepare the list of objects of this type
                List<TValue> list;

                // If we don't have this type yet
                if (!ContainsKey(type))
                {
                    // Initialize the list
                    list = new List<TValue>();

                    // And add to the dictionary
                    Add(type, list);
                }
                else
                {
                    // Otherwise take the list from the dictionary
                    list = (List<TValue>)base[type];
                }

                // Finally add the object to it
                list.Add(@object);

                // Add it to the list of all objects as well
                allObjects.Add(@object);
            }

            // Set the list of all objects
            AllObjects = allObjects;
        }

        #endregion

        #region Protected abstract methods

        /// <summary>
        /// Gets the key for a given value.
        /// </summary>
        /// <param name="value">The value</param>
        /// <returns>The key.</returns>
        protected abstract TKey GetKey(TValue value);

        #endregion
    }
}
