using GeoGen.Core;
using GeoGen.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using static GeoGen.Core.TheoremType;

namespace GeoGen.TheoremProver
{
    /// <summary>
    /// Represents <see cref="ITheoremDeriver"/> that derives theorems using the <see cref="DerivationRule.Transitivity"/>.
    /// </summary>
    public class TransitivityDeriver : TheoremDeriverBase
    {
        /// <summary>
        /// Takes new theorems and based on logical reason comes up with relationships between them,
        /// i.e. which theorem would be sufficient to prove to come up with some other one of them.
        /// </summary>
        /// <param name="theorems">The theorems between which we're finding relationships.</param>
        /// <returns>The enumerable of all found relationships, i.e. assumptions and the theorem that follows from them.</returns>
        public override IEnumerable<(IReadOnlyList<Theorem> assumptions, Theorem impliedTheorem)> DeriveTheorems(TheoremMap theorems)
        {
            // Go through all pairs of type, theorems 
            foreach (var (theoremType, theoremsOfType) in theorems)
            {
                // Get the number of needed objects that identifies this theorem type
                var neededObjects = theoremType switch
                {
                    ConcyclicPoints => 3,
                    CollinearPoints => 2,
                    ConcurrentLines => 2,
                    ParallelLines => 1,
                    EqualLineSegments => 1,
                    EqualAngles => 1,

                    // All other cases don't support transitivity
                    _ => 0
                };

                // Return nothing if the theorem doesn't support transitivity
                if (neededObjects == 0)
                    continue;

                // Otherwise take all pairs of theorems to see what transitivities they imply
                foreach (var (theorem1, theorem2) in theoremsOfType.UnorderedPairs())
                {
                    // Get their common objects
                    var commonObjects = theorem1.InvolvedObjects.Intersect(theorem2.InvolvedObjects).ToArray();

                    // If there is not enough of them, we can't imply anything
                    if (commonObjects.Length < neededObjects)
                        continue;

                    // Otherwise any (n+1)-tuple, where 'n' is the needed number of objects, represents a true theorem. 
                    var impliedTheorems = theorem1.InvolvedObjects.Union(theorem2.InvolvedObjects).Subsets(neededObjects + 1)
                        // We make theorems for each
                        .Select(objects => new Theorem(theorem1.Configuration, theoremType, objects))
                        // Take those that are equivalent to our theorems
                        .Where(theorem => !theorem.Equals(theorem1) && !theorem.Equals(theorem2));

                    // Enumerate all implies theorems
                    foreach (var impliedTheorem in impliedTheorems)
                        yield return (new[] { theorem1, theorem2 }, impliedTheorem);
                }
            }
        }
    }
}
