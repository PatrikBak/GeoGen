using System;
using System.Collections;
using System.Collections.Generic;

namespace GeoGen.Utilities
{
    /// <summary>
    /// The implementation of <see cref="IReadOnlyHashSet{T}"/> that simply wraps another <see cref="HashSet{T}"/>. 
    /// This class conveniently implements equals and hash code by internally comparing the inner sets.
    /// </summary>
    /// <typeparam name="T">The type of items of the set.</typeparam>
    public class ReadOnlyHashSet<T> : IReadOnlyHashSet<T>
    {
        #region Public static fields

        /// <summary>
        /// An empty read only hash set.
        /// </summary>
        public static readonly ReadOnlyHashSet<T> Empty = new ReadOnlyHashSet<T>(new HashSet<T>());

        #endregion

        #region Private static fields

        /// <summary>
        /// The instance of the comparer of hash sets of this type used in the 
        /// <see cref="GetHashCode"/> and <see cref="Equals(object)"/> methods.
        /// </summary>
        private static readonly IEqualityComparer<HashSet<T>> SetComparer = HashSet<T>.CreateSetComparer();

        #endregion

        #region Private fields

        /// <summary>
        /// The wrapped hash set.
        /// </summary>
        private readonly HashSet<T> _set;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyHashSet{T}"/> class.
        /// </summary>
        /// <param name="set">The set to be wrapped as read-only one.</param>
        public ReadOnlyHashSet(HashSet<T> set) => _set = set ?? throw new ArgumentNullException(nameof(set));

        #endregion

        #region IReadOnlyHashSet implementation

        /// <summary>
        /// Finds out if a given item is present in the set.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>true, if the set contains the item; false otherwise.</returns>
        public bool Contains(T item) => _set.Contains(item);

        #endregion

        #region IEnumerable implementation

        /// <summary>
        /// Gets the generic enumerator.
        /// </summary>
        /// <returns>The enumerator.</returns>
        public IEnumerator<T> GetEnumerator() => _set.GetEnumerator();

        /// <summary>
        /// Gets the non-generic enumerator.
        /// </summary>
        /// <returns>The non-generic enumerator.</returns>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion

        #region IReadOnly collection implementation

        /// <summary>
        /// Gets the number of items in the set.
        /// </summary>
        public int Count => _set.Count;

        #endregion

        #region HashCode and Equals

        /// <summary>
        /// Gets the hash code of this object.
        /// </summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode() => SetComparer.GetHashCode(_set);

        /// <summary>
        /// Finds out if a passed object is equal to this one.
        /// </summary>
        /// <param name="otherObject">The passed object.</param>
        /// <returns>true, if they are equal; false otherwise.</returns>

        public override bool Equals(object otherObject)
        {
            // Either the references are equals
            return otherObject == this
                // Or the object is not null
                || otherObject != null
                // And is a read-only hash set
                && otherObject is ReadOnlyHashSet<T> otherSet
                // And the comparer says they're equal
                && SetComparer.Equals(_set, otherSet._set);
        }

        #endregion
    }
}
