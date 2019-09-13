using GeoGen.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.Core
{
    /// <summary>
    /// Represents a geometric object that is be used to express <see cref="Theorem"/>s.
    /// </summary>
    public abstract class TheoremObject
    {
        #region Public abstract methods

        /// <summary>
        /// Recreates the theorem object by applying a given mapping of the inner configuration objects.
        /// Every <see cref="ConfigurationObject"/> internally contained in this theorem object must be
        /// present in the mapping. If the mapping cannot be done (for example because 2 points
        /// making a line are mapped to the same point), then null is returned.
        /// </summary>
        /// <param name="mapping">The dictionary representing the mapping.</param>
        /// <returns>The remapped theorem object, or null, if the mapping cannot be done.</returns>
        public abstract TheoremObject Remap(Dictionary<ConfigurationObject, ConfigurationObject> mapping);

        #endregion

        #region Protected helper methods

        /// <summary>
        /// Tries to find the object corresponding to a given one with respect to a given mapping.
        /// If the object is not present in the mapping, it assumed the passed object is 
        /// a constructed one and tries to construct it by remapping its arguments. If it fails,
        /// a <see cref="GeoGenException"/> is thrown. (This is a helper method used by implementations
        /// of <see cref="Remap(Dictionary{ConfigurationObject, ConfigurationObject})"/>).
        /// </summary>
        /// <param name="configurationObject">The configuration object to be mapped.</param>
        /// <param name="mapping">The dictionary representing the mapping.</param>
        /// <returns>The remapped configuration object, or null, if mapping would yield a new incorrect constructed object.</returns>
        protected static ConfigurationObject Map(ConfigurationObject configurationObject, Dictionary<ConfigurationObject, ConfigurationObject> mapping)
        {
            // If the object is directly present in the map, return it
            if (mapping.ContainsKey(configurationObject))
                return mapping[configurationObject];

            // Otherwise we assume the passed object is a constructed one
            var constructedObject = configurationObject as ConstructedConfigurationObject
                // If not, make aware
                ?? throw new GeoGenException("Cannot do the mapping, because the passed object is not in the map, nor it's constructed.");

            // Convert the individual passed objects
            var mappedObjects = constructedObject.PassedArguments.FlattenedList
                // Look for the inner objects in the mapping
                .Select(innerObject => mapping.GetOrDefault(innerObject)
                    // They must be there
                    ?? throw new GeoGenException("Cannot do the mapping, because not all the arguments of the passed object are in the map."))
                // Enumerate
                .ToArray();

            // If the objects are not same, the mapping cannot be done
            if (mappedObjects.Distinct().Count() != mappedObjects.Length)
                return null;

            // Otherwise construct the remapped object
            return new ConstructedConfigurationObject(constructedObject.Construction, mappedObjects);
        }

        #endregion
    }
}