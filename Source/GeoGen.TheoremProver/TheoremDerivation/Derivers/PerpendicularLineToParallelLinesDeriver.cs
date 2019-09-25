using GeoGen.Core;
using GeoGen.Utilities;
using System.Collections.Generic;
using System.Linq;
using static GeoGen.Core.TheoremType;

namespace GeoGen.TheoremProver
{
    /// <summary>
    /// Represents <see cref="ITheoremDeriver"/> that derives theorems using the <see cref="DerivationRule.PerpendicularLineToParallelLines"/>.
    /// </summary>
    public class PerpendicularLineToParallelLinesDeriver : TheoremDeriverBase
    {
        /// <summary>
        /// Takes new theorems and based on logical reason comes up with relationships between them,
        /// i.e. which theorem would be sufficient to prove to come up with some other one of them.
        /// </summary>
        /// <param name="theorems">The theorems between which we're finding relationships.</param>
        /// <returns>The enumerable of all found relationships, i.e. assumptions and the theorem that follows from them.</returns>
        public override IEnumerable<(IReadOnlyList<Theorem> assumptions, Theorem impliedTheorem)> DeriveTheorems(TheoremMap theorems)
        {
            // Go through all the pair of perpendicular lines
            foreach (var perpendicularLineTheorems in theorems.GetTheoremsOfTypes(PerpendicularLines).Subsets(2))
            {
                // Get the theorems for comfort
                var theorem1 = perpendicularLineTheorems[0];
                var theorem2 = perpendicularLineTheorems[1];

                // Get the common line
                var commonLine = theorem1.InvolvedObjects.Intersect(theorem2.InvolvedObjects).FirstOrDefault();

                // If there is none, we can't do much
                if (commonLine == null)
                    continue;

                // Get the other lines
                var otherLines = theorem1.InvolvedObjects.Concat(theorem2.InvolvedObjects).Except(commonLine.ToEnumerable());

                // They should be parallel
                var parallelLines = new Theorem(theorem1.Configuration, ParallelLines, otherLines);

                // Any two of these theorems can imply the third
                yield return (new[] { theorem1, theorem2 }, parallelLines);
                yield return (new[] { theorem1, parallelLines }, theorem2);
                yield return (new[] { theorem2, parallelLines }, theorem1);
            }
        }
    }
}