using System.Collections.Generic;

namespace GeoGen.Utilities
{
    /// <summary>
    /// Extension methods for <see cref="ICollection{T}"/>
    /// </summary>
    public static class CollectionExtensions
    {
        /// <summary>
        /// Sets given items as the items of a given collection.
        /// </summary>
        /// <typeparam name="T">The type of items.</typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="newItems">The new items to be set.</param>
        public static void SetItems<T>(this ICollection<T> collection, IEnumerable<T> newItems)
        {
            // Clear the collection
            collection.Clear();

            // If the collection is list, the most efficient way is to use the AddRange method
            if (collection is List<T> list)
            {
                list.AddRange(newItems);
                return;
            }

            // Otherwise we add all items by one
            newItems.ForEach(collection.Add);
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
        /// <param name="collection">The collections.</param>
        /// <param name="items">The items to be added.</param>
        public static void Add<T>(this ICollection<T> collection, IEnumerable<T> items) => items.ForEach(collection.Add);
    }
}