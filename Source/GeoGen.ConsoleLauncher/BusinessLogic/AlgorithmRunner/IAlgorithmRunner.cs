namespace GeoGen.ConsoleLauncher
{
    /// <summary>
    /// Represents a service that runs the algorithm on a given input.
    /// </summary>
    public interface IAlgorithmRunner
    {
        /// <summary>
        /// Runs the algorithm on a given output.
        /// </summary>
        /// <param name="input">The input for the algorithm.</param>
        void Run(LoadedAlgorithmInput input);
    }
}
