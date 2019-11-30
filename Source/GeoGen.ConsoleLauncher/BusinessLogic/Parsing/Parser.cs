using GeoGen.Algorithm;
using GeoGen.Core;
using GeoGen.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace GeoGen.ConsoleLauncher
{
    /// <summary>
    /// The static parser of GeoGen core objects. Its method throw a <see cref="ParsingException"/>.
    /// </summary>
    public static class Parser
    {
        /// <summary>
        /// Parses a construction from its name. The available constructions are taken from <see cref="Constructions"/>.
        /// </summary>
        /// <param name="constructionName">The name of the construction.</param>
        /// <returns>The parsed construction.</returns>
        public static Construction ParseConstruction(string constructionName)
        {
            // Local helper that returns a string informing about available constructions
            static string AvailableConstructions() => $"Available: \n\n" +
                // Get all constructions, prepend -, sort, and make every on a single line
                $"{Constructions.GetAllConstructions().Select(c => $"  {c.Name}").Ordered().ToJoinedString("\n")}.\n";

            // Try to find if it is a predefined one
            if (Enum.TryParse<PredefinedConstructionType>(constructionName, out var type))
                return Constructions.GetPredefinedconstruction(type);

            // Try to find if the construction doesn't start with RandomPointOn
            var match = Regex.Match(constructionName, "^RandomPointOn(.*)$");

            // If yes
            if (match.Success)
            {
                // Get the rest of the name
                var restOfName = match.Groups[1].Value;

                // Try to parse it 
                // If it worked out, wrap the result in the RandomPointOn construction
                if (Enum.TryParse(restOfName, out type))
                    return RandomPointOnConstruction.RandomPointOn(type);

                // Try to get the composed one
                var composedConstruction = Constructions.GetComposedConstruction(restOfName);

                // If it worked out, wrap the result in the RandomPointOn construction 
                if (composedConstruction != null)
                    return RandomPointOnConstruction.RandomPointOn(composedConstruction);

                // Otherwise we won't allow a composed construction with a name like this
                throw new ParsingException("If the name of a construction starts with RandomPointOn, " +
                    $"then the rest must be a correct construction name. {AvailableConstructions()}");
            }

            // If not, it should be a composed one. 
            // Right now they are all stored in a static class. Try to find it there...
            return Constructions.GetComposedConstruction(constructionName)
                // if it's not found, make the user aware
                ?? throw new ParsingException($"Unknown construction '{constructionName}'. {AvailableConstructions()}");
        }

        /// <summary>
        /// Parses a configuration from configuration lines.
        /// </summary>
        /// <param name="lines">The lines with the configuration's definition.</param>
        /// <returns>The tuple consisting the parsed configuration and the dictionary mapping declared object names to their real objects.</returns>
        public static (Configuration configuration, IReadOnlyDictionary<string, ConfigurationObject> namesToObjects) ParseConfiguration(IReadOnlyList<string> lines)
        {
            // Make sure there is at least one line
            if (lines.Count == 0)
                throw new ParsingException("The configuration has no definition.");

            // Prepare a dictionary between names and parsed objects
            var namesToObjects = new Dictionary<string, ConfigurationObject>();

            #region Parse loose objects

            // The loose objects should be specified in the first line
            var looseObjectsMatch = Regex.Match(lines[0], "^(.+):(.+)$");

            // Make sure there is a match
            if (!looseObjectsMatch.Success)
                throw new ParsingException("The first line of the configuration specification should be in the form 'Layout: objects', where objects are separated by commas.");

            // Get the layout string
            var layoutString = looseObjectsMatch.Groups[1].Value.Trim();

            // Try to parse the layout
            if (!Enum.TryParse(looseObjectsMatch.Groups[1].Value, out LooseObjectsLayout layout))
                throw new ParsingException($"Cannot parse the loose objects layout '{looseObjectsMatch.Groups[1].Value}'");

            // Get the loose objects names, separated by commas
            var looseObjectsNames = looseObjectsMatch.Groups[2].Value.Split(",").Select(s => s.Trim()).ToList();

            // Find the needed types of loose objects for the layout
            var neededObjectTypes = layout.ObjectTypes();

            // Make sure the number of found objects matches the number of needed ones
            if (neededObjectTypes.Count != looseObjectsNames.Count)
                throw new ParsingException("The number of parsed loose objects doesn't match the layout.");

            // Create real loose objects from names
            var looseObjects = looseObjectsNames.Select((name, index) =>
            {
                // Make sure the name hasn't been used
                if (namesToObjects.ContainsKey(name))
                    throw new ParsingException($"Error while parsing loose objects. The object with the name '{name}' has been already at least twice.");

                // Make sure the name is correct
                if (!name.All(char.IsLetterOrDigit))
                    throw new ParsingException($"Error while parsing loose objects. Name of an object can contain only letters and digits, this one is '{name}'");

                // Create a loose object of the needed type
                var looseObject = new LooseConfigurationObject(neededObjectTypes[index]);

                // Associate it with the name
                namesToObjects.Add(name, looseObject);

                // Return it
                return looseObject;
            })
            // Enumerate
            .ToList();

            // Create the holder of loose objects
            var holder = new LooseObjectsHolder(looseObjects, layout);

            #endregion

            #region Parse constructed objects

            // Skip the first line and parse the rest
            var constructedObjects = lines.Skip(1).Select(line =>
            {
                // Let us match the name and the definition first
                var nameDefinitionMatch = Regex.Match(line, "^(.+)=(.+)$");

                // Make sure there's a match...
                if (!nameDefinitionMatch.Success)
                    throw new ParsingException($"Error while parsing '{line}'. The line should be in the form 'name = definition'.");

                // Get the name
                var newObjectName = nameDefinitionMatch.Groups[1].Value.Trim();

                // Make sure the name is correct
                if (!newObjectName.All(char.IsLetterOrDigit))
                    throw new ParsingException($"Error while parsing '{line}'. Name of an object can contain only letters and digits, this one is '{newObjectName}'");

                // Make sure the name hasn't been used
                if (namesToObjects.ContainsKey(newObjectName))
                    throw new ParsingException($"Error while parsing '{line}'. The object with the name '{newObjectName}' has been already declared at least twice.");

                // Get the object string
                var objectString = nameDefinitionMatch.Groups[2].Value.Trim();

                try
                {
                    // Try to create a constructed object
                    var constructedObject = ParseConstructedObject(objectString, namesToObjects);

                    // Associate it
                    namesToObjects.Add(newObjectName, constructedObject);

                    // Return it
                    return constructedObject;
                }
                catch (ParsingException e)
                {
                    // Make sure the user is aware of the problem
                    throw new ParsingException($"Error while parsing '{line}'. Couldn't parse the object. {e.Message}");
                }
            })
            // Enumerate
            .ToList();

            #endregion

            // Finally create the result
            return (new Configuration(holder, constructedObjects), namesToObjects);
        }

        /// <summary>
        /// Parses the constructed object represented by a given string. It's inner objects have to be named.
        /// </summary>
        /// <param name="objectString">The string to be parsed.</param>
        /// <param name="namesToObjects">The dictionary mapping declared object names to their real objects.</param>
        /// <returns>The parsed constructed object.</returns>
        public static ConstructedConfigurationObject ParseConstructedObject(string objectString, IReadOnlyDictionary<string, ConfigurationObject> namesToObjects)
        {
            // Match the construction and input objects from the definition
            var definitionMatch = Regex.Match(objectString, "^(.+)\\((.*)\\)$");

            // Make sure there's a match...
            if (!definitionMatch.Success)
                throw new ParsingException($"Error while parsing '{objectString}'. The definition should be in the form 'ConstructioName(objects)'.");

            // Prepare the construction
            var construction = default(Construction);

            try
            {
                // Try to get it 
                construction = ParseConstruction(definitionMatch.Groups[1].Value);
            }
            catch (ParsingException e)
            {
                // Re-throw the exception with the line info
                throw new ParsingException($"Error while parsing '{objectString}'. {e.Message}");
            }

            // Get the passed objects
            var passedObjects = definitionMatch.Groups[2].Value.Split(",").Select(s => s.Trim()).Where(s => !s.IsNullOrEmpty()).Select(name =>
            {
                // Make sure the name has been declared before
                if (!namesToObjects.ContainsKey(name))
                    throw new ParsingException($"Error while parsing '{objectString}'. Undeclared object '{name}'");

                // Return the corresponding object
                return namesToObjects[name];
            })
            // Enumerate
            .ToArray();

            try
            {
                // Try to create a constructed object
                return new ConstructedConfigurationObject(construction, passedObjects);
            }
            catch (GeoGenException e)
            {
                // Make sure the user is aware of the problem
                throw new ParsingException(e.Message);
            }
        }

        /// <summary>
        /// Parses a theorem from a theorem line.
        /// </summary>
        /// <param name="theoremLine">The line with the theorem's description.</param>
        /// <param name="namesToObjects">The dictionary mapping declared object names to their real objects.</param>
        /// <returns>The parsed theorem.</returns>
        public static Theorem ParseTheorem(string theoremLine, IReadOnlyDictionary<string, ConfigurationObject> namesToObjects)
        {
            #region Parsing type

            // Match the type
            var typeDefinitionMatch = Regex.Match(theoremLine, "^(.+):(.+)$");

            // Make sure there is a match...
            if (!typeDefinitionMatch.Success)
                throw new ParsingException($"Error while parsing '{theoremLine}'. The theorem should be in the form 'name:definition'");

            // Parse the type
            if (!Enum.TryParse<TheoremType>(typeDefinitionMatch.Groups[1].Value, out var theoremType))
                throw new ParsingException($"Error while parsing '{theoremLine}'. Unknown theorem type '{typeDefinitionMatch.Groups[1].Value}'");

            #endregion

            #region Parsing objects

            // Get the definition string
            var definition = typeDefinitionMatch.Groups[2].Value.Trim();

            // In order to find theorem objects, we will look for those commas
            // that are not in any unclosed brackets of any type. For simplicity,
            // we will replace them with semicolons and then split the split by them
            // (speed does not matter here very much)

            // Copy the definition so we can edit it
            var definitionCopy = new StringBuilder(definition);

            // Prepare the number of unclosed brackets
            var unclosedBrackets = 0;

            // Go through the characters
            definition.ForEach((c, index) =>
            {
                // If we have an open bracket, count it in
                if (c == '(' || c == '[')
                    unclosedBrackets++;

                // If we have a closed one, count it out
                else if (c == ')' || c == ']')
                    unclosedBrackets--;

                // If we have a comma and there are no unclosed brackets,
                // replace it with a semicolon
                else if (c == ',' && unclosedBrackets == 0)
                    definitionCopy[index] = ';';
            });

            // Split the copied string by created semicolons to get particular objects
            var theoremObjectsStrings = definitionCopy.ToString().Split(';').Select(s => s.Trim());

            // Parse the objects
            var theoremObjects = theoremObjectsStrings.Select(theoremObject => ParseTheoremObject(theoremType, theoremObject, namesToObjects)).ToList();

            #endregion

            try
            {
                // Finally construct the theorem
                return Theorem.DeriveFromFlattenedObjects(theoremType, theoremObjects);
            }
            catch (GeoGenException e)
            {
                // Re-throw possible exceptions
                throw new ParsingException($"Error while parsing '{theoremLine}'. {e.Message}");
            }
        }

        /// <summary>
        /// Parses theorem object from a given string.
        /// </summary>
        /// <param name="theoremType">The type of theorem being parsed.</param>
        /// <param name="objectString">The string containing the object's definition.</param>
        /// <param name="namesToObjects">The dictionary mapping declared object names to their real objects.</param>
        /// <returns>The parsed theorem object.</returns>
        public static TheoremObject ParseTheoremObject(TheoremType theoremType, string objectString, IReadOnlyDictionary<string, ConfigurationObject> namesToObjects)
        {
            // First handle the two special types of theorems
            switch (theoremType)
            {
                // In these types we have two types of objects: 
                // Named ones or explicitly defined ones (via a construction with arguments)
                case TheoremType.Incidence:
                case TheoremType.EqualObjects:

                    // Prepare the parser object
                    var parsedObject = default(ConfigurationObject);

                    // Try to parse it with a name
                    if (namesToObjects.ContainsKey(objectString))
                        parsedObject = namesToObjects[objectString];

                    // Otherwise
                    else
                        try
                        {
                            // Or try to parse it as a configuration object
                            parsedObject = ParseConstructedObject(objectString, namesToObjects);
                        }
                        catch (ParsingException e)
                        {
                            // Re-thrown an exception with the right message
                            throw new ParsingException($"Error while parsing '{objectString}'. Object has to be defined correctly. {e.Message}");
                        }

                    // Switch based on the figured type which constructor we'll use
                    return parsedObject.ObjectType switch
                    {
                        // Point case
                        ConfigurationObjectType.Point => new PointTheoremObject(parsedObject) as TheoremObject,

                        // Line case
                        ConfigurationObjectType.Line => new LineTheoremObject(parsedObject),

                        // Circle case
                        ConfigurationObjectType.Circle => new CircleTheoremObject(parsedObject),

                        // Default case
                        _ => throw new GeoGenException($"Unhandled type of configuration object: {parsedObject.ObjectType}"),
                    };
            }

            // Try to match a line defined implicitly
            var implicitLineMatch = Regex.Match(objectString, "^\\[(.+)\\]$");

            // Try to match a circle defined implicitly
            var implicitCircleMatch = Regex.Match(objectString, "^\\((.+)\\)$");

            // Get the implicit points names. 
            var pointNames = implicitLineMatch.Success ? implicitLineMatch.Groups[1].Value :
                             implicitCircleMatch.Success ? implicitCircleMatch.Groups[1].Value :
                             // They might not be defined at all, if the object is defined explicitly
                             null;

            // Parse the points, if there are any
            var theoremPoints = pointNames?.Split(",").Select(s => s.Trim()).Select(pointName =>
            {
                // Make sure such a point exist s
                if (!namesToObjects.ContainsKey(pointName))
                    throw new ParsingException($"Error while parsing '{objectString}'. Unknown object '{pointName}'.");

                // Get the corresponding object
                var pointObject = namesToObjects[pointName];

                // Make sure it is a point
                if (pointObject.ObjectType != ConfigurationObjectType.Point)
                    throw new ParsingException($"Error while parsing '{objectString}'. Object {pointName} is not a point.");

                // Return it
                return pointObject;
            })
            // Enumerate
            .ToArray();

            // If points are defined, we can return a line or circle
            if (theoremPoints != null)
            {
                try
                {
                    // If line matches
                    if (implicitLineMatch.Success)
                    {
                        // Make sure we have enough points
                        if (theoremPoints.Length != 2)
                            throw new ParsingException("Line must have exactly 2 points.");

                        // If yes, we're there
                        return new LineTheoremObject(theoremPoints[0], theoremPoints[1]);
                    }
                    // Otherwise circle matches 
                    else
                    {
                        // Make sure we have enough points
                        if (theoremPoints.Length != 3)
                            throw new ParsingException("Circle must have exactly 3 points.");

                        // If yes, we're there
                        return new CircleTheoremObject(theoremPoints[0], theoremPoints[1], theoremPoints[2]);
                    }
                }
                catch (GeoGenException e)
                {
                    // Re-throw a possible exception
                    throw new ParsingException($"Error while parsing '{objectString}'. {e.Message}");
                }
            }

            // If we don't have points, then the object is defined implicitly and the string is it's name
            // Make sure the name has been defined
            if (!namesToObjects.ContainsKey(objectString))
                throw new ParsingException($"Error while parsing '{objectString}'. Unknown object '{objectString}'.");

            // Get the object
            var configurationObject = namesToObjects[objectString];

            // Switch based on the type which constructor we'll use
            return configurationObject.ObjectType switch
            {
                // Point case
                ConfigurationObjectType.Point => new PointTheoremObject(configurationObject) as TheoremObject,

                // Line case
                ConfigurationObjectType.Line => new LineTheoremObject(configurationObject),

                // Circle case
                ConfigurationObjectType.Circle => new CircleTheoremObject(configurationObject),

                // Default case
                _ => throw new GeoGenException($"Unhandled type of configuration object: {configurationObject.ObjectType}"),
            };
        }
    }
}