using GeoGen.ProblemGenerator.InputProvider;
using GeoGen.Utilities;
using Serilog;
using System.Diagnostics;

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
                Log.Information("Running algorithm for input file {number} with path {path}", index + 1, input.FilePath);

                try
                {
                    // Try to run it
                    _runner.Run(input);
                }
                catch (Exception e)
                {
                    // Log which file failed and the exception
                    Log.Error(e, "Couldn't perform the algorithm on input file {number} with path {path}", index + 1, input.FilePath);
                }
            });

            // Stop the stopwatch
            stopwatch.Stop();

            // Log how long it all took
            Log.Information("Running algorithm on all files took {time} ms.", stopwatch.ElapsedMilliseconds);
        }

        #endregion
    }
}
