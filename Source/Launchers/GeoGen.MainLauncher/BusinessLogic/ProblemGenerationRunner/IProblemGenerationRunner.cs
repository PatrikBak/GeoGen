using GeoGen.ProblemGenerator;

namespace GeoGen.MainLauncher
{
    /// <summary>
    /// Represents a service that runs <see cref="IProblemGenerator"/>.
    /// </summary>
    public interface IProblemGenerationRunner
    {
        /// <summary>
        /// Runs problem generation on a given output.
        /// </summary>
        /// <param name="input">The input for the problem generator.</param>
        void Run(LoadedProblemGeneratorInput input);
    }
}
