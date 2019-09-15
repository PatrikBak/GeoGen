using GeoGen.Core;
using GeoGen.Utilities;
using System;
using System.Collections.Generic;

namespace GeoGen.TheoremProver
{
    /// <summary>
    /// Represents at attempt at a proof of a <see cref="Theorem"/> by a <see cref="Rule"/>.
    /// This rule might require other theorems to be proven. These theorems are contained
    /// in the properties <see cref="ProvenAssumptions"/> and <see cref="UnprovenAssumptions"/>.
    /// Therefore the proof <see cref="IsSuccessful"/> if and only if there are no unproven theorems.
    /// </summary>
    public class TheoremProofAttempt
    {
        #region Public properties

        /// <summary>
        /// The theorem that was attempted to be proven, maybe successfully (<see cref="IsSuccessful"/>).
        /// </summary>
        public Theorem Theorem { get; }

        /// <summary>
        /// The data of this attempt.
        /// </summary>
        public TheoremDerivationData Data { get; }

        /// <summary>
        /// The used derivation rule.
        /// </summary>
        public DerivationRule Rule => Data.Rule;

        /// <summary>
        /// The proven assumptions of the used <see cref="Rule"/>.
        /// </summary>
        public IReadOnlyList<TheoremProofAttempt> ProvenAssumptions { get; internal set; }

        /// <summary>
        /// The unproven assumption of the used <see cref="Rule"/>, with all the attempts to prove each of them.
        /// </summary>
        public IReadOnlyList<(Theorem theorem, IReadOnlyList<TheoremProofAttempt> unfinishedProofs)> UnprovenAssumptions { get; internal set; }

        /// <summary>
        /// Indicates whether the proof of the theorem was successful, i.e. there is nothing left to prove.
        /// </summary>
        public bool IsSuccessful => UnprovenAssumptions.IsEmpty();

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TheoremProofAttempt"/> class.
        /// </summary>
        /// <param name="theorem">The theorem that was attempted to be proven.</param>
        /// <param name="data">The data of this attempt.</param>
        /// <param name="provenAssumptions">The proven assumptions of the used <see cref="Rule"/>.</param>
        /// <param name="unprovenAssumptions">The unproven assumptions of the used <see cref="Rule"/>, with all the attempts to prove each of them.</param> 
        public TheoremProofAttempt(Theorem theorem,
                                   TheoremDerivationData data,
                                   IReadOnlyList<TheoremProofAttempt> provenAssumptions,
                                   IReadOnlyList<(Theorem theorem, IReadOnlyList<TheoremProofAttempt> unfinishedProofs)> unprovenAssumptions)
        {
            Theorem = theorem ?? throw new ArgumentNullException(nameof(theorem));
            Data = data ?? throw new ArgumentNullException(nameof(data));
            ProvenAssumptions = provenAssumptions ?? throw new ArgumentNullException(nameof(provenAssumptions));
            UnprovenAssumptions = unprovenAssumptions ?? throw new ArgumentNullException(nameof(unprovenAssumptions));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TheoremProofAttempt"/> class,
        /// with deferred setting of <see cref="ProvenAssumptions"/> and <see cref="UnprovenAssumptions"/>.
        /// </summary>
        /// <param name="theorem">The theorem that was attempted to be proven.</param>
        /// <param name="data">The data of this attempt.</param>
        internal TheoremProofAttempt(Theorem theorem, TheoremDerivationData data)
        {
            Theorem = theorem ?? throw new ArgumentNullException(nameof(theorem));
            Data = data ?? throw new ArgumentNullException(nameof(data));
        }

        #endregion
    }
}