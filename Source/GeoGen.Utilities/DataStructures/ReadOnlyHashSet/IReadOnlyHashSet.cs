using System.Collections.Generic;

namespace GeoGen.Utilities
{
    /// <summary>
    /// Represents a strongly-typed, read-only set of element.
    /// </summary>
    /// <typeparam name="T">The type of the elements.</typeparam>
    public interface IReadOnlyHashSet<T> : IReadOnlyCollection<T>
    {
        /// <summary>
        /// Finds out if a given item is present in the set.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>true, if the set contains the item; false otherwise.</returns>
        bool Contains(T item);
    }
}
