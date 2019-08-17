using GeoGen.Constructor;
using GeoGen.Core;
using System.Collections.Generic;

namespace GeoGen.TheoremsFinder
{
    /// <summary>
    /// Represents a service that is able to find theorems in <see cref="ContextualPicture"/>s.
    /// </summary>
    public interface ITheoremsFinder
    {
        /// <summary>
        /// Finds all theorems that hold true in the configuration represented by a given
        /// contextual picture.
        /// </summary>
        /// <param name="contextualPicture">The contextual picture that represents the configuration.</param>
        /// <returns>The enumerable of true theorems in the configuration.</returns>
        IEnumerable<Theorem> FindAllTheorems(ContextualPicture contextualPicture);

        /// <summary>
        /// Finds all theorems that hold true in the configuration represented by a given 
        /// contextual picture and in their statement use the last object of the configuration.
        /// </summary>
        /// <param name="contextualPicture">The contextual picture that represents the configuration.</param>
        /// <returns>The enumerable of true theorems in the configuration that use the last object.</returns>
        IEnumerable<Theorem> FindNewTheorems(ContextualPicture contextualPicture);
    }
}