namespace GeoGen.Core.Generator
{
    /// <summary>
    /// An abstract factory for creating generators from a generator input.
    /// </summary>
    public interface IGeneratorFactory
    {
        /// <summary>
        /// Creates a generator from a given generator input.
        /// </summary>
        /// <param name="generatorInput">The generator input.</param>
        /// <returns>The generator.</returns>
        IGenerator CreateGenerator(GeneratorInput generatorInput);
    }
}