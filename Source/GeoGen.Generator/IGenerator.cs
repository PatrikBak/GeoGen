using System.Collections.Generic;

namespace GeoGen.Generator
{
    /// <summary>
    /// Represents a service that performs the generator algorithm.
    /// </summary>
    public interface IGenerator
    {
        /// <summary>
        /// Starts the generation process and lazily returns the output. 
        /// </summary>
        /// <param name="input">The input for the generator.</param>
        /// <returns>A lazy enumerable of generator outputs.</returns>
        IEnumerable<GeneratorOutput> Generate(GeneratorInput input);
    }
}