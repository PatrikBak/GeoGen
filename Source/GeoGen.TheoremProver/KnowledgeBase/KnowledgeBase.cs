using GeoGen.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.TheoremProver
{
    /// <summary>
    /// Represents a service that stores relationships between theorems of any type and is able to prove them. 
    /// Each theorem has a set of assumptions. A theorem is proven if and only all its assumptions are proven 
    /// (i.e. there has to be at least one theorem with no assumption). New relationships between theorems
    /// are added via <see cref="AddDerivation(TTheorem, TData, IEnumerable{TTheorem})"/> method. The results
    /// of the class are exposed via <see cref="TheoremDerivation{TTheorem, TData}"/> objects. 
    /// </summary>
    /// <typeparam name="TTheorem">The type of theorem being derived.</typeparam>
    /// <typeparam name="TData">The type of metadata packed with the derivations.</typeparam>
    public class KnowledgeBase<TTheorem, TData>
    {
        #region DerivationData class

        /// <summary>
        /// A helper class holding info about some derivation added via <see cref="AddDerivation(TTheorem, TData, IEnumerable{TTheorem})"/>
        /// method. This info includes the constant metadata, and the two sets of assumptions: proven and to be proven.
        /// When some assumption is proven, it should be moved to the right set, which is done by the <see cref="MarkProven(TTheorem)"/> method.
        /// </summary>
        private class DerivationData
        {
            #region Public properties

            /// <summary>
            /// The theorem that is being attempted to derive.
            /// </summary>
            public TTheorem Theorem { get; }

            /// <summary>
            /// The metadata of the derivation.
            /// </summary>
            public TData Data { get; }

            /// <summary>
            /// The set of theorems that have been already proven.
            /// </summary>
            public HashSet<TTheorem> ProvenTheorems { get; } = new HashSet<TTheorem>();

            /// <summary>
            /// The set of theorems that are still there to be proven.
            /// </summary>
            public HashSet<TTheorem> TheoremsToBeProven { get; } = new HashSet<TTheorem>();

            /// <summary>
            /// The public read-only view of this class that of this private class. 
            /// </summary>
            public TheoremDerivation<TTheorem, TData> TheoremDerivation { get; }

            #endregion

            #region Constructor

            /// <summary>
            /// Initializes a new instance of the <see cref="DerivationData"/> class.
            /// </summary>
            /// <param name="theorem">The theorem that is being attempted to derive.</param>
            /// <param name="data">The metadata of the derivation.</param>
            public DerivationData(TTheorem theorem, TData data)
            {
                // Set the data and theorem
                Data = data;
                Theorem = theorem;

                // Create the read-only public view of this class
                TheoremDerivation = new TheoremDerivation<TTheorem, TData>(Theorem, Data, ProvenTheorems, TheoremsToBeProven);
            }

            #endregion
        }

        #endregion

        #region Public events

        /// <summary>
        /// Fires when a theorem in the knowledge base is proven.
        /// </summary>
        public event Action<TTheorem> TheoremProven = provenTheorem => { };

        #endregion

        #region Private fields

        /// <summary>
        /// The dictionary of theorems and attempts to derive them. If a theorem is not in the 
        /// dictionary, then it hasn't been attempted at yet.
        /// </summary>
        private readonly Dictionary<TTheorem, List<DerivationData>> _derivationAttempts = new Dictionary<TTheorem, List<DerivationData>>();

        /// <summary>
        /// The dictionary of theorems and proofs. If a theorem is not in the dictionary,
        /// then it hasn't been proven yet.
        /// </summary>
        private readonly Dictionary<TTheorem, DerivationData> _proofs = new Dictionary<TTheorem, DerivationData>();

        /// <summary>
        /// The dictionary of theorems and attempts that have these theorems as needed
        /// assumptions.
        /// </summary>
        private readonly Dictionary<TTheorem, HashSet<DerivationData>> _parentDerivations = new Dictionary<TTheorem, HashSet<DerivationData>>();

        #endregion

        #region Public methods

        /// <summary>
        /// Adds the derivation of a theorem with needed assumptions and custom metadata to the knowledge base.
        /// </summary>
        /// <param name="theorem">The derived theorem.</param>
        /// <param name="data">The derivation metadata.</param>
        /// <param name="neededAssumptions">The assumptions needed for this derivation.</param>
        public void AddDerivation(TTheorem theorem, TData data, IEnumerable<TTheorem> neededAssumptions)
        {
            // Create a new empty derivation data with the passed data
            var derivation = new DerivationData(theorem, data);

            // Add all the assumptions to the proven / to be proven set of the derivation
            foreach (var assumedTheorem in neededAssumptions)
            {
                // If the theorem has been proven...
                if (IsProven(assumedTheorem))
                    // Add it to the set of proven theorems of the derivation
                    derivation.ProvenTheorems.Add(assumedTheorem);
                // If it's not proven...
                else
                {
                    // Add it to the set of not proven theorems of the derivation
                    derivation.TheoremsToBeProven.Add(assumedTheorem);

                    // Make sure it's marked in the dictionary of parent derivations
                    _parentDerivations.GetOrAdd(assumedTheorem, () => new HashSet<DerivationData>()).Add(derivation);
                }
            }

            // Add the current derivation to the attempts list
            _derivationAttempts.GetOrAdd(theorem, () => new List<DerivationData>()).Add(derivation);

            // If the current derivation has all the assumed theorems proven
            if (derivation.TheoremsToBeProven.IsEmpty())
            {
                // Then we can set this derivation to be the proof of this theorem
                _proofs.Add(theorem, derivation);

                // And mark this theorem as proven
                MarkProven(theorem);
            }
        }

        /// <summary>
        /// Finds out whether a theorem has already been proven.
        /// </summary>
        /// <param name="theorem">The theorem to be checked.</param>
        /// <returns>true, if it has been proven; false otherwise.</returns>
        public bool IsProven(TTheorem theorem) => _proofs.ContainsKey(theorem);

        /// <summary>
        /// Gets the found proof of the theorem.
        /// </summary>
        /// <returns>The derivation that turned out to be a proof.</returns>
        public TheoremDerivation<TTheorem, TData> GetProof(TTheorem theorem) =>
            // Try to get the derivation from the dictionary
            _proofs.GetOrDefault(theorem)?.TheoremDerivation
            // Throw an exception in case when it's not proven yet
            ?? throw new TheoremProverException("The theorem hasn't been proven yet.");

        /// <summary>
        /// Gets the enumerable of tried derivations of a theorem. If there hasn't been
        /// any attempt, an empty enumerable will be returned.
        /// </summary>
        /// <param name="theorem">The theorem.</param>
        /// <returns>The enumerable of derivations.</returns>
        public IEnumerable<TheoremDerivation<TTheorem, TData>> GetDerivationAttempts(TTheorem theorem) =>
            // Try to get the derivations from the dictionary
            _derivationAttempts.GetOrDefault(theorem)?.Select(attempt => attempt.TheoremDerivation)
            // If there are not there, turn en empty enumerable
            ?? Enumerable.Empty<TheoremDerivation<TTheorem, TData>>();

        #endregion

        #region Private methods

        /// <summary>
        /// Marks a given theorem as proven.
        /// </summary>
        /// <param name="provenTheorem">The proven theorem.</param>
        private void MarkProven(TTheorem provenTheorem)
        {
            // Prepare the queue of theorems to be proven
            var toBeProven = new Queue<TTheorem>();

            // Add the current theorem to the queue
            toBeProven.Enqueue(provenTheorem);

            // Perform until there are some proven theorems
            while (toBeProven.Any())
            {
                // Get the current one
                provenTheorem = toBeProven.Dequeue();

                // Fire an event that the theorem has been proven
                TheoremProven(provenTheorem);

                // If we have a proven theorem that is not an assumption
                // for any other theorem (knowledge ahead), we can't do more
                if (!_parentDerivations.ContainsKey(provenTheorem))
                    continue;

                // Otherwise get the parent derivations
                var parentDerivations = _parentDerivations[provenTheorem];

                // Prepare the set of finished derivations that should be removed
                var toBeRemoved = new HashSet<DerivationData>();

                // Go through the derivations in which this theorem appear
                foreach (var derivation in parentDerivations)
                {
                    // Remove it from the to be proven theorems set
                    derivation.TheoremsToBeProven.Remove(provenTheorem);

                    // Add it to the proven theorems set
                    derivation.ProvenTheorems.Add(provenTheorem);

                    // If the theorem is not proved, we can't do more
                    if (derivation.TheoremsToBeProven.Any())
                        continue;

                    // Otherwise we have proven a new theorem
                    var newProvenTheorem = derivation.Theorem;

                    // This derivation is therefore done and should be removed
                    toBeRemoved.Remove(derivation);

                    // It might have happened that this theorem has been proven
                    // We have found a different proof, but we don't need it
                    if (IsProven(newProvenTheorem))
                        continue;

                    // If this is the first prove we can set this derivation to be the proof
                    _proofs.Add(newProvenTheorem, derivation);

                    // Enqueue the newly proven theorem for being marked as proven as well
                    toBeProven.Enqueue(newProvenTheorem);
                }

                // Remove all finished derivations
                parentDerivations.ExceptWith(toBeRemoved);
            }
        }

        #endregion
    }
}