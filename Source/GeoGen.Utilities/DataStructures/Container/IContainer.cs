using System.Collections.Generic;

namespace GeoGen.Utilities
{
    /// <summary>
    /// Represents a container of distinct items that is able to find the equal version of an item, if there is any.
    /// </summary>
    /// <typeparam name="T">The type of items in the container.</typeparam>
    public interface IContainer<T> : IEnumerable<T>
    {
        /// <summary>
        /// Tries to add a given item to the container. If an equal version of the item is present 
        /// in the container, the item won't be added and the <paramref name="equalItem"/> will be set 
        /// to this equal version. Otherwise the item will be added and the <paramref name="equalItem"/> 
        /// will be set to the default value of <typeparamref name="T"/>.
        /// </summary>
        /// <param name="item">The item to be added.</param>
        /// <param name="equalItem">Either the equal version of the passed item from the container (if there's any), or the default value of the type <typeparamref name="T"/>.</param>
        void TryAdd(T item, out T equalItem);

        /// <summary>
        /// Finds an item in the container equal to a given one.
        /// </summary>
        /// <param name="item">The item which equal version we're seeking.</param>
        /// <returns>The item from the container equal to this one, if it exists; otherwise default(T).</returns>
        T FindEqualItem(T item);
    }
}
