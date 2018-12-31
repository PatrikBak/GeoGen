using GeoGen.Utilities;
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
        public static string ConfigurationToString(Configuration configuration, string objectsSeparator = ", ")
        {
            // It's good to name the objects first. 
            // For simplicity we're going to name all the objects 
            // with capital letters starting from 'A'
            var names = configuration.ObjectsMap.AllObjects.ToDictionary((obj, index) => obj, (obj, index) => (char) ('A' + index));

            // Now we cast each object to its string representation
            var objectStrings = configuration.ObjectsMap.AllObjects.Select(obj =>
            {
                // Switch according to the type
                switch (obj)
                {
                    case LooseConfigurationObject _:

                        // For loose objects it's simple
                        return $"{names[obj]}={obj.ObjectType}[{obj.Id}]";

                    case ConstructedConfigurationObject constructedObject:

                        // For a constructed object we first convert its flattened arguments using the found names
                        var arguments = constructedObject.PassedArguments.FlattenedList.Select(passedObject => names[passedObject]);

                        // And construct the final string
                        return $"{names[obj]}={constructedObject.Construction.Name}({string.Join(",", arguments)})[{obj.Id}]";

                    default:
                        throw new GeoGenException("Unknown object type");
                }
            });

            // And create the final result
            return $"{(configuration.HasId ? $"[{configuration.Id}] " : "")}{string.Join(objectsSeparator, objectStrings)}";
        }

        /// <summary>
        /// Converts a given object to a string.
        /// </summary>
        /// <param name="configurationObject">The object to be converted.</param>
        /// <returns>A human-readable string representation of the object.</returns>
        public static string ObjectToString(ConfigurationObject configurationObject, string internalObjectsSeparator = ", ")
        {
            // We're going to find the objects that define this one including itself
            var definingObjects = configurationObject.AsEnumerable().Concat(configurationObject.GetInternalObjects()).Distinct().ToList();

            // Sort them according to their ids (so we know which ones were created first)
            definingObjects.Sort((o1, o2) => o1.Id - o2.Id);

            // Separate the loose and the constructed ones
            var looseObjects = definingObjects.OfType<LooseConfigurationObject>().ToList();
            var constructedObjects = definingObjects.OfType<ConstructedConfigurationObject>().ToList();

            // Create a configuration simulating the definition of the object
            var configuration = new Configuration(looseObjects, constructedObjects);

            // And use the configuration converted
            return ConfigurationToString(configuration, internalObjectsSeparator);
        }

        /// <summary>
        /// Converts a given construction to a string.
        /// </summary>
        /// <param name="construction">The construction to be converted.</param>
        /// <returns>A human-readable string representation of a construction.</returns>
        public static string ConstructionToString(Construction construction)
        {
            // When it's needed, we'll do something better
            return construction.Name;
        }
    }
}
