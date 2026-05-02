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
            var ids = new List<string>();

            switch (theorem.Type)
            {
                case TheoremType.ParallelLines:
                case TheoremType.PerpendicularLines:
                case TheoremType.ConcurrentLines:
                case TheoremType.LineTangentToCircle:
                    // Highlight just the lines (and circles, for LineTangentToCircle). The
                    // theorem is a statement about the lines themselves, not their defining points.
                    foreach (var to in theorem.InvolvedObjects)
                        AddLineOrCircleOnly(to, ids);
                    break;

                case TheoremType.TangentCircles:
                    // Two circles touching. Just the circles.
                    foreach (var to in theorem.InvolvedObjects)
                        AddLineOrCircleOnly(to, ids);
                    break;

                case TheoremType.ConcyclicPoints:
                    // The points participating in the relationship plus the circle through them.
                    foreach (var to in theorem.InvolvedObjects)
                        if (to is PointTheoremObject pto)
                            ids.Add(DiagramBuilder.ObjectId(pto.ConfigurationObject));
                    var concyclic = theorem.InvolvedObjects.OfType<PointTheoremObject>()
                        .Select(p => p.ConfigurationObject)
                        .ToArray();
                    if (concyclic.Length >= 3)
                        ids.Add(DiagramBuilder.CircleThroughPointsId(concyclic));
                    break;

                case TheoremType.CollinearPoints:
                    // Just the points. No synthetic line — collinearity is a property of the
                    // points, not of any particular line object.
                    foreach (var to in theorem.InvolvedObjects)
                        if (to is PointTheoremObject pto)
                            ids.Add(DiagramBuilder.ObjectId(pto.ConfigurationObject));
                    break;

                case TheoremType.Incidence:
                    // Point on line/circle. Both must be visible to read "where they meet".
                    foreach (var to in theorem.InvolvedObjects)
                    {
                        if (to is PointTheoremObject pto)
                            ids.Add(DiagramBuilder.ObjectId(pto.ConfigurationObject));
                        else
                            AddLineOrCircleOnly(to, ids);
                    }
                    break;

                case TheoremType.EqualLineSegments:
                    // Segments are visually thin; we light their 4 endpoints too so the user can
                    // read which segments are claimed equal even at small scale.
                    foreach (var to in theorem.InvolvedObjects)
                    {
                        if (to is LineSegmentTheoremObject seg)
                        {
                            var endpoints = seg.PointSet.OfType<PointTheoremObject>()
                                .Select(p => p.ConfigurationObject)
                                .ToArray();
                            foreach (var pto in endpoints)
                                ids.Add(DiagramBuilder.ObjectId(pto));
                            if (endpoints.Length == 2)
                                ids.Add(DiagramBuilder.SegmentId(endpoints[0], endpoints[1]));
                        }
                    }
                    break;

                case TheoremType.EqualObjects:
                default:
                    // Fall-through: highlight whatever the theorem references directly. EqualObjects
                    // can be points, lines, or circles; light the matching primary object for each.
                    foreach (var to in theorem.InvolvedObjects)
                    {
                        if (to is PointTheoremObject pto)
                            ids.Add(DiagramBuilder.ObjectId(pto.ConfigurationObject));
                        else
                            AddLineOrCircleOnly(to, ids);
                    }
                    break;
            }

            return ids.Distinct().ToList();
        }

        /// <summary>
        /// Add the id of just the primary line/circle object — explicit ConfigurationObject
        /// when present (named line/circle), or the canonical segment/circle-through-points id
        /// when the theorem refers to it via points.
        /// </summary>
        private static void AddLineOrCircleOnly(TheoremObject theoremObject, List<string> ids)
        {
            switch (theoremObject)
            {
                case LineTheoremObject lto:
                    if (lto.DefinedByExplicitObject)
                    {
                        ids.Add(DiagramBuilder.ObjectId(lto.ConfigurationObject));
                    }
                    else if (lto.PointsList is not null && lto.PointsList.Count == 2)
                    {
                        // Point-defined line — its visual representation is the segment between
                        // those two points. Use the canonical seg-id so it matches the diagram.
                        ids.Add(DiagramBuilder.SegmentId(lto.PointsList[0], lto.PointsList[1]));
                    }
                    break;

                case CircleTheoremObject cto:
                    if (cto.DefinedByExplicitObject)
                    {
                        ids.Add(DiagramBuilder.ObjectId(cto.ConfigurationObject));
                    }
                    else if (cto.PointsList is not null && cto.PointsList.Count >= 3)
                    {
                        ids.Add(DiagramBuilder.CircleThroughPointsId(cto.PointsList));
                    }
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
