using GeoGen.Core;
using GeoGen.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace GeoGen.MainLauncher
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
                // Get all constructions, prepend spaces, sort, and make every on a single line
                $"{Constructions.GetAllConstructions().Select(construction => $"  {construction.Name}").Ordered().ToJoinedString("\n")}.\n";

            // Try to find if it is a predefined one
            if (Enum.TryParse<PredefinedConstructionType>(constructionName, out var type))
                return Constructions.GetPredefinedconstruction(type);

            // If it's not a predefined one, then it should be a composed one. 
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
        public static (Configuration configuration, Dictionary<string, ConfigurationObject> namesToObjects) ParseConfiguration(IReadOnlyList<string> lines)
        {
            // Make sure there is at least one line
            if (lines.Count == 0)
                throw new ParsingException("The configuration has an empty definition.");

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
            if (!Enum.TryParse(layoutString, out LooseObjectLayout layout))
                throw new ParsingException($"Cannot parse the loose objects layout '{layoutString}'");

            // Get the objects string
            var objectsString = looseObjectsMatch.Groups[2].Value.Trim();

            // Get the loose objects names, separated by commas
            var looseObjectsNames = objectsString.Split(",").Select(looseObjectName => looseObjectName.Trim()).ToList();

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
                    throw new ParsingException($"Error while parsing loose objects. The object with the name '{name}' has been declared twice.");

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
            var holder = new LooseObjectHolder(looseObjects, layout);

            #endregion

            #region Parse constructed objects

            // Skip the first line and parse the rest
            var constructedObjects = lines.Skip(1)
                // Each line is parsed without auto-creating of unknown objects (all objects have to be defined)
                .Select(line => ParseObjectWithDefinition(line, namesToObjects, autocreateUnnamedObjects: false))
                // Enumerate
                .ToList();

            #endregion

            // Finally create the result
            return (new Configuration(holder, constructedObjects), namesToObjects);
        }

        /// <summary>
        /// Parses the constructed object represented as 'name = definition'. 
        /// </summary>
        /// <param name="definitionString">The name and the definition of the object of the form 'name = definition'</param>
        /// <param name="namesToObjects">The dictionary mapping declared object names to their real objects.</param>
        /// <param name="autocreateUnnamedObjects">If this value is true, then objects without names will be automatically created as loose ones.</param>
        /// <returns>The parsed constructed object.</returns>
        public static ConstructedConfigurationObject ParseObjectWithDefinition(string definitionString, Dictionary<string, ConfigurationObject> namesToObjects, bool autocreateUnnamedObjects)
        {
            // Let us match the name and the definition first
            var nameDefinitionMatch = Regex.Match(definitionString, "^(.+)=(.+)$");

            // Make sure there's a match...
            if (!nameDefinitionMatch.Success)
                throw new ParsingException($"Error while parsing '{definitionString}'. The line should be in the form 'name = definition'.");

            // Get the name
            var newObjectName = nameDefinitionMatch.Groups[1].Value.Trim();

            // Make sure the name is correct
            if (!newObjectName.All(char.IsLetterOrDigit))
                throw new ParsingException($"Error while parsing '{definitionString}'. Name of an object can contain only letters and digits, this one is '{newObjectName}'");

            // Make sure the name hasn't been used
            if (namesToObjects.ContainsKey(newObjectName))
                throw new ParsingException($"Error while parsing '{definitionString}'. The object with the name '{newObjectName}' has been already declared at least twice.");

            // Get the string defining the object
            var objectString = nameDefinitionMatch.Groups[2].Value.Trim();

            try
            {
                // Try to create a constructed object
                var constructedObject = ParseConstructedObject(objectString, namesToObjects, autocreateUnnamedObjects);

                // Associate it with the name
                namesToObjects.Add(newObjectName, constructedObject);

                // Return it
                return constructedObject;
            }
            catch (ParsingException e)
            {
                // Make sure the user is aware of the problem
                throw new ParsingException($"Error while parsing '{definitionString}'. Couldn't parse the object. {e.Message}");
            }
        }

        /// <summary>
        /// Parses the constructed object represented by a given string. It's inner objects have to be named.
        /// </summary>
        /// <param name="objectString">The string to be parsed.</param>
        /// <param name="namesToObjects">The dictionary mapping declared object names to their real objects.</param>
        /// <param name="autocreateUnnamedObjects">If this value is true, then objects without names will be automatically created as loose ones.</param>
        /// <returns>The parsed constructed object.</returns>
        public static ConstructedConfigurationObject ParseConstructedObject(string objectString, Dictionary<string, ConfigurationObject> namesToObjects, bool autocreateUnnamedObjects)
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
            var passedObjects = definitionMatch.Groups[2].Value
                // That should by split by 
                .Split(",")
                // Trim
                .Select(objectName => objectName.Trim())
                // Parse each
                .Select((name, index) =>
                {
                    // If the object is named, return it directly
                    if (namesToObjects.ContainsKey(name))
                        return namesToObjects[name];

                    // Otherwise if we don't have the auto-create feature, then we have a problem
                    if (!autocreateUnnamedObjects)
                        throw new ParsingException($"Error while parsing '{objectString}'. Undeclared object '{name}'");

                    // Otherwise we create this object as a loose one. We can get its type from the construction
                    var looseObject = new LooseConfigurationObject(construction.Signature.ObjectTypes[index]);

                    // Make sure the name is correct
                    if (!name.All(char.IsLetterOrDigit))
                        throw new ParsingException($"Error while parsing '{objectString}'. Name of an object can contain only letters and digits, this one is '{name}'");

                    // Name it
                    namesToObjects.Add(name, looseObject);

                    // Return it
                    return looseObject;
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
        /// Parses a theorem from a theorem line in form type: objects
        /// </summary>
        /// <param name="theoremLine">The line with the theorem's description.</param>
        /// <param name="namesToObjects">The dictionary mapping declared object names to their real objects.</param>
        /// <param name="autocreateUnnamedObjects">If this value is true, then objects without names will be automatically created as loose ones.</param>
        /// <returns>The parsed theorem.</returns>
        public static Theorem ParseTheorem(string theoremLine, Dictionary<string, ConfigurationObject> namesToObjects, bool autocreateUnnamedObjects)
        {
            #region Parsing type

            // Match the type
            var typeDefinitionMatch = Regex.Match(theoremLine, "^(.+):(.+)$");

            // Make sure there is a match...
            if (!typeDefinitionMatch.Success)
                throw new ParsingException($"Error while parsing '{theoremLine}'. The theorem should be in the form 'type: objects'");

            // Get the theorem type string
            var typeString = typeDefinitionMatch.Groups[1].Value.Trim();

            // Parse the type
            if (!Enum.TryParse<TheoremType>(typeString, out var theoremType))
                throw new ParsingException($"Error while parsing '{theoremLine}'. Unknown theorem type '{typeString}'");

            #endregion

            #region Parsing objects

            // Get the definition string
            var definition = typeDefinitionMatch.Groups[2].Value.Trim();

            // In order to find theorem objects, we will look for those commas
            // that are not in any unclosed brackets of any type. For simplicity,
            // we will replace them with semicolons 
            var theoremObjects = definition.ReplaceBalancedCommasWithSemicolons()
                // Now we can split by semicolons
                .Split(';')
                // Trim
                .Select(theoremObjectString => theoremObjectString.Trim())
                // Parse each
                .Select(theoremObjectString =>
                {
                    // Prepare the hint for the object type based on the theorem type
                    var typeHint = theoremType switch
                    {
                        // Points (EqualLineSegments are flattened)
                        TheoremType.CollinearPoints => typeof(PointTheoremObject),
                        TheoremType.ConcyclicPoints => typeof(PointTheoremObject),
                        TheoremType.EqualLineSegments => typeof(PointTheoremObject),

                        // Lines
                        TheoremType.ConcurrentLines => typeof(LineTheoremObject),
                        TheoremType.PerpendicularLines => typeof(LineTheoremObject),
                        TheoremType.ParallelLines => typeof(LineTheoremObject),

                        // Circles
                        TheoremType.TangentCircles => typeof(CircleTheoremObject),

                        // In other cases we can't do it 
                        _ => null
                    };

                    // Parse it
                    return ParseTheoremObject(theoremObjectString, namesToObjects, autocreateUnnamedObjects, typeHint);
                })
                // Enumerate
                .ToList();

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
        /// <param name="objectString">The string containing the object's definition.</param>
        /// <param name="namesToObjects">The dictionary mapping declared object names to their real objects.</param>
        /// <param name="autocreateUnnamedObjects">If this value is true, then objects without names will be automatically created as loose ones.</param>
        /// <param name="typeHint">Represents a hint for the type of the object. This value work only with automatic object creation.</param>
        /// <returns>The parsed theorem object.</returns>
        public static TheoremObject ParseTheoremObject(string objectString, Dictionary<string, ConfigurationObject> namesToObjects, bool autocreateUnnamedObjects, Type typeHint = null)
        {
            // Local function that converts an explicit configuration object to a theorem object
            static TheoremObject Convert(ConfigurationObject configurationObject) => configurationObject.ObjectType switch
            {
                // Point case
                ConfigurationObjectType.Point => new PointTheoremObject(configurationObject) as TheoremObject,

                // Line case
                ConfigurationObjectType.Line => new LineTheoremObject(configurationObject),

                // Circle case
                ConfigurationObjectType.Circle => new CircleTheoremObject(configurationObject),

                // Unhandled cases
                _ => throw new ParsingException($"Unhandled value of {nameof(ConfigurationObjectType)}: {configurationObject.ObjectType}"),
            };

            // If the object is named, then parse it as an explicit object
            if (namesToObjects.ContainsKey(objectString))
                return Convert(namesToObjects[objectString]);

            #region Parsing implicit object

            // Otherwise we have to dig deeper. First we assume it is defined implicitly
            // Try to match a line defined implicitly
            var implicitLineMatch = Regex.Match(objectString, "^\\[(.+)\\]$");

            // Try to match a circle defined implicitly
            var implicitCircleMatch = Regex.Match(objectString, "^\\((.+)\\)$");

            // Get the implicit points names. 
            var pointNames = implicitLineMatch.Success ? implicitLineMatch.Groups[1].Value :
                             implicitCircleMatch.Success ? implicitCircleMatch.Groups[1].Value :
                             // They might not be defined at all, if the object is defined explicitly
                             null;

            // Parse the points, if there are any. They are separated by commas, but there might 
            // be inner ones...Thus we will replace balanced commas with semicolons
            var theoremPoints = pointNames?.ReplaceBalancedCommasWithSemicolons()
                // Now we can split by semicolons
                .Split(";")
                // Trim
                .Select(pointDefinition => pointDefinition.Trim())
                // Parse each point
                .Select(pointDefinition =>
                {
                    // Prepare the point
                    var pointObject = default(ConfigurationObject);

                    // If the point is named, then we simply get it from the names dictionary
                    if (namesToObjects.ContainsKey(pointDefinition))
                        pointObject = namesToObjects[pointDefinition];

                    // Otherwise...
                    else
                        try
                        {
                            // We might try to parse the point as a constructed one
                            pointObject = ParseConstructedObject(pointDefinition, namesToObjects, autocreateUnnamedObjects);
                        }
                        catch (ParsingException)
                        {
                            // Otherwise, if the auto-create feature is not on, we have a problem
                            if (!autocreateUnnamedObjects)
                                throw new ParsingException($"Error while parsing '{objectString}'. Unknown point '{pointDefinition}'.");

                            // Otherwise we assume this is the name for a loose point
                            pointObject = new LooseConfigurationObject(ConfigurationObjectType.Point);

                            // Make sure the name is correct
                            if (!pointDefinition.All(char.IsLetterOrDigit))
                                throw new ParsingException($"Error while parsing '{objectString}'. Name of an object can contain only letters and digits, this one is '{pointDefinition}'");

                            // And name it
                            namesToObjects.Add(pointDefinition, pointObject);
                        }

                    // Make sure we have a point
                    if (pointObject.ObjectType != ConfigurationObjectType.Point)
                        throw new ParsingException($"Error while parsing '{objectString}'. Object {pointDefinition} is not a point.");

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
                        // Make sure we have the right number of points
                        if (theoremPoints.Length != 2)
                            throw new ParsingException("Line must have exactly 2 points.");

                        // If yes, we're there
                        return new LineTheoremObject(theoremPoints[0], theoremPoints[1]);
                    }
                    // Otherwise circle matches 
                    else
                    {
                        // Make sure we have the right number of points
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

            #endregion

            #region Trying to parse the object explicitly

            // If we got here, then we have an unnamed explicitly stated object
            try
            {
                // Try to parse it as a constructed object
                var parsedObject = ParseConstructedObject(objectString, namesToObjects, autocreateUnnamedObjects);

                // Convert it to a theorem object
                return Convert(parsedObject);
            }
            catch (ParsingException)
            {
                // If it cannot be then, then we're going to assume it's an unnamed object wrapping a loose object
                LooseConfigurationObject looseObject;

                // If there is a hint to its type, we can simply infer the right choice from there
                if (typeHint != null)
                {
                    // Point case
                    if (typeHint.Equals(typeof(PointTheoremObject)))
                        looseObject = new LooseConfigurationObject(ConfigurationObjectType.Point);

                    // Line case
                    else if (typeHint.Equals(typeof(LineTheoremObject)))
                        looseObject = new LooseConfigurationObject(ConfigurationObjectType.Line);

                    // Circle case
                    else if (typeHint.Equals(typeof(CircleTheoremObject)))
                        looseObject = new LooseConfigurationObject(ConfigurationObjectType.Circle);

                    // Unhandled cases
                    else
                        throw new ParsingException($"Unhandled or incorrect type hint '{typeHint}'");

                    // Name it
                    namesToObjects.Add(objectString, looseObject);
                }
                // If there is no type hint...
                else
                {
                    // We still have a chance to parse it. There are some theorem types where
                    // it is unclear what particular objects mean, for example Incidence: x, y. 
                    // But, in those cases we expect to receive a string {type}({name})
                    // Let's try to match it
                    var match = Regex.Match(objectString, "^(.*)\\((.*)\\)$");

                    // If there is no match, we've failed
                    if (!match.Success)
                        throw new ParsingException($"Error while parsing {objectString}. It cannot be automatically inferred.");

                    // Otherwise we get the type string
                    var typeString = match.Groups[1].Value.Trim();

                    // We need to parse this type
                    if (!Enum.TryParse<ConfigurationObjectType>(typeString, out var type))
                        throw new ParsingException($"Error while parsing {objectString}. Failed while trying to parse the type '{typeString}'");

                    // Get the name
                    var name = match.Groups[2].Value.Trim();

                    // If the object is already there...
                    if (namesToObjects.ContainsKey(name))
                    {
                        // Get it
                        var namedObject = namesToObjects[name];

                        // If it's not loose, we have a problem
                        if (!(namedObject is LooseConfigurationObject looseNamedObject))
                            throw new ParsingException($"Object with a name '{name}' has been declared twice inconsistently.");

                        // If we got here, the object is okay...
                        looseObject = looseNamedObject;
                    }
                    // If the object doesn't have a name
                    else
                    {
                        // Create a new one of the parsed type
                        looseObject = new LooseConfigurationObject(type);

                        // Name it
                        namesToObjects.Add(name, looseObject);
                    }
                }

                // Finally convert the loose object to a theorem object
                return Convert(looseObject);
            }
        }

        #endregion
    }
}