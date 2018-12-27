using System;
using System.Collections;
using System.Collections.Generic;

namespace GeoGen.Core
{
    /// <summary>
    /// Represents a <see cref="IContainer{T}"/> of distinct items that are compared based on their string versions.
    /// </summary>
    /// <typeparam name="T">The type of items in the container.</typeparam>
    public class StringBasedContainer<T> : IContainer<T>
    {
        #region Private fields

        /// <summary>
        /// The dictionary mapping the string versions of items to the items itself.
        /// </summary>
        private readonly Dictionary<string, T> _items = new Dictionary<string, T>();

        /// <summary>
        /// The converter of items to a string.
        /// </summary>
        private readonly IToStringConverter<T> _converter;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="StringBasedContainer{T}"/> class.
        /// </summary>
        /// <param name="converter">The converter of items to a string.</param>
        public StringBasedContainer(IToStringConverter<T> converter)
        {
            _converter = converter ?? throw new ArgumentNullException(nameof(converter));
        }

        #endregion

        #region IContainer implementation

        /// <summary>
        /// Adds a given item to the container. If an equal version of the item is present 
        /// in the container, the item won't be added and the <paramref name="equalItem"/> will be set 
        /// to this equal version. Otherwise the <paramref name="equalItem"/> will be set
        /// to the default value of <typeparamref name="T"/>.
        /// </summary>
        /// <param name="item">The item to be added.</param>
        /// <param name="equalItem">Either the equal version of the passed item from the container (if there's any), or the default value of the type <typeparamref name="T"/>.</param>
        public void Add(T item, out T equalItem)
        {
            // Convert the object to a string
            var stringRepresentation = _converter.ConvertToString(item);

            // If we have it cached, we can return it directly 
            if (_items.ContainsKey(stringRepresentation))
            {
                // Set the equal object
                equalItem = _items[stringRepresentation];

                // Terminate
                return;
            }

            // Otherwise add the item to the container 
            _items.Add(stringRepresentation, item);

            // Set that there is no equal item
            equalItem = default(T);
        }

        #endregion

        #region IEnumerable implementation

        /// <summary>
        /// Gets a generic enumerator.
        /// </summary>
        /// <returns>The enumerator.</returns>
        public IEnumerator<T> GetEnumerator() => _items.Values.GetEnumerator();

        /// <summary>
        /// Gets a non-generic enumerator.
        /// </summary>
        /// <returns>The enumerator.</returns>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion
    }
}