using GeoGen.Core;
using System;

namespace GeoGen.TheoremProver
{
    /// <summary>
    /// Represents metadata of a derivation that uses <see cref="DerivationRule.Subtheorem"/>.
    /// </summary>
    public class SubtheoremDerivationData : TheoremDerivationData
    {
        #region Public properties

        /// <summary>
        /// The theorem that is assumed to be true and was found implying some theorem.
        /// </summary>
        public Theorem TemplateTheorem { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="SubtheoremDerivationData"/> class.
        /// </summary>
        /// <param name="templateTheorem">The theorem that is assumed to be true and was found implying some theorem.</param>
        public SubtheoremDerivationData(Theorem templateTheorem)
            : base(DerivationRule.Subtheorem)
        {
            TemplateTheorem = templateTheorem ?? throw new ArgumentNullException(nameof(templateTheorem));
        }

        #endregion
    }
}