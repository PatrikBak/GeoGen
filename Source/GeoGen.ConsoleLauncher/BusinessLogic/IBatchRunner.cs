using System.Threading.Tasks;

namespace GeoGen.ConsoleLauncher
{
    /// <summary>
    /// Represents a service that finds available input files and can run the algorithm on them.
    /// </summary>
    public interface IBatchRunner
    {
        /// <summary>
        /// Scans the folder with input files and runs the algorithm on them.
        /// </summary>
        /// <returns>The task representing the action.</returns>
        Task FindAllInputFilesAndRunAlgorithmsAsync();
    }
}
