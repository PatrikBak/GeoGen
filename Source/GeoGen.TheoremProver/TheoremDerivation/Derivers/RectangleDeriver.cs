using GeoGen.Core;
using GeoGen.Utilities;
using System.Collections.Generic;
using System.Linq;
using static GeoGen.Core.TheoremType;

namespace GeoGen.TheoremProver
{
    /// <summary>
    /// Represents <see cref="ITheoremDeriver"/> that derives theorems using the <see cref="DerivationRule.Rectangle"/>.
    /// </summary>
    public class RectangleDeriver : TheoremDeriverBase
    {
        /// <summary>
        /// Takes new theorems and based on logical reason comes up with relationships between them,
        /// i.e. which theorem would be sufficient to prove to come up with some other one of them.
        /// </summary>
        /// <param name="theorems">The theorems between which we're finding relationships.</param>
        /// <returns>The enumerable of all found relationships, i.e. assumptions and the theorem that follows from them.</returns>
        public override IEnumerable<(IReadOnlyList<Theorem> assumptions, Theorem impliedTheorem)> DeriveTheorems(TheoremMap theorems)
        {
            // Go through the pairs of parallelities
            foreach (var parallelities in theorems.GetObjectsForKeys(ParallelLines).Subsets(2))
            {
                // Get the lines
                var lines = parallelities.Select(theorem => theorem.InvolvedObjects.Cast<LineTheoremObject>().ToArray()).ToArray();

                // Take them only if they all are defined by points
                if (lines.Flatten().Any(line => !line.DefinedByPoints))
                    continue;

                // Continue only if they have 4 points altogether
                if (lines.Flatten().SelectMany(line => line.PointsList).Distinct().Count() != 4)
                    continue;

                // We'll get the points A, B, C, D in such an order that AB || CD and BC || AD
                // Initially try this
                var A = lines[0][0].PointsList[0];
                var B = lines[0][0].PointsList[1];
                var C = lines[0][1].PointsList[0];
                var D = lines[0][1].PointsList[1];

                // If we didn't do it right, we need to exchange C and D and it should be fine
                if (!lines[1][0].Equals(new LineTheoremObject(B, C)) && !lines[1][1].Equals(new LineTheoremObject(B, C)))
                    GeneralUtilities.Swap(ref C, ref D);

                // Prepare the potential perpendicular lines theorems that would mean this is a rectangle
                var perpendicularity1 = new Theorem(PerpendicularLines, new[]
                {
                    new LineTheoremObject(A, B),
                    new LineTheoremObject(A, D)
                });
                var perpendicularity2 = new Theorem(PerpendicularLines, new[]
                {
                    new LineTheoremObject(B, A),
                    new LineTheoremObject(B, C)
                });
                var perpendicularity3 = new Theorem(PerpendicularLines, new[]
                {
                    new LineTheoremObject(C, B),
                    new LineTheoremObject(C, D)
                });
                var perpendicularity4 = new Theorem(PerpendicularLines, new[]
                {
                    new LineTheoremObject(D, A),
                    new LineTheoremObject(D, C)
                });

                // We won't continue if it is not there
                if (!theorems.ContainsTheorem(perpendicularity1))
                    continue;

                // Get the equal diagonals theorem
                var equalDiagonals = new Theorem(EqualLineSegments, new[]
                {
                    new LineSegmentTheoremObject(A, C),
                    new LineSegmentTheoremObject(B, D)
                });

                // Otherwise our parallel lines with any perpendicularity imply the equal diagonals
                yield return (new[] { parallelities[0], parallelities[1], perpendicularity1 }, equalDiagonals);
                yield return (new[] { parallelities[0], parallelities[1], perpendicularity2 }, equalDiagonals);
                yield return (new[] { parallelities[0], parallelities[1], perpendicularity3 }, equalDiagonals);
                yield return (new[] { parallelities[0], parallelities[1], perpendicularity4 }, equalDiagonals);

                // NOTE: We can imply even equal line segments of the opposite sides, but this is handled by Parallelogram
            }
        }
    }
}
