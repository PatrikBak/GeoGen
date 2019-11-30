using GeoGen.Core;
using GeoGen.TheoremSimplifier;
using GeoGen.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static GeoGen.ConsoleLauncher.Log;

namespace GeoGen.ConsoleLauncher
{
    /// <summary>
    /// The default implementation of <see cref="ISimplificationRulesProvider"/> that gets the rules from a file.
    /// </summary>
    public class SimplificationRulesProvider : ISimplificationRulesProvider
    {
        #region Private fields

        /// <summary>
        /// The settings for the provider.
        /// </summary>
        private readonly SimplificationRulesProviderSettings _settings;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="SimplificationRulesProvider"/> class.
        /// </summary>
        /// <param name="settings">The settings for the provider.</param>
        public SimplificationRulesProvider(SimplificationRulesProviderSettings settings)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        #endregion

        #region ISimplificationRulesProvider implementation

        /// <summary>
        /// Gets simplification rules.
        /// </summary>
        /// <returns>The simplification rules.</returns>
        public async Task<IReadOnlyList<SimplificationRule>> GetSimplificationRulesAsync()
        {
            #region Loading the file

            // Prepare the file content
            var fileContent = default(string);

            try
            {
                // Get the content of the file
                fileContent = await File.ReadAllTextAsync(_settings.FilePath);
            }
            catch (Exception)
            {
                // Warn
                LoggingManager.LogWarning($"Couldn't load the simplification file {_settings.FilePath}.");

                // We're done
                return new List<SimplificationRule>();
            }

            #endregion

            #region Parsing it

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
                LoggingManager.LogWarning($"Empty simplification rules file {_settings.FilePath}");

                // We're done
                return new List<SimplificationRule>();
            }

            try
            {
                // Try to parse the rules
                var rules = ParseRules(lines);

                // Log their count
                LoggingManager.LogInfo($"Loaded {rules.Count} simplification rule(s).");

                // Return them
                return rules;
            }
            catch (ParsingException)
            {
                // Warn the exception
                LoggingManager.LogError($"Couldn't parse the simplification rules file {_settings.FilePath}.");

                // Log the content
                LoggingManager.LogDebug($"Loaded content:\n\n{fileContent}\n");

                // Throw further
                throw;
            }

            #endregion
        }

        /// <summary>
        /// Parses given lines to simplification rules.
        /// </summary>
        /// <param name="lines">The trimmed non-empty lines without comments to be parsed.</param>
        /// <returns>The parsed simplification rules.</returns>
        private static IReadOnlyList<SimplificationRule> ParseRules(IReadOnlyList<string> lines)
        {
            // Each line is either the beginning of a rule, or an assumption
            // Assumption lines start with '-'. Make sure the first line isn't an assumption line
            if (lines[0].StartsWith('-'))
                throw new ParsingException($"The first line cannot start with '-', since this character starts lines with assumptions.");

            // We need to find lines that are not assumption lines, i.e. those that rules
            // First index lines
            return lines.Select((line, index) => (line, index))
                // The rules not starting with '-' are what we want 
                .Where(pair => !pair.line.StartsWith('-'))
                // Now these are lines that we want
                .Select(pair =>
                {
                    // Deconstruct
                    var (ruleLine, index) = pair;

                    #region Parsing rule

                    // Now the actual parsing is about to happen
                    // Match the format of the rules. They should in the form "TheoremObject --> TheoremObject:
                    var ruleMatch = Regex.Match(ruleLine, "^(.*) --> (.*)$");

                    // If there is no match, make aware 
                    if (!ruleMatch.Success)
                        throw new ParsingException($"Error while parsing {ruleLine}. It should be in the form TheoremObject --> TheoremObject.");

                    // Otherwise prepare the dictionary with object names
                    var namesToObjects = new Dictionary<string, ConfigurationObject>();

                    #region Parsing simplifiable object

                    // Get the simplifiable object string
                    var simplifableObjectString = ruleMatch.Groups[1].Value;

                    // Prepare a variable for it
                    var simplifiableObject = default(TheoremObject);

                    try
                    {
                        // Try to parse it, while automatically creating loose points for unknown objects
                        simplifiableObject = Parser.ParseTheoremObject(simplifableObjectString, namesToObjects, autocreateUnnamedObjects: true);
                    }
                    catch (ParsingException e)
                    {
                        // If there is a problem, make aware
                        throw new ParsingException($"Error while parsing {simplifableObjectString}. {e.Message}");
                    }

                    #endregion

                    #region Parsing simplified object

                    // Get the simplified object string
                    var simplifiedObjectString = ruleMatch.Groups[2].Value;

                    // Prepare a variable for it
                    var simplifiedObject = default(TheoremObject);

                    try
                    {
                        // Try to parse it while automatically creating loose points for unknown objects
                        simplifiedObject = Parser.ParseTheoremObject(simplifiedObjectString, namesToObjects, autocreateUnnamedObjects: true);
                    }
                    catch (ParsingException e)
                    {
                        // If there is a problem, make aware
                        throw new ParsingException($"Error while parsing {simplifiedObjectString}. {e.Message}");
                    }

                    #endregion

                    #endregion

                    #region Parsing assumptions

                    // Find assumption lines by going from the next line
                    var assumptions = lines.ItemsBetween(index + 1, lines.Count)
                        // But only until we hit a line that is not an assumption
                        .TakeWhile(potentialAssumption => potentialAssumption.StartsWith('-'))
                        // Parse each
                        .Select(assumptionLine =>
                        {
                            // We now know it starts with -, get rid of it
                            assumptionLine = assumptionLine.Substring(1);

                            // Parse it, assuming it's object will be automatically created
                            return Parser.ParseTheorem(assumptionLine, namesToObjects, autocreateUnnamedObjects: true);
                        })
                        // Enumerate
                        .ToReadOnlyHashSet();

                    #endregion

                    // Return the final rule
                    return new SimplificationRule(simplifiableObject, simplifiedObject, assumptions);

                })
                // Enumerate
                .ToList();
        }

        #endregion
    }
}
