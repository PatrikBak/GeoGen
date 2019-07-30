using GeoGen.Core;
using GeoGen.TheoremsAnalyzer;
using GeoGen.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

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
        /// <param name="analyzer">The analyzer of theorem providing feedback whether they are olympiad or not.</param>
        public AlgorithmRunner(AlgorithmRunnerSettings settings, IAlgorithm algorithm, ICompleteTheoremsFinder finder)
        {
            _algorithm = algorithm ?? throw new ArgumentNullException(nameof(algorithm));
            _finder = finder ?? throw new ArgumentNullException(nameof(finder));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        #endregion

        #region IAlgorithmRunner implementation

        /// <summary>
        /// Runs the algorithm on a given output.
        /// </summary>
        /// <param name="input">The input for the algorithm.</param>
        public void Run(LoadedGeneratorInput input)
        {
            // Prepare the output path
            var outputPath = Path.Combine(_settings.OutputFolder, $"{_settings.OutputFilePrefix}{input.Id}.{_settings.OutputFileExtention}");

            // Prepare the writer for the output
            using (var outputWriter = new StreamWriter(new FileStream(outputPath, FileMode.Create, FileAccess.Write)))
            {
                // Prepare the formatter for the initial configuration
                var initialFormatter = new OutputFormatter(input.InitialConfiguration);

                // Find its theorem
                var initialTheorems = _finder.FindAllTheorems(input.InitialConfiguration);

                // Write it
                outputWriter.WriteLine("Initial configuration:");
                outputWriter.WriteLine();
                outputWriter.WriteLine(initialFormatter.FormatConfiguration());

                // Write its theorems, if there are any
                if (initialTheorems.Any())
                {
                    outputWriter.WriteLine("\nTheorems:\n");
                    outputWriter.WriteLine(initialTheorems.Select(initialFormatter.FormatTheorem).Select(s => $" - {s}").ToJoinedString("\n"));
                }

                // Write iterations
                outputWriter.WriteLine($"\nIterations: {input.NumberOfIterations}\n");

                // Write constructions
                outputWriter.WriteLine($"Constructions:\n");
                input.Constructions.ForEach(construction => outputWriter.WriteLine($" - {construction}"));
                outputWriter.WriteLine();

                // Write results header
                outputWriter.WriteLine($"Results:");
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

                    // Write the configuration
                    outputWriter.WriteLine("------------------------------------------------");
                    outputWriter.WriteLine($"{generatedConfigurations}.");
                    outputWriter.WriteLine("------------------------------------------------");
                    outputWriter.WriteLine();
                    outputWriter.WriteLine(formatter.FormatConfiguration());

                    // Write theorems
                    outputWriter.WriteLine("\nTheorems:\n");
                    outputWriter.WriteLine(TheoremsToString(formatter, algorithmOutput.Theorems, algorithmOutput.AnalyzerOutput));
                    outputWriter.WriteLine();
                }

                // Log that we're done
                Log.LoggingManager.LogInfo($"Algorithm has finished, the number of generated configurations is {generatedConfigurations}, the running time {stopwatch.ElapsedMilliseconds} milliseconds.\n");
            }
        }

        /// <summary>
        /// Writes given theorems as a string with potential feedback.
        /// </summary>
        /// <param name="formatter">The formatter of the configuration where the theorems hold.</param>
        /// <param name="theorems">The theorems to be written.</param>
        /// <param name="feedback">The feedback from the theorems analyzer.</param>
        /// <returns>The string representing the theorems.</returns>
        private string TheoremsToString(OutputFormatter formatter, List<Theorem> theorems, Dictionary<Theorem, TheoremFeedback> feedback)
        {
            // Convert all theorems
            return theorems.Select(theorem =>
            {
                // Get the basic string from the formatter
                var theoremString = formatter.FormatTheorem(theorem);

                // Find the feedback part (this will be changed later)
                var feedbackString = !feedback.ContainsKey(theorem) ? "" : " - trivial theorem";

                // Construct the final string
                return $" - {theoremString}{feedbackString}";

            })
            // Make each on a separate line
            .ToJoinedString("\n");
        }

        #endregion
    }
}