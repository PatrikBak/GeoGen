namespace GeoGen.Utilities
{
    /// <summary>
    /// Extension methods for <see cref="HashSet{T}"/>.
    /// </summary>
    public static class HashSetExtensions
    {
        /// <summary>
        /// Casts a given hash set to a read only hash set.
        /// </summary>
        /// <typeparam name="T">The type of items in the set.</typeparam>
        /// <param name="set">The set.</param>
        /// <returns>A read-only wrapper of the set.</returns>
        public static ReadOnlyHashSet<T> AsReadOnly<T>(this HashSet<T> set) => new ReadOnlyHashSet<T>(set);
    }
}
