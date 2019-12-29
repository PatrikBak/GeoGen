using GeoGen.Core;
using GeoGen.Utilities;
using System.Collections.Generic;
using System.Linq;
using static GeoGen.Core.TheoremType;

namespace GeoGen.TheoremProver
{
    /// <summary>
    /// Represents <see cref="ITheoremDeriver"/> that derives theorems using the <see cref="DerivationRule.ThalesTheorem"/>.
    /// </summary>
    public class ThalesTheoremDeriver : TheoremDeriverBase
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
            foreach (var perpendicularLineTheorems in theorems.GetObjectsForKeys(PerpendicularLines).Subsets(2))
            {
                // Get the lines
                var lines = perpendicularLineTheorems.SelectMany(theorem => theorem.InvolvedObjects).Cast<LineTheoremObject>();

                // Take them only if they all are defined by points
                if (lines.Any(line => !line.DefinedByPoints))
                    continue;

                // Get the common point from each theorem
                var commonPoints = perpendicularLineTheorems.Select(theorem =>
                        // For a theorem we take the points of the first line
                        theorem.InvolvedObjectsList[0].GetInnerConfigurationObjects()
                        // Intersect with the points of the second line
                        .Intersect(theorem.InvolvedObjectsList[1].GetInnerConfigurationObjects())
                        // Take the first, if there is some, or null, if there is none
                        .FirstOrDefault())
                    // Enumerate
                    .ToArray();

                // Continue only if both of them have some common point 
                if (commonPoints.Any(point => point == null))
                    continue;

                // Otherwise get the other points
                var otherPoints = lines.SelectMany(line => line.Points).Except(commonPoints).ToArray();

                // Continue only if there are exactly two of them
                if (otherPoints.Length != 2)
                    continue;

                // Prepare the concyclity theorem
                var concyclity = new Theorem(ConcyclicPoints, commonPoints[0], commonPoints[1], otherPoints[0], otherPoints[1]);

                // Any two of our theorems can imply the third
                yield return (new[] { perpendicularLineTheorems[0], perpendicularLineTheorems[1] }, concyclity);
                yield return (new[] { concyclity, perpendicularLineTheorems[1] }, perpendicularLineTheorems[0]);
                yield return (new[] { concyclity, perpendicularLineTheorems[0] }, perpendicularLineTheorems[1]);
            }
        }
    }
}