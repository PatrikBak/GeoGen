namespace GeoGen.Generator
{
    /// <summary>
    /// A factory for creating a <see cref="IGenerator"/> for a given <see cref="GeneratorInput"/>.
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