using System.Collections.Generic;
using GeoGen.Core;

namespace GeoGen.Analyzer
{
    /// <summary>
    /// Represents a verifier of a theorem of a single type.
    /// </summary>
    internal interface ITheoremVerifier
    {
        /// <summary>
        /// Gets the enumerable of verifier outputs that pulls objects from
        /// a given contextual container (that represents the configuration)
        /// </summary>
        /// <param name="container">The container.</param>
        /// <returns>The outputs.</returns>
        IEnumerable<VerifierOutput> GetOutput(IContextualContainer container);
    }
}