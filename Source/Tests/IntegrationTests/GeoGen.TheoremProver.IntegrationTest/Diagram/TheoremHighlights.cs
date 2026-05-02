#nullable enable
using GeoGen.Core;

namespace GeoGen.TheoremProver.IntegrationTest.Diagram
{
    /// <summary>
    /// Computes the set of diagram object ids to highlight when a given theorem is selected.
    /// <para>
    /// The minimal rule is: highlight every <see cref="ConfigurationObject"/> the theorem mentions.
    /// For point-defined lines we additionally highlight the defining points so the segment reads
    /// visually as a connection between specific labeled points, not just an abstract line.
    /// </para>
    /// </summary>
    public static class TheoremHighlights
    {
        /// <summary>
        /// Object ids the diagram should light up for <paramref name="theorem"/>. Returns an empty
        /// list if the theorem references no diagrammable objects (which would be a bug elsewhere).
        /// </summary>
        public static IReadOnlyList<string> ObjectIdsFor(Theorem theorem)
        {
            var ids = new List<string>();

            foreach (var theoremObject in theorem.InvolvedObjects)
                CollectIds(theoremObject, ids);

            // ConcyclicPoints synthesizes a circle through its 4 points; light it up too.
            if (theorem.Type == TheoremType.ConcyclicPoints)
            {
                var pts = theorem.InvolvedObjects.OfType<PointTheoremObject>()
                    .Select(p => p.ConfigurationObject)
                    .ToArray();
                if (pts.Length >= 3)
                    ids.Add(DiagramBuilder.CircleThroughPointsId(pts));
            }

            // Deduplicate while preserving order — gives stable hover/click highlights.
            return ids.Distinct().ToList();
        }

        private static void CollectIds(TheoremObject theoremObject, List<string> ids)
        {
            switch (theoremObject)
            {
                case PointTheoremObject pto:
                    ids.Add(DiagramBuilder.ObjectId(pto.ConfigurationObject));
                    break;

                case LineTheoremObject lto:
                    AddLineWithDefiningPoints(lto, ids);
                    break;

                case CircleTheoremObject cto:
                    AddCircleWithDefiningPoints(cto, ids);
                    break;

                case LineSegmentTheoremObject seg:
                    // A segment is two PointTheoremObjects. Highlight both endpoints AND the segment
                    // between them — the synthesized DiagramSegment carries that id.
                    var endpoints = seg.PointSet.OfType<PointTheoremObject>().Select(p => p.ConfigurationObject).ToArray();
                    foreach (var pto in endpoints)
                        ids.Add(DiagramBuilder.ObjectId(pto));
                    if (endpoints.Length == 2)
                        ids.Add(DiagramBuilder.SegmentId(endpoints[0], endpoints[1]));
                    break;
            }
        }

        private static void AddLineWithDefiningPoints(LineTheoremObject lto, List<string> ids)
        {
            // If the line is defined by an explicit configuration object, add it.
            if (lto.DefinedByExplicitObject)
                ids.Add(DiagramBuilder.ObjectId(lto.ConfigurationObject));

            // If it's defined by points (or even when it's explicit AND we know the defining points),
            // highlight those points and the segment between them so the line reads visually.
            if (lto.PointsList is not null && lto.PointsList.Count == 2)
            {
                ids.Add(DiagramBuilder.ObjectId(lto.PointsList[0]));
                ids.Add(DiagramBuilder.ObjectId(lto.PointsList[1]));
                ids.Add(DiagramBuilder.SegmentId(lto.PointsList[0], lto.PointsList[1]));
            }
            else if (lto.Points is not null)
            {
                foreach (var point in lto.Points)
                    ids.Add(DiagramBuilder.ObjectId(point));
            }
        }

        private static void AddCircleWithDefiningPoints(CircleTheoremObject cto, List<string> ids)
        {
            if (cto.DefinedByExplicitObject)
                ids.Add(DiagramBuilder.ObjectId(cto.ConfigurationObject));

            if (cto.PointsList is not null && cto.PointsList.Count >= 3)
            {
                foreach (var point in cto.PointsList)
                    ids.Add(DiagramBuilder.ObjectId(point));
                ids.Add(DiagramBuilder.CircleThroughPointsId(cto.PointsList));
            }
            else if (cto.Points is not null)
            {
                foreach (var point in cto.Points)
                    ids.Add(DiagramBuilder.ObjectId(point));
            }
        }
    }
}
