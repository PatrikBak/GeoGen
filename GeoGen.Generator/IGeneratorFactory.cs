namespace GeoGen.Generator
{
    /// <summary>
    /// An abstract factory for creating <see cref="IGenerator"/> from a generator input.
    /// </summary>
    public interface IGeneratorFactory
    {
        /// <summary>
        /// Creates a generator for a given generator input.
        /// </summary>
        /// <param name="generatorInput">The generator input.</param>
        /// <returns>The generator.</returns>
        IGenerator CreateGenerator(GeneratorInput generatorInput);
    }
}