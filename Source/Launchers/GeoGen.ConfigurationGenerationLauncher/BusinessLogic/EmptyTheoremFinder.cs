using GeoGen.Constructor;
using GeoGen.Core;
using GeoGen.TheoremFinder;
using System;

namespace GeoGen.ConfigurationGenerationLauncher
{
    /// <summary>
    /// The implementation of <see cref="ITheoremFinder"/> that returns no theorems.
    /// </summary>
    public class EmptyTheoremFinder : ITheoremFinder
    {
        /// <summary>
        /// Finds all theorems that hold true in the configuration represented by a given contextual picture.
        /// </summary>
        /// <param name="contextualPicture">The contextual picture that represents the configuration.</param>
        /// <returns>The enumerable of true theorems in the configuration.</returns>
        public TheoremMap FindAllTheorems(ContextualPicture contextualPicture) => new TheoremMap(Array.Empty<Theorem>());

        /// <summary>
        /// Finds all theorems that hold true in the configuration represented by a given contextual picture
        /// and in their statement use the last object of the configuration.
        /// </summary>
        /// <param name="contextualPicture">The contextual picture that represents the configuration.</param>
        /// <param name="oldTheorems">The theorems that hold true in the configuration without the last object.</param>
        /// <returns>The enumerable of true theorems in the configuration that use the last object.</returns>
        public TheoremMap FindNewTheorems(ContextualPicture contextualPicture, TheoremMap oldTheorems) => new TheoremMap(Array.Empty<Theorem>());
    }
}
