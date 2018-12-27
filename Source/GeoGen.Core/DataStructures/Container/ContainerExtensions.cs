namespace GeoGen.Core
{
    /// <summary>
    /// Extension methods for <see cref="IContainer{T}"/>.
    /// </summary>
    public static class ContainerExtensions
    {
        /// <summary>
        /// Adds a item to the container.
        /// </summary>
        /// <typeparam name="T">The type of the item.</typeparam>
        /// <param name="container">The container.</param>
        /// <param name="item">The item to be added.</param>
        public static void Add<T>(this IContainer<T> container, T item) => container.Add(item, out var _);
    }
}
