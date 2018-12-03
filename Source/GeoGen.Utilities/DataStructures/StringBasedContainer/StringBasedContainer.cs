using System;
using System.Collections;
using System.Collections.Generic;

namespace GeoGen.Utilities
{
    /// <summary>
    /// Represents a container of distinct items that are compared based on their string versions.
    /// </summary>
    /// <typeparam name="T">The type of items in the container.</typeparam>
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

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="converter">The converter of items to string.</param>
        protected StringBasedContainer(IToStringConverter<T> converter)
        {
            Converter = converter ?? throw new ArgumentNullException(nameof(converter));
        }

        #endregion

        #region Protected methods

        /// <summary>
        /// Adds a given item to the container.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>true, if the container's content has changed, false otherwise </returns>
        protected bool Add(T item)
        {
            // Convert the item to string
            var stringVersion = Converter.ConvertToString(item);

            // If the item with this string representation is present, return failure
            if (Items.ContainsKey(stringVersion))
                return false;

            // Otherwise add the item to the container
            Items.Add(stringVersion, item);

            // Return success
            return true;
        }

        #endregion

        #region IEnumerable implementation

        /// <summary>
        /// Gets a generic enumerator.
        /// </summary>
        /// <returns>The enumerator.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            return Items.Values.GetEnumerator();
        }

        /// <summary>
        /// Gets a non-generic enumerator.
        /// </summary>
        /// <returns>The enumerator.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}