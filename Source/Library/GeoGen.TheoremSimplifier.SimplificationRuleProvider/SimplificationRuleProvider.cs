using GeoGen.Core;
using GeoGen.Utilities;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GeoGen.TheoremSimplifier.SimplificationRuleProvider
{
    /// <summary>
    /// The default implementation of <see cref="ISimplificationRuleProvider"/> that gets the rules from a file.
    /// </summary>
    public class SimplificationRuleProvider : ISimplificationRuleProvider
    {
        #region Private fields

        /// <summary>
        /// The settings for the provider.
        /// </summary>
        private readonly SimplificationRuleProviderSettings _settings;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="SimplificationRuleProvider"/> class.
        /// </summary>
        /// <param name="settings">The settings for the provider.</param>
        public SimplificationRuleProvider(SimplificationRuleProviderSettings settings)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        #endregion

        #region ISimplificationRuleProvider implementation

        /// <inheritdoc/>
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
            catch (Exception e)
            {
                // If it cannot be done, make aware
                throw new SimplificationRuleProviderException($"Couldn't load the simplification rule file '{_settings.FilePath}'", e);
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
                Log.Warning("Empty simplification rule file {path}", _settings.FilePath);

                // We're done
                return new List<SimplificationRule>();
            }

            try
            {
                // Try to parse the rules
                var rules = ParseRules(lines);

                // Log their count
                Log.Information("Loaded {count} simplification rule(s).", rules.Count);

                // Return them
                return rules;
            }
            catch (ParsingException e)
            {
                // Log the content
                Log.Debug("Loaded content:\n\n{content}\n", fileContent);

                // Throw further
                throw new SimplificationRuleProviderException($"Couldn't parse the simplification rule file {_settings.FilePath}.", e);
            }

            #endregion
        }

        /// <summary>
        /// Parses given lines to simplification rules.
        /// </summary>
        /// <param name="lines">The trimmed non-empty lines without comments to be parsed.</param>
        /// <returns>The parsed simplification rules.</returns>
        private static IReadOnlyList<SimplificationRule> ParseRules(IReadOnlyList<string> lines)
            // There should be a rule on each line
            => lines.Select(ruleLine =>
            {
                // Match the format of the rules. They should in the form "TheoremObject --> TheoremObject:
                var ruleMatch = Regex.Match(ruleLine, "^(.*) --> (.*)$");

                // If there is no match, make aware 
                if (!ruleMatch.Success)
                    throw new ParsingException($"Error while parsing {ruleLine}. It should be in the form TheoremObject --> TheoremObject.");

                // Otherwise prepare the dictionary with object names
                var namesToObjects = new Dictionary<string, ConfigurationObject>();

                #region Parsing simplifiable object

                // Get the simplifiable object string
                var simplifableObjectString = ruleMatch.Groups[1].Value.Trim();

                // Prepare a variable for the parsed object
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
                var simplifiedObjectString = ruleMatch.Groups[2].Value.Trim();

                // Prepare a variable for the parsed object
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

                // Return the final rule
                return new SimplificationRule(simplifiableObject, simplifiedObject);

            })
            // Enumerate
            .ToList();

        #endregion
    }
}