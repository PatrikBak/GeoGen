using System.Collections.Generic;

namespace GeoGen.Generator
{
    /// <summary>
    /// Represents the interface of the whole generation process. 
    /// </summary>
    public interface IGenerator
    {
        /// <summary>
        /// Starts the generation process and lazily returns the output.
        /// </summary>
        /// <returns>The generator output enumerable.</returns>
        IEnumerable<GeneratorOutput> Generate();
    }
}