using GeoGen.Algorithm;
using GeoGen.ConsoleLauncher;
using GeoGen.Core;
using GeoGen.Infrastructure;
using System;

namespace GeoGen.GenerationLauncher
{
    /// <summary>
    /// Represents an <see cref="IAlgorithmRunner"/> that performs the algorithms and does nothing
    /// but count the number of generated configurations and messages this count.
    /// </summary>
    public class GenerationAlgorithmRunner : IAlgorithmRunner
    {
        #region Dependencies

        /// <summary>
        /// The algorithm that is run.
        /// </summary>
        private readonly IAlgorithm _algorithm;

        #endregion

        #region Private fields

        /// <summary>
        /// The settings for the runner.
        /// </summary>
        private readonly GenerationAlgorithmRunnerSettings _settings;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="GenerationAlgorithmRunner"> class.
        /// </summary>
        /// <param name="settings">The settings for the runner.</param>
        /// <param name="algorithm">The algorithm that is run.</param>
        public GenerationAlgorithmRunner(GenerationAlgorithmRunnerSettings settings, IAlgorithm algorithm)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _algorithm = algorithm ?? throw new ArgumentNullException(nameof(algorithm));
        }

        #endregion

        #region IAlgorithmRunner implementation

        /// <summary>
        /// Runs the algorithm on a given output.
        /// </summary>
        /// <param name="input">The input for the algorithm.</param>
        public void Run(LoadedAlgorithmInput input)
        {
            // Prepare the variable holding the number of generated configurations
            var generatedConfigurations = 0;

            // Run the algorithm loop
            foreach (var output in _algorithm.Run(input).generationOutputs)
            {
                // Switch based on how we calculate the count
                switch (_settings.CountingMode)
                {
                    // If we're counting in only the last iteration
                    case CountingMode.LastIteration:

                        // Check if this is the last iteration
                        if (output.Configuration.IterationIndex == input.NumberOfIterations)
                            // Count it in
                            generatedConfigurations++;

                        // Otherwise we may move on
                        else
                            continue;

                        break;

                    // If we're counting in everything
                    case CountingMode.All:

                        // Do it
                        generatedConfigurations++;

                        break;

                    // Unhandled cases
                    default:
                        throw new GeoGenException($"Unhandled value of {nameof(CountingMode)}: {_settings.CountingMode}");
                }

                // If we're logging and if we should log, do it
                if (_settings.LogProgress && generatedConfigurations % _settings.GenerationProgresLoggingFrequency == 0)
                    Log.LoggingManager.LogInfo($"Number of generated configurations: {generatedConfigurations}");
            }

            // Log the final number
            Log.LoggingManager.LogInfo($"The total number of generated configurations is {generatedConfigurations}");
        }

        #endregion
    }
}
