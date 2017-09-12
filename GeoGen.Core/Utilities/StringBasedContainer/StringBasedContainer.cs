using System;
using System.Collections;
using System.Collections.Generic;

namespace GeoGen.Core.Utilities.StringBasedContainer
{
    /// <summary>
    /// Represents a container of distinct items that are compared based on their string versions.
    /// These are provided by the abstract property. 
    /// </summary>
    /// <typeparam name="T">The type of items</typeparam>
    public abstract class StringBasedContainer<T> : IEnumerable<T>
    {
        #region Protected fields

        /// <summary>
        /// The set of all items.
        /// </summary>
        protected readonly Dictionary<string, T> Items = new Dictionary<string, T>();

        #endregion

        #region Abstract methods

        /// <summary>
        /// Converts a given item to string.
        /// </summary>
        /// <param name="item">The given item.</param>
        /// <returns>The string representation.</returns>
        protected abstract string ItemToString(T item);

        #endregion

        #region Protected methods

        /// <summary>
        /// Adds a given item to the container.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>true, if the container's content has changed, false otherwise </returns>
        protected bool Add(T item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            var stringVersion = ItemToString(item);

            lock (this)
            {
                if (Items.ContainsKey(stringVersion))
                    return false;

                Items.Add(stringVersion, item);
                return true;
            }
        }

        #endregion

        #region IEnumerable methods

        public IEnumerator<T> GetEnumerator()
        {
            return Items.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}