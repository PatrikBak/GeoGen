using System;
using System.Collections;
using System.Collections.Generic;
using GeoGen.Utilities.DataStructures;

namespace GeoGen.Utilities
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
        /// The dictionary mapping string versions of items to items itself.
        /// </summary>
        protected readonly Dictionary<string, T> Items = new Dictionary<string, T>();

        /// <summary>
        /// The converter of items to string.
        /// </summary>
        protected readonly IToStringConverter<T> Converter;

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

            var stringVersion = Converter.ConvertToString(item);

            if (Items.ContainsKey(stringVersion))
                return false;

            Items.Add(stringVersion, item);
            return true;
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