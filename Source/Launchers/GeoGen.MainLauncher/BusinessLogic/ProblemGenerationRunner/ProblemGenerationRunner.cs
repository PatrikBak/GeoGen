using GeoGen.Core;
using GeoGen.Infrastructure;
using GeoGen.ProblemAnalyzer;
using GeoGen.ProblemGenerator;
using GeoGen.ProblemGenerator.InputProvider;
using GeoGen.TheoremProver;
using GeoGen.TheoremProver.InferenceRuleProvider;
using GeoGen.TheoremRanker;
using GeoGen.TheoremRanker.RankedTheoremIO;
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
    /// The runner provides lots of useful debugging output configurable via <see cref="ProblemGenerationRunnerSettings"/>. 
    /// It can write output with or without proofs into human-readable or JSON format (that can be processed via Drawer).
    /// Human-readable files contain information about asymmetric exclusion and ranking results.
    /// </para>
    /// <para>
    /// It uses <see cref="ITheoremSorterTypeResolver"/> to find globally best theorems across multiple runs for each type. These
    /// theorems are optionally written to human-readable files or JSON files.
    /// </para>
    /// <para>It also provides <see cref="IInferenceRuleUsageTracker"/> in case we need to analyze how the theorem prover works.</para>
    /// </summary>
    public class ProblemGenerationRunner : IProblemGenerationRunner
    {
        #region Dependencies

        /// <summary>
        /// The settings for this runner.
        /// </summary>
        private readonly ProblemGenerationRunnerSettings _settings;

        /// <summary>
        /// The generator of problems.
        /// </summary>
        private readonly IProblemGenerator _generator;

        /// <summary>
        /// The analyzer of problem generator outputs.
        /// </summary>
        private readonly IGeneratedProblemAnalyzer _analyzer;

        /// <summary>
        /// The resolver of sorters for each theorem type that are used to find globally best theorems.
        /// </summary>
        private readonly ITheoremSorterTypeResolver _resolver;

        /// <summary>
        /// The factory for creating lazy writers of a JSON output.
        /// </summary>
        private readonly IRankedTheoremJsonLazyWriterFactory _factory;

        /// <summary>
        /// The tracker of the used inference rules in theorem proofs.
        /// </summary>
        private readonly IInferenceRuleUsageTracker _tracker;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ProblemGenerationRunner"/> class.
        /// </summary>
        /// <param name="settings"><inheritdoc cref="_settings" path="/summary"/></param>
        /// <param name="generator"><inheritdoc cref="_generator" path="/summary"/></param>
        /// <param name="analyzer"><inheritdoc cref="_analyzer" path="/summary"/>.</param>
        /// <param name="resolver"><inheritdoc cref="_resolver" path="/summary"/></param>
        /// <param name="factory"><inheritdoc cref="_factory" path="/summary"/></param>
        /// <param name="tracker"><inheritdoc cref="_tracker" path="/summary"/></param>
        public ProblemGenerationRunner(ProblemGenerationRunnerSettings settings,
                                       IProblemGenerator generator,
                                       IGeneratedProblemAnalyzer analyzer,
                                       ITheoremSorterTypeResolver resolver,
                                       IRankedTheoremJsonLazyWriterFactory factory,
                                       IInferenceRuleUsageTracker tracker)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _generator = generator ?? throw new ArgumentNullException(nameof(generator));
            _analyzer = analyzer ?? throw new ArgumentNullException(nameof(analyzer));
            _resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));
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

            #region Write other setup

            // Write iterations
            WriteLineToBothReadableWriters($"\nIterations: {input.NumberOfIterations}");

            // Write maximal numbers of objects of particular types
            WriteLineToBothReadableWriters($"{input.MaximalNumbersOfObjectsToAdd.Select(pair => $"MaximalNumberOf{pair.Key}s: {pair.Value}").ToJoinedString("\n")}\n");

            // Write whether we're excluding symmetry
            WriteLineToBothReadableWriters($"GenerateOnlySymmetricConfigurations: {input.ExcludeAsymmetricConfigurations}");

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

            // Prepare the variable indicating whether we're writing best theorems,
            // which happens when we want to write them either readable or JSON form
            var writeBestTheorems = _settings.WriteReadableBestTheorems || _settings.WriteJsonBestTheorems;

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
                        // How many interesting theorems
                        $"with {numberOfInterestingTheorems} theorems" +
                        // How many interesting theorems after merge
                        $"{(writeBestTheorems ? $" ({_resolver.AllSorters.Select(pair => pair.sorter.BestTheorems.Count()).Sum()} after global merge)" : "")}" +
                        // How many configurations
                        $" in {numberOfConfigurationsWithInterestingTheorem} configurations.");

                #endregion

                // Skip configurations without theorems
                if (generatorOutput.NewTheorems.AllObjects.Count == 0)
                    continue;

                // Prepare the output of the analyzer
                GeneratedProblemAnalyzerOutputBase analyzerOutput;

                #region Analyzer call

                try
                {
                    // The analyzer needs to know whether it should deem asymmetric problems as interesting.
                    // This value is pulled from the input setup
                    var areAsymetricProblemsInteresting = !input.ExcludeAsymmetricConfigurations;

                    // If we should look for proofs (because we should be writing them or analyze the inner inferences)
                    analyzerOutput = _settings.WriteInferenceRuleUsages || _settings.WriteReadableOutputWithProofs
                        // Then call the analysis that construct them
                        ? (GeneratedProblemAnalyzerOutputBase)_analyzer.AnalyzeWithProofConstruction(generatorOutput, areAsymetricProblemsInteresting)
                        // Otherwise we don't need them
                        : _analyzer.AnalyzeWithoutProofConstruction(generatorOutput, areAsymetricProblemsInteresting);
                }
                catch (Exception e)
                {
                    // If there is any sort of problem, we should make aware of it. 
                    LoggingManager.LogError($"There has been an exception while analyzing the configuration:\n\n" +
                        // Write the problematic configuration
                        $"{new OutputFormatter(generatorOutput.Configuration.AllObjects).FormatConfiguration(generatorOutput.Configuration)}\n" +
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

                // Write JSON output
                jsonOutputWriter?.Write(analyzerOutput.InterestingTheorems);

                #region Handling best theorems

                // If we are supposed to be handling best theorems, do so
                if (writeBestTheorems)
                {
                    // Take the interesting theorems
                    var theoremsToBeJudged = analyzerOutput.InterestingTheorems
                        // Group by type
                        .GroupBy(rankedTheorem => rankedTheorem.Theorem.Type);

                    try
                    {
                        // Prepare the set of sorters whose content changed
                        var updatedSorterTypes = new HashSet<TheoremType>();

                        // Mark all interesting theorems
                        analyzerOutput.InterestingTheorems
                            // Grouped by type
                            .GroupBy(rankedTheorem => rankedTheorem.Theorem.Type)
                            // Handle each group
                            .ForEach(group =>
                            {
                                // Let the sorter judge the theorems
                                _resolver.GetSorterForType(group.Key).AddTheorems(group, out var localBestTheoremChanged);

                                // If there is any local change, mark it
                                if (localBestTheoremChanged)
                                    updatedSorterTypes.Add(group.Key);
                            });

                        // If we should write best theorems continuously, do it
                        if (_settings.WriteBestTheoremsContinuously)
                            RewriteBestTheorems(updatedSorterTypes);
                    }
                    catch (Exception e)
                    {
                        // If there is any sort of problem, we should make aware of it. 
                        LoggingManager.LogError($"There has been an exception while sorting theorems of the configuration:\n\n" +
                            // Write the problematic configuration
                            $"{new OutputFormatter(generatorOutput.Configuration.AllObjects).FormatConfiguration(generatorOutput.Configuration)}\n" +
                            // And also the exception
                            $"Exception: {e}");
                    }
                }

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

            // Prepare the string explaining the state after merge
            var afterMergeString = $"{(writeBestTheorems ? $"{_resolver.AllSorters.Select(pair => pair.sorter.BestTheorems.Count()).Sum()}" : "-")}";

            // Write end
            WriteLineToBothReadableWriters("\n------------------------------------------------");
            WriteLineToBothReadableWriters($"Generated configurations: {numberOfGeneratedConfigurations}");
            WriteLineToBothReadableWriters($"Configurations with an interesting theorem: {numberOfConfigurationsWithInterestingTheorem}");
            WriteLineToBothReadableWriters($"Interesting theorems: {numberOfInterestingTheorems}");
            WriteLineToBothReadableWriters($"Interesting theorems after global merge: {afterMergeString}");
            WriteLineToBothReadableWriters($"Run-time: {stopwatch.ElapsedMilliseconds} ms");

            // Log these stats as well
            LoggingManager.LogInfo($"Generated configurations: {numberOfGeneratedConfigurations}");
            LoggingManager.LogInfo($"Configurations with an interesting theorem: {numberOfConfigurationsWithInterestingTheorem}");
            LoggingManager.LogInfo($"Interesting theorems: {numberOfInterestingTheorems}");
            LoggingManager.LogInfo($"Interesting theorems after global merge: {afterMergeString}");
            LoggingManager.LogInfo($"Run-time: {stopwatch.ElapsedMilliseconds} ms");

            // Close the JSON output writer
            jsonOutputWriter?.EndWriting();
        }

        /// <summary>
        /// Rewrites the best theorem files, i.e. the readable files for each type or the JSON files based on settings.
        /// </summary>
        /// <param name="typesToWrite">The theorem types to be rewritten. If the value is null (by default), then all sorters are rewritten.</param>
        private void RewriteBestTheorems(IReadOnlyCollection<TheoremType> typesToWrite = null)
        {
            // Prepare the sorters to be rewritten by analyzing the passed types
            var sorters = typesToWrite
                // For each find the sorter
                ?.Select(type => (type, sorter: _resolver.GetSorterForType(type)))
                // Or if the types are null, take all sorters
                ?? _resolver.AllSorters;

            // For every type and sorter
            foreach (var (type, sorter) in sorters)
            {
                // If we should write readable output...
                if (_settings.WriteReadableBestTheorems)
                {
                    // Prepare the path by combining the folder and the theorem type
                    var theoremFilePath = $"{Path.Combine(_settings.ReadableBestTheoremFolder, type.ToString())}.{_settings.FileExtension}";

                    // Prepare the writer
                    using var readableBestTheoremWriter = new StreamWriter(new FileStream(theoremFilePath, FileMode.Create, FileAccess.Write, FileShare.Read));

                    // Rewrite the file
                    readableBestTheoremWriter.Write(RankedTheoremsToString(sorter.BestTheorems));
                }

                // If we should write JSON output
                if (_settings.WriteJsonBestTheorems)
                {
                    // Prepare the path by combining the folder and the theorem type
                    var theoremFilePath = $"{Path.Combine(_settings.JsonBestTheoremFolder, type.ToString())}.json";

                    // Rewrite the JSON output
                    _factory.Create(theoremFilePath).WriteEagerly(sorter.BestTheorems);
                }
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
        private static string RankedTheoremsToString(IEnumerable<RankedTheorem> rankedTheorems)
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
                    // Add the total ranking
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

            #region Asymmetric theorems

            // If there are any asymmetric theorems
            if (analyzerOutput.NotInterestringAsymmetricTheorems.Any())
            {
                // Append the header
                result = $"{result.TrimEnd()}\n\nAsymmetric theorems:\n\n";

                // Append the asymmetric theorems by taking them
                result += analyzerOutput.NotInterestringAsymmetricTheorems
                    // Format them
                    .Select(formatter.FormatTheorem)
                    // Order by the statement
                    .Ordered()
                    // Prepend the local index and increase it
                    .Select(theoremString => $" {localTheoremIndex++}. {theoremString}")
                    // Make each on a separate line
                    .ToJoinedString("\n");
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
                .Select((rankedTheorem, index) =>
                {
                    // Prepare the local result with the local index (while increasing it)
                    var result = $" {localTheoremIndex++}. {formatter.FormatTheorem(rankedTheorem.Theorem)}";

                    // Write the total ranking
                    result += $" - total ranking {rankedTheorem.Ranking.TotalRanking.ToStringWithDecimalDot()}\n\n";

                    // Write the ranking table
                    result += TheoremRankingToString(rankedTheorem.Ranking);

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
                .Select(pair => $"  {pair.Key,-32}weight = {pair.Value.Weight.ToStringWithDecimalDot(),-10}" +
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