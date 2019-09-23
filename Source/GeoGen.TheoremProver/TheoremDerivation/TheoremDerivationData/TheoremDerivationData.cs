namespace GeoGen.TheoremProver
{
    /// <summary>
    /// Represents metadata of a theorem derivation. By default it includes only the <see cref="DerivationRule"/>.
    /// </summary>
    public class TheoremDerivationData
    {
        #region Public properties

        /// <summary>
        /// The used derivation rule.
        /// </summary>
        public DerivationRule Rule { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initialize a new instance of the <see cref="TheoremDerivationData"/> class.
        /// </summary>
        /// <param name="rule">The used derivation rule.</param>
        public TheoremDerivationData(DerivationRule rule)
        {
            Rule = rule;
        }

        #endregion
    }
}