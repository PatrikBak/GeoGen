using GeoGen.Core;
using System.Collections.Generic;

namespace GeoGen.TheoremProver
{
    /// <summary>
    /// Represents a service that combines already found theorems and comes up with relationships
    /// between them using a particular <see cref="Rule"/> to this reasoning.
    /// </summary>
    public interface ITheoremDeriver
    {
        /// <summary>
        /// The logical rule that this deriver applies in order to find out which theorems imply which.
        /// </summary>
        public DerivationRule Rule { get; }

        /// <summary>
        /// Takes new theorems and based on logical reason comes up with relationships between them,
        /// i.e. which theorem would be sufficient to prove to come up with some other one of them.
        /// </summary>
        /// <param name="theorems">The theorems between which we're finding relationships.</param>
        /// <returns>The enumerable of all found relationships, i.e. assumptions and the theorem that follows from them.</returns>
        IEnumerable<(IReadOnlyList<Theorem> assumptions, Theorem impliedTheorem)> DeriveTheorems(TheoremMap theorems);
    }
}
