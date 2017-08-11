using System;
using System.Collections.Generic;

namespace GeoGen.Core.Utilities
{
    public static class CollectionExtensions
    {
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