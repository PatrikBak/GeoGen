using System.Collections.Generic;
using System.Linq;

namespace GeoGen.Utilities
{
    /// <summary>
    /// Extension methods for <see cref="ICollection{T}"/>.
    /// </summary>
    public static class CollectionExtensions
    {
        /// <summary>
        /// Sets given items to be the content of the collection.
        /// </summary>
        /// <typeparam name="T">The type of items.</typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="items">The items that should be the content of the collection.</param>
        public static void SetItems<T>(this ICollection<T> collection, IEnumerable<T> items)
        {
            // Clear the collection
            collection.Clear();

            // If the collection is a list, then the most efficient way is to use the AddRange method
            if (collection is List<T> list)
            {
                list.AddRange(items);
                return;
            }

            // Otherwise we add all the items by one
            items.ForEach(collection.Add);
        }

        /// <summary>
        /// Adds given items to the collection.
        /// </summary>
        /// <typeparam name="T">The type of items.</typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="items">The items to be added.</param>
        public static void Add<T>(this ICollection<T> collection, params T[] items) => items.ForEach(collection.Add);

        /// <summary>
        /// Adds given items to the collection.
        /// </summary>
        /// <typeparam name="T">The type of items.</typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="items">The items to be added.</param>
        public static void Add<T>(this ICollection<T> collection, IEnumerable<T> items) => items.ForEach(collection.Add);

        /// <summary>
        /// Determines if the collection contains two equal items.
        /// </summary>
        /// <typeparam name="T">The type of items.</typeparam>
        /// <param name="collection">The collection.</param>
        /// <returns>true, if the collection contains duplicate items; false otherwise.</returns>
        public static bool AnyDuplicates<T>(this IReadOnlyCollection<T> collection) => collection.Count != collection.Distinct().Count();
    }
}