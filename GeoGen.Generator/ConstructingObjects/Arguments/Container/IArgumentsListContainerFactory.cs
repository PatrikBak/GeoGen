namespace GeoGen.Generator.ConstructingObjects.Arguments.Container
{
    /// <summary>
    /// Represents a factory for creating new instances of <see cref="IArgumentsListContainer"/>.
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