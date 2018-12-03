using GeoGen.Utilities;
using System.Collections.Generic;

namespace GeoGen.Core
{
    /// <summary>
    /// Represents an equality comparer of <see cref="Theorem"/>s. It assumes the theorems
    /// have the same type and then compares their theorem objects as sets, using
    /// our private TheoremObject comparer.
    /// </summary>
    public class TheoremEqualityComparer : IEqualityComparer<Theorem>
    {
        #region Instance

        /// <summary>
        /// The singleton instance of this comparer.
        /// </summary>
        public static readonly TheoremEqualityComparer Instance = new TheoremEqualityComparer();

        #endregion

        #region Private properties

        /// <summary>
        /// A shortcut to access the theorem objects comparer.
        /// </summary>
        private IEqualityComparer<TheoremObject> ObjectsComparer => TheoremObjectEqualityComparer.Instance;

        #endregion

        #region IEquaityComparar implementation

        /// <summary>
        /// Finds out if two given theorems are equal.
        /// </summary>
        /// <param name="x">The first theorem.</param>
        /// <param name="y">The second theorem.</param>
        /// <returns>true, if they are equal; false otherwise.</returns>
        public bool Equals(Theorem x, Theorem y)
        {
            // Compare the involved objects of theorems as sets
            // This assumes that the passed theorems already have the same type
            return x.InvolvedObjects.ToSet(ObjectsComparer).SetEquals(y.InvolvedObjects.ToSet(ObjectsComparer));
        }

        /// <summary>
        /// Gets the hash code of a given theorem.
        /// </summary>
        /// <param name="obj">The theorem.</param>
        /// <returns>The hash code.</returns>
        public int GetHashCode(Theorem obj)
        {
            // The involved objects of theorems are order-independent
            return obj.InvolvedObjects.GetOrderIndependentHashCode(ObjectsComparer.GetHashCode);
        }

        #endregion
    }
}
