namespace GeoGen.Generator.Constructing.Arguments.Container
{
    /// <summary>
    /// An abstract factory for creating <see cref="IArgumentsContainer"/>.
    /// </summary>
    internal interface IArgumentsContainerFactory
    {
        /// <summary>
        /// Creates an empty arguments container.
        /// </summary>
        /// <returns>The arguments container.</returns>
        IArgumentsContainer CreateContainer();
    }
}
