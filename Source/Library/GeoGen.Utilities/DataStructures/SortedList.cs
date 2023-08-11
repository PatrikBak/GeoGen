using System.Collections;

namespace GeoGen.Utilities
{
    /// <summary>
    /// Represents a sorted list of items that doesn't contain duplicates. It internally
    /// reuses a <see cref="SortedList{TKey, TValue}"/> by setting TKey=TValue.
    /// <para>This is not the ideal way for performance</para>
    /// </summary>
    /// <typeparam name="T">The type of items stored in the list.</typeparam>
    public class SortedList<T> : IReadOnlyList<T>
    {
        #region Private fields

        /// <summary>
        /// The actual storage of sorted items.
        /// </summary>
        private readonly SortedList<T, T> _items;

        #endregion

        #region IReadOnlyList implementation

        /// <inheritdoc/>
        public T this[int index] => _items.Keys[index];

        /// <inheritdoc/>
        public int Count => _items.Count;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="SortedList"/> class.
        /// </summary>
        /// <param name="comparer">The comparer of items. If the value is then, the default comparer is used.</param>
        public SortedList(IComparer<T> comparer = null)
        {
            // Either use this comparer, if it's not null, of the default one
            comparer ??= Comparer<T>.Default;

            // Initialize the items
            _items = new SortedList<T, T>(comparer);
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Tries to add a given item to the list. If the item is already there, nothing happens.
        /// </summary>
        /// <param name="item">The item to be added.</param>
        public void TryAdd(T item)
        {
            // If the item is not there, add it
            if (!_items.ContainsKey(item))
                _items.Add(item, item);
        }

        #endregion

        #region IEnumerable implementation

        /// <inheritdoc/>
        public IEnumerator<T> GetEnumerator() => _items.Keys.GetEnumerator();

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion
    }
}
