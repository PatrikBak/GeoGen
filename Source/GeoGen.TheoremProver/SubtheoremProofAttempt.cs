using GeoGen.Core;
using System;
using System.Collections.Generic;

namespace GeoGen.TheoremProver
{
    /// <summary>
    /// Represents a <see cref="TheoremProofAttempt"/> that uses <see cref="DerivationRule.Subtheorem"/>.
    /// It contains the <see cref="TemplateTheorem"/> that was used in the attempt.
    /// </summary>
    public class SubtheoremProofAttempt : TheoremProofAttempt
    {
        #region Public properties

        /// <summary>
        /// The theorem that is assumed to be true and was found implying our theorem
        /// </summary>
        public Theorem TemplateTheorem { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SubtheoremProofAttempt"/> class.
        /// </summary>
        /// <param name="theorem">The theorem that was attempted to be proven.</param>
        /// <param name="templateTheorem">The theorem that is assumed to be true and was found implying our theorem.</param>
        /// <param name="provenAssumptions">The proven assumptions of the used <see cref="Rule"/>.</param>
        /// <param name="unprovenAssumptions">The unproven assumptions of the used <see cref="Rule"/>, with all the attempts to prove each of them.</param>
        public SubtheoremProofAttempt(Theorem theorem,
                                      Theorem templateTheorem,
                                      IReadOnlyList<TheoremProofAttempt> provenAssumptions,
                                      IReadOnlyList<(Theorem theorem, IReadOnlyList<TheoremProofAttempt> unfinishedProofs)> unprovenAssumptions)
            // Use the base constructor with the subtheorem rule
            : base(theorem, DerivationRule.Subtheorem, provenAssumptions, unprovenAssumptions)
        {
            TemplateTheorem = templateTheorem ?? throw new ArgumentNullException(nameof(templateTheorem));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SubtheoremProofAttempt"/> class,
        /// with deferred setting of <see cref="TheoremProofAttempt.ProvenAssumptions"/> and 
        /// <see cref="TheoremProofAttempt.UnprovenAssumptions"/>.
        /// </summary>
        /// <param name="theorem">The theorem that was attempted to be proven.</param>
        /// <param name="templateTheorem">The theorem that is assumed to be true and was found implying our theorem.</param>
        internal SubtheoremProofAttempt(Theorem theorem, Theorem templateTheorem)
            // Use the base constructor with the subtheorem rule
            : base(theorem, DerivationRule.Subtheorem)
        {
            TemplateTheorem = templateTheorem ?? throw new ArgumentNullException(nameof(templateTheorem));
        }

        #endregion
    }
}