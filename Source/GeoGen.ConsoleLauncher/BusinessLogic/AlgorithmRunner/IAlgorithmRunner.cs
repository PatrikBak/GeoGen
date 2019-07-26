using System.IO;

namespace GeoGen.ConsoleLauncher
{
    /// <summary>
    /// Represents a service that runs the algorithm on a given input and outputs the results
    /// to a given text writer.
    /// </summary>
    public interface IAlgorithmRunner
    {
        /// <summary>
        /// Runs the algorithm on a given output and outputs the results to a given text writer.
        /// </summary>
        /// <param name="input">The input for the algorithm.</param>
        /// <param name="outputWriter">The writer where the results are written.</param>
        void Run(AlgorithmInput input, TextWriter outputWriter);
    }
}
