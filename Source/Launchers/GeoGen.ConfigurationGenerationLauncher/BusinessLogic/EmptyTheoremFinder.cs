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
        /// <inheritdoc/>
        public TheoremMap FindAllTheorems(ContextualPicture contextualPicture) => new TheoremMap(Array.Empty<Theorem>());

        /// <inheritdoc/>
        public TheoremMap FindNewTheorems(ContextualPicture contextualPicture, TheoremMap oldTheorems) => new TheoremMap(Array.Empty<Theorem>());
    }
}
