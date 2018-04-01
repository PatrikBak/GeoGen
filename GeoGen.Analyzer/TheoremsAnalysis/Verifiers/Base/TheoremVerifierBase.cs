using GeoGen.Core;
using GeoGen.Utilities;
using System.Collections.Generic;

namespace GeoGen.Analyzer
{
    /// <summary>
    /// A base implementation of <see cref="ITheoremVerifier"/> that infers the
    /// type of the theorem from a name that should be in the form "{type}Verifier".
    /// </summary>
    internal abstract class TheoremVerifierBase : ITheoremVerifier
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
        /// Gets the enumerable of verifier outputs that pulls objects from
        /// a given contextual container (that represents the configuration)
        /// </summary>
        /// <param name="container">The container.</param>
        /// <returns>The outputs.</returns>
        public abstract IEnumerable<VerifierOutput> GetOutput(IContextualContainer container);

        #endregion
    }
}
