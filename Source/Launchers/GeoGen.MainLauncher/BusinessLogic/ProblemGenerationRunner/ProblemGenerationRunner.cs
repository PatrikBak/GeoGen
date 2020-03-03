﻿using GeoGen.Core;
using GeoGen.Infrastructure;
using GeoGen.ProblemAnalyzer;
using GeoGen.ProblemGenerator;
using GeoGen.TheoremProver;
using GeoGen.TheoremRanker;
using GeoGen.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using static GeoGen.Infrastructure.Log;

namespace GeoGen.MainLauncher
{
    /// <summary>
    /// Represents a runner of <see cref="IProblemGenerator"/> that subsequently uses <see cref="IGeneratedProblemAnalyzer"/>.
    /// <para>
    /// The runner provides lots of useful debugging output configurable via <see cref="ProblemGenerationRunnerSettings"/>. It can write
    /// output with or without proofs into human-readable or JSON format (that can be processed via Drawer). Human-readable files
    /// contain information about simplification and ranking results.
    /// </para>
    /// <para>
    /// It uses <see cref="IBestTheoremFinder"/> to find globally best theorems across multiple runs and provides an option to write
    /// this information into a human-readable or JSON file.
    /// </para>
    /// <para>It also provides <see cref="IInferenceRuleUsageTracker"/> in case we need to analyze how the theorem prover works.</para>
    /// </summary>
    public class ProblemGenerationRunner : IProblemGenerationRunner
    {
        #region Dependencies

        /// <summary>
        /// The generator of problems.
        /// </summary>
        private readonly IProblemGenerator _generator;

        /// <summary>
        /// The analyzer of problem generator outputs.
        /// </summary>
        private readonly IGeneratedProblemAnalyzer _analyzer;

        /// <summary>
        /// The finder of best theorems.
        /// </summary>
        private readonly IBestTheoremFinder _finder;

        /// <summary>
        /// The factory for creating lazy writers of a JSON output.
        /// </summary>
        private readonly IRankedTheoremJsonLazyWriterFactory _factory;

        /// <summary>
        /// The tracker of the used inference rules in theorem proofs.
        /// </summary>
        private readonly IInferenceRuleUsageTracker _tracker;

        #endregion

        #region Private fields

        /// <summary>
        /// The settings for this runner.
        /// </summary>
        private readonly ProblemGenerationRunnerSettings _settings;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ProblemGenerationRunner"/> class.
        /// </summary>
        /// <param name="settings">The settings for this runner.</param>
        /// <param name="generator">The generator of problems.</param>
        /// <param name="analyzer">The analyzer of problem generator outputs.</param>
        /// <param name="finder">The finder of best theorems.</param>
        /// <param name="factory">The factory for creating lazy writers of a JSON output.</param>
        /// <param name="tracker">The tracker of the used inference rules in theorem proofs.</param>
        public ProblemGenerationRunner(ProblemGenerationRunnerSettings settings,
                                       IProblemGenerator generator,
                                       IGeneratedProblemAnalyzer analyzer,
                                       IBestTheoremFinder finder,
                                       IRankedTheoremJsonLazyWriterFactory factory,
                                       IInferenceRuleUsageTracker tracker)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _generator = generator ?? throw new ArgumentNullException(nameof(generator));
            _analyzer = analyzer ?? throw new ArgumentNullException(nameof(analyzer));
            _finder = finder ?? throw new ArgumentNullException(nameof(finder));
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
            _tracker = tracker ?? throw new ArgumentNullException(nameof(tracker));
        }

        #endregion

        #region IProblemGenerationRunner implementation

        /// <inheritdoc/>
        public void Run(LoadedProblemGeneratorInput input)
        {
            #region Prepare readable writers

            // Prepare the name of readable output files
            var nameOfReadableFiles = $"{_settings.OutputFilePrefix}{input.Id}.{_settings.FileExtension}";

            // If we should write readable output without proofs
            using var readableOutputWithoutProofsWriter = _settings.WriteReadableOutputWithoutProofs
                // Prepare the writer for it
                ? new StreamWriter(new FileStream(Path.Combine(_settings.ReadableOutputWithoutProofsFolder, nameOfReadableFiles), FileMode.Create, FileAccess.Write, FileShare.Read))
                // Otherwise null
                : null;

            // If we should write readable output with proofs
            using var readableOutputWithProofsWriter = _settings.WriteReadableOutputWithProofs
                // Prepare the writer for it
                ? new StreamWriter(new FileStream(Path.Combine(_settings.ReadableOutputWithProofsFolder, nameOfReadableFiles), FileMode.Create, FileAccess.Write, FileShare.Read))
                // Otherwise null
                : null;

            // Local function that writes a line to both readable writers, if they are available
            void WriteLineToBothReadableWriters(string line = "")
            {
                // Write to the standard writer
                readableOutputWithoutProofsWriter?.WriteLine(line);

                // Write to the writer with proofs
                readableOutputWithProofsWriter?.WriteLine(line);
            }

            #endregion

            #region Prepare JSON writer

            // Prepare the name of the JSON output file
            var jsonOutputFileName = $"{_settings.OutputFilePrefix}{input.Id}.json";

            // If we should write the JSON output
            var jsonOutputWriter = _settings.WriteJsonOutput
                // Prepare the writer for it
                ? _factory.Create(Path.Combine(_settings.JsonOutputFolder, jsonOutputFileName))
                // Otherwise null
                : null;

            #endregion

            // Call the generation algorithm
            var (initialTheorems, outputs) = _generator.Generate(input);

            #region Write constructions

            // Write the constructions header
            WriteLineToBothReadableWriters($"Constructions:\n");

            // Write all of them
            input.Constructions.ForEach(construction => WriteLineToBothReadableWriters($" - {construction}"));

            // An empty line
            WriteLineToBothReadableWriters();

            #endregion

            #region Write the initial configuration

            // Prepare the formatter for the initial configuration
            var initialFormatter = new OutputFormatter(input.InitialConfiguration.AllObjects);

            // Write it
            WriteLineToBothReadableWriters("Initial configuration:\n");
            WriteLineToBothReadableWriters(initialFormatter.FormatConfiguration(input.InitialConfiguration));

            // Write its theorems, if there are any
            if (initialTheorems.Any())
            {
                WriteLineToBothReadableWriters("\nTheorems:\n");
                WriteLineToBothReadableWriters(InitialTheoremsToString(initialFormatter, initialTheorems));
            }

            #endregion

            #region Write iterations and maximal object counts

            // Write iterations
            WriteLineToBothReadableWriters($"\nIterations: {input.NumberOfIterations}");

            // Write maximal numbers of objects of particular types
            WriteLineToBothReadableWriters($"{input.MaximalNumbersOfObjectsToAdd.Select(pair => $"MaximalNumberOf{pair.Key}s: {pair.Value}").ToJoinedString("\n")}\n");

            #endregion

            // Write results header
            WriteLineToBothReadableWriters($"Results:");

            // Log that we've started
            LoggingManager.LogInfo("Generation has started.");

            #region Tracking variables

            // Prepare the number of generated configurations
            var numberOfGeneratedConfigurations = 0;

            // Prepare the total number of interesting theorems
            var numberOfInterestingTheorems = 0;

            // Prepare the total number of configurations with an interesting theorem
            var numberOfConfigurationsWithInterestingTheorem = 0;

            #endregion

            #region Start stopwatch

            // Prepare a stopwatch to measure the execution time
            var stopwatch = new Stopwatch();

            // Start it
            stopwatch.Start();

            #endregion

            // Begin writing of the JSON output file
            jsonOutputWriter?.BeginWriting();

            #region Generation loop

            // Run the generation
            foreach (var generatorOutput in outputs)
            {
                // Mark the configuration
                numberOfGeneratedConfigurations++;

                #region Logging progress

                // Find out if we should log progress and if yes, do it
                if (_settings.LogProgress && numberOfGeneratedConfigurations % _settings.ProgressLoggingFrequency == 0)
                    // How many configurations did we generate?
                    LoggingManager.LogInfo($"Generated configurations: {numberOfGeneratedConfigurations}, " +
                        // How long did it take?
                        $"after {stopwatch.ElapsedMilliseconds} ms, " +
                        // With the info about the frequency
                        $"{_settings.ProgressLoggingFrequency} in " +
                        // Average time
                        $"{(double)stopwatch.ElapsedMilliseconds / numberOfGeneratedConfigurations * _settings.ProgressLoggingFrequency:F2} ms on average, " +
                        // How many interesting configurations and theorems
                        $"with {numberOfConfigurationsWithInterestingTheorem} interesting configurations ({numberOfInterestingTheorems} theorems in total).");

                #endregion

                // Skip configurations without theorems
                if (generatorOutput.NewTheorems.AllObjects.Count == 0)
                    continue;

                // If this is not a configuration from the last iterations and we should skip those, do it
                if (_settings.AnalyzeOnlyLastIteration && generatorOutput.Configuration.IterationIndex != input.NumberOfIterations)
                    continue;

                // Prepare the output of the analyzer
                GeneratedProblemAnalyzerOutputBase analyzerOutput = null;

                #region Analyzer call

                try
                {
                    // If we should look for proofs (because we should be writing them or analyze the inner inferences)
                    analyzerOutput = _settings.WriteInferenceRuleUsages || _settings.WriteReadableOutputWithProofs
                        // Then call the analysis that construct them
                        ? (GeneratedProblemAnalyzerOutputBase)_analyzer.AnalyzeWithProofConstruction(generatorOutput)
                        // Otherwise we don't need them
                        : _analyzer.AnalyzeWithoutProofConstruction(generatorOutput);
                }
                catch (Exception e)
                {
                    // If there is any sort of problem, we should make aware of it. 
                    LoggingManager.LogError($"There has been an exception while analyzing the configuration:\n\n" +
                        // Write the problematic configuration
                        $"{new OutputFormatter(generatorOutput.Configuration.AllObjects)}\n" +
                        // And also the exception
                        $"Exception: {e}");

                    // And move on, we still might get something cool
                    continue;
                }

                #endregion

                // Count in interesting theorems
                numberOfInterestingTheorems += analyzerOutput.InterestingTheorems.Count;

                // If this is a configuration with an interesting theorem, count it in
                if (analyzerOutput.InterestingTheorems.Any())
                    numberOfConfigurationsWithInterestingTheorem++;

                // Wrap interesting theorems into objects that are tracked by the best theorem finder
                var rankedTheorems = analyzerOutput.InterestingTheorems
                    // Construct the ranked theorem
                    .Select(theorem => new RankedTheorem(theorem, analyzerOutput.TheoremRankings[theorem], generatorOutput.Configuration))
                    // Enumerate
                    .ToArray();

                // Write JSON output
                jsonOutputWriter?.Write(rankedTheorems);

                #region Marking ranked theorems to the finder

                // If we should take just one theorem per configuration
                var theoremsToBeMarked = _settings.TakeAtMostInterestingTheoremPerConfiguration
                    // Then take the first (which has the best ranking)
                    ? rankedTheorems.FirstOrDefault()?.ToEnumerable() ?? Enumerable.Empty<RankedTheorem>()
                    // Otherwise all of them
                    : rankedTheorems;

                // Let the finder judge the theorem or theorems
                _finder.AddTheorems(theoremsToBeMarked, out var bestTheoremsChanged);

                // If we should write best theorems continuously and there are some changes, do it
                if (_settings.WriteBestTheoremsContinuously && bestTheoremsChanged)
                    RewriteBestTheorems();

                #endregion

                #region Human-readable output

                // Prepare a formatter for the generated configuration
                var formatter = new OutputFormatter(generatorOutput.Configuration.AllObjects);

                // Prepare the header so we can measure it
                var header = $"Configuration {numberOfGeneratedConfigurations}";

                // Construct the header with dashes
                var headerWithConfiguration = $"\n{new string('-', header.Length)}\n{header}\n{new string('-', header.Length)}\n\n" +
                    // And the configuration
                    $"{formatter.FormatConfiguration(generatorOutput.Configuration)}";

                #region Writing to the writer of readable output without proofs

                // If there is anything interesting to write
                if (analyzerOutput.InterestingTheorems.Any())
                {
                    // Write the header
                    readableOutputWithoutProofsWriter?.Write(headerWithConfiguration);

                    // Write the analysis results without proofs
                    readableOutputWithoutProofsWriter?.Write(AnalyzerOutputToString(formatter, analyzerOutput, writeProofs: false));

                    // Flush it
                    readableOutputWithProofsWriter?.Flush();
                }

                #endregion

                #region Writing to the writer of output with proofs

                // Write the header
                readableOutputWithProofsWriter?.Write(headerWithConfiguration);

                // Write the analysis results with proofs
                readableOutputWithProofsWriter?.Write(AnalyzerOutputToString(formatter, analyzerOutput, writeProofs: true));

                // Flush it
                readableOutputWithoutProofsWriter?.Flush();

                #endregion

                #endregion

                #region Inference rule usage statistics

                // If we should write inference rule usages
                if (_settings.WriteInferenceRuleUsages)
                {
                    // Mark the proofs
                    _tracker.MarkProofs(((GeneratedProblemAnalyzerOutputWithProofs)analyzerOutput).TheoremProofs.Values);

                    // Prepare the writer
                    using var inferenceRuleUsageWriter = new StreamWriter(new FileStream(_settings.InferenceRuleUsageFilePath, FileMode.Create, FileAccess.Write, FileShare.Read));

                    // Rewrite the stats file
                    inferenceRuleUsageWriter.Write(InferenceRuleUsagesToString());
                }

                #endregion
            }

            #endregion

            #region Writing best theorems

            // If we didn't write best theorems continuously, do it now
            if (!_settings.WriteBestTheoremsContinuously)
                RewriteBestTheorems();

            #endregion

            // Write end
            WriteLineToBothReadableWriters("\n------------------------------------------------");
            WriteLineToBothReadableWriters($"Generated configurations: {numberOfGeneratedConfigurations}");
            WriteLineToBothReadableWriters($"Configurations with an interesting theorem: {numberOfConfigurationsWithInterestingTheorem}");
            WriteLineToBothReadableWriters($"Interesting theorems: {numberOfInterestingTheorems}");
            WriteLineToBothReadableWriters($"Run-time: {stopwatch.ElapsedMilliseconds} ms");

            // Log these stats as well
            LoggingManager.LogInfo($"Generated configurations: {numberOfGeneratedConfigurations}");
            LoggingManager.LogInfo($"Configurations with an interesting theorem: {numberOfConfigurationsWithInterestingTheorem}");
            LoggingManager.LogInfo($"Interesting theorems: {numberOfInterestingTheorems}");
            LoggingManager.LogInfo($"Run-time: {stopwatch.ElapsedMilliseconds} ms");

            // Close the JSON output writer
            jsonOutputWriter?.EndWriting();
        }

        /// <summary>
        /// Rewrites the best theorem files, i.e. the readable file or the JSON file, based on whether we are told to do
        /// it via settings.
        /// </summary>
        private void RewriteBestTheorems()
        {
            // If we should write readable output...
            if (_settings.WriteReadableBestTheoremFile)
            {
                // Prepare the writer
                using var readableBestTheoremWriter = new StreamWriter(new FileStream(_settings.ReadableBestTheoremFilePath, FileMode.Create, FileAccess.Write, FileShare.Read));

                // Rewrite the file
                readableBestTheoremWriter.Write(RankedTheoremsToString(_finder.BestTheorems));
            }

            // If we should write JSON output
            if (_settings.WriteJsonBestTheoremFile)
            {
                // Rewrite the JSON output
                _factory.Create(_settings.JsonBestTheoremFilePath).WriteEagerly(_finder.BestTheorems);
            }
        }

        /// <summary>
        /// Converts given initial theorems to a string. 
        /// </summary>
        /// <param name="initialFormatter">The formatter of the initial configuration.</param>
        /// <param name="initialTheorems">The theorems found in the initial configuration.</param>
        /// <returns>The string representing the theorems.</returns>
        private string InitialTheoremsToString(OutputFormatter initialFormatter, TheoremMap initialTheorems)
            // Process every theorem
            => initialTheorems.AllObjects
                // Use formatter for each
                .Select(initialFormatter.FormatTheorem)
                // Sort them alphabetically
                .Ordered()
                // Append an index to each
                .Select((theoremString, index) => $" {index + 1,2}. {theoremString}")
                // Make each on a separate line
                .ToJoinedString("\n");

        /// <summary>
        /// Converts given ranked theorems to a string.
        /// </summary>
        /// <param name="rankedTheorems">The ranked theorems to be converted.</param>
        /// <returns>The string representing the ranked theorems.</returns>
        private string RankedTheoremsToString(IEnumerable<RankedTheorem> rankedTheorems)
            // Go through the theorems
            => rankedTheorems.Select((rankedTheorem, index) =>
            {
                // Prepare the formatter of the configuration
                var formatter = new OutputFormatter(rankedTheorem.Configuration.AllObjects);

                // Prepare the header
                var header = $"Theorem {index + 1}";

                // Prepare the result where the header is framed in dashes
                var result = $"{new string('-', header.Length)}\n{header}\n{new string('-', header.Length)}\n\n";

                // Add the configuration
                result += formatter.FormatConfiguration(rankedTheorem.Configuration);

                // Add the theorem
                result += $"\n\n{formatter.FormatTheorem(rankedTheorem.Theorem)}" +
                    // With the ranking
                    $" - total ranking {rankedTheorem.Ranking.TotalRanking.ToStringWithDecimalDot()}\n\n";

                // Add the ranking
                result += TheoremRankingToString(rankedTheorem.Ranking);

                // Finally return the result
                return result;
            })
            // Make each on a separate line
            .ToJoinedString("\n\n");

        /// <summary>
        /// Converts given theorems to a string, optionally with their proofs.
        /// </summary>
        /// <param name="formatter">The formatter of the configuration where the theorems hold.</param>
        /// <param name="analyzerOutput">The analyzer output to be converted to a string.</param>
        /// <param name="writeProofs">Indicates whether we should format theorem proofs.</param>
        /// <returns>The string representing the analyzer output.</returns>
        private string AnalyzerOutputToString(OutputFormatter formatter, GeneratedProblemAnalyzerOutputBase analyzerOutput, bool writeProofs)
        {
            // Prepare the result
            var result = "";

            // Prepare a local theorem index
            var localTheoremIndex = 1;

            #region Theorem proofs

            // If we should write proofs and there are any
            if (writeProofs)
            {
                // Get them
                var proofs = ((GeneratedProblemAnalyzerOutputWithProofs)analyzerOutput).TheoremProofs;

                // If there are any
                if (proofs.Any())
                {
                    // Add the header
                    result += $"\n\nProved theorems:\n\n";

                    // Append theorem proofs by taking them
                    result += proofs
                        // Sorting by the statement of the proved theorem
                        .OrderBy(pair => formatter.FormatTheorem(pair.Key))
                        // Write proof for each with a local id
                        .Select(pair => $" {localTheoremIndex}. {formatter.FormatTheoremProof(pair.Value, tag: $"{localTheoremIndex++}.")}")
                        // Make each on a separate line
                        .ToJoinedString("\n");
                }
            }

            #endregion

            #region Simplified theorems

            // If there are any simplified theorems
            if (analyzerOutput.SimplifiedTheorems.Any())
            {
                // Append the header
                result = $"{result.TrimEnd()}\n\nSimplified theorems:\n\n";

                // Append the simplified theorems by taking them
                result += analyzerOutput.SimplifiedTheorems
                    // Sort by their statement
                    .OrderBy(pair => formatter.FormatTheorem(pair.Key))
                    // Handle each pair
                    .Select(pair =>
                    {
                        // Deconstruct
                        var (oldTheorem, simplificationPair) = pair;

                        // Deconstruct
                        var (newTheorem, newConfiguration) = simplificationPair;

                        // Prepare the local result with the local index (while increasing it)
                        var result = $" {localTheoremIndex++}. {formatter.FormatTheorem(oldTheorem)} - can be simplified:\n\n";

                        // Prepare the formatter for the new configuration
                        var newFormatter = new OutputFormatter(newConfiguration.AllObjects);

                        // Add the new configuration
                        result += $"{newFormatter.FormatConfiguration(newConfiguration).Indent(3)}\n\n";

                        // Add the new theorem
                        result += $"{newFormatter.FormatTheorem(newTheorem).Indent(3)}";

                        // Return it
                        return result;
                    })
                    // Make each on a separate line
                    .ToJoinedString("\n\n");
            }

            #endregion

            #region Interesting theorems

            // If there are no interesting theorems, we're done
            if (analyzerOutput.InterestingTheorems.IsEmpty())
                // Return the final string with an empty line
                return $"{result.TrimEnd()}\n";

            // Otherwise add the header
            result = $"{result.TrimEnd()}\n\nInteresting theorems:\n\n";

            // Append interesting theorems by taking them
            result += analyzerOutput.InterestingTheorems
                // Converting each
                .Select((theorem, index) =>
                {
                    // Prepare the local result with the local index (while increasing it)
                    var result = $" {localTheoremIndex++}. {formatter.FormatTheorem(theorem)}";

                    // Write the total ranking
                    result += $" - total ranking {analyzerOutput.TheoremRankings[theorem].TotalRanking.ToStringWithDecimalDot()}\n\n";

                    // Write the ranking table
                    result += TheoremRankingToString(analyzerOutput.TheoremRankings[theorem]);

                    // Return the final result
                    return result;
                })
                // Make each on a separate line
                .ToJoinedString("\n\n");

            #endregion

            // Return the final string with an empty line
            return $"{result.TrimEnd()}\n";
        }

        /// <summary>
        /// Converts a given theorem ranking to a string.
        /// </summary>
        /// <param name="ranking">The ranking to be converted.</param>
        /// <returns>The string representing the ranking table.</returns>
        private static string TheoremRankingToString(TheoremRanking ranking)
            // Take the individual rankings ordered by the total contribution (ASC) and then the aspect name
            => ranking.Rankings.OrderBy(pair => (-pair.Value.Contribution, pair.Key.ToString()))
                // Add each on an individual line with info about the weight
                .Select(pair => $"  {pair.Key,-25}weight = {pair.Value.Weight.ToStringWithDecimalDot(),-10}" +
                    // The ranking
                    $"ranking = {pair.Value.Ranking.ToStringWithDecimalDot(),-10}" +
                    // And the total contribution
                    $"contribution = {pair.Value.Contribution.ToStringWithDecimalDot(),-10}")
                // Make each on a separate line
                .ToJoinedString("\n");

        /// <summary>
        /// Composes the string representing the current usages of inference rules tracked by <see cref="_tracker"/>.
        /// </summary>
        /// <returns>The string representing inference rule usages.</returns>
        private string InferenceRuleUsagesToString()
            // Take the usages
            => _tracker.RuleUsages
                // Sort them by count ASC
                .OrderBy(pair => -pair.Value)
                // Convert each to string
                .Select(pair =>
                {
                    // The rule should be loaded
                    var loadedRule = (LoadedInferenceRule)pair.Key;

                    // Get the count
                    var count = pair.Value;

                    // Compose the string
                    return $"[{count}] --> {loadedRule}";
                })
                // Make each on a separate line
                .ToJoinedString("\n");

        #endregion
    }
}