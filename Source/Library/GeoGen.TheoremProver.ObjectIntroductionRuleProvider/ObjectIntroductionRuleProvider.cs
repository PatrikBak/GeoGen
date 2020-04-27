using GeoGen.Core;
using GeoGen.Infrastructure;
using GeoGen.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static GeoGen.Infrastructure.Log;

namespace GeoGen.TheoremProver.ObjectIntroductionRuleProvider
{
    /// <summary>
    /// The default implementation of <see cref="IObjectIntroductionRuleProvider"/> that gets the rules from a file.
    /// </summary>
    public class ObjectIntroductionRuleProvider : IObjectIntroductionRuleProvider
    {
        #region Private fields

        /// <summary>
        /// The settings for the provider.
        /// </summary>
        private readonly ObjectIntroductionRuleProviderSettings _settings;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectIntroductionRuleProvider"/> class.
        /// </summary>
        /// <param name="settings">The settings for the provider.</param>
        public ObjectIntroductionRuleProvider(ObjectIntroductionRuleProviderSettings settings)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        #endregion

        #region IObjectIntroductionRuleProvider implementation

        /// <inheritdoc/>
        public async Task<IReadOnlyList<ObjectIntroductionRule>> GetObjectIntroductionRulesAsync()
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
                throw new ObjectIntroductionRuleProviderException($"Couldn't load the object introduction rule file '{_settings.FilePath}'", e);
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
                LoggingManager.LogWarning($"Empty object introduction rule file {_settings.FilePath}");

                // We're done
                return new List<ObjectIntroductionRule>();
            }

            try
            {
                // Try to parse the rules
                var rules = ParseRules(lines);

                // Make sure their constructions are distinct
                if (rules.Select(rule => rule.ExistingObject.Construction).ToArray().AnyDuplicates())
                    throw new ParsingException("The loaded object introduction rules contains duplicate rules for the same construction.");

                // Log their count
                LoggingManager.LogInfo($"Loaded {rules.Count} object introduction rule(s).");

                // Return them
                return rules;
            }
            catch (ParsingException e)
            {
                // Throw further
                throw new ObjectIntroductionRuleProviderException($"Couldn't parse the object introduction rule file {_settings.FilePath}.", e);
            }

            #endregion
        }

        /// <summary>
        /// Parses given lines to object introduction rules.
        /// </summary>
        /// <param name="lines">The trimmed non-empty lines without comments to be parsed.</param>
        /// <returns>The parsed object introduction rules.</returns>
        private static IReadOnlyList<ObjectIntroductionRule> ParseRules(IReadOnlyList<string> lines)
        {
            // Each line is either the beginning of a rule, or an object
            // Object lines start with '-'. Make sure the first line isn't an object line
            if (lines[0].StartsWith('-'))
                throw new ParsingException($"The first line cannot start with '-', since this character starts lines with new objects.");

            // We need to find lines that are not object lines, i.e. those that begin rules
            // First index lines
            return lines.Select((line, index) => (line, index))
                // The rules not starting with '-' are what we want 
                .Where(pair => !pair.line.StartsWith('-'))
                // Now these are lines that we want
                .Select(pair =>
                {
                    // Deconstruct
                    var (ruleLine, index) = pair;

                    // Prepare the dictionary with object names
                    var namesToObjects = new Dictionary<string, ConfigurationObject>();

                    // Prepare the existing object template for parsing
                    var existingObject = default(ConstructedConfigurationObject);

                    try
                    {
                        // Let it be parsed by the parser that will automatically create the inner loose objects for us
                        existingObject = Parser.ParseObjectWithDefinition(ruleLine, namesToObjects, autocreateUnnamedObjects: true);
                    }
                    catch (ParsingException e)
                    {
                        // If there is a problem, make aware
                        throw new ParsingException($"Error while parsing {ruleLine}. {e.Message}");
                    }

                    // Go through the lines with a new object
                    var newObjects = lines.ItemsBetween(index + 1, lines.Count)
                        // But only until we hit a line that is not a new object
                        .TakeWhile(potentialNewObjectLine => potentialNewObjectLine.StartsWith('-'))
                        // Parse each to a new object
                        .Select(newObjectLine =>
                        {
                            // We now know it starts with -, get rid of it
                            newObjectLine = newObjectLine.Substring(1).Trim();

                            try
                            {
                                // Parse the new constructed object. All objects must be named and defined now
                                return Parser.ParseConstructedObject(newObjectLine, namesToObjects, autocreateUnnamedObjects: false);
                            }
                            catch (ParsingException e)
                            {
                                // If there is a problem, make aware
                                throw new ParsingException($"Error while parsing '{newObjectLine}' of the rule for {ruleLine}. {e.Message}");
                            }
                        })
                        // Enumerate
                        .ToArray();

                    // Return the final rule
                    return new ObjectIntroductionRule(existingObject, newObjects);
                })
                // Enumerate
                .ToList();
        }

        #endregion
    }
}
