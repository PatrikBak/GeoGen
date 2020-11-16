using GeoGen.MainLauncher;
using GeoGen.ProblemGenerator;
using GeoGen.ProblemGenerator.InputProvider;
using GeoGen.Utilities;
using Serilog;
using System;
using System.Diagnostics;

namespace GeoGen.ConfigurationGenerationLauncher
{
    /// <summary>
    /// Represents an <see cref="IProblemGenerationRunner"/> that performs the generation algorithm and does nothing
    /// but count the number of generated configurations and messages this count.
    /// </summary>
    public class GenerationOnlyProblemGenerationRunner : IProblemGenerationRunner
    {
        #region Dependencies

        /// <summary>
        /// The generator of problems.
        /// </summary>
        private readonly IProblemGenerator _generator;

        #endregion

        #region Private fields

        /// <summary>
        /// The settings for the runner.
        /// </summary>
        private readonly GenerationOnlyProblemGenerationRunnerSettings _settings;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="GenerationOnlyProblemGenerationRunner"> class.
        /// </summary>
        /// <param name="settings">The settings for the runner.</param>
        /// <param name="generator">The generator of problems.</param>
        public GenerationOnlyProblemGenerationRunner(GenerationOnlyProblemGenerationRunnerSettings settings, IProblemGenerator generator)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _generator = generator ?? throw new ArgumentNullException(nameof(generator));
        }

        #endregion

        #region IProblemGenerationRunner implementation

        /// <inheritdoc/>
        public void Run(LoadedProblemGeneratorInput input)
        {
            // Prepare the variable holding the number of generated configurations
            var generatedConfigurations = 0;

            // Prepare the stopwatch
            var stopwatch = new Stopwatch();

            // Start them
            stopwatch.Start();

            // Run the generation loop
            foreach (var output in _generator.Generate(input).generationOutputs)
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
                        throw new ConfigurationGenerationLauncherException($"Unhandled value of {nameof(CountingMode)}: {_settings.CountingMode}");
                }

                // If we're logging and if we should log
                if (_settings.LogProgress && generatedConfigurations % _settings.GenerationProgresLoggingFrequency == 0)
                {
                    // Prepare the used memory string
                    var usedMemory = ((double)GC.GetTotalMemory(forceFullCollection: true) / 1000000).ToStringWithDecimalDot();

                    // Log the number of generated configurations as well as the memory
                    Log.Information("Number of generated configurations: {count}, used memory: {memory} MB", generatedConfigurations, usedMemory);
                }
            }

            // Stop the timing
            stopwatch.Stop();

            // Log the final number and time
            Log.Information("The total number of generated configurations is {count} in {time} ms.", generatedConfigurations, stopwatch.ElapsedMilliseconds);
        }

        #endregion
    }
}