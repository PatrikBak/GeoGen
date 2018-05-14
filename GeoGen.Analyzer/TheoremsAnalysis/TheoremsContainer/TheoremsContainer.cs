using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Core;

namespace GeoGen.Analyzer
{
    /// <summary>
    /// A default implementation of <see cref="ITheoremsContainer"/>.
    /// </summary>
    internal class TheoremsContainer : ITheoremsContainer
    {
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
                // Use the default comparer of theorems
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