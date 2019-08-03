using GeoGen.Utilities;
using System.Collections.Generic;

namespace GeoGen.Core
{
    /// <summary>
    /// Represents a geometric object that is be used to express <see cref="Theorem"/>s.
    /// </summary>
    public abstract class TheoremObject
    {
        #region Public static properties

        /// <summary>
        /// Gets the single instance of the equality comparer of two theorem objects that uses the 
        /// <see cref="IsEquivalentTo(TheoremObject)"/> method and a constant hash code function 
        /// (i.e. using it together with a dictionary / hash set would make all the operations O(n)).
        /// </summary>
        public static readonly IEqualityComparer<TheoremObject> EquivalencyComparer = new SimpleEqualityComparer<TheoremObject>((t1, t2) => t1.IsEquivalentTo(t2), t => 0);

        #endregion

        #region Public abstract properties

        /// <summary>
        /// Enumerates every possible set of objects that are altogether needed to define this object (this includes even 
        /// defining objects of objects, see <see cref="ConfigurationObjectsExtentions.GetDefiningObjects(ConfigurationObject)"/>.
        /// For example: If we have a line 'l' with points A, B, C on it, then this line has 4 possible definitions: 
        /// l, [A, B], [A, C], [B, C]. 
        /// </summary>
        /// <returns>The enumerable of objects representing a definition.</returns>
        public abstract IEnumerable<IEnumerable<ConfigurationObject>> GetAllDefinitions();

        /// <summary>
        /// Determines if a given theorem object is equivalent to this one,
        /// i.e. if they represent the same object of a configuration.
        /// </summary>
        /// <param name="otherObject">The theorem object.</param>
        /// <returns>true if they are equivalent; false otherwise.</returns>
        public abstract bool IsEquivalentTo(TheoremObject otherObject);

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
        protected ConfigurationObject Map(ConfigurationObject configurationObject, Dictionary<ConfigurationObject, ConfigurationObject> mapping)
        {
            // Try to get it from the mapping    
            return mapping.GetOrDefault(configurationObject)
                // If it can't be done, make the developer aware
                ?? throw new GeoGenException("Cannot create a remapped theorem, because the passed mapping doesn't contain its object.");
        }

        #endregion
    }
}