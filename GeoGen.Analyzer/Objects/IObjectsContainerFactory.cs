namespace GeoGen.Analyzer
{
    /// <summary>
    /// Represents a factory that takes care of creating <see cref="IObjectsContainer"/>s.
    /// </summary>
    internal interface IObjectsContainerFactory
    {
        /// <summary>
        /// Creates an empty objects container.
        /// </summary>
        /// <returns>An empty objects container.</returns>
        IObjectsContainer CreateContainer();
    }
}