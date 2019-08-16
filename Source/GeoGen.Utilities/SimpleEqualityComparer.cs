using System;
using System.Collections.Generic;

namespace GeoGen.Utilities
{
    /// <summary>
    /// A helper class that allows to construct <see cref="IEqualityComparer{T}"/> using <see cref="Func{T1, T2, TResult}"/>.
    /// </summary>
    /// <typeparam name="T">The type of items being compared.</typeparam>
    public class SimpleEqualityComparer<T> : IEqualityComparer<T>
    {
        #region Private fields

        /// <summary>
        /// The comparer of elements, i.e. a function takes two elements and returns if they are equal.
        /// </summary>
        private readonly Func<T, T, bool> _comparer;

        /// <summary>
        /// The function that calculates the hash code of the item.
        /// </summary>
        private readonly Func<T, int> _hashCoder;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleEqualityComparer{T}"/> class.
        /// </summary>
        /// <param name="comparer">The comparer of elements, i.e. a function takes two elements and returns if they are equal.</param>
        /// <param name="hashCoder">The function that calculates the hash code of the item. If it's null, than the base function will be used.</param>
        public SimpleEqualityComparer(Func<T, T, bool> comparer, Func<T, int> hashCoder = null)
        {
            _comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));
            _hashCoder = hashCoder ?? (t => t.GetHashCode());
        }

        #endregion

        #region IEqualityComparer methods

        /// <summary>
        /// Finds out if the given elements are equal.
        /// </summary>
        /// <param name="firstElement">The first element.</param>
        /// <param name="secondElement">The second element.</param>
        /// <returns></returns>
        public bool Equals(T firstElement, T secondElement) => _comparer(firstElement, secondElement);

        /// <summary>
        /// Gets the hash code of the element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>The hash code of the element.</returns>
        public int GetHashCode(T element) => _hashCoder(element);

        #endregion

        #region Static helpers

        /// <summary>
        /// Creates an equality comparer that uses given property selector. The equality is then determined
        /// by the result of Equals(property(t1), property(t2)), and hash code is simply property(t).GetHashCode
        /// (or 0, if the property(t) is null).
        /// </summary>
        /// <param name="propertySelector">The function that for selects property based on which we should compare.</param>
        /// <returns>The comparer.</returns>
        public static SimpleEqualityComparer<T> Create(Func<T, object> propertySelector)
        {
            // Return a new comparer
            return new SimpleEqualityComparer<T>(
                // That compares the objects based on our equals method for the properties
                (t1, t2) => Equals(propertySelector(t1), propertySelector(t2)),
                // And gets the hash code from the property, or 0, is the property is null
                t => propertySelector(t)?.GetHashCode() ?? 0);
        }

        #endregion
    }
}
