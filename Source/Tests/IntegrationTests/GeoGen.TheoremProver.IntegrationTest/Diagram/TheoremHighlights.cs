#nullable enable
using GeoGen.Core;

namespace GeoGen.TheoremProver.IntegrationTest.Diagram
{
    /// <summary>
    /// Computes the set of diagram object ids to highlight when a given theorem is selected.
    /// <para>
    /// Each theorem type maps to a minimal "what's the theorem about" set, so hovering a row
    /// lights up only the primary geometric objects — not also their defining points.
    /// Examples:
    /// <list type="bullet">
    /// <item><c>ConcurrentLines: [A,D], [B,C], [F,G]</c> → only the 3 line segments. The points
    /// A, D, B, C, F, G stay unlit because they're construction details, not what the theorem
    /// claims.</item>
    /// <item><c>CollinearPoints: A, B, G</c> → the 3 points (the theorem is *about* the points
    /// being on a line, not about a particular line object).</item>
    /// <item><c>EqualLineSegments: A, F, A, G</c> → the 2 segments AND their 4 endpoints —
    /// segments without endpoints are visually ambiguous, so for this case we keep both.</item>
    /// </list>
    /// Earlier versions added defining points alongside every line/circle, which lit up 9+
    /// objects on a single ConcurrentLines hover; that drowned out the theorem's actual subject.
    /// </para>
    /// </summary>
    public static class TheoremHighlights
    {
        public static IReadOnlyList<string> ObjectIdsFor(Theorem theorem)
        {
            var ids = new HashSet<string>();
            switch (theorem.Type)
            {
                // Lines / circles only — these theorems claim something about the lines or circles
                // themselves; their defining points aren't the subject.
                case TheoremType.ParallelLines:
                case TheoremType.PerpendicularLines:
                case TheoremType.ConcurrentLines:
                case TheoremType.LineTangentToCircle:
                case TheoremType.TangentCircles:
                    foreach (var to in theorem.InvolvedObjects)
                        AddLineOrCircleOnly(to, ids);
                    break;

                // Points only — the claim is about a property of the points themselves; no
                // synthesized line object should light up.
                case TheoremType.CollinearPoints:
                    foreach (var pto in theorem.InvolvedObjects.OfType<PointTheoremObject>())
                        ids.Add(DiagramBuilder.ObjectId(pto.ConfigurationObject));
                    break;

                // Points plus the circle through them.
                case TheoremType.ConcyclicPoints:
                    var concyclic = new List<ConfigurationObject>();
                    foreach (var pto in theorem.InvolvedObjects.OfType<PointTheoremObject>())
                    {
                        ids.Add(DiagramBuilder.ObjectId(pto.ConfigurationObject));
                        concyclic.Add(pto.ConfigurationObject);
                    }
                    if (concyclic.Count >= 3)
                        ids.Add(DiagramBuilder.CircleThroughPointsId(concyclic));
                    break;

                // Segments are visually thin; light their endpoints too so the user can read which
                // segments are claimed equal even at small scale.
                case TheoremType.EqualLineSegments:
                    foreach (var seg in theorem.InvolvedObjects.OfType<LineSegmentTheoremObject>())
                    {
                        var endpoints = seg.PointSet.OfType<PointTheoremObject>()
                            .Select(p => p.ConfigurationObject)
                            .ToArray();
                        foreach (var pto in endpoints)
                            ids.Add(DiagramBuilder.ObjectId(pto));
                        if (endpoints.Length == 2)
                            ids.Add(DiagramBuilder.SegmentId(endpoints[0], endpoints[1]));
                    }
                    break;

                // Mixed: highlight whatever each involved object is. Covers Incidence, EqualObjects,
                // and any theorem type added later — a sensible default.
                default:
                    foreach (var to in theorem.InvolvedObjects)
                    {
                        if (to is PointTheoremObject pto)
                            ids.Add(DiagramBuilder.ObjectId(pto.ConfigurationObject));
                        else
                            AddLineOrCircleOnly(to, ids);
                    }
                    break;
            }
            return ids.ToArray();
        }

        /// <summary>
        /// Add the id of just the primary line/circle object — explicit ConfigurationObject
        /// when present (named line/circle), or the canonical segment/circle-through-points id
        /// when the theorem refers to it via points.
        /// </summary>
        private static void AddLineOrCircleOnly(TheoremObject theoremObject, HashSet<string> ids)
        {
            switch (theoremObject)
            {
                case LineTheoremObject lto when lto.DefinedByExplicitObject:
                    ids.Add(DiagramBuilder.ObjectId(lto.ConfigurationObject));
                    break;
                // Point-defined line — its visual representation is the segment between those two
                // points. Use the canonical seg-id so it matches the diagram.
                case LineTheoremObject lto when lto.PointsList?.Count == 2:
                    ids.Add(DiagramBuilder.SegmentId(lto.PointsList[0], lto.PointsList[1]));
                    break;

                case CircleTheoremObject cto when cto.DefinedByExplicitObject:
                    ids.Add(DiagramBuilder.ObjectId(cto.ConfigurationObject));
                    break;
                case CircleTheoremObject cto when cto.PointsList?.Count >= 3:
                    ids.Add(DiagramBuilder.CircleThroughPointsId(cto.PointsList));
                    break;

                case LineSegmentTheoremObject seg:
                    var endpoints = seg.PointSet.OfType<PointTheoremObject>()
                        .Select(p => p.ConfigurationObject)
                        .ToArray();
                    if (endpoints.Length == 2)
                        ids.Add(DiagramBuilder.SegmentId(endpoints[0], endpoints[1]));
                    break;
            }
        }
    }
}
