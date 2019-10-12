using GeoGen.Algorithm;
using GeoGen.Core;
using GeoGen.TheoremProver;
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
            using var outputWriter = new StreamWriter(new FileStream(outputPath, FileMode.Create, FileAccess.Write, FileShare.Read));

            // Prepare the path for the full output
            var fullOutputPath = Path.Combine(_settings.OutputFolder, $"{_settings.OutputFilePrefix}{input.Id}{_settings.FullReportSuffix}.{_settings.OutputFileExtention}");

            // Prepare the writer for the full output, if it's requested
            using var fullOutputWriter = _settings.GenerateFullReport ? new StreamWriter(new FileStream(fullOutputPath, FileMode.Create, FileAccess.Write, FileShare.Read)) : null;

            // Helper function that writes to both the writers
            void WriteLineToBothReports(string line = "")
            {
                // Write to the default
                outputWriter.WriteLine(line);

                // Write to the full, if it's specified
                fullOutputWriter?.WriteLine(line);
            }

            // Helper function that writes to the full writer
            void WriteLineToFullReport(string line = "") => fullOutputWriter?.WriteLine(line);

            #endregion            

            #region Writing initial configuration

            // Prepare the formatter for the initial configuration
            var initialFormatter = new OutputFormatter(input.InitialConfiguration);

            // Write it
            WriteLineToBothReports("Initial configuration:");
            WriteLineToBothReports();
            WriteLineToBothReports(initialFormatter.FormatConfiguration());

            // Write its theorems, if there are any
            if (initialTheorems.Any())
            {
                WriteLineToBothReports("\nTheorems:\n");
                WriteLineToBothReports(InitialTheoremsToString(initialFormatter, initialTheorems));
            }

            #endregion

            #region Writing Iterations, Constructions and Results title

            // Write iterations
            WriteLineToBothReports($"\nIterations: {input.NumberOfIterations}\n");

            // Write constructions
            WriteLineToBothReports($"Constructions:\n");
            input.Constructions.ForEach(construction => WriteLineToBothReports($" - {construction}"));
            WriteLineToBothReports();

            // Write results header
            WriteLineToBothReports($"Results:");

            #endregion

            // Log that we've started
            Log.LoggingManager.LogInfo("Algorithm has started.");

            #region Tracking variables

            // Prepare the number of generated configurations
            var generatedConfigurations = 0;

            // Prepare the total number of theorems
            var totalTheorems = 0;

            // Prepare the total number of interesting theorems
            var interestingTheorems = 0;

            #endregion

            #region Start stopwatch

            // Prepare a stopwatch to measure the execution time
            var stopwatch = new Stopwatch();

            // Start it
            stopwatch.Start();

            #endregion

            // Run the algorithm
            foreach (var algorithmOutput in outputs)
            {
                // Mark the configuration
                generatedConfigurations++;

                // Find out if we should log and if yes, do it
                if (_settings.LogProgress && generatedConfigurations % _settings.GenerationProgresLoggingFrequency == 0)
                    Log.LoggingManager.LogInfo($"Number of generated configurations: {generatedConfigurations}, " +
                        $"after {stopwatch.ElapsedMilliseconds} milliseconds, " +
                        $"with {interestingTheorems} interesting theorem(s).");

                // Skip configurations without theorems
                if (algorithmOutput.Theorems.AllObjects.Count == 0)
                    continue;

                // Count in all theorems
                totalTheorems += algorithmOutput.Theorems.AllObjects.Count;

                // Count in interesting theorems, i.e. the ones have weren't proven
                interestingTheorems += algorithmOutput.Theorems.AllObjects.Count - algorithmOutput.ProverOutput.ProvenTheorems.Count;

                // Find out if there is any interesting theorem, which is when the number
                // of proven theorems is distinct from the number of all theorems
                var anyInterestingTheorem = algorithmOutput.ProverOutput.ProvenTheorems.Count != algorithmOutput.Theorems.AllObjects.Count;

                // Prepare a formatter for the generated configuration
                var formatter = new OutputFormatter(algorithmOutput.Configuration);

                // Prepare the writing function that writes to both reports only when
                // there is anything interesting to write (i.e. interesting theorems)
                Action<string> WriteLine = anyInterestingTheorem ? (Action<string>)WriteLineToBothReports : WriteLineToFullReport;

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
                    outputWriter.WriteLine(TheoremsToString(formatter, algorithmOutput.Theorems, includeProven: false, algorithmOutput.ProverOutput));

                // Write the full report too (if it shouldn't be written, the function will do nothing)
                WriteLineToFullReport(TheoremsToString(formatter, algorithmOutput.Theorems, includeProven: true, algorithmOutput.ProverOutput));

                // Flush after each output so that we can see the results gradually
                outputWriter.Flush();
                fullOutputWriter?.Flush();
            }

            // Write end
            WriteLineToBothReports("\n------------------------------------------------");
            WriteLineToBothReports($"Generated configurations: {generatedConfigurations}");
            WriteLineToBothReports($"Theorems: {totalTheorems}");
            WriteLineToBothReports($"Interesting theorems: {interestingTheorems}");
            WriteLineToBothReports($"Run-time: {stopwatch.ElapsedMilliseconds} ms");

            // Log these stats as well
            Log.LoggingManager.LogInfo($"Algorithm has finished in {stopwatch.ElapsedMilliseconds} ms.");
            Log.LoggingManager.LogInfo($"Generated configurations: {generatedConfigurations}");
            Log.LoggingManager.LogInfo($"Theorems: {totalTheorems}");
            Log.LoggingManager.LogInfo($"Interesting theorems: {interestingTheorems}");
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
        /// Converts given theorems to a string, with their proofs or attempts to prove.
        /// </summary>
        /// <param name="formatter">The formatter of the configuration where the theorems hold.</param>
        /// <param name="theorems">The theorems to be written.</param>
        /// <param name="includeProven">Indicates if we should include the proved theorems as well.</param>
        /// <param name="proverOutput">The output of the theorem analyzer.</param>
        /// <returns>The string representing the theorems.</returns>
        private string TheoremsToString(OutputFormatter formatter, TheoremMap theorems, bool includeProven, TheoremProverOutput proverOutput)
        {
            // First we find which theorems we're going to convert
            var allTheorems = theorems.AllObjects
                // Include each theorem, if we should include proven ones, otherwise only proven ones
                .Where(theorem => includeProven || !proverOutput.ProvenTheorems.ContainsKey(theorem))
                // Order them by their formatted string
                .OrderBy(theorem => formatter.FormatTheorem(theorem))
                // Enumerate
                .ToList();

            // Prepare the dictionary where we will store unique strings associated with 
            // these (and potentially other) theorems used in the reports
            var theoremTags = new Dictionary<Theorem, string>();

            // Initially assign the value 'index + 1' to each of our theorem as their appear in the list
            allTheorems.ForEach((theorem, index) => theoremTags.Add(theorem, $"{index + 1}."));

            // Process every theorem
            var theoremsString = allTheorems.Select((theorem, index) =>
            {
                // Handle unfinished proofs
                if (proverOutput.UnprovenTheorems.ContainsKey(theorem))
                    return $" {index + 1,2}. {UnfinishedProofsReport(theorem, proverOutput.UnprovenTheorems[theorem], formatter, theoremTags)}";

                // Handle finished proofs
                else if (proverOutput.ProvenTheorems.ContainsKey(theorem))
                    return $" {index + 1,2}. {ProofReport(proverOutput.ProvenTheorems[theorem], formatter, theoremTags)}";

                // Other cases shouldn't be possible
                throw new GeoGenException("Incorrect output from the prover - not every theorem is included.");
            })
            // Make each on a separate line
            .ToJoinedString("\n")
            // Ensure no white spaces at the end
            .TrimEnd();

            // If there are any unproved discovered theorems and we are displaying them within attempts
            if (proverOutput.UnprovenDiscoveredTheorems.Any() && _settings.DisplayProofAttempts)
            {
                // Convert them to a string
                var unprovenDiscoveredTheoremsString = proverOutput.UnprovenDiscoveredTheorems
                    // Initially sort them by their formatted versions
                    .OrderBy(pair => formatter.FormatTheorem(pair.Key))
                    // Convert each to a string with the right index and the tag (which should be already assigned)
                    .Select((pair, index) => $" {allTheorems.Count + index + 1,2}. {formatter.FormatTheorem(pair.Key)} - theorem {theoremTags[pair.Key]}")
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
        /// <param name="previousTag">The tag of the proof under which this one falls.</param>
        /// <returns>The string containing the report.</returns>
        private string UnfinishedProofsReport(Theorem theorem,
                                              IReadOnlyList<TheoremProofAttempt> unfinishedProofs,
                                              OutputFormatter formatter,
                                              Dictionary<Theorem, string> theoremTags,
                                              string previousTag = "")
        {
            // First format the unfinished theorem
            var result = formatter.FormatTheorem(theorem);

            // If we are not supposed to display attempts, then we're done
            if (!_settings.DisplayProofAttempts)
                return result;

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
                var newTag = previousTag.IsEmpty() ? $"    {theoremTags[theorem]}" : $"  {previousTag}";

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
                    var untrimmedTag = $"{(previousTag.IsEmpty() ? $"    {theoremTags[proofAttempt.Theorem]}" : $"  {previousTag}")}{index++}.";

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
                       UnfinishedProofsReport(theorem, attempts, formatter, theoremTags, untrimmedTag) :
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
                // There are no needed theorems to make this conclusion, i.e. the theorem is right
                case DerivationRule.DefinableSimpler:

                    // Get the redundant objects
                    var redundantObjects = ((DefinableSimplerDerivationData)proofAttempt.Data).RedundantObjects
                        // Get their names
                        .Select(formatter.GetNameOfObject)
                        // Sort them
                        .Ordered()
                        // Join together
                        .ToJoinedString();

                    // Get the string stating that we needed reformulation
                    var reformulation = proofAttempt.ProvenAssumptions.Any() || proofAttempt.UnprovenAssumptions.Any()
                        ? " after reformulation " : " ";

                    // Return the final conclusion with these objects included
                    return isProved ? $"can be stated{reformulation}without {redundantObjects}" : $"could be stated{reformulation}without {redundantObjects}";

                // Case when it's a trivial consequence of the construction of the object
                // There are no needed theorems to make this conclusion, i.e. the theorem is right
                case DerivationRule.TrivialTheorem:
                    return "trivial theorem";

                // Case when the theorem was attempted as a reformulation of a theorem
                case DerivationRule.ReformulatedTheorem:
                    return "reformulation of a theorem";

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

                // Case when we have two perpendicular lines and one parallelity
                case DerivationRule.PerpendicularLineToParallelLines:
                    return "two perpendicular lines with a common line are equivalent with parallelity";

                // Case when we combine incidences to restate a theorem
                case DerivationRule.ExplicitLineWithIncidences:
                    return "restating using incidences";

                // Case when we put together 3 incidences and collinearity:
                case DerivationRule.IncidencesAndCollinearity:
                    return "incidences are related to collinearity";

                // Case when we put together 4 incidences and concyclicity:
                case DerivationRule.IncidencesAndConcyclity:
                    return "incidences are related to concyclicity";

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
                    return "Parallelogram";

                // Case when we have a rectangle
                case DerivationRule.Rectangle:
                    return "Rectangle";

                // Default case
                default:
                    throw new GeoGenException($"Unhandled type of derivation rule: {proofAttempt.Data.Rule}");
            }
        }

        #endregion
    }
}