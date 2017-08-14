using System;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.Core.Utilities
{
    /// <summary>
    /// An implementation of <see cref="IEqualityComparer{T}"/>, where T is <see cref="IReadOnlyList{T}"/>.
    /// It uses a provided <see cref="IEqualityComparer{T}"/> for comparing the items. 
    /// </summary>
    /// <typeparam name="T">The type of items within lists.</typeparam>
    public class ListEqualityComparer<T> : IEqualityComparer<IReadOnlyList<T>>
    {
        #region Private fields

        /// <summary>
        /// The comparer used for list's items.
        /// </summary>
        private readonly IEqualityComparer<T> _equalityComparer;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructs a new list equality comparer using a given item comparer.
        /// </summary>
        /// <param name="equalityComparer">The equality comparer for items.</param>
        public ListEqualityComparer(IEqualityComparer<T> equalityComparer)
        {
            _equalityComparer = equalityComparer ?? throw new ArgumentNullException(nameof(equalityComparer));
        }

        #endregion

        #region IEqualityComparer methods

        /// <summary>
        /// Checks if the two given lists are equal, i.e. if they contain the same
        /// number of elements and particular elements are equal according to the
        /// provided equality comparer for type T. 
        /// </summary>
        /// <param name="x">The first element.</param>
        /// <param name="y">The second element.</param>
        /// <returns>true, if the lists are equal, false otherwise.</returns>
        public bool Equals(IReadOnlyList<T> x, IReadOnlyList<T> y)
        {
            return x.Count == y.Count && x.Where((t, i) => !_equalityComparer.Equals(t, y[i])).Empty();
        }

        /// <summary>
        /// Gets the hash code of a given list.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <returns>The hash code.</returns>
        public int GetHashCode(IReadOnlyList<T> list)
        {
            return HashCodeUtilities.GetOrderDependentHashCode(list, arg => _equalityComparer.GetHashCode(arg));
        }

        #endregion
    }
}