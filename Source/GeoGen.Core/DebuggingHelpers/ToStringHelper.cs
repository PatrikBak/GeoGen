using GeoGen.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.Core
{
    /// <summary>
    /// A helper class that providers methods for converting core objects to human-readable strings.
    /// </summary>
    public static class ToStringHelper
    {
        /// <summary>
        /// Converts a given configuration to a string.
        /// </summary>
        /// <param name="configuration">The configuration to be converted.</param>
        /// <param name="objectsSeparator">The separator of individual objects in the final string.</param>
        /// <param name="displayId">Indicates if we should include the object's id.</param>
        /// <returns>A human-readable string representation of the configuration.</returns>
        public static string ConfigurationToString(Configuration configuration, string objectsSeparator = ", ", bool displayId = true)
        {
            // Get the objects strings
            var objectStrings = ObjectsToString(configuration.ObjectsMap.AllObjects, displayId);

            // And create the final result
            return $"{(configuration.HasId ? $"[{configuration.Id}] " : "")}{string.Join(objectsSeparator, objectStrings)}";
        }

        /// <summary>
        /// Converts given configuration objects to string.
        /// </summary>
        /// <param name="objects">The objects to be converted.</param>
        /// <param name="displayId">Indicates if we should include the object's id.</param>
        /// <returns>The string representations of individual objects.</returns>
        public static IEnumerable<string> ObjectsToString(IEnumerable<ConfigurationObject> objects, bool displayId = true)
        {
            // It's good to name the objects first. 
            // For simplicity we're going to name all the objects 
            // with capital letters starting from 'A'
            var names = objects.ToDictionary((obj, index) => obj, (obj, index) => (char) ('A' + index));

            // Now we cast each object to its string representation
            return objects.Select(obj =>
            {
                // Get the id string
                var idString = displayId ? $"[{ obj.Id}]" : "";

                // Switch according to the type
                switch (obj)
                {
                    case LooseConfigurationObject _:

                        // For loose objects it's simple
                        return $"{names[obj]}={obj.ObjectType}{idString}";

                    case ConstructedConfigurationObject constructedObject:

                        // For a constructed object we first convert its arguments using the found names
                        var arguments = ArgumentsToString(constructedObject.PassedArguments, names);

                        // And construct the final string
                        return $"{names[obj]}={constructedObject.Construction.Name}({string.Join(",", arguments)}){idString}";

                    default:
                        throw new GeoGenException("Unknown object type");
                }
            });
        }

        /// <summary>
        /// Converts a given object to a string.
        /// </summary>
        /// <param name="configurationObject">The object to be converted.</param>
        /// <param name="objectsSeparator">The separator of individual objects in the final string.</param>
        /// <param name="displayId">Indicates if we should include the object's id.</param>
        /// <returns>A human-readable string representation of the object.</returns>
        public static string ObjectToString(ConfigurationObject configurationObject, string objectsSeparator = ", ", bool displayId = true)
        {
            // We're going to find the objects that define this one including itself
            var definingObjects = configurationObject.AsEnumerable().Concat(configurationObject.GetInternalObjects()).Distinct().ToList();

            // Sort them according to their ids (so we know which ones were created first)
            definingObjects.Sort((o1, o2) => o1.Id - o2.Id);

            // Separate the loose and the constructed ones
            var looseObjects = definingObjects.OfType<LooseConfigurationObject>().ToList();
            var constructedObjects = definingObjects.OfType<ConstructedConfigurationObject>().ToList();

            // Create a configuration simulating the definition of the object
            var configuration = new Configuration(new LooseObjectsHolder(looseObjects), constructedObjects);

            // And use the configuration converted
            return ConfigurationToString(configuration, objectsSeparator, displayId);
        }

        /// <summary>
        /// Converts a given construction to a string.
        /// </summary>
        /// <param name="construction">The construction to be converted.</param>
        /// <returns>A human-readable string representation of a construction.</returns>
        public static string ConstructionToString(Construction construction) => $"{construction.Name} ({construction.Signature})";

        /// <summary>
        /// Converts a given signature to a string.
        /// </summary>
        /// <param name="signature">The signature to be converted.</param>
        /// <returns>A human-readable string representation of a signature.</returns>
        public static string SignatureToString(Signature signature)
        {
            // Local function to convert a construction parameter to a string
            string ConvertParameter(ConstructionParameter parameter)
            {
                // If we have an object parameter...
                if (parameter is ObjectConstructionParameter objectParameter)
                {
                    // Then we get the first capital letter of the expected type
                    return objectParameter.ObjectType.ToString()[0].ToString();
                }

                // Otherwise we have a set parameter
                var setParameter = (SetConstructionParameter) parameter;

                // We convert the underlying parameter
                var innerParameterString = ConvertParameter(setParameter.TypeOfParameters);

                // And repeat it the needed number of times
                return $"{{{string.Join(",", Enumerable.Repeat(innerParameterString, setParameter.NumberOfParameters))}}}";
            }

            // Convert individual parameters
            return string.Join(",", signature.Select(ConvertParameter));
        }

        /// <summary>
        /// Converts given arguments to a string, using the provided names map.
        /// </summary>
        /// <param name="arguments">The arguments to be converted.</param>
        /// <param name="names">The mapping of objects to their names.</param>
        /// <returns>A human-readable string representation of arguments.</returns>
        public static string ArgumentsToString(Arguments arguments, Dictionary<ConfigurationObject, char> names)
        {
            // Local function to convert a construction argument to a string
            string ConvertArgument(ConstructionArgument argument)
            {
                // If we have an object argument...
                if (argument is ObjectConstructionArgument objectArgument)
                {
                    // Then we simply get the result from the map
                    return names[objectArgument.PassedObject].ToString();
                }

                // Otherwise we have a set argument
                var setArgument = (SetConstructionArgument) argument;

                // Then we convert its inner arguments
                return $"{{{string.Join(",", setArgument.PassedArguments.Select(ConvertArgument))}}}";
            }

            // Convert individual arguments
            return string.Join(",", arguments.Select(ConvertArgument));
        }
    }
}