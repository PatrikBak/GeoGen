using GeoGen.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.Core
{
    /// <summary>
    /// Extension methods for <see cref="ConfigurationObject"/>s.
    /// </summary>
    public static class ConfigurationObjectEnumerableExtensions
    {
        /// <summary>
        /// Finds the objects that define these objects, in an order in which they can be constructed, including these ones.
        /// </summary>
        /// <returns>The collection of the defining objects.</returns>
        public static IReadOnlyList<ConfigurationObject> GetDefiningObjects(this IEnumerable<ConfigurationObject> objects)
        {
            #region Building graph

            // Prepare a set of objects that are already examined
            var examinedObjects = new HashSet<ConfigurationObject>();

            // Prepare the dictionary mapping each object to the set of the ones that are used directly in its construction
            var objectToWhatUses = new Dictionary<ConfigurationObject, HashSet<ConfigurationObject>>();

            // Prepare a function that recursively find uses of an object + uses of its internal objects
            void FindUsesAllDefiningObjects(ConfigurationObject configurationObject)
            {
                // If the object has been examined, do nothing
                if (examinedObjects.Contains(configurationObject))
                    return;

                // Get the set holding the objects directly used in the definition based on the object type
                var usedObjects = configurationObject switch
                {
                    // If we have a constructed object, then we look at its flattened arguments
                    ConstructedConfigurationObject constructedObject => constructedObject.PassedArguments.FlattenedList.ToSet(),

                    // If we have a loose object, we have no internal objects
                    LooseConfigurationObject _ => new HashSet<ConfigurationObject>(),

                    // Default case
                    _ => throw new GeoGenException($"Unhandled type of configuration object: {configurationObject.GetType()}")
                };

                // Mark found object
                objectToWhatUses.Add(configurationObject, usedObjects);

                // Mark that it's been examined
                examinedObjects.Add(configurationObject);

                // Recursively find uses for the internal objects
                usedObjects.ForEach(FindUsesAllDefiningObjects);
            }

            // Find uses for all the objects
            objects.ForEach(FindUsesAllDefiningObjects);

            #endregion

            #region DFS search

            // Prepare a list holding the final result, initializes with the loose objects,
            // that will definitely be the first ones that define everything
            var result = new List<ConfigurationObject>(objectToWhatUses.Keys.OfType<LooseConfigurationObject>());

            // Prepare a set of already visited objects, initializes with the loose objects
            // already added in the current result
            var visitedObjects = new HashSet<ConfigurationObject>(result);

            // Local function that performs a visit of an object
            // It will recursively visit its dependent objects and after 
            // that add itself to the result. Clearly the objects that 
            // depend on some other one will be added to the result later
            void Visit(ConfigurationObject configurationObject)
            {
                // If the object has been visited, do nothing
                if (visitedObjects.Contains(configurationObject))
                    return;

                // Recursively visit all the dependent objects
                objectToWhatUses[configurationObject].ForEach(Visit);

                // Add the object to the resulting list
                result.Add(configurationObject);

                // Mark the object as visited
                visitedObjects.Add(configurationObject);
            }

            // Visit all the objects
            objectToWhatUses.Keys.ForEach(Visit);

            #endregion

            // Now the result is prepared
            return result;
        }

        /// <summary>
        /// Finds the objects that define this object, in an order in which they can be constructed, including this one.
        /// </summary>
        /// <returns>The collection of the defining objects.</returns>
        public static IReadOnlyList<ConfigurationObject> GetDefiningObjects(this ConfigurationObject configurationObject) => configurationObject.ToEnumerable().GetDefiningObjects();
    }
}