#nullable enable
using System.Globalization;
using GeoGen.AnalyticGeometry;
using GeoGen.Constructor;
using GeoGen.Core;

namespace GeoGen.TheoremProver.IntegrationTest.Diagram
{
    /// <summary>
    /// Constructs a <see cref="DiagramModel"/> from a <see cref="Picture"/>: extracts coordinates,
    /// computes the world-space bounding box, and clips unbounded lines to that box.
    /// <para>
    /// Object ids are derived from the GeoGen <see cref="OutputFormatter"/> object names so they
    /// match the labels users see in the report's text sections — joining "highlight A and B" to
    /// the diagram is then a string lookup.
    /// </para>
    /// </summary>
    public static class DiagramBuilder
    {
        /// <summary>
        /// Margin around the visible content, expressed as a fraction of the bounding-box diagonal.
        /// Picked so the diagram has a small breathing area on each side without leaving the
        /// geometry stranded in a sea of white space — a common bug with fixed-size margins when
        /// configurations vary in scale.
        /// </summary>
        private const double DefaultMarginFraction = 0.06;

        /// <summary>
        /// Floor on the absolute margin in world units. Prevents a degenerate single-point or
        /// near-collinear configuration from getting essentially no margin at all.
        /// </summary>
        private const double MinAbsoluteMargin = 0.05;

        public static DiagramModel Build(Picture picture, OutputFormatter formatter, Configuration configuration, IEnumerable<Theorem> theorems)
        {
            var points = new List<DiagramPoint>();
            var rawLines = new List<(string Id, string Label, Line Equation)>();
            var circles = new List<DiagramCircle>();

            // Pass 1: extract every object the picture knows about.
            foreach (var (configurationObject, analyticObject) in picture)
            {
                var id = ObjectId(configurationObject);
                var label = formatter.GetObjectName(configurationObject);

                switch (analyticObject)
                {
                    case Point p:
                        points.Add(new DiagramPoint(id, label, p.X, p.Y));
                        break;
                    case Line l:
                        rawLines.Add((id, label, l));
                        break;
                    case Circle c:
                        circles.Add(new DiagramCircle(id, label, c.Center.X, c.Center.Y, c.Radius));
                        break;
                }
            }

            // Pass 2: synthesize segments and circles for theorems and layout. The picture only stores
            // explicitly-constructed analytic objects, but theorems frequently reference implicit lines
            // (point pairs) and circles (concyclic point sets). These would otherwise vanish from the
            // diagram, leaving just isolated points.
            var pointById = points.ToDictionary(p => p.Id);
            var segments = new List<DiagramSegment>();
            var seenSegmentKeys = new Dictionary<string, int>(); // key -> index in segments
            SynthesizeFromLayout(configuration, formatter, picture, pointById, segments, seenSegmentKeys);
            SynthesizeFromTheorems(theorems, formatter, picture, pointById, circles, segments, seenSegmentKeys);

            // Pass 3: bounding box over points + circle perimeters + segment endpoints. Lines (full
            // analytic) are infinite and contribute nothing — they get clipped to whatever box the
            // finite content produces.
            var rawBounds = ComputeBounds(points, circles, segments);
            var diagonal = Math.Sqrt(rawBounds.Width * rawBounds.Width + rawBounds.Height * rawBounds.Height);
            var margin = Math.Max(MinAbsoluteMargin, diagonal * DefaultMarginFraction);
            var bounds = rawBounds.Expand(margin);

            // Pass 4: clip each line to the bounding box.
            var lines = new List<DiagramLine>(rawLines.Count);
            foreach (var (id, label, equation) in rawLines)
            {
                if (TryClipLineToBounds(equation, bounds, out var x1, out var y1, out var x2, out var y2))
                    lines.Add(new DiagramLine(id, label, x1, y1, x2, y2));
            }

            return new DiagramModel(points, lines, circles, segments, bounds);
        }

        /// <summary>
        /// Add the segments implied by the configuration's loose-object layout — most importantly
        /// the three sides of a Triangle layout, which are universal but never appear in the picture.
        /// </summary>
        private static void SynthesizeFromLayout(
            Configuration configuration,
            OutputFormatter formatter,
            Picture picture,
            Dictionary<string, DiagramPoint> pointById,
            List<DiagramSegment> segments,
            Dictionary<string, int> seenKeys)
        {
            // The loose-object set varies by layout. For Triangle (the only layout used in the
            // integration tests), it's three points; we want all three sides.
            var loosePoints = configuration.LooseObjects
                .Where(o => o.ObjectType == ConfigurationObjectType.Point)
                .ToArray();

            if (loosePoints.Length < 2)
                return;

            // For up to ~6 loose points, all-pairs is cheap. For larger sets it would explode, but
            // GeoGen layouts cap out at small fixed sizes.
            for (var i = 0; i < loosePoints.Length; i++)
                for (var j = i + 1; j < loosePoints.Length; j++)
                    AddSegmentBetweenPoints(loosePoints[i], loosePoints[j], formatter, picture, pointById, segments, seenKeys);
        }

        /// <summary>
        /// For each theorem, add a segment for every point-defined LineTheoremObject, and a circle
        /// for every point-defined CircleTheoremObject (or for the 4-point set inside a ConcyclicPoints
        /// theorem). The id of each synthesized object is canonicalized so multiple theorems referring
        /// to the same line or circle share one rendered element.
        /// </summary>
        private static void SynthesizeFromTheorems(
            IEnumerable<Theorem> theorems,
            OutputFormatter formatter,
            Picture picture,
            Dictionary<string, DiagramPoint> pointById,
            List<DiagramCircle> circles,
            List<DiagramSegment> segments,
            Dictionary<string, int> seenKeys)
        {
            // Track which canonical circle ids we've already added so duplicates collapse.
            var seenCircleIds = new HashSet<string>(circles.Select(c => c.Id));

            foreach (var theorem in theorems)
            {
                foreach (var theoremObject in theorem.InvolvedObjects)
                {
                    switch (theoremObject)
                    {
                        case LineTheoremObject lto when lto.Points is not null && lto.PointsList.Count == 2:
                            AddSegmentBetweenPoints(lto.PointsList[0], lto.PointsList[1], formatter, picture, pointById, segments, seenKeys);
                            // If the theorem-object is also tied to an explicit configuration object,
                            // record the explicit id as an alias so highlighting "the explicit line"
                            // also lights this synthesized segment.
                            if (lto.DefinedByExplicitObject)
                                AliasSegment(seenKeys, segments, lto.PointsList[0], lto.PointsList[1], ObjectId(lto.ConfigurationObject));
                            break;

                        case CircleTheoremObject cto when cto.Points is not null && cto.PointsList.Count == 3:
                            TryAddCircleThroughPoints(cto.PointsList, formatter, picture, circles, seenCircleIds);
                            break;

                        case LineSegmentTheoremObject seg:
                            // A segment theorem object wraps two PointTheoremObjects. Draw the segment.
                            var endpoints = seg.PointSet.OfType<PointTheoremObject>().Select(p => p.ConfigurationObject).ToArray();
                            if (endpoints.Length == 2)
                                AddSegmentBetweenPoints(endpoints[0], endpoints[1], formatter, picture, pointById, segments, seenKeys);
                            break;
                    }
                }

                // ConcyclicPoints: the four involved point-objects all lie on one circle. Synthesize it.
                if (theorem.Type == TheoremType.ConcyclicPoints)
                {
                    var cycPoints = theorem.InvolvedObjects.OfType<PointTheoremObject>()
                        .Select(p => p.ConfigurationObject)
                        .ToArray();
                    if (cycPoints.Length >= 3)
                        TryAddCircleThroughPoints(cycPoints, formatter, picture, circles, seenCircleIds);
                }
            }
        }

        private static void AddSegmentBetweenPoints(
            ConfigurationObject p1,
            ConfigurationObject p2,
            OutputFormatter formatter,
            Picture picture,
            Dictionary<string, DiagramPoint> pointById,
            List<DiagramSegment> segments,
            Dictionary<string, int> seenKeys)
        {
            var key = SegmentId(p1, p2);
            if (seenKeys.ContainsKey(key))
                return;

            // Need both endpoints' coordinates from the picture.
            if (!pointById.TryGetValue(ObjectId(p1), out var dp1) || !pointById.TryGetValue(ObjectId(p2), out var dp2))
                return;

            var label = $"[{formatter.GetObjectName(p1)}, {formatter.GetObjectName(p2)}]";
            segments.Add(new DiagramSegment(new[] { key }, label, dp1.X, dp1.Y, dp2.X, dp2.Y));
            seenKeys[key] = segments.Count - 1;
        }

        /// <summary>
        /// Add an alias id to an already-added segment so that <c>data-highlight</c> matching by
        /// either id lights up the same segment.
        /// </summary>
        private static void AliasSegment(
            Dictionary<string, int> seenKeys,
            List<DiagramSegment> segments,
            ConfigurationObject p1,
            ConfigurationObject p2,
            string alias)
        {
            var key = SegmentId(p1, p2);
            if (!seenKeys.TryGetValue(key, out var idx))
                return;
            var existing = segments[idx];
            if (existing.Ids.Contains(alias))
                return;
            segments[idx] = existing with { Ids = existing.Ids.Concat(new[] { alias }).ToList() };
        }

        private static void TryAddCircleThroughPoints(
            IReadOnlyList<ConfigurationObject> pts,
            OutputFormatter formatter,
            Picture picture,
            List<DiagramCircle> circles,
            HashSet<string> seenCircleIds)
        {
            var id = CircleThroughPointsId(pts);
            if (!seenCircleIds.Add(id))
                return;

            // Need analytic coordinates for at least 3 points to construct a circle.
            var coords = new List<Point>();
            foreach (var p in pts)
            {
                if (picture.Get(p) is Point pt)
                    coords.Add(pt);
            }
            if (coords.Count < 3)
                return;

            try
            {
                var circle = new Circle(coords[0], coords[1], coords[2]);
                var label = string.Join(", ", pts.Select(formatter.GetObjectName));
                circles.Add(new DiagramCircle(id, $"({label})", circle.Center.X, circle.Center.Y, circle.Radius));
            }
            catch
            {
                // Three points are collinear → no circle. Silently skip.
            }
        }

        /// <summary>
        /// Build a stable string id for a configuration object. Reference identity is the only thing
        /// guaranteed to be unique in both Debug (where ConfigurationObject.Id exists) and Release.
        /// </summary>
        public static string ObjectId(ConfigurationObject obj)
            => "obj-" + System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(obj).ToString(CultureInfo.InvariantCulture);

        /// <summary>
        /// Stable id for a segment defined by an unordered pair of points. Two theorems mentioning
        /// "the line through A and B" should produce the same id regardless of point order.
        /// </summary>
        public static string SegmentId(ConfigurationObject p1, ConfigurationObject p2)
        {
            var h1 = System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(p1);
            var h2 = System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(p2);
            var (lo, hi) = h1 < h2 ? (h1, h2) : (h2, h1);
            return $"seg-{lo.ToString(CultureInfo.InvariantCulture)}-{hi.ToString(CultureInfo.InvariantCulture)}";
        }

        /// <summary>
        /// Stable id for a circle defined by an unordered set of points (typically 3 or 4 concyclic).
        /// Order-independent so a "ConcyclicPoints" theorem on (A,B,C,D) hashes the same regardless
        /// of the order the inner objects come back in.
        /// </summary>
        public static string CircleThroughPointsId(IEnumerable<ConfigurationObject> points)
        {
            var sorted = points
                .Select(p => System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(p))
                .OrderBy(x => x)
                .ToArray();
            return "cir-" + string.Join("-", sorted.Select(h => h.ToString(CultureInfo.InvariantCulture)));
        }

        // --- Bounding box ---

        private static BoundingBox ComputeBounds(
            IReadOnlyList<DiagramPoint> points,
            IReadOnlyList<DiagramCircle> circles,
            IReadOnlyList<DiagramSegment> segments)
        {
            // Degenerate fallback if there is literally nothing.
            if (points.Count == 0 && circles.Count == 0 && segments.Count == 0)
                return new BoundingBox(-1, -1, 1, 1);

            var minX = double.PositiveInfinity;
            var minY = double.PositiveInfinity;
            var maxX = double.NegativeInfinity;
            var maxY = double.NegativeInfinity;

            foreach (var p in points)
            {
                if (p.X < minX) minX = p.X;
                if (p.Y < minY) minY = p.Y;
                if (p.X > maxX) maxX = p.X;
                if (p.Y > maxY) maxY = p.Y;
            }

            foreach (var c in circles)
            {
                if (c.Cx - c.R < minX) minX = c.Cx - c.R;
                if (c.Cy - c.R < minY) minY = c.Cy - c.R;
                if (c.Cx + c.R > maxX) maxX = c.Cx + c.R;
                if (c.Cy + c.R > maxY) maxY = c.Cy + c.R;
            }

            foreach (var s in segments)
            {
                if (s.X1 < minX) minX = s.X1;
                if (s.X2 < minX) minX = s.X2;
                if (s.Y1 < minY) minY = s.Y1;
                if (s.Y2 < minY) minY = s.Y2;
                if (s.X1 > maxX) maxX = s.X1;
                if (s.X2 > maxX) maxX = s.X2;
                if (s.Y1 > maxY) maxY = s.Y1;
                if (s.Y2 > maxY) maxY = s.Y2;
            }

            return new BoundingBox(minX, minY, maxX, maxY);
        }

        // --- Line clipping (Liang-Barsky) ---

        /// <summary>
        /// Clip the infinite line <c>Ax + By + C = 0</c> to the rectangle <paramref name="box"/>.
        /// Returns false if the line does not intersect the rectangle (which is rare in well-formed
        /// configurations but possible).
        /// </summary>
        private static bool TryClipLineToBounds(Line line, BoundingBox box,
            out double x1, out double y1, out double x2, out double y2)
        {
            x1 = y1 = x2 = y2 = 0;

            // Parametric form: pick a point on the line and a direction.
            // Direction vector for Ax+By+C=0 is (B, -A); a point on the line satisfies the equation.
            // We special-case |A| vs |B| dominance to keep the parametric point well-conditioned.
            Point p0;
            Point dir;
            if (Math.Abs(line.A) >= Math.Abs(line.B))
            {
                // x = (-B*y - C) / A; pick y = 0 ⇒ x = -C/A
                p0 = new Point(-line.C / line.A, 0);
                dir = new Point(line.B, -line.A);
            }
            else
            {
                // y = (-A*x - C) / B; pick x = 0 ⇒ y = -C/B
                p0 = new Point(0, -line.C / line.B);
                dir = new Point(line.B, -line.A);
            }

            // Find parameter t-range such that p0 + t*dir lies inside box.
            // For each of the four box edges, derive a t-bound. Intersect them.
            var tMin = double.NegativeInfinity;
            var tMax = double.PositiveInfinity;

            // x-bounds: box.MinX ≤ p0.X + t*dir.X ≤ box.MaxX
            if (Math.Abs(dir.X) < 1e-12)
            {
                // Vertical line — no x-motion. Either always inside x-range or never.
                if (p0.X < box.MinX || p0.X > box.MaxX)
                    return false;
            }
            else
            {
                var t1 = (box.MinX - p0.X) / dir.X;
                var t2 = (box.MaxX - p0.X) / dir.X;
                if (t1 > t2) (t1, t2) = (t2, t1);
                if (t1 > tMin) tMin = t1;
                if (t2 < tMax) tMax = t2;
            }

            // y-bounds
            if (Math.Abs(dir.Y) < 1e-12)
            {
                if (p0.Y < box.MinY || p0.Y > box.MaxY)
                    return false;
            }
            else
            {
                var t1 = (box.MinY - p0.Y) / dir.Y;
                var t2 = (box.MaxY - p0.Y) / dir.Y;
                if (t1 > t2) (t1, t2) = (t2, t1);
                if (t1 > tMin) tMin = t1;
                if (t2 < tMax) tMax = t2;
            }

            if (tMin > tMax)
                return false; // doesn't intersect the box

            x1 = p0.X + tMin * dir.X;
            y1 = p0.Y + tMin * dir.Y;
            x2 = p0.X + tMax * dir.X;
            y2 = p0.Y + tMax * dir.Y;
            return true;
        }
    }
}
