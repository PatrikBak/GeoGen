using GeoGen.Core;
using System;
using System.Collections.Generic;

namespace GeoGen.TheoremProver
{
    /// <summary>
    /// Represents an output of a <see cref="ITheoremProver"/>.
    /// </summary>
    public class TheoremProverOutput
    {
        #region Public properties

        /// <summary>
        /// The dictionary mapping proved theorems to their successful proof attempts.
        /// These theorems are taken from <see cref="TheoremProverInput.NewTheorems"/>.
        /// </summary>
        public IReadOnlyDictionary<Theorem, TheoremProofAttempt> ProvenTheorems { get; }

        /// <summary>
        /// The dictionary mapping proved theorems to their unsuccessful proof attempts.
        /// These theorems are taken from <see cref="TheoremProverInput.NewTheorems"/>.
        /// The list might be empty if there was no attempt to prove the theorem.
        /// </summary>
        public IReadOnlyDictionary<Theorem, IReadOnlyList<TheoremProofAttempt>> UnprovenTheorems { get; }

        /// <summary>
        /// The dictionary mapping discovered theorems to their unsuccessful proof attempts.
        /// These theorems are not present in the output, but the prover found them while 
        /// attempting to prove <see cref="TheoremProverInput.NewTheorems"/>.
        /// </summary>
        public IReadOnlyDictionary<Theorem, IReadOnlyList<TheoremProofAttempt>> UnprovenDiscoveredTheorems { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="TheoremProverOutput"/> class.
        /// </summary>
        /// <param name="provenTheorems">The dictionary mapping proved theorems to their successful proof attempts.</param>
        /// <param name="unprovenTheorems">The dictionary mapping proved theorems to their unsuccessful proof attempts.</param>
        /// <param name="unprovenDiscoveredTheorems">The dictionary mapping discovered theorems to their unsuccessful proof attempts.</param>
        public TheoremProverOutput(IReadOnlyDictionary<Theorem, TheoremProofAttempt> provenTheorems,
                                   IReadOnlyDictionary<Theorem, IReadOnlyList<TheoremProofAttempt>> unprovenTheorems,
                                   IReadOnlyDictionary<Theorem, IReadOnlyList<TheoremProofAttempt>> unprovenDiscoveredTheorems)
        {
            ProvenTheorems = provenTheorems ?? throw new ArgumentNullException(nameof(provenTheorems));
            UnprovenTheorems = unprovenTheorems ?? throw new ArgumentNullException(nameof(unprovenTheorems));
            UnprovenDiscoveredTheorems = unprovenDiscoveredTheorems ?? throw new ArgumentNullException(nameof(unprovenDiscoveredTheorems));
        }

        #endregion
    }
}