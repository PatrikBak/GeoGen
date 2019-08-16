using GeoGen.Core;
using GeoGen.Generator;
using System.Collections.Generic;

namespace GeoGen.Algorithm
{
    /// <summary>
    /// Represents the main algorithm that generates geometric configurations together with theorems.
    /// </summary>
    public interface IAlgorithm
    {
        /// <summary>
        /// Executes the algorithm for a given generator input.
        /// </summary>
        /// <param name="input">The input for the algorithm.</param>
        /// <returns>The theorems in the initial configuration and a lazy enumerable of all the generated output.</returns>
        (TheoremsMap initialTheorems, IEnumerable<AlgorithmOutput> generationOutputs) Run(GeneratorInput input);
    }
}