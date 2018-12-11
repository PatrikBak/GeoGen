using System.Collections.Generic;
using GeoGen.Core;

namespace GeoGen.Generator
{
    /// <summary>
    /// A basic interface for the generator module that executes the generation
    /// process. The goal is to generate <see cref="Configuration"/>s and <see cref="Theorem"/>s
    /// that hold true in them, using a <see cref="GeneratorInput"/>.
    /// </summary>
    public interface IGenerator
    {
        /// <summary>
        /// Starts the generation process and lazily returns the output.
        /// </summary>
        /// <returns>The generator output enumerable.</returns>
        IEnumerable<GeneratorOutput> Generate(int numberOfIterations);
    }
}