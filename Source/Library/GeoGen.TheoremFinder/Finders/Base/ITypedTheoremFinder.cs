using GeoGen.Constructor;
using GeoGen.Core;

namespace GeoGen.TheoremFinder
{
    /// <summary>
    /// Represents a service that is able to find theorems in <see cref="ContextualPicture"/>s.
    /// </summary>
    public interface ITypedTheoremFinder
    {
        /// <summary>
        /// The type of theorems that this theorem finder finds.
        /// </summary>
        TheoremType Type { get; }

        /// <summary>
        /// Finds all theorems of the sought type that hold true in the configuration 
        /// represented by a given contextual picture.
        /// </summary>
        /// <param name="contextualPicture">The contextual picture that represents the configuration.</param>
        /// <returns>The enumerable of true theorems of the sought type in the configuration.</returns>      
        IEnumerable<Theorem> FindAllTheorems(ContextualPicture contextualPicture);

        /// <summary>
        /// Finds all theorems of the sought type that hold true in the configuration 
        /// represented by a given contextual picture and in their statement use the
        /// last object of the configuration, while there is no geometrically distinct
        /// way to state them without this last object.
        /// </summary>
        /// <param name="contextualPicture">The contextual picture that represents the configuration.</param>
        /// <returns>The enumerable of true theorems of the sought type in the configuration that need the last object.</returns>
        IEnumerable<Theorem> FindNewTheorems(ContextualPicture contextualPicture);

        /// <summary>
        /// Finds out if a given old theorem is still valid.
        /// </summary>
        /// <param name="contextualPicture">The contextual picture where the old theorem was found.</param>
        /// <param name="oldTheorem">The old theorem to be validated.</param>
        /// <returns>true if the old theorem is still valid; false otherwise.</returns>
        bool ValidateOldTheorem(ContextualPicture contextualPicture, Theorem oldTheorem);
    }
}