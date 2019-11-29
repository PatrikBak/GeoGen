using GeoGen.Core;
using System.Collections.Generic;

namespace GeoGen.Algorithm
{
    /// <summary>
    /// Represents a service that takes an <see cref="AlgorithmOutput"/> and based on it finds 
    /// which of the found theorems will be considered interesting.
    /// </summary>
    public interface IBestTheoremsFinder
    {
        /// <summary>
        /// Processes a given algorithm output in order to find interesting theorems. 
        /// </summary>
        /// <param name="output">The algorithm output to be processed.</param>
        /// <returns>The interesting theorems of the output.</returns>
        IEnumerable<Theorem> FindInterestingTheorems(AlgorithmOutput output);
    }
}
