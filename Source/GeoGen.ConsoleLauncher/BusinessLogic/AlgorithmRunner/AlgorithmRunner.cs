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
        public AlgorithmRunner(AlgorithmRunnerSettings settings, IAlgorithm algorithm)
        {
            _algorithm = algorithm ?? throw new ArgumentNullException(nameof(algorithm));
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
            // Call the algorithm
            var (initialTheorems, outputs) = _algorithm.Run(input);

            #region Prepare writers

            // Prepare the output path
            var outputPath = Path.Combine(_settings.OutputFolder, $"{_settings.OutputFilePrefix}{input.Id}.{_settings.OutputFileExtention}");

            // Prepare the writer for the output
            using var outputWriter = new StreamWriter(new FileStream(outputPath, FileMode.Create, FileAccess.ReadWrite));

            // Prepare the path for the full output
            var fullOutputPath = Path.Combine(_settings.OutputFolder, $"{_settings.OutputFilePrefix}{input.Id}{_settings.FullReportSuffix}.{_settings.OutputFileExtention}");

            // Prepare the writer for the full output, if it's requested
            using var fullOutputWriter = _settings.GenerateFullReport ? new StreamWriter(new FileStream(fullOutputPath, FileMode.Create, FileAccess.ReadWrite)) : null;

            // Helper function that writes to both writes
            void WriteLineToBoth(string line = "")
            {
                // Write to the default
                outputWriter.WriteLine(line);

                // Write to the full, if it's specified
                fullOutputWriter?.WriteLine(line);
            }

            // Helper function that writes to the full writer
            void WriteLineToFull(string line = "") => fullOutputWriter?.WriteLine(line);

            #endregion            

            #region Writing initial configuration

            // Prepare the formatter for the initial configuration
            var initialFormatter = new OutputFormatter(input.InitialConfiguration);

            // Write it
            WriteLineToBoth("Initial configuration:");
            WriteLineToBoth();
            WriteLineToBoth(initialFormatter.FormatConfiguration());

            // Write its theorems, if there are any
            if (initialTheorems.Any())
            {
                WriteLineToBoth("\nTheorems:\n");
                WriteLineToBoth(initialTheorems.AllObjects.Select(t => $" - {initialFormatter.FormatTheorem(t)}").ToJoinedString("\n"));
            }

            #endregion

            #region Writing Iterations, Constructions and Results title

            // Write iterations
            WriteLineToBoth($"\nIterations: {input.NumberOfIterations}\n");

            // Write constructions
            WriteLineToBoth($"Constructions:\n");
            input.Constructions.ForEach(construction => WriteLineToBoth($" - {construction}"));
            WriteLineToBoth();

            // Write results header
            WriteLineToBoth($"Results:");

            #endregion

            // Log that we've started
            Log.LoggingManager.LogInfo("Algorithm has started.");

            #region Tracking variables

            // Prepare the number of generated configurations
            var generatedConfigurations = 0;

            // Prepare the total number of theorems
            var totalTheorems = 0;

            // Prepare the number of interesting theorems
            var interestingTheorems = 0;

            #endregion

            #region Start stopwatch

            // Prepare a stopwatch to measure the time
            var stopwatch = new Stopwatch();

            // Start it
            stopwatch.Start();

            #endregion

            // Get the 

            // Run the algorithm
            foreach (var algorithmOutput in outputs)
            {
                // Mark the configuration
                generatedConfigurations++;

                // Find out if we should log and if yes, do it
                if (_settings.LogProgress && generatedConfigurations % _settings.GenerationProgresLoggingFrequency == 0)
                    Log.LoggingManager.LogInfo($"Number of generated configurations: {generatedConfigurations}, after {stopwatch.ElapsedMilliseconds} milliseconds.");

                // Skip configurations without theorems
                if (algorithmOutput.Theorems.Count == 0)
                    continue;

                // Count all theorems
                totalTheorems += algorithmOutput.Theorems.Count;

                // Count interesting theorems
                interestingTheorems += algorithmOutput.Theorems.Count - algorithmOutput.AnalyzerOutput.Count;

                // Find out if there is any interesting theorem
                var anyInterestingTheorem = algorithmOutput.AnalyzerOutput.Count != algorithmOutput.Theorems.Count;

                // Prepare the formatter for the generated configuration
                var formatter = new OutputFormatter(algorithmOutput.Configuration);

                // Prepare the writing function for this case
                Action<string> WriteLine = anyInterestingTheorem ? (Action<string>)WriteLineToBoth : WriteLineToFull;

                // Write the configuration
                WriteLine("\n------------------------------------------------");
                WriteLine($"Configuration {generatedConfigurations}");
                WriteLine("------------------------------------------------");
                WriteLine("");
                WriteLine(formatter.FormatConfiguration());

                // Write the title
                WriteLine("\nTheorems:\n");

                // Write the basic theorem string to the default output
                if (anyInterestingTheorem)
                    outputWriter.WriteLine(TheoremsToString(formatter, algorithmOutput.Theorems, algorithmOutput.AnalyzerOutput, includeResolved: false));

                // If we're requested, write the full too
                WriteLineToFull(TheoremsToString(formatter, algorithmOutput.Theorems, algorithmOutput.AnalyzerOutput, includeResolved: true));
            }

            // Write end
            WriteLineToBoth("\n------------------------------------------------");
            WriteLineToBoth($"Generated configurations: {generatedConfigurations}");
            WriteLineToBoth($"Theorems: {totalTheorems}");
            WriteLineToBoth($"Interesting theorems: {interestingTheorems}");
            WriteLineToBoth($"Run-time: {stopwatch.ElapsedMilliseconds} ms");

            // Log that we're done
            Log.LoggingManager.LogInfo($"Algorithm has finished in {stopwatch.ElapsedMilliseconds} ms.");
        }

        /// <summary>
        /// Writes given theorems as a string with potential feedback.
        /// </summary>
        /// <param name="formatter">The formatter of the configuration where the theorems hold.</param>
        /// <param name="theorems">The theorems to be written.</param>
        /// <param name="feedback">The feedback from the theorems analyzer.</param>
        /// <param name="includeResolved">Indicates if we should include the resolved theorems as well.</param>
        /// <returns>The string representing the theorems.</returns>
        private string TheoremsToString(OutputFormatter formatter, IReadOnlyList<Theorem> theorems, Dictionary<Theorem, TheoremFeedback> feedback, bool includeResolved)
        {
            // Convert either all theorems, if we should include resolved, or only not resolved ones
            return theorems.Where(theorem => includeResolved || !feedback.ContainsKey(theorem)).Select((theorem, index) =>
            {
                // Get the basic string from the formatter
                var theoremString = $" {index + 1,2}. {formatter.FormatTheorem(theorem)}";

                // If the theorem has no feedback, we can't write more
                if (!feedback.ContainsKey(theorem))
                    return theoremString;

                // Otherwise switch on the feedback
                switch (feedback[theorem])
                {
                    // Trivial theorem
                    case TrivialTheoremFeedback _:
                        return $"{theoremString} - trivial theorem";

                    // Sub-theorem
                    case SubtheoremFeedback subtheoremFeedback:

                        // In this case we know the template theorem has our additional info
                        var templateTheorem = (TemplateTheorem)subtheoremFeedback.TemplateTheorem;

                        // We can now construct more descriptive string
                        return $"{theoremString} - sub-theorem implied from theorem {templateTheorem.Number} from file {templateTheorem.FileName}";

                    // Definable in a simpler configuration
                    case DefineableSimplerFeedback _:
                        return $"{theoremString} - can be defined in a simpler configuration";

                    // Transitivity
                    case TransitivityFeedback transitivityFeedback:

                        // Local function that converts a fact to a string
                        string FactToString(Theorem fact)
                        {
                            // Try to find it in our theorems
                            var equalTheoremIndex = theorems.IndexOf(fact, Theorem.EquivalencyComparer);

                            // If it's found, i.e. not -1, then return just the number
                            if (equalTheoremIndex != -1)
                                return $"{equalTheoremIndex + 1}";

                            // Otherwise Convert the fact
                            return $"{formatter.FormatTheorem(fact, includeType: false)} (this is true in a simpler configuration)";
                        }

                        // Compose the final string
                        return $"{theoremString} - is true because of {FactToString(transitivityFeedback.Fact1)} and {FactToString(transitivityFeedback.Fact2)}";

                    // Otherwise...
                    default:
                        throw new GeoGenException($"Unhandled type of feedback: {feedback[theorem].GetType()}");
                }
            })
            // Make each on a separate line
            .ToJoinedString("\n");
        }

        #endregion
    }
}