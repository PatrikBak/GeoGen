using GeoGen.Core;
using GeoGen.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace GeoGen.ConsoleLauncher
{
    /// <summary>
    /// The default implementation of <see cref="AlgorithmRunner"/>.
    /// </summary>
    public class AlgorithmRunner : IAlgorithmRunner
    {
        #region Dependencies

        /// <summary>
        /// The algorithm that is run.
        /// </summary>
        private readonly IAlgorithm _algorithm;

        /// <summary>
        /// The finder of all theorems used in the initial configuration.
        /// </summary>
        private readonly ICompleteTheoremsFinder _finder;

        #endregion

        #region Private fields

        /// <summary>
        /// The settings for this runner.
        /// </summary>
        private readonly AlgorithmRunnerSettings _settings;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="AlgorithmRunner"/> class.
        /// </summary>
        /// <param name="settings">The settings for this runner.</param>
        /// <param name="algorithm">The algorithm that is run.</param>
        /// <param name="finder">The finder of all theorems used in the initial configuration.</param>
        public AlgorithmRunner(AlgorithmRunnerSettings settings, IAlgorithm algorithm, ICompleteTheoremsFinder finder)
        {
            _algorithm = algorithm ?? throw new ArgumentNullException(nameof(algorithm));
            _finder = finder ?? throw new ArgumentNullException(nameof(finder));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        #endregion

        #region IAlgorithmRunner implementation

        /// <summary>
        /// Runs the algorithm on a given output and outputs the results to a given text writer.
        /// </summary>
        /// <param name="input">The input for the algorithm.</param>
        /// <param name="outputWriter">The writer where the results are written.</param>
        public void Run(AlgorithmInput input, TextWriter outputWriter)
        {
            // Helper function that writes theorems if there are any
            void WriteTheorems(OutputFormatter formatter, List<Theorem> theorems)
            {
                // If there are theorems
                if (theorems.Count != 0)
                {
                    // Write them
                    outputWriter.WriteLine("\nTheorems:\n");
                    outputWriter.WriteLine(formatter.FormatTheorems(theorems));
                    outputWriter.WriteLine();
                }
            }

            // Prepare the formatter for the initial configuration
            var initialFormatter = new OutputFormatter(input.GeneratorInput.InitialConfiguration);

            // Write it with theorems
            outputWriter.WriteLine("Initial configuration:");
            outputWriter.WriteLine("------------------------------------------------");
            outputWriter.WriteLine(initialFormatter.FormatConfiguration());
            WriteTheorems(initialFormatter, _finder.FindAllTheorems(input.GeneratorInput.InitialConfiguration));
            outputWriter.WriteLine();

            // Write iterations
            outputWriter.WriteLine($"Iterations: {input.GeneratorInput.NumberOfIterations}\n");

            // Write constructions
            outputWriter.WriteLine($"Constructions:\n");
            input.GeneratorInput.Constructions.ForEach(construction => outputWriter.WriteLine($" - {construction}"));
            outputWriter.WriteLine();

            // Write results header
            outputWriter.WriteLine($"Results:\n");
            outputWriter.WriteLine();

            // Log that we've started
            Log.LoggingManager.LogInfo("Algorithm has started.");

            // Prepare the number of generated configurations
            var generatedConfigurations = 0;

            // Prepare a stopwatch to measure the time
            var stopwatch = new Stopwatch();

            // Start it
            stopwatch.Start();

            // Run the algorithm
            foreach (var algorithmOutput in _algorithm.GenerateOutputs(input))
            {
                // Mark the configuration
                generatedConfigurations++;

                // Find out if we should log and if yes, do it
                if (_settings.LogProgress && generatedConfigurations % _settings.GenerationProgresLoggingFrequency == 0)
                    Log.LoggingManager.LogInfo($"Number of generated configurations: {generatedConfigurations}, after {stopwatch.ElapsedMilliseconds} milliseconds.");

                // Skip configurations without theorems
                if (algorithmOutput.Theorems.Count == 0)
                    continue;

                // Prepare the formatter for the generated configuration
                var formatter = new OutputFormatter(algorithmOutput.GeneratorOutput.Configuration);

                // Write the configuration with its theorems
                outputWriter.WriteLine("------------------------------------------------");
                outputWriter.WriteLine($"{generatedConfigurations}.");
                outputWriter.WriteLine("------------------------------------------------");
                outputWriter.WriteLine();
                outputWriter.WriteLine(formatter.FormatConfiguration());
                WriteTheorems(formatter, algorithmOutput.Theorems);
            }

            // Log that we're done
            Log.LoggingManager.LogInfo($"Algorithm has finished, the number of generated configurations is {generatedConfigurations}, the running time {stopwatch.ElapsedMilliseconds} milliseconds.");
        }

        #endregion
    }
}
