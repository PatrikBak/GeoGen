namespace GeoGen.Generator
{
    /// <summary>
    /// The factory for creating instances of <see cref="IConfigurationFilter"/>.
    /// <para>The implementation is supposed to be provided by the dependency injection management system.</para>
    /// </summary>
    public interface IConfigurationFilterFactory
    {
        /// <summary>
        /// Creates the configuration filter using a given generator input.
        /// </summary>
        /// <param name="generatorInput">The input for the generator.</param>
        /// <returns>The configuration filter.</returns>
        IConfigurationFilter Create(GeneratorInput generatorInput);
    }
}