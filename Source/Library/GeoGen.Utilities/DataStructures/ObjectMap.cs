namespace GeoGen.Utilities
{
    /// <summary>
    /// Represents a dictionary mapping <see cref="TKey"/>s to lists of <see cref="TValue"/>s.
    /// </summary>
    public abstract class ObjectMap<TKey, TValue> : Dictionary<TKey, IReadOnlyList<TValue>>
    {
        #region Public properties

        /// <summary>
        /// Gets the list of all objects contained in this map.
        /// </summary>
        public IReadOnlyList<TValue> AllObjects { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectMap"/> class wrapping given objects.
        /// </summary>
        /// <param name="objects">The objects to be contained in the map.</param>
        protected ObjectMap(IEnumerable<TValue> objects)
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

        #region Public methods

        /// <summary>
        /// Enumerates all objects associated with given keys.
        /// </summary>
        /// <param name="keys">The keys.</param>
        /// <returns>The enumerable of all the values corresponding to the requested keys.</returns>
        public IEnumerable<TValue> GetObjectsForKeys(params TKey[] keys) =>
            // Merge the vales from each key, if they're present
            keys.Distinct().Where(ContainsKey).SelectMany(key => this[key]);

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
