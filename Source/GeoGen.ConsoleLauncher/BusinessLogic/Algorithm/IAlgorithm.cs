using System.Collections.Generic;

namespace GeoGen.ConsoleLauncher
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
        /// <returns>A lazy enumerable of all the generated output.</returns>
        IEnumerable<AlgorithmOutput> GenerateOutputs(AlgorithmInput input);
    }
}