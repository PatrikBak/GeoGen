using GeoGen.Core;
using GeoGen.Infrastructure;
using GeoGen.TheoremProver;
using GeoGen.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static GeoGen.Infrastructure.Log;

namespace GeoGen.ConsoleLauncher
{
    /// <summary>
    /// The default implementation of <see cref="IInferenceRuleProvider"/> that looks for <see cref="LoadedInferenceRule"/>s
    /// in a folder specified via <see cref="InferenceRuleProviderSettings"/>.
    /// </summary>
    public class InferenceRuleProvider : IInferenceRuleProvider
    {
        #region Private fields

        /// <summary>
        /// The settings of the inference rule folder.
        /// </summary>
        private readonly InferenceRuleProviderSettings _settings;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="InferenceRuleProvider"/> class.
        /// </summary>
        /// <param name="settings">The settings of the inference rule folder.</param>
        public InferenceRuleProvider(InferenceRuleProviderSettings settings)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        #endregion

        #region IInferenceRuleProvider implementation

        /// <inheritdoc/>
        public async Task<IReadOnlyList<LoadedInferenceRule>> GetInferenceRulesAsync()
        {
            // If the dictionary doesn't exist...
            if (!Directory.Exists(_settings.RuleFolderPath))
            {
                // Warn
                LoggingManager.LogWarning($"The directory for inference rules {_settings.RuleFolderPath} doesn't exist.");

                // No rules
                return new List<LoadedInferenceRule>();
            }

            // Prepare the result
            var result = new List<LoadedInferenceRule>();

            // Prepare the inference rule files loaded from the rules folder and its subdirectories
            var ruleFiles = Directory.EnumerateFiles(_settings.RuleFolderPath, $"*.{_settings.FileExtension}", SearchOption.AllDirectories);

            // Go through all the files
            foreach (var inferenceRuleFile in ruleFiles)
            {
                #region Reading the inference rule file

                // Prepare the content
                var fileContent = default(string);

                try
                {
                    // Get the content
                    fileContent = await File.ReadAllTextAsync(inferenceRuleFile);
                }
                catch (Exception e)
                {
                    // If it cannot be done, make aware
                    throw new GeoGenException($"Couldn't load the inference rule file '{inferenceRuleFile}'", e);
                }

                #endregion

                #region Parsing the inference rule file

                // Get the lines 
                var lines = fileContent.Split('\n')
                    // Trimmed
                    .Select(line => line.Trim())
                    // That are not comments or empty ones
                    .Where(line => !line.StartsWith('#') && !string.IsNullOrEmpty(line))
                    // As a list
                    .ToList();

                // If there is no content
                if (lines.IsEmpty())
                {
                    // Warn
                    LoggingManager.LogWarning($"Empty inference rule file {inferenceRuleFile}");

                    // Move on
                    continue;
                }

                try
                {
                    // Try to parse it
                    var inferenceRules = ParseInferenceRules(lines)
                        // Create loaded rules
                        .Select(triple => new LoadedInferenceRule
                        (
                            // Copy the info from the parsed file
                            declaredObjects: triple.rule.DeclaredObjects,
                            assumptionGroups: triple.rule.AssumptionGroups,
                            negativeAssumptions: triple.rule.NegativeAssumptions,
                            conclusion: triple.rule.Conclusion,

                            // Add the info about the relative file name
                            relativeFileName: Path.GetRelativePath(_settings.RuleFolderPath, inferenceRuleFile),

                            // Add the info about the index of the rule within the file
                            number: triple.lineIndex + 1,

                            // Set the message
                            adjustmentMessage: triple.adjustmentMessage
                        ))
                        // Enumerate
                        .ToArray();

                    // If there are no rules, make aware
                    if (inferenceRules.IsEmpty())
                    {
                        // Warn
                        LoggingManager.LogWarning($"No rules in inference rule file {inferenceRuleFile}");

                        // Move on
                        continue;
                    }

                    // Add them to the result
                    result.AddRange(inferenceRules);
                }
                catch (ParsingException e)
                {
                    // If something went wrong, log the content
                    LoggingManager.LogDebug($"Loaded content:\n\n{fileContent}\n");

                    // Throw further
                    throw new GeoGenException($"Couldn't parse the inference rule file {inferenceRuleFile}.", e);
                }

                #endregion
            }

            #region Logging database stats

            // Log count
            LoggingManager.LogInfo($"{result.Count} inference rules");

            // Prepare the map of types and number of rules proving this type
            // Take all the rules
            var typeToCountsString = result
                // Group them by the theorem they prove
                .GroupBy(rule => rule.Conclusion.Type)
                // Order by the count of rules of each type ASC
                .OrderBy(group => -group.Count())
                // Compose strings
                .Select(group => $"  - {group.Key} --> {group.Count()}")
                // Join
                .ToJoinedString("\n");

            // Log it if there are some rules
            if (result.Any())
                LoggingManager.LogInfo($"The loaded rules prove the following:\n\n{typeToCountsString}\n");

            #endregion

            // Return the result
            return result;
        }

        /// <summary>
        /// Parses given lines representing the content of a single file to a list of inference rules with their potential 
        /// adjustment messages and line indices.
        /// </summary>
        /// <param name="lines">The trimmed non-empty lines without comments to be parsed.</param>
        /// <returns>The parsed inference rules with adjustment messages and their line indices.</returns>
        private static IEnumerable<(InferenceRule rule, string adjustmentMessage, int lineIndex)> ParseInferenceRules(IReadOnlyList<string> lines)
        {
            #region Finding sections

            // The first line has to be the objects
            if (lines[0] != "Objects:")
                throw new ParsingException("The first line should be in the form: 'Objects:'");

            // Get the index of the theorems starting line
            var theoremsLineIndex = lines.IndexOf("Theorems:");

            // Make sure there is some
            if (theoremsLineIndex == -1)
                throw new ParsingException("No line starting the theorems section in the form: 'Theorems:'");

            // Get the index of the implications starting line
            var implicationsLineIndex = lines.IndexOf("Implications:");

            // Make sure there is some
            if (implicationsLineIndex == -1)
                throw new ParsingException("No line starting the implications section in the form: 'Implications:'");

            // Make sure it is below the theorems
            if (implicationsLineIndex < theoremsLineIndex)
                throw new ParsingException("Theorems section has to be below Implications section");

            #endregion

            // Prepare the object names dictionary
            var namesToObject = new Dictionary<string, ConfigurationObject>();

            #region Parsing objects

            // Parse all objects by taking the lines
            var declaredObjects = lines
                // That represent object declarations
                .ItemsBetween(1, theoremsLineIndex)
                // Parse each
                .Select(line => Parser.ParseObjectWithDefinition(line, namesToObject, autocreateUnnamedObjects: true))
                // Enumerate
                .ToArray();

            #endregion

            // Prepare the theorem names dictionary
            var namesToTheorems = new Dictionary<string, Theorem>();

            #region Parsing theorems

            // Get the theorem lines
            lines.ItemsBetween(theoremsLineIndex + 1, implicationsLineIndex)
                // Parse each
                .ForEach(line =>
                {
                    // The form should be {name} = {definition}
                    var match = Regex.Match(line, "^(.*)=(.*)$");

                    // If there is no match, make aware
                    if (!match.Success)
                        throw new ParsingException($"Error while parsing '{line}', each theorem declaration should be in the form of name = definition");

                    // Get the name
                    var name = match.Groups[1].Value.Trim();

                    // Make sure the name is correct
                    if (!name.All(char.IsLetterOrDigit))
                        throw new ParsingException($"Error while parsing '{name}'. The name of a theorem can contain only letters and digits, this one is '{name}'");

                    // Make sure the name is unique
                    if (namesToTheorems.ContainsKey(name))
                        throw new ParsingException($"A theorem with a name '{name}' already exists.");

                    // Get the theorem string
                    var theoremString = match.Groups[2].Value.Trim();

                    // Parse the theorem
                    var theorem = Parser.ParseTheorem(theoremString, namesToObject, autocreateUnnamedObjects: true);

                    // Add it to the result
                    namesToTheorems.Add(name, theorem);
                });

            #endregion

            #region Parsing implications

            // Take the lines
            return lines
                // Representing implications
                .ItemsBetween(implicationsLineIndex + 1, lines.Count)
                // Parse each to an inference rule
                .Select(line =>
                {
                    // The line should be in the form 'assumption theorems => conclusion theorems'
                    var match = Regex.Match(line, "^\\((.*)\\) => (.*)$");

                    // If there is no match, make aware
                    if (!match.Success)
                        throw new ParsingException($"The implication line should be in the form " +
                            $"'(groups)' => conclusion, where each group is either a theorem name " +
                            $"(might start with '!' which indicates it is a negative assumption), or " +
                            $"a comma-separated list of theorem names enclosed in curly brackets.");

                    // Get the group string
                    var groupString = match.Groups[1].Value.Trim();

                    // Prepare the final assumption groups that have been parsed from the group string.
                    var assumptionGroups = new List<IReadOnlyHashSet<Theorem>>();

                    // Prepare the negative assumptions that have been parsed from the group string.
                    var negativeAssumptions = new List<Theorem>();

                    // We need to find the groups within curly braces, for that 
                    // we replaces the commas that are outside of groups with semicolons
                    groupString.ReplaceBalancedCommasWithSemicolons()
                        // Now we can split by semicolons
                        .Split(';')
                        // Trim
                        .Select(groupString => groupString.Trim())
                        // Take non-empty ones
                        .Where(groupString => !groupString.IsEmpty())
                        // Each string now represents either a group of theorems in curly braces,
                        // or a solo theorem, which might be either positive or negative 
                        .ForEach(groupString =>
                        {
                            // Find out if this is a single negative assumption, which is when it starts with '!'
                            if (groupString.StartsWith('!'))
                            {
                                // If yes, get the name by removing the !
                                var negativeAssumptionName = groupString.Substring(1);

                                // Parse it
                                var negativeAssumption = namesToTheorems.GetValueOrDefault(negativeAssumptionName)
                                    // Make aware if it is not named
                                    ?? throw new ParsingException($"Error while parsing '{line}', unknown theorem '{negativeAssumptionName}'");

                                // Add the parsed theorem to the list
                                negativeAssumptions.Add(negativeAssumption);

                                // We're done
                                return;
                            }

                            // If we got here, we don't have a negative assumption. Try to match it as a group of theorems
                            var match = Regex.Match(groupString, "^{(.*)}$");

                            // If there is no match
                            var theoremsString = !match.Success
                                // Then this is a solo theorem
                                ? groupString.ToEnumerable()
                                // Otherwise we split the inner group by commas
                                : match.Groups[1].Value.Trim().Split(',');

                            // Now we can finally handle the particular theorems
                            var assumptionGroup = theoremsString
                                // Trim each
                                .Select(assumptionName => assumptionName.Trim())
                                // Each is supposed to be named
                                .Select(assumptionName => namesToTheorems.GetValueOrDefault(assumptionName)
                                        // Make aware it is named
                                        ?? throw new ParsingException($"Error while parsing '{line}', unknown theorem '{assumptionName}'"))
                                // Enumerate to a read-only hash set
                                .ToReadOnlyHashSet();

                            // Add the parsed group to the result
                            assumptionGroups.Add(assumptionGroup);
                        });

                    // Get the conclusion string
                    var conclusionString = match.Groups[2].Value.Trim();

                    // Either the conclusion is named...
                    var conclusion = namesToTheorems.ContainsKey(conclusionString)
                        // And then we get it from the map
                        ? namesToTheorems[conclusionString]
                        // Or we need to parse it separately
                        : Parser.ParseTheorem(conclusionString, namesToObject, autocreateUnnamedObjects: true);

                    // Finally we have everything
                    return new InferenceRule(declaredObjects, assumptionGroups.ToReadOnlyHashSet(), negativeAssumptions, conclusion);
                })
                // Add the index
                .Select((rule, lineIndex) => (rule, lineIndex))
                // Now we're going to adjust those rules that are needed
                // The point is to move undeclared conclusion objects into declared ones
                .SelectMany(pair =>
                {
                    // Deconstruct
                    var (rule, lineIndex) = pair;

                    // Find the undeclared conclusion objects by taking the inner conclusion's objects
                    var undeclaredObjects = rule.Conclusion.GetInnerConfigurationObjects()
                        // That are constructed
                        .OfType<ConstructedConfigurationObject>()
                        // That are not among declared ones
                        .Where(innerObject => !rule.DeclaredObjects.Contains(innerObject))
                        // Enumerate
                        .ToArray();

                    // If there are none, no adjustment is needed
                    if (undeclaredObjects.IsEmpty())
                        return (rule, adjustmentMessage: "", lineIndex).ToEnumerable();

                    // Otherwise something will happen
                    // We could use a local helper that does the redeclaration
                    (InferenceRule rule, string adjustmentMessage, int lineIndex) Redeclare(ConstructedConfigurationObject[] redeclaredObjects)
                        // Return the tuple with the rule
                        => (new InferenceRule
                            (
                                // The declared objects gets new ones
                                declaredObjects: rule.DeclaredObjects.Concat(redeclaredObjects).ToArray(),

                                // Other data stay the same
                                rule.AssumptionGroups, rule.NegativeAssumptions, rule.Conclusion
                            ),
                            // Set the message as well based on the count of the redeclared objects
                            adjustmentMessage: redeclaredObjects.Count() == 1
                                // If there is just one object, we mention its construction explicitly
                                ? $"adjusted by declaring {redeclaredObjects[0].Construction.Name}"
                                // Otherwise all have been redeclared
                                : $"adjusted by declaring all objects",
                            // Set the line index as well
                            lineIndex);

                    // If this is an equality...
                    if (rule.Conclusion.Type == TheoremType.EqualObjects)
                    {
                        // No redeclaration is needed if there is one object
                        if (undeclaredObjects.Count() == 1)
                            return (rule, adjustmentMessage: "", lineIndex).ToEnumerable();

                        // Otherwise there are two options
                        return undeclaredObjects.Select(equalityObject => Redeclare(new[] { equalityObject }));
                    }

                    // If this is not an equality, then we simply redeclare all the objects
                    return Redeclare(undeclaredObjects).ToEnumerable();
                });

            #endregion
        }

        #endregion
    }
}