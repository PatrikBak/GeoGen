using GeoGen.Core;
using System.Collections.Generic;

namespace GeoGen.Algorithm
{
    /// <summary>
    /// Represents the facade for multiple GeoGen algorithms that are combined to turn an <see cref="AlgorithmInput"/>
    /// into various <see cref="AlgorithmOutput"/>s. The algorithm also provides the theorems of the initial configuration. 
    /// The basic idea behind the whole program is the following: The <see cref="AlgorithmInput.InitialConfiguration"/> is 
    /// extended using <see cref="AlgorithmInput.Constructions"/> by <see cref="AlgorithmInput.NumberOfIterations"/> objects
    /// to get new configurations, which are tested for theorems, tried to be proven, ranked (<see cref="AlgorithmOutput"/>). 
    /// </summary>
    public interface IAlgorithmFacade
    {
        /// <summary>
        /// Executes the algorithm for a given algorithm input.
        /// </summary>
        /// <param name="input">The input for the algorithm.</param>
        /// <returns>The theorems in the initial configuration and a lazy enumerable of all the generated output.</returns>
        (TheoremMap initialTheorems, IEnumerable<AlgorithmOutput> generationOutputs) Run(AlgorithmInput input);
    }
}