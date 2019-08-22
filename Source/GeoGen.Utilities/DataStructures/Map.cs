using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.Utilities
{
    /// <summary>
    /// Represents a data structure that acts like a <see cref="Dictionary{TKey,TValue}"/>, but in both ways
    /// (i.e. you can get TKey items from TValues). It implements <see cref="IEnumerable{T}"/>, where 'T'
    /// is a tuple of the mapped items.
    /// </summary>
    /// <typeparam name="T1">The type of first items.</typeparam>
    /// <typeparam name="T2">The type of second items.</typeparam>
    public class Map<T1, T2> : IEnumerable<(T1, T2)>
    {
        #region Private fields

        /// <summary>
        /// The dictionary mapping T1 items to T2 ones.
        /// </summary>
        private readonly Dictionary<T1, T2> _leftToRight = new Dictionary<T1, T2>();

        /// <summary>
        /// The dictionary mapping T2 items to T1 ones.
        /// </summary>
        /// 
        private readonly Dictionary<T2, T1> _rightToLeft = new Dictionary<T2, T1>();

        #endregion

        #region Public methods

        /// <summary>
        /// Adds given items to the map.
        /// </summary>
        /// <param name="item1">The first item.</param>
        /// <param name="item2">The second item.</param>
        public void Add(T1 item1, T2 item2)
        {
            _leftToRight.Add(item1, item2);
            _rightToLeft.Add(item2, item1);
        }

        /// <summary>
        /// Gets the T1 item corresponding to a given T2 key.
        /// </summary>
        /// <param name="key">The T2 key.</param>
        /// <returns>The corresponding T1 item.</returns>
        public T1 GetLeftValue(T2 key) => _rightToLeft[key];

        /// <summary>
        /// Gets the T2 item corresponding to a given T1 key.
        /// </summary>
        /// <param name="key">The T1 key.</param>
        /// <returns>The corresponding T2 item.</returns>
        public T2 GetRightValue(T1 key) => _leftToRight[key];

        /// <summary>
        /// Gets the T1 item corresponding to a given T2 key, or the default value,
        /// if the key is not present.
        /// </summary>
        /// <param name="key">The T2 key.</param>
        /// <returns>The corresponding T1 item, or null, if the key is not present.</returns>
        public T1 GetLeftValueOrDefault(T2 key) => _rightToLeft.GetOrDefault(key);

        /// <summary>
        /// Gets the T2 item corresponding to a given T1 key, or the default value,
        /// if the key is not present.
        /// </summary>
        /// <param name="key">The T1 key.</param>
        /// <returns>The corresponding T2 item; or null, if the key is not present.</returns>
        public T2 GetRightValueOrDefault(T1 key) => _leftToRight.GetOrDefault(key);

        /// <summary>
        /// Checks if the map contains a given T1 item.
        /// </summary>
        /// <param name="item">The T1 item.</param>
        /// <returns>true, if the map contains the item; false otherwise</returns>
        public bool ContainsLeftKey(T1 item) => _leftToRight.ContainsKey(item);

        /// <summary>
        /// Checks if the map contains a given T2 item.
        /// </summary>
        /// <param name="item">The T2 item.</param>
        /// <returns>true, if the map contains the item; false otherwise</returns>
        public bool ContainsRightKey(T2 item) => _rightToLeft.ContainsKey(item);

        /// <summary>
        /// Clears the content of the map.
        /// </summary>
        public void Clear()
        {
            // Clear both dictionaries
            _leftToRight.Clear();
            _rightToLeft.Clear();
        }

        /// <summary>
        /// Sets the content of the map to the items from a given enumerable of items pairs.
        /// </summary>
        /// <param name="items">The enumerable of the pairs of items that should be the content of the map.</param>
        public void SetItems(IEnumerable<(T1, T2)> items)
        {
            // First clear
            Clear();

            // Then add all the items
            items.ForEach(itemsPair => Add(itemsPair.Item1, itemsPair.Item2));
        }

        #endregion

        #region IEnumerable implementation

        /// <summary>
        /// Gets a generic enumerator.
        /// </summary>
        /// <returns>A generic enumerator.</returns>
        public IEnumerator<(T1, T2)> GetEnumerator() => _leftToRight.Select(pair => (pair.Key, pair.Value)).GetEnumerator();

        /// <summary>
        /// Gets a non-generic enumerator.
        /// </summary>
        /// <returns>A non-generic enumerator.</returns>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion
    }
}