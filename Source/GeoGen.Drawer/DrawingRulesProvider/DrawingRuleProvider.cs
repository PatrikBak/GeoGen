using GeoGen.ConsoleLauncher;
using GeoGen.Core;
using GeoGen.Infrastructure;
using GeoGen.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static GeoGen.Core.ConfigurationObjectType;
using static GeoGen.Infrastructure.Log;

namespace GeoGen.Drawer
{
    /// <summary>
    /// The default implementation of <see cref="IDrawingRulesProvider"/> that gets the rules from a file.
    /// </summary>
    public class DrawingRulesProvider : IDrawingRulesProvider
    {
        #region Private fields

        /// <summary>
        /// The settings for the provider.
        /// </summary>
        private readonly DrawingRulesProviderSettings _settings;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="DrawingRulesProviderSettings"/> class.
        /// </summary>
        /// <param name="settings">The settings for the provider.</param>
        public DrawingRulesProvider(DrawingRulesProviderSettings settings)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        #endregion

        #region IDrawingRulesProvider implementation

        /// <summary>
        /// Gets drawing rules.
        /// </summary>
        /// <returns>The drawing rules.</returns>
        public async Task<IReadOnlyList<DrawingRule>> GetDrawingRulesAsync()
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
                throw new GeoGenException($"Couldn't load the drawing rules file '{_settings.FilePath}'", e);
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
                LoggingManager.LogWarning($"Empty drawing rules file {_settings.FilePath}");

                // We're done
                return new List<DrawingRule>();
            }

            try
            {
                // Try to parse the rules
                var rules = ParseRules(lines);

                // Make sure their constructions are distinct
                if (rules.Select(rule => rule.ObjectToDraw.Construction).ToArray().AnyDuplicates())
                    throw new GeoGenException("The loaded drawing rules contains duplicate rules for the same construction.");

                // Log their count
                LoggingManager.LogInfo($"Loaded {rules.Count} drawing rule(s).");

                // Return them
                return rules;
            }
            catch (ParsingException e)
            {
                // Log the content
                LoggingManager.LogDebug($"Loaded content:\n\n{fileContent}\n");

                // Throw further
                throw new GeoGenException($"Couldn't parse the drawing rules file {_settings.FilePath}.", e);
            }

            #endregion
        }

        /// <summary>
        /// Parses given lines to drawing rules.
        /// </summary>
        /// <param name="lines">The trimmed non-empty lines without comments to be parsed.</param>
        /// <returns>The parsed drawing rules.</returns>
        private static IReadOnlyList<DrawingRule> ParseRules(IReadOnlyList<string> lines)
        {
            // Each line is either the beginning of a rule, or a command
            // Command lines start with '-'. Make sure the first line isn't a command
            if (lines[0].StartsWith('-'))
                throw new ParsingException($"The first line cannot start with '-', since this character starts lines with command.");

            // We need to find lines that are not command lines, i.e. those that begin rules
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

                    // Object the object to be drawn for parsing
                    var objectToDraw = default(ConstructedConfigurationObject);

                    try
                    {
                        // Let it be parsed by the parser that will automatically create the inner loose objects for us
                        objectToDraw = Parser.ParseObjectWithDefinition(ruleLine, namesToObjects, autocreateUnnamedObjects: true);
                    }
                    catch (ParsingException e)
                    {
                        // If there is a problem, make aware
                        throw new ParsingException($"Error while parsing {ruleLine}. {e.Message}");
                    }

                    // Prepare auxiliary objects
                    var auxiliaryObjects = new List<ConstructedConfigurationObject>();

                    // Prepare drawing commands
                    var drawingCommands = new List<DrawingCommand>();

                    // Go through the lines with a command 
                    lines.ItemsBetween(index + 1, lines.Count)
                        // But only until we hit a line that is not a command
                        .TakeWhile(potentialCommand => potentialCommand.StartsWith('-'))
                        // Parse each
                        .ForEach(commandLine =>
                        {
                            // We now know it starts with -, get rid of it
                            commandLine = commandLine.Substring(1).Trim();

                            // Match the line defining an auxiliary object, which is in the form 'let name = definition'
                            var auxiliaryObjectMatch = Regex.Match(commandLine, "^let (.*)$");

                            // Match the line defining an actual command, which is in the form 'draw Type(arguments)' and might
                            // or might not end with the word 'auxiliary'
                            var drawingCommandMatch = Regex.Match(commandLine, "^draw (.*)\\((.*)\\)(?: (auxiliary))?$");

                            // If we have an auxiliary object match...
                            if (auxiliaryObjectMatch.Success)
                            {
                                // Then the definition is in the first group
                                var objectDefinition = auxiliaryObjectMatch.Groups[1].Value.Trim();

                                try
                                {
                                    // Try to parse it. All the objects must be defined now
                                    var auxiliaryObject = Parser.ParseObjectWithDefinition(objectDefinition, namesToObjects, autocreateUnnamedObjects: false);

                                    // Add it to the auxiliary object list
                                    auxiliaryObjects.Add(auxiliaryObject);

                                    // Move on
                                    return;
                                }
                                catch (ParsingException e)
                                {
                                    // If there is a problem, make aware
                                    throw new ParsingException($"Error while parsing '{commandLine}' of the rule for {ruleLine}. {e.Message}");
                                }
                            }

                            // If we have a drawing command match...
                            if (drawingCommandMatch.Success)
                            {
                                #region Parsing drawing command type

                                // Then the first group is the name of the command
                                var commandName = drawingCommandMatch.Groups[1].Value.Trim();

                                // Try to parse the command name
                                if (!Enum.TryParse<DrawingCommandType>(commandName, out var drawingCommandType))
                                    throw new ParsingException($"Error while parsing '{commandLine}' of the rule for {ruleLine}. Unknown command name '{commandName}'");

                                #endregion

                                #region Parsing arguments

                                // The second group are the arguments of the command
                                var arguments = drawingCommandMatch.Groups[2].Value.Trim();

                                // Find the actual objects for them
                                var argumentObjects = arguments
                                    // Split them by a comma
                                    .Split(",")
                                    // Trim each
                                    .Select(objectName => objectName.Trim())
                                    // Find the object (that must already exist)
                                    .Select(objectName => namesToObjects.GetOrDefault(objectName)
                                        // If it doesn't exist, make aware
                                        ?? throw new ParsingException($"Error while parsing '{commandLine}' of the rule for {ruleLine}. Unknown object '{objectName}'"))
                                    // Enumerate
                                    .ToArray();

                                // Get their types for control
                                var typesOfArgumentObjects = argumentObjects.Select(configurationObject => configurationObject.ObjectType).ToArray();

                                // We need to make sure we have passed objects or the right count and the right type
                                // For that we need to distinguish cases based on the command type
                                switch (drawingCommandType)
                                {
                                    // Point mark
                                    case DrawingCommandType.Point:

                                        // There should be one point
                                        if (typesOfArgumentObjects.Length != 1 || typesOfArgumentObjects[0] != Point)
                                            throw new ParsingException($"Error while parsing '{commandLine}' of the rule for {ruleLine}. {drawingCommandType} accepts 1 point.");

                                        break;

                                    // Line segment or shifted line segment
                                    case DrawingCommandType.Segment:
                                    case DrawingCommandType.ShiftedSegment:

                                        // There should be two points
                                        if (typesOfArgumentObjects.Length != 2 || !typesOfArgumentObjects.All(type => type == Point))
                                            throw new ParsingException($"Error while parsing '{commandLine}' of the rule for {ruleLine}. {drawingCommandType} accepts 2 points.");

                                        break;

                                    // Line
                                    case DrawingCommandType.Line:

                                        // There should be one line and the rest should be points
                                        if (typesOfArgumentObjects.Length < 1 || typesOfArgumentObjects[0] != Line || !typesOfArgumentObjects.Skip(1).All(type => type == Point))
                                            throw new ParsingException($"Error while parsing '{commandLine}' of the rule for {ruleLine}. {drawingCommandType} accepts 1 line and optional points.");

                                        break;

                                    // Shifted point
                                    case DrawingCommandType.ShiftedLine:

                                        // There should be one line and the rest should be points, at least one
                                        if (typesOfArgumentObjects.Length < 2 || typesOfArgumentObjects[0] != Line || !typesOfArgumentObjects.Skip(1).All(type => type == Point))
                                            throw new ParsingException($"Error while parsing '{commandLine}' of the rule for {ruleLine}. {drawingCommandType} accepts 1 line and at least 1 point.");

                                        break;

                                    // Circle
                                    case DrawingCommandType.Circle:

                                        // There should be one circle
                                        if (typesOfArgumentObjects.Length != 1 || typesOfArgumentObjects[0] != Circle)
                                            throw new ParsingException($"Error while parsing '{commandLine}' of the rule for {ruleLine}. {drawingCommandType} accepts 1 circle.");

                                        break;

                                    // If something else...
                                    default:
                                        throw new ParsingException($"Unhandled type of {nameof(DrawingCommandType)}: {drawingCommandType}.");
                                }

                                #endregion

                                #region Figuring out object style

                                // And the third group is either not there, or should contain the word 'auxiliary'
                                var auxiliarityFlag = drawingCommandMatch.Groups.Count == 2 ? "" : drawingCommandMatch.Groups[3].Value.Trim();

                                // Get the object style based on this flag
                                var objectStyle = auxiliarityFlag switch
                                {
                                    // The default style is normal
                                    "" => ObjectDrawingStyle.NormalObject,

                                    // Auxiliary objects have to be specified literary
                                    "auxiliary" => ObjectDrawingStyle.AuxiliaryObject,

                                    // Nothing else is supported
                                    _ => throw new ParsingException($"Error while parsing '{commandLine}' of the rule for {ruleLine}. The style must be either empty, or 'auxiliary'.")
                                };

                                #endregion

                                // Add the created command
                                drawingCommands.Add(new DrawingCommand(drawingCommandType, objectStyle, argumentObjects));

                                // Move on
                                return;
                            }

                            // If we got here, then neither type of line matches
                            throw new ParsingException($"Error while parsing '{commandLine}'. The line should either define an object " +
                                    $"via let name = definition, or it should contain a drawing command in the form 'draw Type(arguments)' " +
                                    $"or 'draw Type(arguments) auxiliary'");
                        });

                    // Return the final rule
                    return new DrawingRule(objectToDraw, auxiliaryObjects, drawingCommands);

                })
                // Enumerate
                .ToList();
        }

        #endregion
    }
}
