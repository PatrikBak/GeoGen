using GeoGen.Algorithm;
using GeoGen.Core;
using GeoGen.Infrastructure;
using GeoGen.TheoremProver;
using GeoGen.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using static GeoGen.Infrastructure.Log;

namespace GeoGen.ConsoleLauncher
{
    /// <summary>
    /// The default implementation of <see cref="DebugAlgorithmRunner"/>.
    /// </summary>
    public class DebugAlgorithmRunner : IAlgorithmRunner
    {
        #region Dependencies

        /// <summary>
        /// The algorithm that is run.
        /// </summary>
        private readonly IAlgorithmFacade _algorithm;

        /// <summary>
        /// The finder of best theorems.
        /// </summary>
        private readonly IBestTheoremsFinder _finder;

        /// <summary>
        /// The writer used to write best theorems to a file.
        /// </summary>
        private readonly IRankedTheoremsWriter _writer;

        /// <summary>
        /// The factory for creating lazy writers of best theorems.
        /// </summary>
        private readonly ITheoremsWithRankingJsonLazyWriterFactory _writerFactory;

        #endregion

        #region Private fields

        /// <summary>
        /// The settings for this runner.
        /// </summary>
        private readonly DebugAlgorithmRunnerSettings _settings;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="DebugAlgorithmRunner"/> class.
        /// </summary>
        /// <param name="settings">The settings for this runner.</param>
        /// <param name="algorithm">The algorithm that is run.</param>
        /// <param name="finder">The finder of best theorems.</param>
        /// <param name="writer">The writer used to write best theorems to a file.</param>
        /// <param name="writerFactory">The factory for creating lazy writers of best theorems.</param>
        public DebugAlgorithmRunner(DebugAlgorithmRunnerSettings settings,
                                    IAlgorithmFacade algorithm,
                                    IBestTheoremsFinder finder,
                                    IRankedTheoremsWriter writer,
                                    ITheoremsWithRankingJsonLazyWriterFactory writerFactory)
        {
            _algorithm = algorithm ?? throw new ArgumentNullException(nameof(algorithm));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _finder = finder ?? throw new ArgumentNullException(nameof(finder));
            _writer = writer ?? throw new ArgumentNullException(nameof(writer));
            _writerFactory = writerFactory ?? throw new ArgumentNullException(nameof(writerFactory));
        }

        #endregion

        #region IAlgorithmRunner implementation

        /// <summary>
        /// Runs the algorithm on a given output.
        /// </summary>
        /// <param name="input">The input for the algorithm.</param>
        public void Run(LoadedAlgorithmInput input)
        {
            // Call the algorithm
            var (initialTheorems, outputs) = _algorithm.Run(input);

            #region Prepare writers

            // Prepare the writer of all the output as JSON
            var outputJsonWriter = _writerFactory.Create(Path.Combine(_settings.OutputJsonFolder, $"{_settings.OutputFilePrefix}{input.Id}.json"));

            // Prepare the writer of best theorems
            var bestTheoremsJsonWriter = _writerFactory.Create(_settings.BestTheoremsJsonFilePath);

            // Prepare the name of the output files
            var fileName = $"{_settings.OutputFilePrefix}{input.Id}.{_settings.FilesExtension}";

            // Prepare the writer for the output
            using var outputWriter = new StreamWriter(new FileStream(Path.Combine(_settings.OutputFolder, fileName), FileMode.Create, FileAccess.Write, FileShare.Read));

            // If we should generate the output with attempts
            using var outputWithAttemptsWriter = _settings.WriteOutputWithAttempts
                // Prepare the writer for it
                ? new StreamWriter(new FileStream(Path.Combine(_settings.OutputWithAttemptsFolder, fileName), FileMode.Create, FileAccess.Write, FileShare.Read))
                // Otherwise take null
                : null;

            // If we should generate the output with attempt and proofs
            using var outputWithAttemptsAndProofsWriter = _settings.WriteOutputWithAttemptsAndProofs
                // Prepare the writer for it
                ? new StreamWriter(new FileStream(Path.Combine(_settings.OutputWithAttemptsAndProofsFolder, fileName), FileMode.Create, FileAccess.Write, FileShare.Read))
                // Otherwise take null
                : null;

            // Local function that writes a line to all reports
            void WriteLineToAllWriters(string line = "")
            {
                // Write to the main output
                outputWriter.WriteLine(line);

                // Write to the output with attempts file if it is provided
                outputWithAttemptsWriter?.WriteLine(line);

                // Write to the output with attempts and proofs file if it is provided
                outputWithAttemptsAndProofsWriter?.WriteLine(line);
            }

            #endregion            

            #region Writing initial configuration

            // Prepare the formatter for the initial configuration
            var initialFormatter = new OutputFormatter(input.InitialConfiguration.AllObjects);

            // Write it
            WriteLineToAllWriters("Initial configuration:");
            WriteLineToAllWriters();
            WriteLineToAllWriters(initialFormatter.FormatConfiguration(input.InitialConfiguration));

            // Write its theorems, if there are any
            if (initialTheorems.Any())
            {
                WriteLineToAllWriters("\nTheorems:\n");
                WriteLineToAllWriters(InitialTheoremsToString(initialFormatter, initialTheorems));
            }

            #endregion

            #region Writing Iterations, Constructions and Results title

            // Write iterations
            WriteLineToAllWriters($"\nIterations: {input.NumberOfIterations}\n");

            // Write constructions
            WriteLineToAllWriters($"Constructions:\n");
            input.Constructions.ForEach(construction => WriteLineToAllWriters($" - {construction}"));
            WriteLineToAllWriters();

            // Write results header
            WriteLineToAllWriters($"Results:");

            #endregion

            // Log that we've started
            LoggingManager.LogInfo("Algorithm has started.");

            #region Tracking variables

            // Prepare the number of generated configurations
            var generatedConfigurations = 0;

            // Prepare the total number of interesting theorems
            var interestingTheorems = 0;

            #endregion

            #region Start stopwatch

            // Prepare a stopwatch to measure the execution time
            var stopwatch = new Stopwatch();

            // Start it
            stopwatch.Start();

            #endregion

            // Begin writing of a JSON output file
            outputJsonWriter.BeginWriting();

            // Run the algorithm
            foreach (var algorithmOutput in outputs)
            {
                // Mark the configuration
                generatedConfigurations++;

                // Find out if we should log progress and if yes, do it
                if (_settings.LogProgress && generatedConfigurations % _settings.GenerationProgresLoggingFrequency == 0)
                    // How many configurations did we generate?
                    LoggingManager.LogInfo($"Number of generated configurations: {generatedConfigurations}, " +
                        // How long did it take?
                        $"after {stopwatch.ElapsedMilliseconds} milliseconds, " +
                        // How many interesting theorem groups did we get?
                        $"with {interestingTheorems} interesting theorems.");

                // Skip configurations without theorems
                if (algorithmOutput.NewTheorems.AllObjects.Count == 0)
                    continue;

                #region Handling interesting theorems

                // Find the best theorems
                var bestTheorems = algorithmOutput.FindInterestingTheorems().ToArray();

                // Count them in
                interestingTheorems += bestTheorems.Length;

                // Let the finder judge them
                _finder.AddTheorems(bestTheorems, out var bestTheoremsChanged);

                // If there are some new ones
                if (bestTheoremsChanged)
                {
                    // Prepare an enumerable that identifies each so they could be found back in the output file
                    var identifiedTheorems = _finder.BestTheorems.Select(theorem => (theorem, $"configuration {generatedConfigurations} of the output file {fileName}"));

                    // Write them
                    _writer.WriteTheorems(identifiedTheorems, _settings.BestTheoremsReadableFilePath);

                    // Write their JSON version too
                    bestTheoremsJsonWriter.WriteEagerly(_finder.BestTheorems);
                }

                #endregion

                // Write JSON output
                outputJsonWriter.Write(bestTheorems);

                #region Human-readable output

                // Prepare a formatter for the generated configuration
                var formatter = new OutputFormatter(algorithmOutput.Configuration.AllObjects);

                // Local function to write line based on the number of interesting theorems
                void WriteLine(string line = "")
                {
                    // If there is any interesting theorem
                    if (algorithmOutput.ProverOutput.UnprovenTheorems.Any())
                    {
                        // Write to all writes, which will take care of potentially not requested ones
                        WriteLineToAllWriters(line);

                        // Terminate
                        return;
                    }

                    // Otherwise there are no interesting theorems. We might however want
                    // to write the proofs of the proven ones. If that is requested, do it
                    outputWithAttemptsAndProofsWriter?.WriteLine(line);
                }

                // Prepare the header
                var header = $"Configuration {generatedConfigurations}";

                // Write the configuration
                WriteLine();
                WriteLine(new string('-', header.Length));
                WriteLine(header);
                WriteLine(new string('-', header.Length));
                WriteLine();
                WriteLine(formatter.FormatConfiguration(algorithmOutput.Configuration));

                // Write the title with simple statistics
                WriteLine($"\nTheorems:\n");

                // If there is any interesting theorem
                if (algorithmOutput.ProverOutput.UnprovenTheorems.Any())
                {
                    // Then write them to the standard output writer
                    outputWriter.WriteLine(TheoremsToString(formatter, algorithmOutput,
                        // We want to include only unproven ones, without their proof attempts
                        includeProvenTheorems: false, displayProofAttempts: false));

                    // We also try to write them to the writer of the output with attempts, if it's provided
                    outputWithAttemptsWriter?.WriteLine(TheoremsToString(formatter, algorithmOutput,
                        // Here we want to include only unproven ones, but with their proof attempts
                        includeProvenTheorems: false, displayProofAttempts: true));
                }

                // In any case (whether we have or haven't interesting theorems) we want to write the info
                // to the writer of theorems with attempts and proofs, if it's provided
                outputWithAttemptsAndProofsWriter?.WriteLine(TheoremsToString(formatter, algorithmOutput,
                    // Here we want to include even proven theorems and also proof attempts of unproven ones
                    includeProvenTheorems: true, displayProofAttempts: true));

                // Flush the writers after each output so that we can see the results gradually
                outputWriter.Flush();
                outputWithAttemptsWriter?.Flush();
                outputWithAttemptsAndProofsWriter?.Flush();

                #endregion
            }

            // Write end
            WriteLineToAllWriters("\n------------------------------------------------");
            WriteLineToAllWriters($"Generated configurations: {generatedConfigurations}");
            WriteLineToAllWriters($"Interesting theorems: {interestingTheorems}");
            WriteLineToAllWriters($"Run-time: {stopwatch.ElapsedMilliseconds} ms");

            // Log these stats as well
            LoggingManager.LogInfo($"Algorithm has finished in {stopwatch.ElapsedMilliseconds} ms.");
            LoggingManager.LogInfo($"Generated configurations: {generatedConfigurations}");
            LoggingManager.LogInfo($"Interesting theorems: {interestingTheorems}");

            // Close JSON output writer
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
                // Append the index to each
                .Select((theoremString, index) => $" {index + 1,2}. {theoremString}")
                // Make each on a separate line
                .ToJoinedString("\n");
        }

        /// <summary>
        /// Converts given theorems to a string, optionally with their proofs or attempts to prove.
        /// </summary>
        /// <param name="formatter">The formatter of the configuration where the theorems hold.</param>
        /// <param name="algorithmOutput">The output of the algorithm.</param>
        /// <param name="includeProvenTheorems">Indicates if we should include the proved theorems as well.</param>
        /// <param name="displayProofAttempts">Indicates whether we should display proof attempts.</param>
        /// <returns>The string representing the theorems.</returns>
        private string TheoremsToString(OutputFormatter formatter,
                                        AlgorithmOutput algorithmOutput,
                                        bool includeProvenTheorems,
                                        bool displayProofAttempts)
        {
            // Prepare the groups in the sorted order that we're going to use
            var sortedUnprovenTheoremsGroups = algorithmOutput.ProverOutput.UnprovenTheoremGroups.Select(group =>
                    // First order the theorems inside one group by their overall ranking (ASC) and then by text
                    group.OrderBy(theorem => (-algorithmOutput.Rankings[theorem].TotalRanking, formatter.FormatTheorem(theorem))).ToArray())
                // Then order these groups first by the ranking of the first theorem (ASC), then group size (ASC), then name
                .OrderBy(group => (-algorithmOutput.Rankings[group[0]].TotalRanking, -group.Length, formatter.FormatTheorem(group[0])))
                // Enumerate
                .ToArray();

            // Prepare the proven theorems, if we should include them
            var provenTheorems = !includeProvenTheorems ? Array.Empty<Theorem>()
                // If yes, order them by their text
                : algorithmOutput.ProverOutput.ProvenTheorems.Keys.OrderBy(formatter.FormatTheorem).ToArray();

            // Prepare the dictionary where we will store unique strings associated with 
            // these (and potentially other) theorems used in the reports
            var theoremTags = new Dictionary<Theorem, string>();

            // Name the groups first
            sortedUnprovenTheoremsGroups.ForEach((group, groupIndex) =>
            {
                // Each group will have its index which will start the tag of each theorem in the group
                // If the group has only one them
                if (group.Length == 1)
                    // Then we don't need to include the theorem index
                    theoremTags.Add(group[0], $"{groupIndex + 1}.");
                // Otherwise
                else
                    // We name each theorem separately
                    group.ForEach((theorem, theoremIndex) => theoremTags.Add(theorem, $"{groupIndex + 1}.{theoremIndex + 1}."));
            });

            // Name the proven theorems too. Remember we have already used some numbers for the group identifiers
            provenTheorems.ForEach((theorem, theoremIndex) => theoremTags.Add(theorem, $"{sortedUnprovenTheoremsGroups.Length + theoremIndex + 1}."));

            // Process every theorem
            var theoremsString = sortedUnprovenTheoremsGroups.Flatten().Concat(provenTheorems).Select(theorem =>
            {
                // If this is a proven theorem, then we display it with its proof (no ranking has been done)
                if (algorithmOutput.ProverOutput.ProvenTheorems.ContainsKey(theorem))
                    return $"{theoremTags[theorem]} {ProofReport(algorithmOutput.ProverOutput.ProvenTheorems[theorem], formatter, theoremTags)}";

                #region Writing ranking

                // Otherwise our theorem is not proven, i.e. we want to display the total ranking
                var result = $"{theoremTags[theorem]} {formatter.FormatTheorem(theorem)} - total ranking {algorithmOutput.Rankings[theorem].TotalRanking}\n\n";

                // Add individual rankings ordered by the total contribution (ASC) and then the aspect name
                algorithmOutput.Rankings[theorem].Ranking.OrderBy(pair => (-pair.Value.Coefficient * pair.Value.Ranking, pair.Key.ToString()))
                    // Add each on an individual line with info about the coefficient
                    .ForEach(pair => result += $" {pair.Key,-25}coefficient = {pair.Value.Coefficient.ToString("G5"),-10}" +
                        // The ranking and the total contribution of this aspect
                        $"contribution = {(pair.Value.Coefficient * pair.Value.Ranking).ToString("G5"),-10}ranking = {pair.Value.Ranking.ToString("G5"),-10}{pair.Value.Message}\n");

                #endregion

                #region Writing simplification

                // If the theorem can be simplified...
                if (algorithmOutput.SimplifiedTheorems.ContainsKey(theorem))
                {
                    // Append the header
                    result += "\nCan be simplified:\n\n";

                    // Get the new configuration and new statement
                    var (newConfiguration, newTheorem) = algorithmOutput.SimplifiedTheorems[theorem];

                    // Prepare the formatter for the new configuration
                    var newFormatter = new OutputFormatter(newConfiguration.AllObjects);

                    // Write the configuration
                    result += $"{newFormatter.FormatConfiguration(newConfiguration).Indent(1)}\n\n";

                    // Write the theorem
                    result += $" {newFormatter.FormatTheorem(newTheorem)}\n";
                }

                #endregion

                // If we shouldn't display proof attempts, then we're done
                if (!displayProofAttempts)
                    return result;

                // Otherwise we include the proof on a new line
                result += $"\nProof attempts{UnfinishedProofsReport(theorem, algorithmOutput.ProverOutput.UnprovenTheorems[theorem], formatter, theoremTags, includeStatement: false)}";

                // And return the correctly trimmed result
                return $"{result.TrimEnd()}\n";
            })
            // Make each on a separate line
            .ToJoinedString("\n")
            // Ensure no white spaces at the end
            .TrimEnd();

            // If we should write unproven theorems and there are any...
            if (algorithmOutput.ProverOutput.UnprovenDiscoveredTheorems.Any() && _settings.IncludeUnprovenDiscoveredTheorems)
            {
                // Convert them to a string
                var unprovenDiscoveredTheoremsString = algorithmOutput.ProverOutput.UnprovenDiscoveredTheorems
                    // Initially sort them by their formatted versions
                    .OrderBy(pair => formatter.FormatTheorem(pair.Key))
                    // Convert each to a string with the right index
                    .Select((pair, index) => $"{sortedUnprovenTheoremsGroups.Length + provenTheorems.Length + index + 1,2}. {formatter.FormatTheorem(pair.Key)}" +
                        // And the tag, which is only present if we display theorem proofs, where it got this tag
                        $"{(displayProofAttempts ? $" - theorem {theoremTags[pair.Key]}" : "")}")
                    // Make each on a separate line
                    .ToJoinedString("\n")
                    // Ensure no white spaces at the end
                    .TrimEnd();

                // Append the created string to the result
                theoremsString += $"\n\nDiscovered unproved theorems:\n\n{unprovenDiscoveredTheoremsString}";
            }

            // Return the final string
            return theoremsString;
        }

        /// <summary>
        /// Creates a report for unfinished proofs of a given theorem.
        /// </summary>
        /// <param name="theorem">The theorem that was attempted to be proven.</param>
        /// <param name="unfinishedProofs">The list of attempts to prove this theorem.</param>
        /// <param name="formatter">The formatter of the configuration in which the theorem holds.</param>
        /// <param name="theoremTags">The dictionary of already tagged theorems.</param>
        /// <param name="includeStatement">Indicates whether we should include the statement of the theorem.</param>
        /// <param name="previousTag">The tag of the proof under which this one falls.</param>
        /// <returns>The string containing the report.</returns>
        private string UnfinishedProofsReport(Theorem theorem,
                                              IReadOnlyList<TheoremProofAttempt> unfinishedProofs,
                                              OutputFormatter formatter,
                                              Dictionary<Theorem, string> theoremTags,
                                              bool includeStatement = true,
                                              string previousTag = "")
        {
            // First format the unfinished theorem, if we should include the statement
            var result = includeStatement ? formatter.FormatTheorem(theorem) : "";

            // Handle if there were no attempt to prove the theorem
            if (unfinishedProofs.Count == 0)
                return $"{result} - no clue";

            // Otherwise there was at least one attempt, append the information about their count
            result += $" - not proven, {unfinishedProofs.Count} attempt{(unfinishedProofs.Count == 1 ? "" : "s")} to prove it:\n\n";

            // We're going to create an enumerable of reports from every (unsuccessful) attempt
            var unfinishedProofsReports = unfinishedProofs.Select((proofAttempt, index) =>
            {
                // Get the string identifying the current attempt, based on the fact
                // whether there is just one or more of them
                var attemptsString = unfinishedProofs.Count == 1 ? "The attempt" : $"Attempt {index + 1}";

                // Create a new tag that will be used for the current theorem
                // If there is no previous tag, we'll use the indented one from the list
                // Otherwise we just add the indentation to the previous one
                var newTag = previousTag.IsEmpty() ? $"  {theoremTags[theorem]}" : $"  {previousTag}";

                // Copy the spaces from the new tag, so that we have correct alignment 
                var spaces = new string(' ', newTag.TakeWhile(c => c == ' ').Count());

                // Find out if we want to append the index of the attempt
                // We certainly want to do it when there are more of them
                if (unfinishedProofs.Count != 1
                    // Or if there is exactly one proof, and the inner assumptions have also exactly
                    // one proof altogether (i.e. they naturally won't be numbered and we want to 
                    // distinguish between this proof and the inner one)
                    || unfinishedProofs[0].ProvenAssumptions.Count + unfinishedProofs[0].UnprovenAssumptions.Count == 1)
                {
                    // Append the index
                    newTag += $"{index + 1}.";
                }

                // Call the general method for the report of a general proof 
                // (that might or might not be successful). 
                var reportString = ProofReport(proofAttempt, formatter, theoremTags, newTag,
                    // We don't we want repeat the statement in the description
                    includeStatement: false);

                // Append the final string with the report to the result, 
                // making sure it has no white-spaces at the end
                return $"{spaces}{attemptsString}{reportString.TrimEnd()}";
            });

            // Enumerate the reports while making them each on separate line with empty ones
            return $"{result}{unfinishedProofsReports.ToJoinedString("\n\n")}\n";
        }

        /// <summary>
        /// Creates a report of a given proof attempt. The method handles both successful and unsuccessful proofs.
        /// </summary>
        /// <param name="proofAttempt">The proof attempt to be reported.</param>
        /// <param name="formatter">The formatter of the configuration in which the theorem holds.</param>
        /// <param name="theoremTags">The dictionary of already tagged theorems.</param>
        /// <param name="previousTag">The tag of the proof under which this one falls.</param>
        /// <param name="includeStatement">Indicates if we should start the report with the statement of the theorem.</param>
        /// <returns>The string containing the report.</returns>
        private string ProofReport(TheoremProofAttempt proofAttempt,
                                   OutputFormatter formatter,
                                   Dictionary<Theorem, string> theoremTags,
                                   string previousTag = "",
                                   bool includeStatement = true)
        {
            // Start composing the result by formatting the theorem, if it's requested
            var result = includeStatement ? formatter.FormatTheorem(proofAttempt.Theorem) : "";

            // Append the explanation
            result += $" - {GetExplanation(proofAttempt, formatter)}";

            // If there is nothing left to write, i.e. no attempts, then we can't do more
            if (proofAttempt.ProvenAssumptions.Count == 0 && proofAttempt.UnprovenAssumptions.Count == 0)
                return result;

            // Otherwise we want an empty line
            result += "\n\n";

            // Prepare the index of the next theorem to be described
            var index = 1;

            // Create an enumerable of reports of proven assumptions, ordered by theorem
            var provenAssumptionsStrings = proofAttempt.ProvenAssumptions.OrderBy(attempt => formatter.FormatTheorem(attempt.Theorem))
                // Process a given one
                .Select(innerAttempt =>
                {
                    // Get the theorem for comfort
                    var theorem = innerAttempt.Theorem;

                    // Compose the tag with spaces. 
                    // If there is no previous tag, get the default one from the tags map (with spaces)
                    // Otherwise, take the previous one with more spaces and append the current index (and increase it)
                    var untrimmedTag = $"{(previousTag.IsEmpty() ? $"  {theoremTags[proofAttempt.Theorem]}" : $"  {previousTag}")}{index++}.";

                    // Find out if the theorem already has a tag
                    var theoremIsUntagged = !theoremTags.ContainsKey(theorem);

                    // If the theorem is not tagged yet, tag it
                    if (theoremIsUntagged)
                        theoremTags.Add(theorem, untrimmedTag.Trim());

                    // Find out the reasoning for the theorem
                    var reason = theoremIsUntagged ?
                        // If it's untagged, recursively find the report for it
                        ProofReport(innerAttempt, formatter, theoremTags, untrimmedTag) :
                        // Otherwise just state it again and add the explanation and the reference to it
                        $"{formatter.FormatTheorem(theorem)} - {GetExplanation(innerAttempt, formatter)} - theorem {theoremTags[theorem]}";

                    // Add the description to the result
                    return $"{untrimmedTag} {reason}";
                });

            // Create an enumerable of reports of proven assumptions, ordered by theorem
            var unprovenAssumptionsStrings = proofAttempt.UnprovenAssumptions.OrderBy(attempt => formatter.FormatTheorem(attempt.theorem))
                // Process a given one
                .Select(pair =>
                {
                    // Deconstruct
                    var (theorem, attempts) = pair;

                    // Start the construction of an untrimmed tag by adding spaces before the previous one
                    var untrimmedTag = $"  {previousTag}";

                    // We want to start indexing these theorems only if there won't be exactly one theorem
                    // (including the ones that are already there)
                    if (proofAttempt.UnprovenAssumptions.Count + proofAttempt.ProvenAssumptions.Count != 1)
                        untrimmedTag += $"{index++}.";

                    // Find out if the theorem already has a tag
                    var theoremIsUntagged = !theoremTags.ContainsKey(theorem);

                    // If the theorem is not tagged yet, tag it
                    if (theoremIsUntagged)
                        theoremTags.Add(theorem, untrimmedTag.Trim());

                    // Find out the reasoning for the theorem
                    var reason = theoremIsUntagged ?
                       // If it's untagged, recursively find the report for it
                       UnfinishedProofsReport(theorem, attempts, formatter, theoremTags, previousTag: untrimmedTag) :
                       // Otherwise just state it again and add the reference to it
                       $"{formatter.FormatTheorem(theorem)} - theorem {theoremTags[theorem]} (unproven)";

                    // Add the description to the result
                    return $"{untrimmedTag} {reason}";
                });

            // Append the particular reports, each on a separate line, while making sure 
            // there is exactly one line break at the end
            return $"{result}{provenAssumptionsStrings.Concat(unprovenAssumptionsStrings).ToJoinedString("\n").TrimEnd()}\n";
        }

        /// <summary>
        /// Returns a string representation of how the theorem was proved or was attempted to be proven.
        /// </summary>
        /// <param name="proofAttempt">The attempt.</param>
        /// <param name="formatter">The formatter of the configuration in which the theorem holds.</param>
        /// <returns>The explanation.</returns>
        private static string GetExplanation(TheoremProofAttempt proofAttempt, OutputFormatter formatter)
        {
            // Prepare the helper variable indicating whether the theorem has been proven
            var isProved = proofAttempt.UnprovenAssumptions.IsEmpty();

            // Switch based on the type of used derivation rule
            switch (proofAttempt.Rule)
            {
                // Case when it's a theorem from a previous configuration
                // There are no needed theorems to make this conclusion, i.e. the theorem is right
                case DerivationRule.TrueInPreviousConfiguration:
                    return "true in a previous configuration";

                // Case when it's not in a previous configuration, but can be declared in one
                case DerivationRule.DefinableSimpler:

                    // Get the redundant objects
                    var redundantObjects = ((DefinableSimplerDerivationData)proofAttempt.Data).RedundantObjects
                        // Get their names
                        .Select(formatter.GetObjectName)
                        // Sort them
                        .Ordered()
                        // Join together
                        .ToJoinedString();

                    // Get the string stating that we needed reformulation
                    // This might happen when this theorem has a needed assumption
                    var reformulation = proofAttempt.ProvenAssumptions.Any() || proofAttempt.UnprovenAssumptions.Any()
                        ? " after reformulation " : " ";

                    // Return the final conclusion with these objects included
                    return isProved ? $"can be stated{reformulation}without {redundantObjects}" : $"could be stated{reformulation}without {redundantObjects}";

                // Case when it's a trivial consequence of the construction of the object
                // There are no needed theorems to make this conclusion, i.e. the theorem is right
                case DerivationRule.TrivialTheorem:
                    return "trivial consequence of the object's construction";

                // Case when the theorem was attempted as a reformulation of a theorem
                case DerivationRule.ReformulatedTheorem:
                    return "reformulation of the theorem";

                // Case when it's been derived using the transitivity rule
                case DerivationRule.Transitivity:

                    // Append the right conclusion based on whether it is proved or not
                    return isProved ? "true because of the transitivity rule" : "attempted with the transitivity rule";

                // Case when it's been derived as a consequence of a template theorem
                case DerivationRule.Subtheorem:

                    // Pull the template theorem
                    var templateTheorem = (TemplateTheorem)((SubtheoremDerivationData)proofAttempt.Data).TemplateTheorem;

                    // Return the right message based on whether is proven
                    return isProved ? $"consequence of {templateTheorem.Number} from {templateTheorem.FileName}"
                                    : $"attempted as a consequence of {templateTheorem.Number} from {templateTheorem.FileName}";

                // Case when we're connecting collinearity with other line theorems
                case DerivationRule.CollinearityWithLinesFromPoints:

                    // If we have a collinearity
                    return proofAttempt.Theorem.Type == TheoremType.CollinearPoints
                        // Get the corresponding message
                        ? "using uniquely constructed line to prove collinearity"
                        // Otherwise the message is a bit different
                        : "reformulation using collinearity";

                // Case when we're connecting concyclity with other circle theorems
                case DerivationRule.ConcyclityWithCirclesFromPoints:
                    return "reformulation using concyclity";

                // Case when we have two perpendicular lines and one parallelity
                case DerivationRule.PerpendicularLineToParallelLines:
                    return "two perpendicular lines with a common line are equivalent with parallelity";

                // Case when we combine incidences to restate a theorem
                case DerivationRule.ExplicitLineWithIncidences:
                    return "restating using incidences";

                // Case when we combine incidences to restate a theorem
                case DerivationRule.ExplicitCircleWithIncidences:
                    return "restating using incidences";

                // Case when we put together 3 incidences and collinearity:
                case DerivationRule.IncidencesAndCollinearity:
                    return "three incidences with the same line are equivalent with collinearity";

                // Case when we put together 4 incidences and concyclicity:
                case DerivationRule.IncidencesAndConcyclity:
                    return "four incidences with the same line are equivalent with concyclity";

                // Case when we have 4 concyclic points and their circumcenter in the same picture
                case DerivationRule.ConcyclicPointsWithExplicitCenter:
                    return "concyclic points with explicit center";

                // Case when we have 3 radical axis implying concurrency, or another concyclity
                case DerivationRule.RadicalAxis:
                    return "radical axis theorem";

                // Case when we have 3 radical axis implying concurrency, or another concyclity
                case DerivationRule.ThalesTheorem:
                    return "Thales theorem";

                // Case when we have a parallelogram
                case DerivationRule.Parallelogram:
                    return "two pairs of parallel lines are equivalent with the points making a parallelogram";

                // Case when we have a rectangle
                case DerivationRule.Rectangle:
                    return "two pairs of parallel lines and perpendicularity are equivalent with the points making a rectangle (with equal diagonals)";

                // Case when we have two equal line segments theorems and a perpendicularity
                case DerivationRule.IsoscelesTrianglesPerpendicularity:
                    return "two isosceles triangles with the same base are equivalent with perpendicularity";

                // Default case
                default:
                    throw new GeoGenException($"Unhandled type of {nameof(DerivationRule)}: {proofAttempt.Data.Rule}");
            }
        }

        #endregion
    }
}