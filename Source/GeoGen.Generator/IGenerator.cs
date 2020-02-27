using System.Collections.Generic;

namespace GeoGen.Generator
{
    /// <summary>
    /// Represents a service that performs the generation algorithm.
    /// </summary>
    public interface IGenerator
    {
        /// <summary>
        /// Performs the generation algorithm on a given input. 
        /// </summary>
        /// <param name="input">The input for the generator.</param>
        /// <returns>The generated configurations.</returns>
        IEnumerable<GeneratedConfiguration> Generate(GeneratorInput input);
    }
}