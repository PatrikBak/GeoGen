using GeoGen.Constructor;
using GeoGen.Core;
using GeoGen.Utilities;

namespace GeoGen.TheoremProver
{
    /// <summary>
    /// Represents a service that is able to prove theorems and construct their proofs. It requires the <see cref="Configuration"/>
    /// to be drawn in a <see cref="ContextualPicture"/>. Theorems are passed in two <see cref="TheoremMap"/>s, one of them are
    /// <see cref="Theorem"/>s that are assumed to be true, and the others are supposed to be proven.
    /// </summary>
    public interface ITheoremProver
    {
        /// <summary>
        /// Proves given theorems that are true in the configuration drawn in a given picture.
        /// </summary>
        /// <param name="provenTheorems">The theorems that are assumed to be proven.</param>
        /// <param name="theoremsToBeProven">The theorems that are to be proven.</param>
        /// <param name="picture">The picture where the configuration in which the theorems hold is drawn.</param>
        /// <returns>The set of proved theorems.</returns>
        IReadOnlyHashSet<Theorem> ProveTheorems(TheoremMap provenTheorems, TheoremMap theoremsToBeProven, ContextualPicture picture);

        /// <summary>
        /// Proves given theorems that are true in the configuration drawn in a given picture and constructs their proofs.
        /// </summary>
        /// <param name="provenTheorems">The theorems that are assumed to be proven.</param>
        /// <param name="theoremsToBeProven">The theorems that are to be proven.</param>
        /// <param name="picture">The picture where the configuration in which the theorems hold is drawn.</param>
        /// <returns>The dictionary mapping proved theorems to their proofs.</returns>
        IReadOnlyDictionary<Theorem, TheoremProof> ProveTheoremsAndConstructProofs(TheoremMap provenTheorems, TheoremMap theoremsToBeProven, ContextualPicture picture);
    }
}