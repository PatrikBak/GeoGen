using System;
using System.Collections;
using System.Collections.Generic;

namespace GeoGen.Core.Utilities.StringBasedContainer
{
    /// <summary>
    /// Represents a container of items that are compared based on their string versions.
    /// These are provided by the abstract property. 
    /// </summary>
    /// <typeparam name="T">The type of items</typeparam>
    public abstract class StringBasedContainer<T> : IEnumerable<T>
    {
        #region Abstract properties

        /// <summary>
        /// Gets the function that converts items to string.
        /// </summary>
        public abstract Func<T, string> ItemToString { get; }

        #endregion

        #region Private fields

        /// <summary>
        /// The container's content collection.
        /// </summary>
        private readonly List<T> _distinctItems = new List<T>();

        /// <summary>
        /// The set of string versions of all items of the collection.
        /// </summary>
        private readonly HashSet<string> _itemsHashes = new HashSet<string>();
        
        #endregion
        
        #region Public methods

        /// <summary>
        /// Adds a given item to the container.
        /// </summary>
        /// <param name="item">The item.</param>
        public virtual void Add(T item)
        {
            AddItem(item);
        }

        /// <summary>
        /// Adds item to the container and returns if the container has changed its content.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>true, if the container changed its content, false otherwise.</returns>
        public bool AddItem(T item)
        {
            if (!_itemsHashes.Add(ItemToString(item)))
                return false;

            _distinctItems.Add(item);

            return true;
        }

        /// <summary>
        /// Clears the container.
        /// </summary>
        public void Clear()
        {
            _distinctItems.Clear();
            _itemsHashes.Clear();
        }
        
        /// <summary>
        /// Checks if the container contains a given item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>true, if the container contains the item, false otherwise</returns>
        public bool Contains(T item)
        {
            return _itemsHashes.Contains(ItemToString(item));
        }

        #endregion

        #region IEnumerable methods

        public IEnumerator<T> GetEnumerator()
        {
            return _distinctItems.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}