using GeoGen.Utilities;
using System.Collections.Generic;

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
        /// If the object is not present in the mapping, throws a <see cref="GeoGenException"/>.
        /// </summary>
        /// <param name="configurationObject">The configuration object to be mapped.</param>
        /// <param name="mapping">The dictionary representing the mapping.</param>
        /// <returns>The remapped configuration object.</returns>
        protected static ConfigurationObject Map(ConfigurationObject configurationObject, Dictionary<ConfigurationObject, ConfigurationObject> mapping)
        {
            // Try to get it from the mapping    
            return mapping.GetOrDefault(configurationObject)
                // If it can't be done, make the developer aware
                ?? throw new GeoGenException("Cannot create a remapped theorem, because the passed mapping doesn't contain its object.");
        }

        #endregion
    }
}