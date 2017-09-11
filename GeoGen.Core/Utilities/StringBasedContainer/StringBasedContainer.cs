using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.Core.Utilities.StringBasedContainer
{
    /// <summary>
    /// Represents a container of distinct items that are compared based on their string versions.
    /// These are provided by the abstract property. 
    /// </summary>
    /// <typeparam name="T">The type of items</typeparam>
    public abstract class StringBasedContainer<T> : IEnumerable<T>
    {
        #region Item class

        /// <summary>
        /// A wrapper class for an item holding the real and the string value.
        /// </summary>
        protected class Item
        {
            #region Public properties

            /// <summary>
            /// Gets the real value of the item.
            /// </summary>
            public T RealValue { get; }

            /// <summary>
            /// The string value of the item.
            /// </summary>
            public string StringValue { get; }

            #endregion

            #region Constructor

            /// <summary>
            /// Constructs a new item from it's real value and a string value.
            /// </summary>
            /// <param name="realValue">The real item's value.</param>
            /// <param name="stringValue">The string value.</param>
            public Item(T realValue, string stringValue)
            {
                RealValue = realValue;
                StringValue = stringValue;
            }

            #endregion

            #region Equals and hash code

            /// <summary>
            /// Determines if a given object is equal to this. This is supposed
            /// to be used only by a hash set of items, therefore it is 
            /// assumed that the object is not null and is of the type Item.
            /// </summary>
            /// <param name="obj">The object.</param>
            /// <returns>The hash code.</returns>
            public override bool Equals(object obj)
            {
                return ((Item) obj)?.StringValue == StringValue;
            }

            /// <summary>
            /// Gets a hashcode of this item. It's calculated as the hashcode
            /// of it's string representation.
            /// </summary>
            /// <returns>true, if they are equal, false otherwise</returns>
            public override int GetHashCode()
            {
                return StringValue.GetHashCode();
            }

            #endregion
        }

        #endregion

        #region Abstract properties

        /// <summary>
        /// Gets the function that converts items to string.
        /// </summary>
        public abstract Func<T, string> ItemToString { get; }

        #endregion

        #region Protected fields

        /// <summary>
        /// The set of all items.
        /// </summary>
        protected readonly HashSet<Item> Items = new HashSet<Item>();

        #endregion

        #region Public methods

        /// <summary>
        /// Adds a given item to the container.
        /// </summary>
        /// <param name="item">The item.</param>
        public virtual void Add(T item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            Items.Add(new Item(item, ItemToString(item)));
        }

        #endregion

        #region IEnumerable methods

        public IEnumerator<T> GetEnumerator()
        {
            return Items.Select(value => value.RealValue).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}