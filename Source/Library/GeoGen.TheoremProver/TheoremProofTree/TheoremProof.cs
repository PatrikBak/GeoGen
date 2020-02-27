using GeoGen.Core;
using System;
using System.Collections.Generic;

namespace GeoGen.TheoremProver
{
    /// <summary>
    /// Represents a proof of a <see cref="Theorem"/> with custom <see cref="TheoremInferenceData"/> explaining
    /// the proof and needed <see cref="ProvedAssumptions"/> to make this conclusion.
    /// </summary>
    public class TheoremProof
    {
        #region Public properties

        /// <summary>
        /// The proved theorem.
        /// </summary>
        public Theorem Theorem { get; }

        /// <summary>
        /// The metadata of the proof.
        /// </summary>
        public TheoremInferenceData Data { get; }

        /// <summary>
        /// The type of the used inference rule.
        /// </summary>
        public InferenceRuleType Rule => Data.RuleType;

        /// <summary>
        /// The proved assumptions.
        /// </summary>
        public IReadOnlyList<TheoremProof> ProvedAssumptions { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="TheoremProof"/> class.
        /// </summary>
        /// <param name="theorem">The proved theorem.</param>
        /// <param name="data">The metadata of the proof.</param>
        /// <param name="provedAssumptions">The proved assumptions.</param>
        public TheoremProof(Theorem theorem, TheoremInferenceData data, IReadOnlyList<TheoremProof> provedAssumptions)
        {
            Theorem = theorem ?? throw new ArgumentNullException(nameof(theorem));
            Data = data ?? throw new ArgumentNullException(nameof(data));
            ProvedAssumptions = provedAssumptions ?? throw new ArgumentNullException(nameof(provedAssumptions));
        }

        #endregion
    }
}