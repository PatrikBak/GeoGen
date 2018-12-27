using GeoGen.Utilities;
using System.Collections.Generic;

namespace GeoGen.Core
{
    /// <summary>
    /// Represents an equality comparer of <see cref="TheoremObject"/>s. It assumes the theorems
    /// have the same type and then compares their theorem objects as sets, using
    /// our private TheoremObject comparer.
    /// </summary>
    public class TheoremObjectEqualityComparer : IEqualityComparer<TheoremObject>
    {
        #region Instance

        /// <summary>
        /// The singleton instance of this comparer.
        /// </summary>
        public static readonly TheoremObjectEqualityComparer Instance = new TheoremObjectEqualityComparer();

        #endregion

        #region IEqualityComparer implementation

        /// <summary>
        /// Finds out if two given theorem objects are equal.
        /// </summary>
        /// <param name="x">The first theorem object.</param>
        /// <param name="y">The second theorem object.</param>
        /// <returns>true, if they are equal; false otherwise.</returns>
        public bool Equals(TheoremObject x, TheoremObject y)
        {
            // If we have distinct then the objects are distinct
            if (x.Type != y.Type)
                return false;

            // We convert both objects to these sets and compare them as sets
            return x.InternalObjects.ToSet().SetEquals(y.InternalObjects.ToSet());
        }

        /// <summary>
        /// Gets the hash code of a given theorem object.
        /// </summary>
        /// <param name="obj">The theorem object.</param>
        /// <returns>The hash code.</returns>
        public int GetHashCode(TheoremObject obj)
        {
            // Get the hash code of the signature
            var typeHash = obj.Type.GetHashCode();

            // Get the order-independent hash code of involved objects using their ids
            var objectsHash = obj.InternalObjects.GetOrderIndependentHashCode(o => o.GetHashCode());

            // Get order-dependent hash code of these two values
            return HashCodeUtilities.GetOrderDependentHashCode(typeHash, objectsHash);
        }

        #endregion
    }
}
