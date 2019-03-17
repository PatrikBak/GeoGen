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
        /// <returns>A human-readable string representation of the configuration.</returns>
        public static string ConfigurationToString(Configuration configuration)
        {
            // Get the objects strings
            var objectStrings = ObjectsToString(configuration.ObjectsMap.AllObjects);

            // Join them to get the final result
            return string.Join(", ", objectStrings);
        }

        /// <summary>
        /// Converts given configuration objects to string.
        /// </summary>
        /// <param name="objects">The objects to be converted.</param>
        /// <returns>The string representations of individual objects.</returns>
        public static IEnumerable<string> ObjectsToString(IEnumerable<ConfigurationObject> objects)
        {
            // It's good to name the objects first. 
            // For simplicity we're going to name all the objects 
            // with capital letters starting from an 'A'
            var names = objects.ToDictionary((obj, index) => obj, (obj, index) => (char) ('A' + index));

            // Now we cast each object to its string representation
            return objects.Select(obj =>
            {
                // Switch according to the type
                switch (obj)
                {
                    case LooseConfigurationObject _:

                        // For loose objects we include just the name and the type
                        return $"{names[obj]}={obj.ObjectType}";

                    case ConstructedConfigurationObject constructedObject:

                        // For a constructed object we first convert its arguments using the found names
                        var arguments = ArgumentsToString(constructedObject.PassedArguments, names);

                        // And construct the final string with the name of the object and construction + arguments
                        return $"{names[obj]}={constructedObject.Construction.Name}({string.Join(",", arguments)})";

                    default:
                        throw new GeoGenException("Unhandled object type");
                }
            });
        }

        /// <summary>
        /// Converts a given object to a string.
        /// </summary>
        /// <param name="configurationObject">The object to be converted.</param>
        /// <returns>A human-readable string representation of the object.</returns>
        public static string ObjectToString(ConfigurationObject configurationObject)
        {
            // Find the list of objects that define this one
            var definingObjects = configurationObject.GetDefiningObjects();

            // Separate the loose and the constructed ones
            var looseObjects = definingObjects.OfType<LooseConfigurationObject>().ToList();
            var constructedObjects = definingObjects.OfType<ConstructedConfigurationObject>().ToList();

            // Create a configuration simulating a definition of the object
            var configuration = new Configuration(new LooseObjectsHolder(looseObjects), constructedObjects);

            // And use the configuration converter
            return ConfigurationToString(configuration);
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
                return $"{{{string.Join(", ", Enumerable.Repeat(innerParameterString, setParameter.NumberOfParameters))}}}";
            }

            // Convert individual parameters
            return string.Join(", ", signature.Select(ConvertParameter));
        }

        /// <summary>
        /// Converts given arguments to a string, using the provided object names map.
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