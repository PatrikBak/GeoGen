using System;
using System.Collections.Generic;

namespace GeoGen.Core.Utilities
{
    /// <summary>
    /// Extension methods for <see cref="ICollection{T}"/>
    /// </summary>
    public static class CollectionExtensions
    {
        /// <summary>
        /// Sets new items to the collection.
        /// </summary>
        /// <typeparam name="T">The type</typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="newItems">The new items to be set.</param>
        public static void SetItems<T>(this ICollection<T> collection, IEnumerable<T> newItems)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            if (newItems == null)
                throw new ArgumentNullException(nameof(newItems));

            collection.Clear();

            foreach (var item in newItems)
            {
                collection.Add(item);
            }
        }
    }
}