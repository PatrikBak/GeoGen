using GeoGen.Constructor;
using GeoGen.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.TheoremsFinder
{
    /// <summary>
    /// The default implementation of <see cref="IRelevantTheoremsAnalyzer"/>. 
    /// </summary>
    public class RelevantTheoremsAnalyzer : IRelevantTheoremsAnalyzer
    {
        #region Dependencies

        /// <summary>
        /// The array of all the available potential theorems analyzers.
        /// </summary>
        private readonly IPotentialTheoremsAnalyzer[] _theoremsAnalyzers;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="RelevantTheoremsAnalyzer"/> class.
        /// </summary>
        /// <param name="theoremsAnalyzers">The array of all the available potential theorems analyzers.</param>
        public RelevantTheoremsAnalyzer(IPotentialTheoremsAnalyzer[] theoremsAnalyzers)
        {
            _theoremsAnalyzers = theoremsAnalyzers ?? throw new ArgumentNullException(nameof(theoremsAnalyzers));
        }

        #endregion

        #region ITheoremsAnalyzer implementation

        /// <summary>
        /// Performs the theorem analysis for a given configuration.
        /// </summary>
        /// <param name="configuration">The configuration where we're looking for theorems.</param>
        /// <param name="manager">The manager of all the pictures where the theorems should be tested.</param>
        /// <param name="contextualPicture">The contextual picture where the configuration is drawn.</param>
        /// <returns>The list of theorems that are true in the configuration.</returns>
        public List<Theorem> Analyze(Configuration configuration, IPicturesManager manager, IContextualPicture contextualPicture)
        {
            // Perform the verification of the theorems.
            return _theoremsAnalyzers.SelectMany(verifier => verifier.FindPotentialTheorems(contextualPicture))
                    // Take only those that are true in every picture
                    .Where(potentialTheorem => manager.All(potentialTheorem.VerificationFunction))
                    // Make them theorems
                    .Select(potentialTheorem => potentialTheorem.ToTheorem(configuration))
                    // Enumerate them to a list
                    .ToList();
        }

        #endregion
    }
}