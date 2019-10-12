using GeoGen.Core;
using GeoGen.Utilities;
using System.Collections.Generic;
using System.Linq;
using static GeoGen.Core.TheoremType;

namespace GeoGen.TheoremProver
{
    /// <summary>
    /// Represents <see cref="ITheoremDeriver"/> that derives theorems using the <see cref="DerivationRule.ConcyclicPointsWithExplicitCenter"/>.
    /// </summary>
    public class ConcyclicPointsWithExplicitCenterDeriver : TheoremDeriverBase
    {
        /// <summary>
        /// Takes new theorems and based on logical reason comes up with relationships between them,
        /// i.e. which theorem would be sufficient to prove to come up with some other one of them.
        /// </summary>
        /// <param name="theorems">The theorems between which we're finding relationships.</param>
        /// <returns>The enumerable of all found relationships, i.e. assumptions and the theorem that follows from them.</returns>
        public override IEnumerable<(IReadOnlyList<Theorem> assumptions, Theorem impliedTheorem)> DeriveTheorems(TheoremMap theorems)
        {
            // Find the points that might be potential centers
            // From the equal line segments theorems
            var relevantObjects = theorems.GetTheoremsOfTypes(EqualLineSegments)
                // Take the inner objects
                .SelectMany(theorem => theorem.GetInnerConfigurationObjects())
                // Only distinct ones
                .Distinct()
                // Enumerate
                .ToArray();

            // Go through the concyclities
            foreach (var concyclity in theorems.GetTheoremsOfTypes(ConcyclicPoints))
            {
                // Get the concyclic points
                var concyclicPoints = concyclity.InvolvedObjects
                    // Each involved object is a point
                    .Select(theoremObject => ((PointTheoremObject)theoremObject).ConfigurationObject)
                    // Enumerate
                    .ToArray();

                // Get the potential objects that might be a center
                foreach (var center in relevantObjects.Except(concyclicPoints))
                {
                    // Create the theorems to detect if this is a center
                    var theorem1 = new Theorem(EqualLineSegments, new[]
                    {
                        new LineSegmentTheoremObject(center, concyclicPoints[0]),
                        new LineSegmentTheoremObject(center, concyclicPoints[1])
                    });
                    var theorem2 = new Theorem(EqualLineSegments, new[]
                    {
                        new LineSegmentTheoremObject(center, concyclicPoints[0]),
                        new LineSegmentTheoremObject(center, concyclicPoints[2])
                    });

                    // It center only if both these theorems are there
                    if (!theorems.ContainsTheorem(theorem1) || !theorems.ContainsTheorem(theorem2))
                        continue;

                    // Create the other theorems
                    var theorem3 = new Theorem(EqualLineSegments, new[]
                    {
                        new LineSegmentTheoremObject(center, concyclicPoints[0]),
                        new LineSegmentTheoremObject(center, concyclicPoints[3])
                    });
                    var theorem4 = new Theorem(EqualLineSegments, new[]
                    {
                        new LineSegmentTheoremObject(center, concyclicPoints[1]),
                        new LineSegmentTheoremObject(center, concyclicPoints[2])
                    });
                    var theorem5 = new Theorem(EqualLineSegments, new[]
                    {
                        new LineSegmentTheoremObject(center, concyclicPoints[1]),
                        new LineSegmentTheoremObject(center, concyclicPoints[3])
                    });
                    var theorem6 = new Theorem(EqualLineSegments, new[]
                    {
                        new LineSegmentTheoremObject(center, concyclicPoints[2]),
                        new LineSegmentTheoremObject(center, concyclicPoints[3])
                    });

                    // Get them all in one array
                    var allTheorems = new[] { theorem1, theorem2, theorem3, theorem4, theorem5, theorem6 };

                    // Go through their triples
                    foreach (var triple in allTheorems.Subsets(3))
                    {
                        // Get the involved points
                        var involvedPoints = triple.SelectMany(theorem => theorem.GetInnerConfigurationObjects()).Distinct().ToArray();

                        // Exclude those that don't cover 5 objects (center + 4 concyclic points)
                        if (involvedPoints.Length != 5)
                            continue;

                        // This triple implies the concyclity
                        yield return (triple, concyclity);
                    }

                    // Go through their pairs
                    foreach (var pair in allTheorems.Subsets(2))
                    {
                        // Get the involved points
                        var involvedPoints = pair.SelectMany(theorem => theorem.GetInnerConfigurationObjects()).Distinct().ToArray();

                        // Exclude those that don't cover 4 objects (center + some 3 concyclic points)
                        if (involvedPoints.Length != 4)
                            continue;

                        // Otherwise this pair implies 'center' is the actual center
                        // Get the equalities
                        var equality1 = pair[0];
                        var equality2 = pair[1];

                        // Get the last point
                        var lastPoint = concyclicPoints.Except(involvedPoints).First();

                        // Go through the involved points distinct from the center
                        foreach (var point in involvedPoints.Except(center.ToEnumerable()))
                        {
                            // Create the equality 
                            var equality = new Theorem(EqualLineSegments, new[]
                            {
                                new LineSegmentTheoremObject(center, lastPoint),
                                new LineSegmentTheoremObject(center, point)
                            });

                            // Our two equalities and the concyclity imply this equality
                            yield return (new[] { concyclity, equality1, equality2 }, equality);
                        }
                    }
                }
            }
        }
    }
}
