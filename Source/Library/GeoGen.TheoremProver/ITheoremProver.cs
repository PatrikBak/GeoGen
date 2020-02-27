using GeoGen.Constructor;
using GeoGen.Core;
using GeoGen.Utilities;
using System.Collections.Generic;

namespace GeoGen.TheoremProver
{
    /// <summary>
    /// Represents a service that is able to prove theorems and construct their proofs. It requires the <see cref="Configuration"/>
    /// to be drawn in a <see cref="ContextualPicture"/>. Theorems are passed in two <see cref="TheoremMap"/>s, one of them with old
    /// <see cref="Theorem"/>s, i.e. ones hold in the configuration without the last object, and the other one with new theorems, i.e.
    /// ones that say something about the last object.
    /// </summary>
    public interface ITheoremProver
    {
        /// <summary>
        /// Proves given theorems that are true in the configuration drawn in a given picture.
        /// </summary>
        /// <param name="oldTheorems">The theorems that hold in the configuration without the last object.</param>
        /// <param name="newTheorems">The theorems that say something about the last object.</param>
        /// <param name="picture">The picture where the configuration in which the theorems hold is drawn.</param>
        /// <returns>The set of proved theorems.</returns>
        IReadOnlyHashSet<Theorem> ProveTheorems(TheoremMap oldTheorems, TheoremMap newTheorems, ContextualPicture picture);

        /// <summary>
        /// Proves given theorems that are true in the configuration drawn in a given picture and constructs their proofs.
        /// </summary>
        /// <param name="oldTheorems">The theorems that hold in the configuration without the last object.</param>
        /// <param name="newTheorems">The theorems that say something about the last object.</param>
        /// <param name="picture">The picture where the configuration in which the theorems hold is drawn.</param>
        /// <returns>The dictionary mapping proved theorems to their proofs.</returns>
        IReadOnlyDictionary<Theorem, TheoremProof> ProveTheoremsAndConstructProofs(TheoremMap oldTheorems, TheoremMap newTheorems, ContextualPicture picture);
    }
}