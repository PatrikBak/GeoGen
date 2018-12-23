using GeoGen.Core;
using GeoGen.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.Analyzer
{
    /// <summary>
    /// A base implementation of <see cref="ITheoremVerifier"/> that takes care
    /// of verifying theorems of a single type. This type is inferred from a class name 
    /// that should be in the form "{type}Verifier".
    /// </summary>
    public abstract class TheoremVerifierBase : ITheoremVerifier
    {
        #region ITheoremVerifier properties

        /// <summary>
        /// Gets the type of the theorem that this verifier checks.
        /// </summary>
        public TheoremType Type { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        protected TheoremVerifierBase()
        {
            // Set the compulsory construction type
            Type = FindTypeFromClassName();
        }

        #endregion

        #region Finding type from class name

        /// <summary>
        /// Infers the type of the verifier from the class name. 
        /// The class name should be in the form {type}Constructor.
        /// </summary>
        /// <returns>The type.</returns>
        private TheoremType FindTypeFromClassName() => EnumUtilities.ParseEnumValueFromClassName<TheoremType>(GetType(), "Verifier");

        #endregion

        #region Abstract methods

        /// <summary>
        /// Finds all potencial unverified theorems wrapped in <see cref="PotentialTheorem"/> objects.
        /// </summary>
        /// <param name="container">The container from which we get the geometrical objects.</param>
        /// <returns>The outputs.</returns>
        public abstract IEnumerable<PotentialTheorem> FindPotencialTheorems(IContextualContainer container);

        #endregion
    }
}