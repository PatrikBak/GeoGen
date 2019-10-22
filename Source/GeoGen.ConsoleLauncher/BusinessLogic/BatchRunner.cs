using System;
using System.Threading.Tasks;
using static GeoGen.ConsoleLauncher.Log;

namespace GeoGen.ConsoleLauncher
{
    /// <summary>
    /// The default implementation of <see cref="IBatchRunner"/>.
    /// </summary>
    public class BatchRunner : IBatchRunner
    {
        #region Dependencies

        /// <summary>
        /// The provider of inputs for the algorithm.
        /// </summary>
        private readonly IAlgorithmInputProvider _inputProvider;

        /// <summary>
        /// The processor of the algorithm output.
        /// </summary>
        private readonly IAlgorithmRunner _runner;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="BatchRunner"/> class.
        /// </summary>
        /// <param name="inputProvider">The provider of inputs for the algorithm.</param>
        /// <param name="runner">The runner of the algorithm for particular input.</param>
        public BatchRunner(IAlgorithmInputProvider inputProvider, IAlgorithmRunner runner)
        {
            _inputProvider = inputProvider ?? throw new ArgumentNullException(nameof(inputProvider));
            _runner = runner ?? throw new ArgumentNullException(nameof(runner));
        }

        #endregion

        #region IBatchRunner implementation

        /// <summary>
        /// Scans the folder with input files and runs the algorithm on them.
        /// </summary>
        /// <returns>The task representing the action.</returns>
        public async Task FindAllInputFilesAndRunAlgorithmsAsync()
        {
            // Load all inputs
            var inputs = await _inputProvider.GetAlgorithmInputsAsync();

            // Run algorithm for each of them
            foreach (var input in inputs)
            {
                // Log the file being processed
                LoggingManager.LogInfo($"Running algorithm for the input file {input.FilePath}.");

                try
                {
                    // Try to run it
                    _runner.Run(input);
                }
                catch (Exception e)
                {
                    // Log which file failed
                    LoggingManager.LogError($"Couldn't perform the algorithm on the input file {input.FilePath}: {e.Message}");

                    // Throw it further
                    throw;
                }
            }
        }

        #endregion
    }
}
