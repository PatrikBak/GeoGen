using System;
using System.Collections.Generic;
using System.Text;

namespace GeoGen.Core
{
    public static class ContainerExtensions
    {
        public static void Add<T>(this IContainer<T> container, T item) => container.Add(item, out var _);
    }
}
