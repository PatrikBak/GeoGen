namespace GeoGen.Analyzer.Objects
{
    /// <summary>
    /// A default implementation of <see cref="IObjectsContainersFactory"/>.
    /// </summary>
    internal sealed class ObjectsContainersFactory : IObjectsContainersFactory
    {
        /// <summary>
        /// Creates an empty objects container.
        /// </summary>
        /// <returns>An empty objects container.</returns>
        public IObjectsContainer CreateContainer()
        {
            return new ObjectsContainer();
        }
    }
}