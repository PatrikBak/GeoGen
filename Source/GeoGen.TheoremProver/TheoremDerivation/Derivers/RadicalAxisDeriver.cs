using GeoGen.Core;
using GeoGen.Utilities;
using System.Collections.Generic;
using System.Linq;
using static GeoGen.Core.TheoremType;

namespace GeoGen.TheoremProver
{
    /// <summary>
    /// Represents <see cref="ITheoremDeriver"/> that derives theorems using the <see cref="DerivationRule.RadicalAxis"/>.
    /// </summary>
    public class RadicalAxisDeriver : TheoremDeriverBase
    {
        /// <summary>
        /// Takes new theorems and based on logical reason comes up with relationships between them,
        /// i.e. which theorem would be sufficient to prove to come up with some other one of them.
        /// </summary>
        /// <param name="theorems">The theorems between which we're finding relationships.</param>
        /// <returns>The enumerable of all found relationships, i.e. assumptions and the theorem that follows from them.</returns>
        public override IEnumerable<(IReadOnlyList<Theorem> assumptions, Theorem impliedTheorem)> DeriveTheorems(TheoremMap theorems)
        {
            // Go through the triples of concyclities
            foreach (var concyclities in theorems.GetObjectsForKeys(ConcyclicPoints).Subsets(3))
            {
                // Get the concyclic points of each concyclity
                var pointQuadruples = concyclities.Select(concyclity => concyclity.InvolvedObjects
                        // Each involved object is a point
                        .Select(theoremObject => ((PointTheoremObject)theoremObject).ConfigurationObject)
                        // Enumerate
                        .ToArray())
                    // Enumerate the whole thing
                    .ToArray();

                // Get all points
                var allPoints = pointQuadruples.Flatten().Distinct().ToArray();

                // If there are not 6 points altogether, then this is not a correct triple
                if (allPoints.Length != 6)
                    continue;

                // Get the common points of pairs of this
                var commonPointsOfPairs = pointQuadruples.Subsets(2)
                    // Intersect points in every subset
                    .Select(pair => pair[0].Intersect(pair[1]).ToArray())
                    // Enumerate
                    .ToArray();

                // If not every two have exactly 2 common points, then this is not a correct triple
                if (commonPointsOfPairs.Any(points => points.Length != 2))
                    continue;

                // It also cannot happen that all of them lie on a circle. Not even five of them
                // could. Create the two concyclic theorems to detect this
                var testConcyclity1 = new Theorem(ConcyclicPoints, allPoints[0], allPoints[1], allPoints[2], allPoints[3]);
                var testConcyclity2 = new Theorem(ConcyclicPoints, allPoints[0], allPoints[1], allPoints[2], allPoints[4]);

                // If both of them are there, then all of them are concyclic
                if (theorems.ContainsTheorem(testConcyclity1) && theorems.ContainsTheorem(testConcyclity2))
                    continue;

                // Otherwise we're sure we have three distinct circles, each two of them intersecting
                // at two different points. We can their radical axes 
                var line1 = new LineTheoremObject(commonPointsOfPairs[0][0], commonPointsOfPairs[0][1]);
                var line2 = new LineTheoremObject(commonPointsOfPairs[1][0], commonPointsOfPairs[1][1]);
                var line3 = new LineTheoremObject(commonPointsOfPairs[2][0], commonPointsOfPairs[2][1]);

                // Create the concurrency theorem
                var concurrency = new Theorem(ConcurrentLines, new[] { line1, line2, line3 });

                // It might still happen that this is not a correct theorem
                // because those lines might be parallel. So we'd rather check the theorem is there
                if (!theorems.ContainsTheorem(concurrency))
                    continue;

                // But if it's okay, then any three of our theorems can imply the fourth
                yield return (new[] { concyclities[0], concyclities[1], concyclities[2] }, concurrency);
                yield return (new[] { concurrency, concyclities[1], concyclities[2] }, concyclities[0]);
                yield return (new[] { concurrency, concyclities[0], concyclities[2] }, concyclities[1]);
                yield return (new[] { concurrency, concyclities[0], concyclities[1] }, concyclities[2]);
            }
        }
    }
}
