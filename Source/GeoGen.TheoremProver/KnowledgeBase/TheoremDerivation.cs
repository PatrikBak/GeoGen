using System;
using System.Collections.Generic;

namespace GeoGen.TheoremProver
{
    /// <summary>
    /// Represents a result of a derivation that happened in a <see cref="KnowledgeBase{TTheorem, TData}"/>.
    /// </summary>
    /// <typeparam name="TTheorem">The type of theorem being derived.</typeparam>
    /// <typeparam name="TData">The type of metadata packed with the derivations.</typeparam>
    public class TheoremDerivation<TTheorem, TData>
    {
        #region Public properties

        /// <summary>
        /// The theorem that was being derived.
        /// </summary>
        public TTheorem Theorem { get; }

        /// <summary>
        /// The metadata of the derivation.
        /// </summary>
        public TData Data { get; }

        /// <summary>
        /// The collection of theorems that were proven.
        /// </summary>
        public IReadOnlyCollection<TTheorem> ProvenTheorems { get; }

        /// <summary>
        /// The collection of theorems that are were needed to be proven, but were not.
        /// </summary>
        public HashSet<TTheorem> TheoremsToBeProven { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="TheoremDerivation{TTheorem, TData}"/> class.
        /// </summary>
        /// <param name="theorem">The theorem that was being derived.</param>
        /// <param name="data">The metadata of the derivation.</param>
        /// <param name="provenTheorems">The collection of theorems that were proven.</param>
        /// <param name="theoremsToBeProven">The collection of theorems that are were needed to be proven, but were not.</param>
        public TheoremDerivation(TTheorem theorem, TData data, IReadOnlyCollection<TTheorem> provenTheorems, HashSet<TTheorem> theoremsToBeProven)
        {
            Theorem = theorem;
            Data = data;
            ProvenTheorems = provenTheorems ?? throw new ArgumentNullException(nameof(provenTheorems));
            TheoremsToBeProven = theoremsToBeProven ?? throw new ArgumentNullException(nameof(theoremsToBeProven));
        }

        #endregion
    }
}