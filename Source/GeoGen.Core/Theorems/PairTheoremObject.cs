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

        #region To String

        /// <summary>
        /// Converts the pair theorem object to a string. 
        /// NOTE: This method is used only for debugging purposes.
        /// </summary>
        /// <returns>A human-readable string representation of the configuration.</returns>
        public override string ToString() => new[] { Object1, Object2 }.Select(o => o.ToString()).Ordered().ToJoinedString();

        #endregion
    }
}