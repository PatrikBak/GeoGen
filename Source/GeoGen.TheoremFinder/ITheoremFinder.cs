using GeoGen.Constructor;
using GeoGen.Core;

namespace GeoGen.TheoremFinder
{
    /// <summary>
    /// Represents a service that is able to find <see cref="Theorem"/>s that hold true
    /// in <see cref="Configuration"/> drawn in a <see cref="ContextualPicture"/>.
    /// </summary>
    public interface ITheoremFinder
    {
        /// <summary>
        /// Finds all theorems that hold true in the configuration represented by a given contextual picture.
        /// </summary>
        /// <param name="contextualPicture">The contextual picture that represents the configuration.</param>
        /// <returns>The enumerable of true theorems in the configuration.</returns>
        TheoremMap FindAllTheorems(ContextualPicture contextualPicture);

        /// <summary>
        /// Finds all theorems that hold true in the configuration represented by a given contextual picture
        /// and in their statement use the last object of the configuration.
        /// </summary>
        /// <param name="contextualPicture">The contextual picture that represents the configuration.</param>
        /// <param name="oldTheorems">The theorems that hold true in the configuration without the last object.</param>
        /// <returns>The enumerable of true theorems in the configuration that use the last object.</returns>
        TheoremMap FindNewTheorems(ContextualPicture contextualPicture, TheoremMap oldTheorems);
    }
}
