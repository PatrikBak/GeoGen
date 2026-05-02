#nullable enable
namespace GeoGen.TheoremProver.IntegrationTest.Diagram
{
    /// <summary>
    /// Pure-data render model for one configuration's diagram. Built once by <see cref="DiagramBuilder"/>
    /// from a <see cref="Constructor.Picture"/>, then consumed by <see cref="DiagramSvgRenderer"/>.
    /// All coordinates are in the picture's native real-number space; the renderer is responsible
    /// for the SVG viewBox transform.
    /// </summary>
    public sealed class DiagramModel
    {
        /// <summary>Stable id assigned to each rendered object. Used as the SVG element id and as the join key for theorem highlights.</summary>
        public IReadOnlyList<DiagramPoint> Points { get; }
        public IReadOnlyList<DiagramLine> Lines { get; }
        public IReadOnlyList<DiagramCircle> Circles { get; }

        /// <summary>
        /// Implicit segments synthesized from theorem statements and the loose-object layout — lines
        /// that the picture doesn't have an explicit <see cref="GeoGen.AnalyticGeometry.Line"/> for,
        /// but which the configuration clearly mentions (e.g., a triangle's sides, or a line defined
        /// by two named points inside a <c>ParallelLines</c> theorem).
        /// </summary>
        public IReadOnlyList<DiagramSegment> Segments { get; }

        /// <summary>The world-space bounding box that should be visible in the SVG viewBox.</summary>
        public BoundingBox Bounds { get; }

        public DiagramModel(
            IReadOnlyList<DiagramPoint> points,
            IReadOnlyList<DiagramLine> lines,
            IReadOnlyList<DiagramCircle> circles,
            IReadOnlyList<DiagramSegment> segments,
            BoundingBox bounds)
        {
            Points = points;
            Lines = lines;
            Circles = circles;
            Segments = segments;
            Bounds = bounds;
        }
    }

    public sealed record DiagramPoint(string Id, string Label, double X, double Y);

    /// <summary>
    /// A line rendered as a clipped segment from an explicit <see cref="GeoGen.AnalyticGeometry.Line"/>
    /// in the picture. <see cref="X1"/>,<see cref="Y1"/>,<see cref="X2"/>,<see cref="Y2"/> are the
    /// world-space endpoints after clipping to the diagram bounds.
    /// </summary>
    public sealed record DiagramLine(string Id, string Label, double X1, double Y1, double X2, double Y2);

    /// <summary>
    /// A point-to-point segment synthesized from a theorem or layout. Carries a <em>list</em> of ids
    /// because the same physical segment can be referenced by multiple theorems under different names
    /// (e.g., "the line through A and B" appears in two theorems).
    /// </summary>
    public sealed record DiagramSegment(IReadOnlyList<string> Ids, string Label, double X1, double Y1, double X2, double Y2);

    public sealed record DiagramCircle(string Id, string Label, double Cx, double Cy, double R);

    public readonly record struct BoundingBox(double MinX, double MinY, double MaxX, double MaxY)
    {
        public double Width => MaxX - MinX;
        public double Height => MaxY - MinY;

        /// <summary>Return a new box expanded by <paramref name="margin"/> in each direction.</summary>
        public BoundingBox Expand(double margin) => new(MinX - margin, MinY - margin, MaxX + margin, MaxY + margin);
    }
}
