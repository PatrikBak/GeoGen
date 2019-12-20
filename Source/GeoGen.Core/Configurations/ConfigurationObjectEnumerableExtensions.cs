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
        /// <param name="objects">The enumerable of objects.</param>
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
        /// <param name="configurationObject">The configuration object.</param>
        /// <returns>The collection of the defining objects.</returns>
        public static IReadOnlyList<ConfigurationObject> GetDefiningObjects(this ConfigurationObject configurationObject) => configurationObject.ToEnumerable().GetDefiningObjects();

        /// <summary>
        /// Finds out if the objects are ordered in such a way that there are no two objects A, B in this order
        /// such that A needs B in its construction. It automatically assumes that objects that are in arguments of
        /// these objects are constructible.
        /// </summary>
        /// <param name="objects">The list of objects.</param>
        /// <returns>true, if the order of the objects is correct; false otherwise.</returns>
        public static bool RepresentsConstructibleOrder(this IReadOnlyList<ConstructedConfigurationObject> objects)
        {
            // We're going through all the objects of the permutation
            for (var i = 0; i < objects.Count; i++)
            {
                // This object is at a correct place if and only if its passed objects
                var isObjectCorrect = objects[i].PassedArguments.FlattenedList
                        // Are either not among the permuted objects
                        .All(definingObject => !objects.Contains(definingObject)
                            // Or are among them, but not after the current one
                            || objects.ItemsBetween(0, i + 1).Contains(definingObject));

                // If the object is not correct, then the whole order is incorrect
                if (!isObjectCorrect)
                    return false;
            }

            // If we got here, then the order is correct
            return true;
        }
    }
}