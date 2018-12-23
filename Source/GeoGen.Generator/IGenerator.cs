using GeoGen.Core;
using System.Collections.Generic;

namespace GeoGen.Generator
{
    /// <summary>
    /// A basic interface for the generator module that executes the generation algorithm.
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