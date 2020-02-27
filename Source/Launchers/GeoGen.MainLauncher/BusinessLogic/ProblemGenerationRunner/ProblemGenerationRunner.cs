using GeoGen.Core;
using GeoGen.Infrastructure;
using GeoGen.ProblemAnalyzer;
using GeoGen.ProblemGenerator;
using GeoGen.TheoremProver;
using GeoGen.TheoremRanker;
using GeoGen.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
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
        /// The writer used to write best theorems to a file.
        /// </summary>
        private readonly IRankedTheoremWriter _writer;

        /// <summary>
        /// The factory for creating a lazy writer of a JSON output.
        /// </summary>
        private readonly ITheoremWithRankingJsonLazyWriterFactory _writerFactory;

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
        /// <param name="writer">The writer used to write best theorems to a file.</param>
        /// <param name="writerFactory">The factory for creating a lazy writer of a JSON output.</param>
        /// <param name="tracker">The tracker of the used inference rules in theorem proofs.</param>
        public ProblemGenerationRunner(ProblemGenerationRunnerSettings settings,
                                       IProblemGenerator generator,
                                       IGeneratedProblemAnalyzer analyzer,
                                       IBestTheoremFinder finder,
                                       IRankedTheoremWriter writer,
                                       ITheoremWithRankingJsonLazyWriterFactory writerFactory,
                                       IInferenceRuleUsageTracker tracker)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _generator = generator ?? throw new ArgumentNullException(nameof(generator));
            _analyzer = analyzer ?? throw new ArgumentNullException(nameof(analyzer));
            _finder = finder ?? throw new ArgumentNullException(nameof(finder));
            _writer = writer ?? throw new ArgumentNullException(nameof(writer));
            _writerFactory = writerFactory ?? throw new ArgumentNullException(nameof(writerFactory));
            _tracker = tracker ?? throw new ArgumentNullException(nameof(tracker));
        }

        #endregion

        #region IProblemGenerationRunner implementation

        /// <inheritdoc/>
        public void Run(LoadedProblemGeneratorInput input)
        {
            // Call the generation algorithm
            var (initialTheorems, outputs) = _generator.Generate(input);

            #region Prepare writers

            // Prepare the name of the JSON output file
            var jsonOutputFileName = $"{_settings.OutputFilePrefix}{input.Id}.json";

            // Prepare the writer of all the output as JSON
            var outputJsonWriter = _writerFactory.Create(Path.Combine(_settings.OutputJsonFolder, jsonOutputFileName));

            // Prepare the writer of best theorems
            var bestTheoremsJsonWriter = _writerFactory.Create(_settings.BestTheoremJsonFilePath);

            // Prepare the name of the other human-readable output files
            var fileName = $"{_settings.OutputFilePrefix}{input.Id}.{_settings.FileExtension}";

            // Prepare the writer for the output
            using var outputWriter = new StreamWriter(new FileStream(Path.Combine(_settings.OutputFolder, fileName), FileMode.Create, FileAccess.Write, FileShare.Read));

            // If we should generate the output with attempts
            using var outputWithProofsWriter = _settings.WriteOutputWithProofs
                // Prepare the writer for it
                ? new StreamWriter(new FileStream(Path.Combine(_settings.OutputFolderWithProofs, fileName), FileMode.Create, FileAccess.Write, FileShare.Read))
                // Otherwise null
                : null;

            // Local function that writes a line to all non-JSON writers
            void WriteLineToAllWriters(string line = "")
            {
                // Write to the main output
                outputWriter.WriteLine(line);

                // Write to the output with attempts file if it is provided
                outputWithProofsWriter?.WriteLine(line);
            }

            #endregion            

            #region Writing constructions

            // Write constructions header
            WriteLineToAllWriters($"Constructions:\n");

            // Write all of them
            input.Constructions.ForEach(construction => WriteLineToAllWriters($" - {construction}"));

            // An empty line
            WriteLineToAllWriters();

            #endregion

            #region Writing initial configuration

            // Prepare the formatter for the initial configuration
            var initialFormatter = new OutputFormatter(input.InitialConfiguration.AllObjects);

            // Write it
            WriteLineToAllWriters("Initial configuration:\n");
            WriteLineToAllWriters(initialFormatter.FormatConfiguration(input.InitialConfiguration));

            // Write its theorems, if there are any
            if (initialTheorems.Any())
            {
                WriteLineToAllWriters("\nTheorems:\n");
                WriteLineToAllWriters(InitialTheoremsToString(initialFormatter, initialTheorems));
            }

            #endregion

            #region Write iterations and maximal object counts

            // Write iterations
            WriteLineToAllWriters($"\nIterations: {input.NumberOfIterations}");

            // Write maximal numbers of objects of particular types
            WriteLineToAllWriters($"{input.MaximalNumbersOfObjectsToAdd.Select(pair => $"MaximalNumberOf{pair.Key}s: {pair.Value}").ToJoinedString("\n")}\n");

            #endregion

            // Write results header
            WriteLineToAllWriters($"Results:");

            // Log that we've started
            LoggingManager.LogInfo("Generation has started.");

            #region Tracking variables

            // Prepare the number of generated configurations
            var numberOfGeneratedConfigurations = 0;

            // Prepare the total number of interesting theorems, i.e. unproven and unsimplifiable ones
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
            outputJsonWriter.BeginWriting();

            #region Generation loop

            // Run the generation
            foreach (var generatorOutput in outputs)
            {
                // Mark the configuration
                numberOfGeneratedConfigurations++;

                #region Logging

                // Find out if we should log progress and if yes, do it
                if (_settings.LogProgress && numberOfGeneratedConfigurations % _settings.GenerationProgresLoggingFrequency == 0)
                    // How many configurations did we generate?
                    LoggingManager.LogInfo($"Generated configurations: {numberOfGeneratedConfigurations}, " +
                        // How long did it take?
                        $"after {stopwatch.ElapsedMilliseconds} ms, " +
                        // With the info about the frequency
                        $"{_settings.GenerationProgresLoggingFrequency} in " +
                        // Average time
                        $"{(double)stopwatch.ElapsedMilliseconds / numberOfGeneratedConfigurations * _settings.GenerationProgresLoggingFrequency:F2} ms on average, " +
                        // How many interesting configurations and theorems
                        $"with {numberOfConfigurationsWithInterestingTheorem} interesting configurations ({numberOfInterestingTheorems} theorems in total).");

                #endregion

                // Skip configurations without theorems
                if (generatorOutput.NewTheorems.AllObjects.Count == 0)
                    continue;

                // Call the analyzer 
                var analyzerOutput = _analyzer.AnalyzeWithProofConstruction(generatorOutput);

                // Count in interesting theorems
                numberOfInterestingTheorems += analyzerOutput.InterestingTheorems.Count;

                // If this is a configuration with an interesting theorem, count it in
                if (analyzerOutput.InterestingTheorems.Any())
                    numberOfConfigurationsWithInterestingTheorem++;

                #region JSON output

                // Wrap interesting theorems into objects that will be serialized to JSON
                var theoremsWithRanking = analyzerOutput.InterestingTheorems.Select(theorem =>
                {
                    // Get the configuration based on whether the simplification was successful
                    var finalConfiguration = analyzerOutput.SimplifiedTheorems.GetValueOrDefault(theorem).newConfiguration ?? generatorOutput.Configuration;

                    // Get the configuration based on whether the simplification was successful
                    var finalTheorem = analyzerOutput.SimplifiedTheorems.GetValueOrDefault(theorem).newTheorem ?? theorem;

                    // Get the ranking 
                    var ranking = analyzerOutput.TheoremRankings[theorem];

                    // Construct the theorem with ranking
                    return new TheoremWithRanking(finalTheorem, ranking, finalConfiguration);
                })
                // Enumerate
                .ToArray();

                // Write JSON output
                outputJsonWriter.Write(theoremsWithRanking);

                #endregion

                #region Human-readable output

                // Prepare a formatter for the generated configuration
                var formatter = new OutputFormatter(generatorOutput.Configuration.AllObjects);

                // Local function to write line based on the number of interesting theorems
                void WriteLine(string line = "")
                {
                    // If there is any interesting theorem
                    if (analyzerOutput.InterestingTheorems.Any())
                    {
                        // Write to all writes, which will take care of potentially not requested ones
                        WriteLineToAllWriters(line);

                        // Terminate
                        return;
                    }

                    // Otherwise there are no interesting theorems. We might however want
                    // to write the proofs of the proven ones. If that is requested, do it
                    outputWithProofsWriter?.WriteLine(line);
                }

                // Prepare the header
                var header = $"Configuration {numberOfGeneratedConfigurations}";

                // Write the configuration
                WriteLine();
                WriteLine(new string('-', header.Length));
                WriteLine(header);
                WriteLine(new string('-', header.Length));
                WriteLine();
                WriteLine(formatter.FormatConfiguration(generatorOutput.Configuration));
                WriteLine();

                // Prepare the global index of the first interesting theorem (starting from 1)
                // The idea is to have these indices in our output files so we can then use them
                // in Drawer to quickly identify them.
                var firstInterestingTheoremIndex = numberOfInterestingTheorems - analyzerOutput.InterestingTheorems.Count + 1;

                // If there is any interesting theorem
                if (analyzerOutput.InterestingTheorems.Any())
                {
                    // Then write them to the standard output writer
                    outputWriter.WriteLine(TheoremsToString(formatter,
                        // Proofs are not relevant now
                        theoremProofs: null,
                        // In this case we don't want to write proofs
                        writeUninterestingTheorems: false,
                        // Pass other interesting data
                        analyzerOutput.InterestingTheorems, analyzerOutput.TheoremRankings, analyzerOutput.SimplifiedTheorems,
                        // Include the global index of the first interesting theorem
                        firstInterestingTheoremGlobalIndex: firstInterestingTheoremIndex));
                }

                // In any case (whether we have or haven't interesting theorems) we want to write the info
                // to the writer of theorems with attempts and proofs, if it's provided
                outputWithProofsWriter?.WriteLine(TheoremsToString(formatter,
                        // Set the proofs
                        theoremProofs: analyzerOutput.TheoremProofs,
                        // In this case we do want to write proofs
                        writeUninterestingTheorems: true,
                        // Pass other interesting data
                        analyzerOutput.InterestingTheorems, analyzerOutput.TheoremRankings, analyzerOutput.SimplifiedTheorems,
                        // Include the global index of the first interesting theorem
                        firstInterestingTheoremGlobalIndex: firstInterestingTheoremIndex));

                // Flush the writers after each output so that we can see the results gradually
                outputWriter.Flush();
                outputWithProofsWriter?.Flush();

                #endregion

                #region Best theorem finder 

                // Let the finder judge them
                _finder.AddTheorems(theoremsWithRanking, out var bestTheoremsChanged);

                // If there are some change in the global ladder
                if (bestTheoremsChanged)
                {
                    // Prepare an enumerable that identifies each so they could be found back in the output file
                    var identifiedTheorems = _finder.BestTheorems.Select(theorem => (theorem, $"configuration {numberOfGeneratedConfigurations} of the output file {fileName}"));

                    // Write them
                    _writer.WriteTheorems(identifiedTheorems, _settings.BestTheoremReadableFilePath);

                    // Write their JSON version too
                    bestTheoremsJsonWriter.WriteEagerly(_finder.BestTheorems);
                }

                #endregion

                #region Inference rule usage statistics

                // Track the proofs
                _tracker.MarkProofs(analyzerOutput.TheoremProofs.Values);

                // Prepare the string representing the current state by taking the usages
                var usagesString = _tracker.UsedRulesCounts
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
                    // Make each on a line new
                    .ToJoinedString("\n");

                // Prepare the writer for the statistics
                using var inferenceRuleUsageWriter = new StreamWriter(new FileStream(_settings.InferenceRuleUsageFile, FileMode.Create, FileAccess.Write, FileShare.Read));

                // Rewrite the stats file
                inferenceRuleUsageWriter.Write(usagesString);

                #endregion
            }

            #endregion

            // Write end
            WriteLineToAllWriters("\n------------------------------------------------");
            WriteLineToAllWriters($"Generated configurations: {numberOfGeneratedConfigurations}");
            WriteLineToAllWriters($"Configurations with an interesting theorem: {numberOfConfigurationsWithInterestingTheorem}");
            WriteLineToAllWriters($"Interesting theorems: {numberOfInterestingTheorems}");
            WriteLineToAllWriters($"Run-time: {stopwatch.ElapsedMilliseconds} ms");

            // Log these stats as well
            LoggingManager.LogInfo($"Generated configurations: {numberOfGeneratedConfigurations}");
            LoggingManager.LogInfo($"Configurations with an interesting theorem: {numberOfConfigurationsWithInterestingTheorem}");
            LoggingManager.LogInfo($"Interesting theorems: {numberOfInterestingTheorems}");
            LoggingManager.LogInfo($"Run-time: {stopwatch.ElapsedMilliseconds} ms");

            // Close the JSON output writer
            outputJsonWriter.EndWriting();
        }

        /// <summary>
        /// Converts given initial theorems to a string. 
        /// </summary>
        /// <param name="initialFormatter">The formatter of the initial configuration.</param>
        /// <param name="initialTheorems">The theorems found in the initial configuration</param>
        /// <returns>The string representing the theorems.</returns>
        private string InitialTheoremsToString(OutputFormatter initialFormatter, TheoremMap initialTheorems)
        {
            // Process every theorem
            return initialTheorems.AllObjects
                // Use formatter for each
                .Select(initialFormatter.FormatTheorem)
                // Sort them alphabetically
                .Ordered()
                // Append an index to each
                .Select((theoremString, index) => $" {index + 1,2}. {theoremString}")
                // Make each on a separate line
                .ToJoinedString("\n");
        }

        /// <summary>
        /// Converts given theorems to a string, optionally with their proofs or attempts to prove.
        /// </summary>
        /// <param name="formatter">The formatter of the configuration where the theorems hold.</param>
        /// <param name="theoremProofs">The dictionary mapping theorems to their proofs.</param>
        /// <param name="writeUninterestingTheorems">Indicates whether we should write uninteresting theorems, i.e. proved or definable simpler ones.</param>
        /// <param name="interestingTheorems">The list of interesting theorems, i.e. unproven and undefinable simpler.</param>
        /// <param name="rankings">The dictionary mapping theorems to their ranking.</param>
        /// <param name="simplifiedTheorems">The dictionary mapping theorems to their simplified versions.</param>
        /// <param name="firstInterestingTheoremGlobalIndex">The global index of the first interesting theorem.</param>
        /// <returns>The string representing the theorems.</returns>
        private string TheoremsToString(OutputFormatter formatter,
                                        IReadOnlyDictionary<Theorem, TheoremProof> theoremProofs,
                                        bool writeUninterestingTheorems,
                                        IReadOnlyList<Theorem> interestingTheorems,
                                        IReadOnlyDictionary<Theorem, TheoremRanking> rankings,
                                        IReadOnlyDictionary<Theorem, (Theorem newTheorem, Configuration newConfiguration)> simplifiedTheorems,
                                        int firstInterestingTheoremGlobalIndex)
        {
            // Prepare the result
            var result = "";

            // Prepare a local theorem index
            var localTheoremIndex = 1;

            #region Proven theorems

            // If we should write uninteresting theorems and there are any proven theorems...
            if (writeUninterestingTheorems && theoremProofs.Any())
            {
                // Add the header
                result = $"{result.TrimEnd()}\n\nProved theorems:\n\n";

                // Append theorem proofs by taking them
                result += theoremProofs
                    // Sorting by the statement of the proved theorem
                    .OrderBy(pair => formatter.FormatTheorem(pair.Key))
                    // Write proof for each with a local id
                    .Select(pair => $"{localTheoremIndex++}. {formatter.FormatTheoremProof(pair.Value)}")
                    // Make each on a separate line
                    .ToJoinedString("\n")
                    // Ensure no white spaces at the end
                    .TrimEnd();
            }

            #endregion

            #region Interesting theorems

            // If there are no interesting theorems, we're done
            if (interestingTheorems.IsEmpty())
                return result.Trim();

            // Add the header
            result = $"{result.TrimEnd()}\n\nInteresting theorems:\n\n";

            // Append interesting theorems by taking them
            result += interestingTheorems
                // Converting each
                .Select((theorem, index) =>
                {
                    // Prepare the local result with the global and local index
                    var result = $"{localTheoremIndex++}. [{firstInterestingTheoremGlobalIndex + index}] {formatter.FormatTheorem(theorem)}";

                    #region Writing ranking

                    // Write the total ranking
                    result += $" - total ranking {rankings[theorem].TotalRanking.ToString("0.##", CultureInfo.InvariantCulture)}\n\n";

                    // Add individual rankings ordered by the total contribution (ASC) and then the aspect name
                    rankings[theorem].Rankings.OrderBy(pair => (-pair.Value.Contribution, pair.Key.ToString()))
                        // Add each on an individual line with info about the weight
                        .ForEach(pair => result += $"  {pair.Key,-25}weight = {pair.Value.Weight.ToString("0.##", CultureInfo.InvariantCulture),-10}" +
                            // The ranking
                            $"ranking = {pair.Value.Ranking.ToString("0.##", CultureInfo.InvariantCulture),-10}" +
                            // And the total contribution
                            $"contribution = {pair.Value.Contribution.ToString("0.##", CultureInfo.InvariantCulture),-10}\n");

                    #endregion

                    #region Writing simplification

                    // If the theorem can be simplified...
                    if (simplifiedTheorems.ContainsKey(theorem))
                    {
                        // Append the header
                        result += "\n Can be simplified:\n\n";

                        // Get the new configuration and new statement
                        var (newTheorem, newConfiguration) = simplifiedTheorems[theorem];

                        // Prepare the formatter for the new configuration
                        var newFormatter = new OutputFormatter(newConfiguration.AllObjects);

                        // Write the configuration
                        result += $"{newFormatter.FormatConfiguration(newConfiguration).Indent(2)}\n\n";

                        // Write the theorem
                        result += $"  {newFormatter.FormatTheorem(newTheorem)}\n";
                    }

                    #endregion

                    // Return the final result
                    return $"{result.Trim()}\n";
                })
                // Make each on a separate line
                .ToJoinedString("\n")
                // Ensure no white spaces at the end
                .TrimEnd();

            // Append a new line
            result += "\n";

            #endregion

            // Return the final string with no trailing spaces
            return result.Trim();
        }

        #endregion
    }
}