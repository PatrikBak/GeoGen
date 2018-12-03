using System.Collections.Generic;

namespace GeoGen.Analyzer
{
    /// <summary>
    /// Represents a verifier that generates <see cref="PotentialTheorem"/>s.
    /// </summary>
    internal interface ITheoremVerifier
    {
        /// <summary>
        /// Finds all potencial unverified theorems wrapped in <see cref="PotentialTheorem"/> objects.
        /// </summary>
        /// <param name="container">The container from which we get the geometrical objects.</param>
        /// <returns>The outputs.</returns>
        IEnumerable<PotentialTheorem> FindPotencialTheorems(IContextualContainer container);
    }
}