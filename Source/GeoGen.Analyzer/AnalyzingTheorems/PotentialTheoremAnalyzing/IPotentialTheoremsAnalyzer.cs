using System.Collections.Generic;

namespace GeoGen.Analyzer
{
    /// <summary>
    /// Represents a service that is able find <see cref="PotentialTheorem"/>s in a <see cref="IContextualContainer"/>.
    /// </summary>
    public interface IPotentialTheoremsAnalyzer
    {
        /// <summary>
        /// Finds all potential (unverified) theorems in a given contextual container.
        /// </summary>
        /// <param name="container">The container from which we get the actual geometric objects.</param>
        /// <returns>An enumerable of found potential theorems.</returns>
        IEnumerable<PotentialTheorem> FindPotentialTheorems(IContextualContainer container);
    }
}