using System.Collections;

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

        /// <inheritdoc/>
        public bool Contains(T item) => _set.Contains(item);

        #endregion

        #region IEnumerable implementation

        /// <inheritdoc/>
        public IEnumerator<T> GetEnumerator() => _set.GetEnumerator();

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion

        #region IReadOnly collection implementation

        /// <inheritdoc/>
        public int Count => _set.Count;

        #endregion

        #region HashCode and Equals

        /// <inheritdoc/>
        public override int GetHashCode() => SetComparer.GetHashCode(_set);

        /// <inheritdoc/>
        public override bool Equals(object otherObject)
            // Either the references are equals
            => otherObject == this
                // Or the object is not null
                || otherObject != null
                // And is a read-only hash set
                && otherObject is ReadOnlyHashSet<T> otherSet
                // And the comparer says they're equal
                && SetComparer.Equals(_set, otherSet._set);

        #endregion
    }
}
