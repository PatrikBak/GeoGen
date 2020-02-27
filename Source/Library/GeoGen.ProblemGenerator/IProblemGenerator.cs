using GeoGen.Core;
using System.Collections.Generic;

namespace GeoGen.ProblemGenerator
{
    /// <summary>
    /// Represents a service that can generate geometry problems, i.e. <see cref="Configuration"/> and theorems.
    /// It takes a <see cref="ProblemGeneratorInput"/> and produced multiple <see cref="ProblemGeneratorOutput"/>s. 
    /// The algorithm also provides the theorems of the initial configuration. 
    /// <para>
    /// The basic idea of the generation algorithm is the following: The <see cref="ProblemGeneratorInput.InitialConfiguration"/> is 
    /// extended using <see cref="ProblemGeneratorInput.Constructions"/> by <see cref="ProblemGeneratorInput.NumberOfIterations"/> objects
    /// to get new configurations and find theorems in them.
    /// </para>
    /// </summary>
    public interface IProblemGenerator
    {
        /// <summary>
        /// Executes the problem generation algorithm for a given algorithm input.
        /// </summary>
        /// <param name="input">The input for the generation algorithm.</param>
        /// <returns>The theorems in the initial configuration and a lazy enumerable of all the generated output.</returns>
        (TheoremMap initialTheorems, IEnumerable<ProblemGeneratorOutput> generationOutputs) Generate(ProblemGeneratorInput input);
    }
}