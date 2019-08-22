using System.Collections.Generic;

namespace GeoGen.Generator
{
    /// <summary>
    /// Represents a service that performs the generator algorithm.
    /// </summary>
    public interface IGenerator
    {
        /// <summary>
        /// Performs the generation algorithm on a given input. It can be adjusted
        /// by specifying <see cref="GenerationCallbacks"/>.
        /// </summary>
        /// <param name="input">The input for the algorithm.</param>
        /// <param name="callbacks">The callbacks to adjust the generation process.</param>
        /// <returns>The generated configurations.</returns>
        IEnumerable<GeneratedConfiguration> Generate(GeneratorInput input, GenerationCallbacks callbacks = null);
    }
}