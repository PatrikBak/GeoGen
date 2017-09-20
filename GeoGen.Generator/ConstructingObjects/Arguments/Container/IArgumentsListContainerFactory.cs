namespace GeoGen.Generator.ConstructingObjects.Arguments.Container
{
    /// <summary>
    /// An abstract factory for creating <see cref="IArgumentsListContainer"/>s.
    /// </summary>
    internal interface IArgumentsListContainerFactory
    {
        /// <summary>
        /// Creates an empty arguments container.
        /// </summary>
        /// <returns>The arguments container.</returns>
        IArgumentsListContainer CreateContainer();
    }
}
