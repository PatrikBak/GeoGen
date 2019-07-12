using GeoGen.Constructor;
using System.Collections.Generic;

namespace GeoGen.Analyzer
{
    /// <summary>
    /// Represents a service that is able find <see cref="PotentialTheorem"/>s in a <see cref="IContextualPicture"/>.
    /// </summary>
    public interface IPotentialTheoremsAnalyzer
    {
        /// <summary>
        /// Finds all potential (unverified) theorems in a given contextual picture.
        /// </summary>
        /// <param name="contextualPicture">The picture from which we get the actual geometric objects.</param>
        /// <returns>An enumerable of found potential theorems.</returns>
        IEnumerable<PotentialTheorem> FindPotentialTheorems(IContextualPicture contextualPicture);
    }
}