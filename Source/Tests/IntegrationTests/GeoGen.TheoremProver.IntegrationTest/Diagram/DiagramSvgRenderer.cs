#nullable enable
using System.Globalization;
using System.Net;
using System.Text;

namespace GeoGen.TheoremProver.IntegrationTest.Diagram
{
    /// <summary>
    /// Renders a <see cref="DiagramModel"/> as an inline SVG fragment. The output is intended to be
    /// embedded directly inside the per-scenario HTML report — no &lt;?xml?&gt; prolog, no external
    /// stylesheet references, just a self-contained &lt;svg&gt; element.
    /// <para>
    /// Convention: world coordinates are y-up (math style). SVG is y-down. We flip Y at render time
    /// (instead of via a transform) so text labels stay right-side-up.
    /// </para>
    /// </summary>
    public static class DiagramSvgRenderer
    {
        /// <summary>Pixel size used for the inline SVG. The viewBox does the world-to-screen scaling.</summary>
        private const int PixelSize = 480;

        public static string Render(DiagramModel model)
        {
            var bounds = model.Bounds;

            // SVG y-axis is flipped — convert by negating Y, so a "high" world point ends up at the
            // top of the SVG. We achieve this by emitting `MinY` as `-MaxY` in the viewBox and
            // negating Y on every coordinate write.
            var viewBox = $"{F(bounds.MinX)} {F(-bounds.MaxY)} {F(bounds.Width)} {F(bounds.Height)}";

            // Point radius scaled to the diagram size so it looks consistent across small/large configurations.
            var diagonal = Math.Sqrt(bounds.Width * bounds.Width + bounds.Height * bounds.Height);
            var pointRadius = diagonal * 0.008;
            var labelOffset = diagonal * 0.018;
            var labelFontSize = diagonal * 0.035;

            var sb = new StringBuilder();
            sb.Append($"<svg class=\"diagram\" viewBox=\"{viewBox}\" width=\"{PixelSize}\" height=\"{PixelSize}\" preserveAspectRatio=\"xMidYMid meet\" xmlns=\"http://www.w3.org/2000/svg\">");

            // Background-style group: full-line analytics, then synthesized segments, then circles.
            // Points sit on top via the second group.
            sb.Append("<g class=\"diagram-strokes\">");
            foreach (var line in model.Lines)
                sb.Append($"<line class=\"diagram-line\" data-object-id=\"{Esc(line.Id)}\" x1=\"{F(line.X1)}\" y1=\"{F(-line.Y1)}\" x2=\"{F(line.X2)}\" y2=\"{F(-line.Y2)}\" vector-effect=\"non-scaling-stroke\"><title>{Esc(line.Label)}</title></line>");
            foreach (var seg in model.Segments)
            {
                // A segment can carry multiple ids (the canonical seg-id plus aliases for any
                // explicit ConfigurationObject that names the same line). Emit all as a comma-list.
                var ids = string.Join(",", seg.Ids);
                sb.Append($"<line class=\"diagram-segment\" data-object-id=\"{Esc(ids)}\" x1=\"{F(seg.X1)}\" y1=\"{F(-seg.Y1)}\" x2=\"{F(seg.X2)}\" y2=\"{F(-seg.Y2)}\" vector-effect=\"non-scaling-stroke\"><title>{Esc(seg.Label)}</title></line>");
            }
            foreach (var circle in model.Circles)
                sb.Append($"<circle class=\"diagram-circle\" data-object-id=\"{Esc(circle.Id)}\" cx=\"{F(circle.Cx)}\" cy=\"{F(-circle.Cy)}\" r=\"{F(circle.R)}\" vector-effect=\"non-scaling-stroke\" fill=\"none\"><title>{Esc(circle.Label)}</title></circle>");
            sb.Append("</g>");

            // Foreground: points and labels.
            sb.Append("<g class=\"diagram-points\">");
            foreach (var point in model.Points)
            {
                // vector-effect="non-scaling-stroke" so the white halo (stroke) renders in screen
                // pixels, not in world units — without it, a 0.5-unit stroke can swallow the dot
                // and turn it into a giant white blob the user sees instead of a point.
                sb.Append($"<circle class=\"diagram-point\" data-object-id=\"{Esc(point.Id)}\" cx=\"{F(point.X)}\" cy=\"{F(-point.Y)}\" r=\"{F(pointRadius)}\" vector-effect=\"non-scaling-stroke\"><title>{Esc(point.Label)}</title></circle>");
                // Label is offset diagonally so it doesn't sit on top of the point.
                sb.Append($"<text class=\"diagram-label\" data-object-id=\"{Esc(point.Id)}\" x=\"{F(point.X + labelOffset)}\" y=\"{F(-point.Y - labelOffset)}\" font-size=\"{F(labelFontSize)}\">{Esc(point.Label)}</text>");
            }
            sb.Append("</g>");

            sb.Append("</svg>");
            return sb.ToString();
        }

        /// <summary>The CSS that styles the SVG. Embedded by the report so the diagram renders without external assets.</summary>
        public const string Css = """
.diagram { background: #ffffff; border: 1px solid #d0d7de; border-radius: 6px; max-width: 100%; display: block; }
/* Default object styling — muted grays so highlights stand out clearly. */
.diagram-line { stroke: #6e7781; stroke-width: 1.5; fill: none; }
.diagram-segment { stroke: #1f2328; stroke-width: 1.75; fill: none; }
.diagram-circle { stroke: #6e7781; stroke-width: 1.5; fill: none; }
.diagram-point { fill: #1f2328; stroke: #ffffff; stroke-width: 1.5; }
.diagram-label { fill: #1f2328; font-family: ui-monospace, SFMono-Regular, Menlo, monospace; pointer-events: none; }

/* Highlight states: width is the only signal of "intensity" (preview vs sticky vs step).
   Color is determined separately by per-theorem-type classes below, so a parallel-lines hover
   shows up as wide+blue, a perpendicular-lines hover as wide+red, etc. */
.diagram-line.preview, .diagram-segment.preview, .diagram-circle.preview { stroke-width: 3; }
.diagram-line.highlight, .diagram-segment.highlight, .diagram-circle.highlight { stroke-width: 3.5; }
.diagram-line.step-active, .diagram-segment.step-active, .diagram-circle.step-active { stroke-width: 4; }
.diagram-label.preview, .diagram-label.highlight, .diagram-label.step-active { font-weight: 700; }

/* Default highlight color (for elements without a specific type class — config tokens, etc.).
   Bright red because it's the most visually distinct from the muted gray default. */
.diagram-line.preview, .diagram-segment.preview, .diagram-circle.preview { stroke: #ff0033; }
.diagram-point.preview { fill: #ff0033; }
.diagram-label.preview { fill: #ff0033; }
.diagram-line.highlight, .diagram-segment.highlight, .diagram-circle.highlight { stroke: #ff0033; }
.diagram-point.highlight { fill: #ff0033; }
.diagram-label.highlight { fill: #ff0033; }

/* Per-theorem-type colors. High-saturation so they pop against the muted gray default. */
.diagram-line.color-ParallelLines, .diagram-segment.color-ParallelLines, .diagram-circle.color-ParallelLines { stroke: #0088ff; }
.diagram-point.color-ParallelLines { fill: #0088ff; }
.diagram-label.color-ParallelLines { fill: #0088ff; }
.diagram-line.color-PerpendicularLines, .diagram-segment.color-PerpendicularLines, .diagram-circle.color-PerpendicularLines { stroke: #ff0033; }
.diagram-point.color-PerpendicularLines { fill: #ff0033; }
.diagram-label.color-PerpendicularLines { fill: #ff0033; }
.diagram-line.color-EqualLineSegments, .diagram-segment.color-EqualLineSegments, .diagram-circle.color-EqualLineSegments { stroke: #00cc44; }
.diagram-point.color-EqualLineSegments { fill: #00cc44; }
.diagram-label.color-EqualLineSegments { fill: #00cc44; }
.diagram-line.color-CollinearPoints, .diagram-segment.color-CollinearPoints, .diagram-circle.color-CollinearPoints { stroke: #aa00ff; }
.diagram-point.color-CollinearPoints { fill: #aa00ff; }
.diagram-label.color-CollinearPoints { fill: #aa00ff; }
.diagram-line.color-ConcyclicPoints, .diagram-segment.color-ConcyclicPoints, .diagram-circle.color-ConcyclicPoints { stroke: #00ccdd; }
.diagram-point.color-ConcyclicPoints { fill: #00ccdd; }
.diagram-label.color-ConcyclicPoints { fill: #00ccdd; }
.diagram-line.color-ConcurrentLines, .diagram-segment.color-ConcurrentLines, .diagram-circle.color-ConcurrentLines { stroke: #ff7700; }
.diagram-point.color-ConcurrentLines { fill: #ff7700; }
.diagram-label.color-ConcurrentLines { fill: #ff7700; }
.diagram-line.color-TangentCircles, .diagram-segment.color-TangentCircles, .diagram-circle.color-TangentCircles { stroke: #ff00aa; }
.diagram-point.color-TangentCircles { fill: #ff00aa; }
.diagram-label.color-TangentCircles { fill: #ff00aa; }
.diagram-line.color-LineTangentToCircle, .diagram-segment.color-LineTangentToCircle, .diagram-circle.color-LineTangentToCircle { stroke: #ff00aa; }
.diagram-point.color-LineTangentToCircle { fill: #ff00aa; }
.diagram-label.color-LineTangentToCircle { fill: #ff00aa; }
.diagram-line.color-Incidence, .diagram-segment.color-Incidence, .diagram-circle.color-Incidence { stroke: #777777; }
.diagram-point.color-Incidence { fill: #777777; }
.diagram-label.color-Incidence { fill: #777777; }
.diagram-line.color-EqualObjects, .diagram-segment.color-EqualObjects, .diagram-circle.color-EqualObjects { stroke: #cc6600; }
.diagram-point.color-EqualObjects { fill: #cc6600; }
.diagram-label.color-EqualObjects { fill: #cc6600; }

/* Step-active overrides type color: always amber so "this is the current step" is unambiguous. */
.diagram-line.step-active, .diagram-segment.step-active, .diagram-circle.step-active { stroke: #bf8700; }
.diagram-point.step-active { fill: #bf8700; }
.diagram-label.step-active { fill: #bf8700; }

/* Theorem-row visual hint: a small color bar matching the diagram color, so readers learn the
   correspondence between row color and SVG color. */
.theorem-item[data-theorem-type] > details > summary::before {
    content: ""; display: inline-block; width: 4px; height: 1em; background: currentColor;
    margin-right: 6px; vertical-align: -0.15em; border-radius: 2px; opacity: 0.7;
}
.theorem-item[data-theorem-type="ParallelLines"] > details > summary { color: #0088ff; }
.theorem-item[data-theorem-type="PerpendicularLines"] > details > summary { color: #ff0033; }
.theorem-item[data-theorem-type="EqualLineSegments"] > details > summary { color: #00cc44; }
.theorem-item[data-theorem-type="CollinearPoints"] > details > summary { color: #aa00ff; }
.theorem-item[data-theorem-type="ConcyclicPoints"] > details > summary { color: #00ccdd; }
.theorem-item[data-theorem-type="ConcurrentLines"] > details > summary { color: #ff7700; }
.theorem-item[data-theorem-type="TangentCircles"] > details > summary { color: #ff00aa; }
.theorem-item[data-theorem-type="LineTangentToCircle"] > details > summary { color: #ff00aa; }
.theorem-item[data-theorem-type="Incidence"] > details > summary { color: #777777; }
.theorem-item[data-theorem-type="EqualObjects"] > details > summary { color: #cc6600; }
/* Restore body text color inside the summary so only the bar itself uses the type color. */
.theorem-item[data-theorem-type] > details > summary > * { color: #1f2328; }

/* Color legend in the diagram aside. */
.color-legend { margin-top: 0.75rem; font-size: 0.78rem; color: #57606a; }
.color-legend-item { display: inline-flex; align-items: center; margin-right: 0.75rem; margin-bottom: 0.2rem; }
.color-legend-swatch { display: inline-block; width: 10px; height: 10px; border-radius: 2px; margin-right: 0.3rem; }

/* Config block: clickable lines and tokens. */
.config { background: #f6f8fa; padding: 0.5rem 0.75rem; border-radius: 4px; font-family: ui-monospace, SFMono-Regular, Menlo, monospace; font-size: 0.9rem; line-height: 1.7; }
.config-line { padding: 0.1rem 0.3rem; border-radius: 3px; cursor: pointer; }
.config-line:hover { background: #ddf4ff; }
.config-line.preview { background: #fff8c5; }
.config-line.highlight { background: #ffe4e0; }
.config-keyword, .config-construction { color: #6f42c1; font-weight: 600; }
.config-token { padding: 0 0.15rem; border-radius: 2px; cursor: pointer; }
.config-token:hover { background: #ddf4ff; }
.config-token.preview { background: #fff8c5; }
.config-token.highlight { background: #ffe4e0; }
""";

        private static string F(double v) => v.ToString("0.######", CultureInfo.InvariantCulture);
        private static string Esc(string s) => WebUtility.HtmlEncode(s);
    }
}
