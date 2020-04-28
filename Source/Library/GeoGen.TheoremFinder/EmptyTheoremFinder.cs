using GeoGen.Constructor;
using GeoGen.Core;
using System;

namespace GeoGen.TheoremFinder
{
    /// <summary>
    /// The implementation of <see cref="ITheoremFinder"/> that returns no theorems.
    /// </summary>
    public class EmptyTheoremFinder : ITheoremFinder
    {
        /// <inheritdoc/>
        public TheoremMap FindAllTheorems(ContextualPicture contextualPicture) => new TheoremMap(Array.Empty<Theorem>());

        /// <inheritdoc/>
        public TheoremMap FindNewTheorems(ContextualPicture contextualPicture, TheoremMap oldTheorems, out Theorem[] invalidOldTheorems)
        {
            // Set no invalid theorems
            invalidOldTheorems = Array.Empty<Theorem>();

            // Return an empty theorem map
            return new TheoremMap(Array.Empty<Theorem>());
        }
    }
}
