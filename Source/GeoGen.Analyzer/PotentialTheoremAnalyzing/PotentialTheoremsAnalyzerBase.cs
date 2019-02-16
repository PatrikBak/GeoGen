using GeoGen.Constructor;
using GeoGen.Core;
using GeoGen.Utilities;
using System.Collections.Generic;

namespace GeoGen.Analyzer
{
    /// <summary>
    /// A base implementation of <see cref="IPotentialTheoremsAnalyzer"/> that infers the
    /// type of theorem from the class name that should be in the form "{type}Analyzer".
    /// </summary>
    public abstract class PotentialTheoremsAnalyzerBase : IPotentialTheoremsAnalyzer
    {
        #region Public properties

        /// <summary>
        /// Gets the type of the theorem that this verifier checks.
        /// </summary>
        public TheoremType Type { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="PotentialTheoremsAnalyzerBase"/> class.
        /// </summary>
        protected PotentialTheoremsAnalyzerBase()
        {
            // Find the type
            Type = FindTypeFromClassName();
        }

        #endregion

        #region Finding type from the class name

        /// <summary>
        /// Infers the type of the predefined constructor from the class name. 
        /// The class name should be in the form {type}Analyzer.
        /// </summary>
        /// <returns>The inferred type.</returns>
        private TheoremType FindTypeFromClassName()
        {
            // Call the utility helper that does the job
            return EnumUtilities.ParseEnumValueFromClassName<TheoremType>(GetType(), classNamePrefix: "Analyzer");
        }

        #endregion

        #region IPotentialTheoremsAnalyzer abstract implementation

        /// <summary>
        /// Finds all potential (unverified) theorems in a given contextual container.
        /// </summary>
        /// <param name="container">The container from which we get the actual geometric objects.</param>
        /// <returns>An enumerable of found potential theorems.</returns>
        public abstract IEnumerable<PotentialTheorem> FindPotentialTheorems(IContextualContainer container);

        #endregion
    }
}