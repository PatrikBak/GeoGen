using System.Collections.Generic;

namespace GeoGen.Utilities
{
    /// <summary>
    /// Represents an equality comparer of two <see cref="ISet{T}"/>s that makes use
    /// SetEquals method of the ISet interface.
    /// </summary>
    /// <typeparam name="T">The type of the elements.</typeparam>
    public class SetComparer<T> : IEqualityComparer<ISet<T>>
    {
        #region Instance

        /// <summary>
        /// The singleton instance of this comparer.
        /// </summary>
        public static readonly SetComparer<T> Instance = new SetComparer<T>();

        #endregion

        #region IEqualityComparer implementation

        /// <summary>
        /// Finds out if two given sets are equal.
        /// </summary>
        /// <param name="x">The first set.</param>
        /// <param name="y">The second set.</param>
        /// <returns></returns>
        public bool Equals(ISet<T> x, ISet<T> y)
        {
            return x.SetEquals(y);
        }

        /// <summary>
        /// Gets the hash code of a given set.
        /// </summary>
        /// <param name="set">The set.</param>
        /// <returns></returns>
        public int GetHashCode(ISet<T> set)
        {
            // Use our helper extension method
            return set.GetOrderIndependentHashCode(arg => arg.GetHashCode());
        }

        #endregion
    }
}