namespace GeoGen.Analyzer.Objects
{
    /// <summary>
    /// Represents a factory that takes care of creating <see cref="IObjectsContainer"/>s.
    /// </summary>
    internal interface IObjectsContainersFactory
    {
        /// <summary>
        /// Creates an empty objects container.
        /// </summary>
        /// <returns>An empty objects container.</returns>
        IObjectsContainer CreateContainer();
    }
}