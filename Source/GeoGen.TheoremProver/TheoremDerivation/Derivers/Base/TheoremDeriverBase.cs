using GeoGen.Core;
using GeoGen.Utilities;
using System.Collections.Generic;

namespace GeoGen.TheoremProver
{
    /// <summary>
    /// The base class for implementations of <see cref="ITheoremDeriver"/>.
    /// </summary>
    public abstract class TheoremDeriverBase : ITheoremDeriver
    {
        #region ITheoremDeriver properties

        /// <summary>
        /// The logical rule that this deriver applies in order to find out which theorems imply which.
        /// </summary>
        public DerivationRule Rule { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="TheoremDeriverBase"/> class.
        /// </summary>
        protected TheoremDeriverBase()
        {
            // Find the rule
            Rule = FindRuleFromClassName();
        }

        #endregion

        #region Finding rule from the class name

        /// <summary>
        /// Infers the rule of the deriver from the class name. The class name should be in the form {rule}Deriver.
        /// </summary>
        /// <returns>The inferred rule.</returns>
        private DerivationRule FindRuleFromClassName()
        {
            // Call the utility helper that does the job
            return EnumUtilities.ParseEnumValueFromClassName<DerivationRule>(GetType(), classNamePrefix: "Deriver");
        }

        #endregion

        #region ITheoremDeriver methods

        /// <summary>
        /// Takes new theorems and based on logical reason comes up with relationships between them,
        /// i.e. which theorem would be sufficient to prove to come up with some other one of them.
        /// </summary>
        /// <param name="theorems">The theorems between which we're finding relationships.</param>
        /// <returns>The enumerable of all found relationships, i.e. assumptions and the theorem that follows from them.</returns>
        public abstract IEnumerable<(IReadOnlyList<Theorem> assumptions, Theorem impliedTheorem)> DeriveTheorems(TheoremMap theorems);

        #endregion
    }
}
