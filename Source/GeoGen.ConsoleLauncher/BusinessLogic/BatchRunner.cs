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
        /// The provider of algorithm inputs.
        /// </summary>
        private readonly IGeneratorInputsProvider _inputsProvider;

        /// <summary>
        /// The processor of the algorithm output.
        /// </summary>
        private readonly IAlgorithmRunner _runner;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="BatchRunner"/> class.
        /// </summary>
        /// <param name="inputsProvider">The provider of algorithm inputs.</param>
        /// <param name="runner">The runner of the algorithm for particular input.</param>
        public BatchRunner(IGeneratorInputsProvider inputsProvider, IAlgorithmRunner runner)
        {
            _inputsProvider = inputsProvider ?? throw new ArgumentNullException(nameof(inputsProvider));
            _runner = runner ?? throw new ArgumentNullException(nameof(runner));
        }

        #endregion

        #region IFolderScanner implementation

        /// <summary>
        /// Scans the folder with input files and runs the algorithm on them.
        /// </summary>
        /// <returns>The task representing the action.</returns>
        public async Task FindAllInputFilesAndRunAlgorithmsAsync()
        {
            // Load all inputs
            var inputs = await _inputsProvider.GetGeneratorInputsAsync();

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
                    // Log the exception
                    LoggingManager.LogError($"Couldn't perform the algorithm on the input file {input.FilePath}: {e.Message}");

                    // Log the internal exception
                    LoggingManager.LogDebug($"{e}\n");

                    // Continue on the next file
                    continue;
                }
            }
        }

        #endregion
    }
}
