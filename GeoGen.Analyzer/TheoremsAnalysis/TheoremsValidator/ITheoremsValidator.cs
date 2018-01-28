using System.Collections.Generic;
using GeoGen.Core;

namespace GeoGen.Analyzer
{
    /// <summary>
    /// Represents a service that converts a verifier output found for a configuration
    /// into theorems.
    /// </summary>
    internal interface ITheoremsValidator
    {
        /// <summary>
        /// Performs the theorems validation.
        /// </summary>
        /// <param name="configuration">The configuration for which the output is found.</param>
        /// <param name="verifiersOutput">The output of all theorems verifiers.</param>
        /// <returns>The theorems.</returns>
        IEnumerable<Theorem> ValidateTheorems(Configuration configuration, IEnumerable<VerifierOutput> verifiersOutput);
    }
}