using GeoGen.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.Core
{
    /// <summary>
    /// Represents a theorem object that consists of two unordered theorem objects of some type.
    /// </summary>
    public abstract class PairTheoremObject : TheoremObject
    {
        #region Public properties

        /// <summary>
        /// Gets the first object of this pair.
        /// </summary>
        public TheoremObject Object1 { get; }

        /// <summary>
        /// Gets the second object of this pair.
        /// </summary>
        public TheoremObject Object2 { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="PairTheoremObject"/> class.
        /// </summary>
        /// <param name="object1">The first object of the pair.</param>
        /// <param name="object2">The second object of the pair.</param>
        protected PairTheoremObject(TheoremObject object1, TheoremObject object2)
        {
            Object1 = object1 ?? throw new ArgumentNullException(nameof(object1));
            Object2 = object2 ?? throw new ArgumentNullException(nameof(object2));
        }

        #endregion

        #region Public abstract methods implementation

        /// <summary>
        /// Enumerates every possible set of objects that are altogether needed to define this object (this includes even 
        /// defining objects of objects, see <see cref="ConfigurationObjectsExtentions.GetDefiningObjects(ConfigurationObject)"/>.
        /// For example: If we have a line 'l' with points A, B, C on it, then this line has 4 possible definitions: 
        /// l, [A, B], [A, C], [B, C]. 
        /// </summary>
        /// <returns>The enumerable of objects representing a definition.</returns>
        public override IEnumerable<IEnumerable<ConfigurationObject>> GetAllDefinitions()
        {
            // Combine definitions of particular objects
            return new[] { Object1.GetAllDefinitions(), Object2.GetAllDefinitions() }.Combine()
                 // Take definitions of these objects
                 .Select(definition => definition.Flatten().Distinct());
        }

        /// <summary>
        /// Determines if a given theorem object is equivalent to this one,
        /// i.e. if they represent the same object of a configuration.
        /// </summary>
        /// <param name="otherObject">The theorem object.</param>
        /// <returns>true if they are equivalent; false otherwise.</returns>
        public override bool IsEquivalentTo(TheoremObject otherObject)
        {
            // Either the instances are the same
            return this == otherObject ||
                // Or the passed object is of this type
                otherObject is PairTheoremObject otherPairObject && otherPairObject.GetType() == GetType() &&
                // And either the first and second objects are equivalent
                ((Object1.IsEquivalentTo(otherPairObject.Object1) && Object2.IsEquivalentTo(otherPairObject.Object2)) ||
                // Or the first one is equivalent to the second and vice versa
                (Object1.IsEquivalentTo(otherPairObject.Object2) && Object2.IsEquivalentTo(otherPairObject.Object1)));
        }

        #endregion

        #region Protected helper methods

        /// <summary>
        /// Remaps the inner objects with respect to the provided mapping. If either
        /// of the objects cannot be mapped, then returns the default value.
        /// </summary>
        /// <param name="mapping">The dictionary representing the mapping.</param>
        /// <returns>The remapped objects, if mapping can be done; (null, null) otherwise.</returns>
        protected (TheoremObject, TheoremObject) RemapObjects(Dictionary<ConfigurationObject, ConfigurationObject> mapping)
        {
            // Map particular objects
            var o1 = Object1.Remap(mapping);
            var o2 = Object2.Remap(mapping);

            // Return tuple only if none of them is null
            return o1 != null && o2 != null ? (o1, o2) : default;
        }

        #endregion
    }
}
