using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Core;
using GeoGen.Utilities;

namespace GeoGen.Analyzer
{
    /// <summary>
    /// A default implementation of <see cref="ITheoremsContainer"/>.
    /// </summary>
    internal class TheoremsContainer : ITheoremsContainer
    {
        #region IEqualityComparer for TheoremObjects

        /// <summary>
        /// Represents an equality comparer of <see cref="TheoremObject"/>s. It assumes the theorems
        /// have the same type and then compares their theorem objects as sets, using
        /// our private TheoremObject comparer.
        /// </summary>
        private class TheoremObjectComparer : IEqualityComparer<TheoremObject>
        {
            #region Instance

            /// <summary>
            /// The singleton instance of this comparer.
            /// </summary>
            public static readonly TheoremObjectComparer Instance = new TheoremObjectComparer();

            #endregion

            #region IEqualityComparer implementation

            /// <summary>
            /// Finds out if two given theorem objects are equal.
            /// </summary>
            /// <param name="x">The first theorem object.</param>
            /// <param name="y">The second theorem object.</param>
            /// <returns>true, if they are equal; false otherwise.</returns>
            public bool Equals(TheoremObject x, TheoremObject y)
            {// If we have distinct then the objects are distinct
                if (x.Type != y.Type)
                    return false;

                // Local function to convert object to set of involved objects' ids
                HashSet<int> ToIdsSet(TheoremObject obj)
                {
                    return obj.InternalObjects
                            .Select(o => o.Id ?? throw new AnalyzerException("Id must be set"))
                            .ToSet();
                }

                // We convert both objects to these sets and compare them as sets
                return ToIdsSet(x).SetEquals(ToIdsSet(y));
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
                var objectsHash = obj.InternalObjects.GetOrderIndependentHashCode(o => o.Id ?? throw new AnalyzerException("Id must be set"));

                // Get order-dependent hash code of these two values
                return HashCodeUtilities.GetOrderDependentHashCode(typeHash, objectsHash);
            }

            #endregion
        }

        #endregion

        #region IEqualityComparer for Theorems

        /// <summary>
        /// Represents an equality comparer of <see cref="Theorem"/>s. It assumes the theorems
        /// have the same type and then compares their theorem objects as sets, using
        /// our private TheoremObject comparer.
        /// </summary>
        private class TheoremEqualityComparer : IEqualityComparer<Theorem>
        {
            #region Instance

            /// <summary>
            /// The singleton instance of this comparer.
            /// </summary>
            public static readonly TheoremEqualityComparer Instance = new TheoremEqualityComparer();

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
                return x.InvolvedObjects.ToSet(TheoremObjectComparer.Instance).SetEquals(y.InvolvedObjects.ToSet(TheoremObjectComparer.Instance));
            }

            /// <summary>
            /// Gets the hash code of a given theorem.
            /// </summary>
            /// <param name="obj">The theorem.</param>
            /// <returns>The hash code.</returns>
            public int GetHashCode(Theorem obj)
            {
                // The involved objects of theorems are order-independent
                return obj.InvolvedObjects.GetOrderIndependentHashCode(TheoremObjectComparer.Instance.GetHashCode);
            }

            #endregion
        }

        #endregion

        #region Private fields

        /// <summary>
        /// The dictionary mapping theorem types to sets of theorems of this type.
        /// </summary>
        private readonly IDictionary<TheoremType, HashSet<Theorem>> _theoremsDictionary;

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        public TheoremsContainer()
        {
            // Create the for theorems
            _theoremsDictionary = new Dictionary<TheoremType, HashSet<Theorem>>();

            // Initialize the theorems dictionary with all available theorem types
            foreach (var value in Enum.GetValues(typeof(TheoremType)))
            {
                // Use our private comparer of theorems
                _theoremsDictionary.Add((TheoremType) value, new HashSet<Theorem>(TheoremEqualityComparer.Instance));
            }
        }

        #endregion

        #region ITheoremsContainer implementation

        /// <summary>
        /// Adds a given theorem to the container.
        /// </summary>
        /// <param name="theorem">The theorem.</param>
        public void Add(Theorem theorem)
        {
            _theoremsDictionary[theorem.Type].Add(theorem);
        }

        /// <summary>
        /// Finds out if a given theorem is present in the container.
        /// </summary>
        /// <param name="theorem">The theorem.</param>
        /// <returns>true, if it is present, false otherwise;</returns>
        public bool Contains(Theorem theorem)
        {
            return _theoremsDictionary[theorem.Type].Contains(theorem);
        }

        #endregion

        #region IEnumerable implementation

        /// <summary>
        /// Gets a generic enumerator.
        /// </summary>
        /// <returns>The enumerator.</returns>
        public IEnumerator<Theorem> GetEnumerator()
        {
            // Merge theorems of all types
            return _theoremsDictionary.Values.SelectMany(s => s).GetEnumerator();
        }

        /// <summary>
        /// Gets a non-generic enumerator.
        /// </summary>
        /// <returns>The enumerator.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}