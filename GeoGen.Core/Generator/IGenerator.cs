using System.Collections.Generic;

namespace GeoGen.Core.Generator
{
    /// <summary>
    /// A basic interface for the generator module.
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