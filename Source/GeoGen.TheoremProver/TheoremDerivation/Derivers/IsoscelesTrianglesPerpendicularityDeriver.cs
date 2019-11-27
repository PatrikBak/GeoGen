using GeoGen.Core;
using GeoGen.Utilities;
using System.Collections.Generic;
using System.Linq;
using static GeoGen.Core.TheoremType;

namespace GeoGen.TheoremProver
{
    /// <summary>
    /// Represents <see cref="ITheoremDeriver"/> that derives theorems using the <see cref="DerivationRule.IsoscelesTrianglesPerpendicularity"/>.
    /// </summary>
    public class IsoscelesTrianglesPerpendicularityDeriver : TheoremDeriverBase
    {
        /// <summary>
        /// Takes new theorems and based on logical reason comes up with relationships between them,
        /// i.e. which theorem would be sufficient to prove to come up with some other one of them.
        /// </summary>
        /// <param name="theorems">The theorems between which we're finding relationships.</param>
        /// <returns>The enumerable of all found relationships, i.e. assumptions and the theorem that follows from them.</returns>
        public override IEnumerable<(IReadOnlyList<Theorem> assumptions, Theorem impliedTheorem)> DeriveTheorems(TheoremMap theorems)
        {
            // Go through the pair of equal line segments theorems
            foreach (var equalLineSegmentTheorems in theorems.GetTheoremsOfTypes(EqualLineSegments).Subsets(2))
            {
                // Get the common point for both the theorems
                var commonPoints = equalLineSegmentTheorems.Select(theorem =>
                        // For a given theorem take the points of the first line segment object
                        theorem.InvolvedObjectsList[0].GetInnerConfigurationObjects()
                        // Intersect with the points of the second line segment 
                        .Intersect(theorem.InvolvedObjectsList[1].GetInnerConfigurationObjects())
                        // There should be at most one
                        .FirstOrDefault())
                    // Enumerate the whole thing
                    .ToArray();

                // Make sure there is a common point for each pair
                if (!commonPoints.All(point => point != null))
                    continue;

                // Get the base points that are equally distanced from the common point
                var basePoints = equalLineSegmentTheorems[0].GetInnerConfigurationObjects().Except(commonPoints).ToArray();

                // Make sure there are exactly two 
                if (!equalLineSegmentTheorems[1].GetInnerConfigurationObjects().Except(commonPoints).OrderlessEquals(basePoints))
                    continue;

                // Now we're sure the situation is okay
                // Prepare the perpendicular lines theorem
                var perpendicularity = new Theorem(PerpendicularLines, new[]
                {
                    new LineTheoremObject(basePoints[0], basePoints[1]),
                    new LineTheoremObject(commonPoints[0], commonPoints[1])
                });

                // Any two of our theorem imply the third
                yield return (new[] { equalLineSegmentTheorems[0], equalLineSegmentTheorems[1] }, perpendicularity);
                yield return (new[] { equalLineSegmentTheorems[0], perpendicularity }, equalLineSegmentTheorems[1]);
                yield return (new[] { equalLineSegmentTheorems[1], perpendicularity }, equalLineSegmentTheorems[0]);
            }
        }
    }
}
