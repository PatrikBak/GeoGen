using GeoGen.Constructor;
using GeoGen.Core;
using System.Collections.Generic;

namespace GeoGen.TheoremsFinder.new_stuff
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
        /// <param name="picture">The contextual picture that represents the configuration.</param>
        /// <returns>The list of true theorems in the configuration.</returns>
        List<Theorem> FindAllTheorems(ContextualPicture picture);

        /// <summary>
        /// Finds all theorems that hold true in the configuration and in their statement use the last 
        /// object of the configuration represented by a given contextual picture.
        /// </summary>
        /// <param name="picture">The contextual picture that represents the configuration.</param>
        /// <returns>The list of true theorems in the configuration that use the last object.</returns>
        List<Theorem> FindNewTheorems(ContextualPicture picture);
    }
}