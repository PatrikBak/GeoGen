using System;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.Core.Utilities
{
    public static class EnumerableExtensions
    {
        public static bool Empty<T>(this IEnumerable<T> enumerable)
        {
            if (enumerable == null)
                throw new NullReferenceException(nameof(enumerable));

            return !enumerable.Any();
        }
    }
}