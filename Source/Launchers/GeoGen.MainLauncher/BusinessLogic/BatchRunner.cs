using GeoGen.Infrastructure;
using GeoGen.Utilities;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using static GeoGen.Infrastructure.Log;

namespace GeoGen.MainLauncher
{
    /// <summary>
    /// The default implementation of <see cref="IBatchRunner"/> that uses <see cref="IProblemGeneratorInputProvider"/>
    /// to find inputs and <see cref="IProblemGenerationRunner"/> to run the problem generation algorithm on them.
    /// </summary>
    public class BatchRunner : IBatchRunner
    {
        #region Dependencies

        /// <summary>
        /// The provider of inputs for the problem generator.
        /// </summary>
        private readonly IProblemGeneratorInputProvider _inputProvider;

        /// <summary>
        /// The processor of the loaded problem generator input.
        /// </summary>
        private readonly IProblemGenerationRunner _runner;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="BatchRunner"/> class.
        /// </summary>
        /// <param name="inputProvider">The provider of inputs for the problem generator.</param>
        /// <param name="runner">The processor of the loaded problem generator input.</param>
        public BatchRunner(IProblemGeneratorInputProvider inputProvider, IProblemGenerationRunner runner)
        {
            _inputProvider = inputProvider ?? throw new ArgumentNullException(nameof(inputProvider));
            _runner = runner ?? throw new ArgumentNullException(nameof(runner));
        }

        #endregion

        #region IBatchRunner implementation

        /// <inheritdoc/>
        public async Task FindAllInputFilesAndRunProblemGenerationAsync()
        {
            // Load all inputs
            var inputs = await _inputProvider.GetProblemGeneratorInputsAsync();

            // Prepare the stopwatch
            var stopwatch = new Stopwatch();

            // Start the stopwatch
            stopwatch.Start();

            // Run algorithm for each of them
            inputs.ForEach((input, index) =>
            {
                // Log the file being processed
                LoggingManager.LogInfo($"Running algorithm for input file {index + 1} with path {input.FilePath}");

                try
                {
                    // Try to run it
                    _runner.Run(input);
                }
                catch (Exception e)
                {
                    // Log which file failed and the exception
                    LoggingManager.LogError($"Couldn't perform the algorithm on input file {index + 1} with path {input.FilePath}: {e}");
                }
            });

            // Stop the stopwatch
            stopwatch.Stop();

            // Log how long it all took
            LoggingManager.LogInfo($"Running algorithm on all files took {stopwatch.ElapsedMilliseconds} ms.");
        }

        #endregion
    }
}
