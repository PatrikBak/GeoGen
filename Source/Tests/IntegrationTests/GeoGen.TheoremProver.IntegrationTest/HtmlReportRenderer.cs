#nullable enable
using System.Globalization;
using System.Net;
using System.Text;
using GeoGen.Core;
using GeoGen.TheoremProver.IntegrationTest.Diagram;

namespace GeoGen.TheoremProver.IntegrationTest
{
    /// <summary>
    /// Renders a single self-contained HTML report describing what the prover did with one configuration.
    /// <para>
    /// The output is one HTML file with inline CSS and JS — no external assets. It contains the
    /// configuration listing, a table of proved theorems with expandable proof trees, a table of
    /// unproved theorems, and the full inference trace recorded by an <see cref="IInferenceTracer"/>.
    /// A search box at the top filters across every visible row.
    /// </para>
    /// <para>
    /// The renderer is purely a function of its inputs — the caller is responsible for I/O.
    /// </para>
    /// </summary>
    public static class HtmlReportRenderer
    {
        /// <summary>
        /// Build the HTML for one configuration's report.
        /// </summary>
        public static string Render(
            string scenarioName,
            Configuration configuration,
            OutputFormatter formatter,
            IReadOnlyDictionary<Theorem, TheoremProof> proofs,
            IReadOnlyCollection<Theorem> unprovedTheorems,
            TracedSession? trace,
            long elapsedMs,
            DiagramModel? diagram)
        {
            var sb = new StringBuilder();
            WriteHeader(sb, scenarioName);
            WriteSummary(sb, scenarioName, configuration, formatter, proofs, unprovedTheorems, trace, elapsedMs);

            // Open the side-by-side wrapper. Diagram floats sticky on the right (or top on narrow
            // screens). Configuration + theorems + trace all flow in the left column so they scroll
            // past while the diagram stays in view.
            sb.AppendLine("<div class=\"main-layout\">");
            sb.AppendLine("<div class=\"main-content\">");

            // Configuration first in the scrollable column.
            WriteConfigurationSection(sb, formatter, configuration);

            // "To prove" overview — every theorem the prover was given, regardless of outcome.
            // Listed up front so readers can see the goal before reading the per-outcome sections.
            var allTheorems = proofs.Keys.Concat(unprovedTheorems).Distinct();
            WriteToProveOverview(sb, formatter, allTheorems);

            WriteProvedTheoremsSection(sb, formatter, proofs);
            WriteUnprovedTheoremsSection(sb, formatter, unprovedTheorems);
            WriteTraceSection(sb, formatter, trace);

            sb.AppendLine("</div>"); // .main-content

            // Theorem types present in the report — used for the diagram-aside legend so users can
            // decode the colors at a glance without an external key.
            var typesPresent = proofs.Keys.Select(t => t.Type)
                .Concat(unprovedTheorems.Select(t => t.Type))
                .Distinct()
                .OrderBy(t => t.ToString(), StringComparer.Ordinal)
                .ToArray();
            WriteDiagramSection(sb, diagram, typesPresent);

            sb.AppendLine("</div>"); // .main-layout

            WriteFooter(sb);
            return sb.ToString();
        }

        private static void WriteDiagramSection(StringBuilder sb, DiagramModel? diagram, IReadOnlyList<TheoremType> typesPresent)
        {
            sb.AppendLine("<aside class=\"diagram-aside\">");
            sb.AppendLine("  <div class=\"diagram-sticky\">");
            sb.AppendLine("    <h2>Diagram</h2>");
            if (diagram is null)
            {
                sb.AppendLine("    <p class=\"muted\">(diagram unavailable for this scenario)</p>");
            }
            else
            {
                sb.AppendLine(DiagramSvgRenderer.Render(diagram));
                sb.AppendLine("    <div class=\"diagram-status\" id=\"diagram-status\">Hover or click a theorem to highlight its objects.</div>");
                sb.AppendLine("    <div class=\"step-controls\" id=\"step-controls\" hidden>");
                sb.AppendLine("      <button id=\"step-prev\" type=\"button\">◀ Prev</button>");
                sb.AppendLine("      <button id=\"step-play\" type=\"button\">▶ Play</button>");
                sb.AppendLine("      <button id=\"step-next\" type=\"button\">Next ▶</button>");
                sb.AppendLine("      <button id=\"step-stop\" type=\"button\">✕ Stop</button>");
                sb.AppendLine("      <span id=\"step-counter\" class=\"muted\"></span>");
                sb.AppendLine("    </div>");
                WriteColorLegend(sb, typesPresent);
            }
            sb.AppendLine("  </div>");
            sb.AppendLine("</aside>");
        }

        private static void WriteColorLegend(StringBuilder sb, IReadOnlyList<TheoremType> typesPresent)
        {
            if (typesPresent.Count == 0)
                return;

            sb.AppendLine("    <div class=\"color-legend\">");
            sb.AppendLine("      <strong>Color key:</strong> ");
            foreach (var t in typesPresent)
            {
                var color = ColorForType(t);
                sb.AppendLine($"      <span class=\"color-legend-item\"><span class=\"color-legend-swatch\" style=\"background:{color}\"></span>{Esc(t.ToString())}</span>");
            }
            sb.AppendLine("    </div>");
        }

        /// <summary>
        /// Color for each theorem type in the legend. Mirrors the per-type CSS rules in
        /// <see cref="DiagramSvgRenderer.Css"/> — keep in sync if either is edited.
        /// </summary>
        private static string ColorForType(TheoremType type) => type switch
        {
            TheoremType.ParallelLines => "#0088ff",
            TheoremType.PerpendicularLines => "#ff0033",
            TheoremType.EqualLineSegments => "#00cc44",
            TheoremType.CollinearPoints => "#aa00ff",
            TheoremType.ConcyclicPoints => "#00ccdd",
            TheoremType.ConcurrentLines => "#ff7700",
            TheoremType.TangentCircles => "#ff00aa",
            TheoremType.LineTangentToCircle => "#ff00aa",
            TheoremType.Incidence => "#777777",
            TheoremType.EqualObjects => "#cc6600",
            _ => "#ff0033",
        };

        // ---------- Sections ----------

        private static void WriteHeader(StringBuilder sb, string scenarioName)
        {
            var title = $"GeoGen Prover Report — {Esc(scenarioName)}";
            sb.AppendLine($"""
<!DOCTYPE html>
<html lang="en">
<head>
<meta charset="UTF-8">
<title>{title}</title>
<style>
{Css}
</style>
</head>
<body>
<header>
  <h1>{title}</h1>
  <input id="filter" type="search" placeholder="Filter rows by substring..." autocomplete="off">
</header>
""");
        }

        private static void WriteSummary(
            StringBuilder sb,
            string scenarioName,
            Configuration configuration,
            OutputFormatter formatter,
            IReadOnlyDictionary<Theorem, TheoremProof> proofs,
            IReadOnlyCollection<Theorem> unproved,
            TracedSession? trace,
            long elapsedMs)
        {
            var traceCount = trace?.Events.Count ?? 0;
            var ruleBreakdown = trace is null
                ? "(no trace captured)"
                : string.Join(", ",
                    trace.Events
                        .GroupBy(e => e.Rule)
                        .OrderByDescending(g => g.Count())
                        .Select(g => $"{g.Key}: {g.Count()}"));

            sb.AppendLine($"""
<section class="summary">
  <div class="stat"><div class="stat-label">Proved</div><div class="stat-value">{proofs.Count}</div></div>
  <div class="stat"><div class="stat-label">Unproved</div><div class="stat-value">{unproved.Count}</div></div>
  <div class="stat"><div class="stat-label">Trace events</div><div class="stat-value">{traceCount}</div></div>
  <div class="stat"><div class="stat-label">Wall time</div><div class="stat-value">{elapsedMs} ms</div></div>
  <div class="stat full"><div class="stat-label">Rule-type breakdown</div><div class="stat-value">{Esc(ruleBreakdown)}</div></div>
</section>
""");
        }

        private static void WriteConfigurationSection(StringBuilder sb, OutputFormatter formatter, Configuration configuration)
        {
            sb.AppendLine("<section>");
            sb.AppendLine("  <h2>Configuration</h2>");
            sb.AppendLine("  <div class=\"config\">");

            // Layout line: e.g., "Triangle: A, B, C" — the loose objects.
            var layoutName = configuration.LooseObjectsHolder.Layout.ToString();
            var looseIds = configuration.LooseObjects.Select(o => Diagram.DiagramBuilder.ObjectId(o)).ToArray();
            sb.Append("    <div class=\"config-line highlightable\" data-highlight=\"").Append(Esc(string.Join(",", looseIds))).Append("\">");
            sb.Append("<span class=\"config-keyword\">").Append(Esc(layoutName)).Append("</span>: ");
            for (var i = 0; i < configuration.LooseObjects.Count; i++)
            {
                if (i > 0) sb.Append(", ");
                AppendObjectToken(sb, configuration.LooseObjects[i], formatter);
            }
            sb.AppendLine("</div>");

            // One line per constructed object: "<name> = <Construction>(<args>)".
            foreach (var constructed in configuration.ConstructedObjects)
            {
                // The line as a whole highlights the constructed object plus every object it references.
                var lineIds = new List<string> { Diagram.DiagramBuilder.ObjectId(constructed) };
                CollectReferencedObjectIds(constructed.PassedArguments, lineIds);

                sb.Append("    <div class=\"config-line highlightable\" data-highlight=\"").Append(Esc(string.Join(",", lineIds.Distinct()))).Append("\">");
                AppendObjectToken(sb, constructed, formatter);
                sb.Append(" = ");
                sb.Append("<span class=\"config-construction\">").Append(Esc(constructed.Construction.Name)).Append("</span>");
                sb.Append("(");
                var args = constructed.PassedArguments.ArgumentsList;
                for (var i = 0; i < args.Count; i++)
                {
                    if (i > 0) sb.Append(", ");
                    AppendArgumentToken(sb, args[i], formatter);
                }
                sb.Append(")");
                sb.AppendLine("</div>");
            }

            sb.AppendLine("  </div>");
            sb.AppendLine("</section>");
        }

        /// <summary>Render one configuration object as a clickable name span.</summary>
        private static void AppendObjectToken(StringBuilder sb, ConfigurationObject obj, OutputFormatter formatter)
        {
            var id = Diagram.DiagramBuilder.ObjectId(obj);
            var name = formatter.GetObjectName(obj);
            // The token stops click propagation so clicking a name doesn't also trigger the
            // surrounding line's highlight handler.
            sb.Append("<span class=\"config-token highlightable\" data-highlight=\"").Append(Esc(id)).Append("\">")
              .Append(Esc(name))
              .Append("</span>");
        }

        /// <summary>Recursively render a construction argument, emitting clickable spans for inner object references.</summary>
        private static void AppendArgumentToken(StringBuilder sb, ConstructionArgument argument, OutputFormatter formatter)
        {
            switch (argument)
            {
                case ObjectConstructionArgument objArg:
                    AppendObjectToken(sb, objArg.PassedObject, formatter);
                    break;
                case SetConstructionArgument setArg:
                    sb.Append("{");
                    // Order to match the existing formatter's stable output (alphabetical via Ordered()).
                    var inner = setArg.PassedArguments
                        .Select(a => (Argument: a, Text: formatter.FormatArgument(a)))
                        .OrderBy(x => x.Text, StringComparer.Ordinal)
                        .ToArray();
                    for (var i = 0; i < inner.Length; i++)
                    {
                        if (i > 0) sb.Append(", ");
                        AppendArgumentToken(sb, inner[i].Argument, formatter);
                    }
                    sb.Append("}");
                    break;
            }
        }

        /// <summary>Walk a construction argument tree and accumulate ids of every referenced ConfigurationObject.</summary>
        private static void CollectReferencedObjectIds(IEnumerable<ConstructionArgument> arguments, List<string> into)
        {
            foreach (var arg in arguments)
                CollectReferencedObjectIds(arg, into);
        }

        private static void CollectReferencedObjectIds(ConstructionArgument argument, List<string> into)
        {
            switch (argument)
            {
                case ObjectConstructionArgument objArg:
                    into.Add(Diagram.DiagramBuilder.ObjectId(objArg.PassedObject));
                    break;
                case SetConstructionArgument setArg:
                    CollectReferencedObjectIds(setArg.PassedArguments, into);
                    break;
            }
        }

        private static void WriteProvedTheoremsSection(StringBuilder sb, OutputFormatter formatter, IReadOnlyDictionary<Theorem, TheoremProof> proofs)
        {
            sb.AppendLine("<section>");
            sb.AppendLine($"  <h2>Proved theorems <span class=\"muted\">({proofs.Count})</span></h2>");

            if (proofs.Count == 0)
            {
                sb.AppendLine("<p class=\"muted\">No proved theorems.</p></section>");
                return;
            }

            sb.AppendLine("<ol class=\"theorem-list\">");

            // Order proofs by the theorem text so the output is stable across runs.
            var ordered = proofs
                .OrderBy(p => formatter.FormatTheorem(p.Key), StringComparer.Ordinal)
                .ToArray();

            foreach (var (theorem, proof) in ordered)
            {
                var headline = Esc(formatter.FormatTheorem(theorem));
                var ruleBadge = RuleBadge(proof.Rule);
                var theoremIds = string.Join(",", TheoremHighlights.ObjectIdsFor(theorem));
                var stepsJson = BuildStepsJson(formatter, proof);

                sb.AppendLine($"  <li class=\"theorem-item filterable highlightable\" data-rule=\"{Esc(proof.Rule.ToString())}\" data-theorem-type=\"{Esc(theorem.Type.ToString())}\" data-highlight=\"{Esc(theoremIds)}\" data-steps=\"{Esc(stepsJson)}\">");
                sb.AppendLine("    <details>");
                sb.AppendLine($"      <summary><button class=\"play-btn\" type=\"button\" title=\"Play proof step-through\">▶</button>{ruleBadge} {headline}</summary>");
                sb.AppendLine("      <div class=\"proof-tree\">");
                WriteProofTree(sb, formatter, proof, depth: 0, seen: new HashSet<Theorem>());
                sb.AppendLine("      </div>");
                sb.AppendLine("    </details>");
                sb.AppendLine("  </li>");
            }

            sb.AppendLine("</ol></section>");
        }

        /// <summary>
        /// Build a JSON array describing the step-through animation for one proved theorem. Each step
        /// is one node in the proof tree, in DFS order, deduplicated so a theorem cited twice fires
        /// once. Each step carries the human-readable theorem text, the rule label, and the object
        /// ids to light up.
        /// </summary>
        private static string BuildStepsJson(OutputFormatter formatter, TheoremProof proof)
        {
            var steps = new List<string>();
            var seen = new HashSet<Theorem>();
            CollectSteps(proof);

            return "[" + string.Join(",", steps) + "]";

            void CollectSteps(TheoremProof p)
            {
                if (!seen.Add(p.Theorem))
                    return;

                // DFS bottom-up so the rendered animation builds the proof from leaves toward root.
                foreach (var child in p.ProvedAssumptions)
                    CollectSteps(child);

                var ids = TheoremHighlights.ObjectIdsFor(p.Theorem);
                var idsArr = "[" + string.Join(",", ids.Select(JsString)) + "]";
                var step = "{"
                    + $"\"theorem\":{JsString(formatter.FormatTheorem(p.Theorem))},"
                    + $"\"rule\":{JsString(p.Rule.ToString())},"
                    + $"\"theoremType\":{JsString(p.Theorem.Type.ToString())},"
                    + $"\"explanation\":{JsString(ExplainRule(p))},"
                    + $"\"ids\":{idsArr}"
                    + "}";
                steps.Add(step);
            }
        }

        /// <summary>JSON string literal, double-quoted, with the necessary characters escaped.</summary>
        private static string JsString(string s)
        {
            // Minimal JSON-string escaping. Backslash, quote, control characters, and the ampersand
            // (which is innocuous in JSON but the value lives inside an HTML attribute, so we want
            // it to round-trip safely).
            var sb = new StringBuilder(s.Length + 2);
            sb.Append('"');
            foreach (var c in s)
            {
                switch (c)
                {
                    case '\\': sb.Append("\\\\"); break;
                    case '"': sb.Append("\\\""); break;
                    case '\n': sb.Append("\\n"); break;
                    case '\r': sb.Append("\\r"); break;
                    case '\t': sb.Append("\\t"); break;
                    default:
                        if (c < 0x20)
                            sb.Append("\\u").Append(((int)c).ToString("x4", CultureInfo.InvariantCulture));
                        else
                            sb.Append(c);
                        break;
                }
            }
            sb.Append('"');
            return sb.ToString();
        }

        private static void WriteProofTree(StringBuilder sb, OutputFormatter formatter, TheoremProof proof, int depth, HashSet<Theorem> seen)
        {
            var explanation = ExplainRule(proof);
            var headline = Esc(formatter.FormatTheorem(proof.Theorem));
            var ruleBadge = RuleBadge(proof.Rule);
            var indentStyle = $"margin-left: {depth * 1.25}rem;";
            // Each step in the proof tree is independently clickable: hovering or clicking it
            // highlights the diagram objects this particular sub-theorem talks about, in the
            // theorem-type's color. Lets users decompose a multi-step proof by walking the tree
            // and watching which objects each step covers.
            var stepIds = string.Join(",", TheoremHighlights.ObjectIdsFor(proof.Theorem));

            sb.AppendLine($"<div class=\"proof-step highlightable\" data-theorem-type=\"{Esc(proof.Theorem.Type.ToString())}\" data-highlight=\"{Esc(stepIds)}\" style=\"{indentStyle}\">{ruleBadge} <span class=\"theorem\">{headline}</span> <span class=\"muted\">— {Esc(explanation)}</span></div>");

            // Don't infinitely recurse if a proof references a theorem already shown above.
            if (proof.ProvedAssumptions.Count == 0 || !seen.Add(proof.Theorem))
                return;

            foreach (var child in proof.ProvedAssumptions.OrderBy(a => formatter.FormatTheorem(a.Theorem), StringComparer.Ordinal))
                WriteProofTree(sb, formatter, child, depth + 1, seen);
        }

        private static void WriteUnprovedTheoremsSection(StringBuilder sb, OutputFormatter formatter, IReadOnlyCollection<Theorem> unproved)
        {
            sb.AppendLine("<section>");
            sb.AppendLine($"  <h2>Unproved theorems <span class=\"muted\">({unproved.Count})</span> <span class=\"muted\" style=\"font-weight:normal\">— what the prover failed to close</span></h2>");

            if (unproved.Count == 0)
            {
                sb.AppendLine("<p class=\"muted\">All theorems were proved.</p></section>");
                return;
            }

            sb.AppendLine("<ol class=\"unproved-list\">");
            foreach (var theorem in unproved.OrderBy(formatter.FormatTheorem, StringComparer.Ordinal))
            {
                var ids = string.Join(",", TheoremHighlights.ObjectIdsFor(theorem));
                sb.AppendLine($"  <li class=\"filterable highlightable unproved-item\" data-theorem-type=\"{Esc(theorem.Type.ToString())}\" data-highlight=\"{Esc(ids)}\">{Esc(formatter.FormatTheorem(theorem))}</li>");
            }
            sb.AppendLine("</ol></section>");
        }

        /// <summary>
        /// "What the prover was asked to handle" — the full set of theorems the prover was given as
        /// input. Rendered as a small bulleted overview at the top so users can see the goal at a
        /// glance, with each item clickable for diagram highlight.
        /// </summary>
        private static void WriteToProveOverview(StringBuilder sb, OutputFormatter formatter, IEnumerable<Theorem> theoremsToProve)
        {
            var list = theoremsToProve.OrderBy(formatter.FormatTheorem, StringComparer.Ordinal).ToArray();
            if (list.Length == 0)
                return;

            sb.AppendLine("<section>");
            sb.AppendLine($"  <h2>To prove <span class=\"muted\">({list.Length})</span> <span class=\"muted\" style=\"font-weight:normal\">— the goals fed to the prover</span></h2>");
            sb.AppendLine("  <ol class=\"to-prove-list\">");
            foreach (var theorem in list)
            {
                var ids = string.Join(",", TheoremHighlights.ObjectIdsFor(theorem));
                sb.AppendLine($"    <li class=\"filterable highlightable to-prove-item\" data-theorem-type=\"{Esc(theorem.Type.ToString())}\" data-highlight=\"{Esc(ids)}\">{Esc(formatter.FormatTheorem(theorem))}</li>");
            }
            sb.AppendLine("  </ol>");
            sb.AppendLine("</section>");
        }

        private static void WriteTraceSection(StringBuilder sb, OutputFormatter formatter, TracedSession? trace)
        {
            sb.AppendLine("<section>");
            sb.AppendLine($"  <h2>Inference trace <span class=\"muted\">({trace?.Events.Count ?? 0} events)</span></h2>");

            if (trace is null || trace.Events.Count == 0)
            {
                sb.AppendLine("<p class=\"muted\">No trace recorded.</p></section>");
                return;
            }

            sb.AppendLine("<table class=\"trace-table\">");
            sb.AppendLine("<thead><tr>");
            sb.AppendLine("  <th data-sort=\"num\">#</th>");
            sb.AppendLine("  <th data-sort=\"text\">Rule</th>");
            sb.AppendLine("  <th data-sort=\"text\">Theorem</th>");
            sb.AppendLine("  <th data-sort=\"text\">Assumptions</th>");
            sb.AppendLine("</tr></thead>");
            sb.AppendLine("<tbody>");

            foreach (var ev in trace.Events)
            {
                var assumptionsCell = ev.Assumptions.Count == 0
                    ? "<span class=\"muted\">(none)</span>"
                    : string.Join("<br>", ev.Assumptions.Select(a => Esc(formatter.FormatTheorem(a))));

                var ruleLabel = ev.Rule == InferenceRuleType.CustomRule && ev.CustomRule is not null
                    ? $"{ev.Rule}: {ev.CustomRule}"
                    : ev.Rule.ToString();

                sb.AppendLine($"  <tr class=\"filterable\" data-rule=\"{Esc(ev.Rule.ToString())}\">");
                sb.AppendLine($"    <td class=\"num\">{ev.Sequence.ToString(CultureInfo.InvariantCulture)}</td>");
                sb.AppendLine($"    <td>{RuleBadge(ev.Rule)} {Esc(ruleLabel)}</td>");
                sb.AppendLine($"    <td class=\"theorem\">{Esc(formatter.FormatTheorem(ev.Theorem))}</td>");
                sb.AppendLine($"    <td>{assumptionsCell}</td>");
                sb.AppendLine("  </tr>");
            }

            sb.AppendLine("</tbody></table></section>");
        }

        private static void WriteFooter(StringBuilder sb)
        {
            sb.AppendLine("<script>");
            sb.AppendLine(Js);
            sb.AppendLine("</script>");
            sb.AppendLine("</body>");
            sb.AppendLine("</html>");
        }

        // ---------- Helpers ----------

        private static string Esc(string text) => WebUtility.HtmlEncode(text);

        private static string RuleBadge(InferenceRuleType rule)
        {
            // Each rule type gets a stable CSS class so users can scan visually.
            var cssClass = rule switch
            {
                InferenceRuleType.AssumedProven => "badge assumed",
                InferenceRuleType.TrivialTheorem => "badge trivial",
                InferenceRuleType.CustomRule => "badge custom",
                InferenceRuleType.ReformulatedTheorem => "badge reform",
                InferenceRuleType.EqualityTransitivity => "badge transit",
                InferenceRuleType.InferableFromSymmetry => "badge sym",
                InferenceRuleType.DefinableSimpler => "badge simpler",
                _ => "badge other",
            };
            return $"<span class=\"{cssClass}\">{Esc(rule.ToString())}</span>";
        }

        private static string ExplainRule(TheoremProof proof) => proof.Rule switch
        {
            InferenceRuleType.AssumedProven => "assumed in a previous configuration",
            InferenceRuleType.TrivialTheorem => "trivial consequence of construction",
            InferenceRuleType.ReformulatedTheorem => "reformulation via equalities",
            InferenceRuleType.CustomRule => $"custom rule: {((CustomInferenceData)proof.Data).Rule}",
            InferenceRuleType.EqualityTransitivity => "transitivity of equality",
            InferenceRuleType.InferableFromSymmetry => "inferable by symmetry",
            InferenceRuleType.DefinableSimpler => $"can be stated without {((DefinableSimplerInferenceData)proof.Data).RedundantObjects.Count} object(s)",
            _ => proof.Rule.ToString(),
        };

        // ---------- Inline CSS ----------

        private static readonly string Css = """
* { box-sizing: border-box; }
body { font-family: -apple-system, BlinkMacSystemFont, "Segoe UI", system-ui, sans-serif; margin: 0; padding: 0; color: #1f2328; background: #f6f8fa; line-height: 1.5; }
header { position: sticky; top: 0; background: #ffffff; border-bottom: 1px solid #d0d7de; padding: 1rem 1.5rem; z-index: 10; box-shadow: 0 2px 4px rgba(0,0,0,0.04); }
header h1 { margin: 0 0 0.5rem; font-size: 1.25rem; }
#filter { width: 100%; max-width: 480px; padding: 0.5rem 0.75rem; border: 1px solid #d0d7de; border-radius: 6px; font-size: 0.95rem; }
section { background: #ffffff; margin: 1rem 1.5rem; padding: 1rem 1.5rem; border-radius: 6px; border: 1px solid #d0d7de; }
section h2 { margin-top: 0; font-size: 1.05rem; }
.muted { color: #57606a; font-weight: normal; }
pre.config { background: #f6f8fa; padding: 0.75rem; border-radius: 4px; font-size: 0.9rem; overflow-x: auto; margin: 0; }
.summary { display: grid; grid-template-columns: repeat(auto-fit, minmax(160px, 1fr)); gap: 0.75rem; }
.stat { background: #f6f8fa; padding: 0.75rem; border-radius: 4px; }
.stat.full { grid-column: 1 / -1; }
.stat-label { font-size: 0.75rem; text-transform: uppercase; color: #57606a; letter-spacing: 0.05em; }
.stat-value { font-size: 1.15rem; font-weight: 600; margin-top: 0.25rem; word-break: break-word; }
.theorem-list, .unproved-list, .to-prove-list { padding-left: 1.5rem; }
.unproved-item, .to-prove-item { cursor: pointer; padding: 0.15rem 0.3rem; border-radius: 3px; font-family: ui-monospace, SFMono-Regular, Menlo, monospace; font-size: 0.85rem; }
.unproved-item:hover, .to-prove-item:hover { background: #f6f8fa; }
.unproved-item.preview, .to-prove-item.preview { background: #fff8c5; }
.unproved-item.highlight, .to-prove-item.highlight { background: #ffe4e0; }
.theorem-item { margin-bottom: 0.5rem; }
.theorem-item details > summary { cursor: pointer; padding: 0.4rem 0.5rem; border-radius: 4px; }
.theorem-item details > summary:hover { background: #f6f8fa; }
.theorem-item details[open] > summary { background: #ddf4ff; }
.theorem-item.preview > details > summary { background: #fff8c5; }
.theorem-item.highlight > details > summary { background: #ffe4e0; }
.play-btn { background: #1f883d; color: #fff; border: none; border-radius: 3px; padding: 0.05rem 0.4rem; margin-right: 0.4rem; cursor: pointer; font-size: 0.75rem; }
.play-btn:hover { background: #1a7a35; }
.proof-tree { margin: 0.5rem 0 0.5rem 1rem; padding: 0.5rem 0.75rem; border-left: 3px solid #d0d7de; background: #f6f8fa; font-size: 0.9rem; }
.proof-step { padding: 0.15rem 0.3rem; border-radius: 3px; cursor: pointer; }
.proof-step:hover { background: #ffffff; }
.proof-step.preview { background: #fff8c5; }
.proof-step.highlight { background: #ffe4e0; }
.theorem { font-family: ui-monospace, SFMono-Regular, Menlo, monospace; font-size: 0.9em; }
.badge { display: inline-block; padding: 0.05rem 0.4rem; border-radius: 3px; font-size: 0.7rem; font-weight: 600; vertical-align: middle; margin-right: 0.25rem; }
.badge.assumed { background: #dafbe1; color: #0a7c32; }
.badge.trivial { background: #fff8c5; color: #7d4e00; }
.badge.custom { background: #ddf4ff; color: #0969da; }
.badge.reform { background: #e0d6ff; color: #5d3aa1; }
.badge.transit { background: #ffe4e0; color: #b94d2f; }
.badge.sym { background: #fdf0e8; color: #9a4f15; }
.badge.simpler { background: #f0f4ff; color: #4040a0; }
.badge.other { background: #eaeef2; color: #57606a; }
table.trace-table { width: 100%; border-collapse: collapse; font-size: 0.85rem; }
.trace-table th, .trace-table td { padding: 0.4rem 0.6rem; text-align: left; border-bottom: 1px solid #eaeef2; vertical-align: top; }
.trace-table th { background: #f6f8fa; cursor: pointer; user-select: none; position: sticky; top: 90px; }
.trace-table th:hover { background: #eaeef2; }
.trace-table td.num { text-align: right; font-variant-numeric: tabular-nums; color: #57606a; }
.trace-table tbody tr:hover { background: #f6f8fa; }
.filterable.hidden { display: none; }
/* Two-column layout: text content on the left, sticky diagram on the right. Stacks on narrow screens. */
.main-layout { display: grid; grid-template-columns: 1fr; gap: 0; }
@media (min-width: 1100px) { .main-layout { grid-template-columns: minmax(0, 1fr) 520px; } }
.main-content { min-width: 0; }
.diagram-aside { padding: 0 1.5rem 1rem 0; }
@media (max-width: 1099px) { .diagram-aside { padding: 0 1.5rem 1rem 1.5rem; } }
.diagram-sticky { position: sticky; top: 100px; background: #ffffff; padding: 1rem 1.25rem; border-radius: 6px; border: 1px solid #d0d7de; }
.diagram-sticky h2 { margin-top: 0; font-size: 1.05rem; }
.diagram-status { font-size: 0.85rem; color: #57606a; margin-top: 0.5rem; min-height: 2.5em; padding: 0.4rem 0.5rem; background: #f6f8fa; border-radius: 4px; }
.step-controls { margin-top: 0.5rem; display: flex; gap: 0.4rem; align-items: center; flex-wrap: wrap; }
.step-controls button { padding: 0.25rem 0.6rem; border: 1px solid #d0d7de; background: #ffffff; border-radius: 4px; cursor: pointer; font-size: 0.85rem; }
.step-controls button:hover { background: #f6f8fa; }
.step-controls button:disabled { opacity: 0.5; cursor: not-allowed; }
""" + DiagramSvgRenderer.Css;

        // ---------- Inline JS ----------

        private const string Js = """
(function() {
  // ---------- Filter ----------
  const filterInput = document.getElementById('filter');
  const filterables = Array.from(document.querySelectorAll('.filterable'));
  filterInput.addEventListener('input', () => {
    const q = filterInput.value.trim().toLowerCase();
    for (const el of filterables) {
      const text = el.textContent.toLowerCase();
      el.classList.toggle('hidden', q.length > 0 && !text.includes(q));
    }
  });

  // ---------- Sortable trace table ----------
  document.querySelectorAll('table.trace-table').forEach(table => {
    const tbody = table.querySelector('tbody');
    table.querySelectorAll('th').forEach((th, colIdx) => {
      let asc = true;
      th.addEventListener('click', () => {
        const sortType = th.dataset.sort || 'text';
        const rows = Array.from(tbody.rows);
        rows.sort((a, b) => {
          const av = a.cells[colIdx].textContent.trim();
          const bv = b.cells[colIdx].textContent.trim();
          if (sortType === 'num') return (parseFloat(av) - parseFloat(bv)) * (asc ? 1 : -1);
          return av.localeCompare(bv) * (asc ? 1 : -1);
        });
        asc = !asc;
        rows.forEach(r => tbody.appendChild(r));
      });
    });
  });

  // ---------- Diagram highlighting ----------
  const status = document.getElementById('diagram-status');
  const stepControls = document.getElementById('step-controls');
  const stepCounter = document.getElementById('step-counter');
  const stepPrev = document.getElementById('step-prev');
  const stepPlay = document.getElementById('step-play');
  const stepNext = document.getElementById('step-next');
  const stepStop = document.getElementById('step-stop');

  // Index every diagram element by its data-object-id for fast lookup. A single configuration
  // object can produce multiple elements (a point also has a label), and a synthesized segment
  // can carry several alias ids (its canonical id plus an explicit-line id) — split on comma.
  const elementsById = {};
  document.querySelectorAll('.diagram [data-object-id]').forEach(el => {
    const raw = el.getAttribute('data-object-id') || '';
    for (const id of raw.split(',')) {
      if (!id) continue;
      (elementsById[id] = elementsById[id] || []).push(el);
    }
  });

  function applyClass(ids, className, on) {
    if (!ids) return;
    for (const id of ids) {
      const els = elementsById[id];
      if (!els) continue;
      for (const el of els) el.classList.toggle(className, on);
    }
  }

  function clearClass(className) {
    document.querySelectorAll('.diagram .' + className).forEach(el => el.classList.remove(className));
  }

  function setStatus(text) { if (status) status.textContent = text; }

  function parseIds(el) {
    const raw = el.getAttribute('data-highlight') || '';
    return raw.length > 0 ? raw.split(',') : [];
  }

  // Read the theorem-type from a row, return the color class name to apply (or null for no color).
  function colorClassFor(item) {
    const t = item.getAttribute('data-theorem-type');
    return t ? 'color-' + t : null;
  }

  // Track currently-applied color classes so we can clear them when the highlight goes away.
  // We can't just remove "color-X" via knowing X — many rows share the SVG element — so we
  // remove every color-* class via classList iteration.
  function clearColorClasses(ids) {
    if (!ids) return;
    for (const id of ids) {
      const els = elementsById[id];
      if (!els) continue;
      for (const el of els) {
        const toRemove = [];
        for (const cls of el.classList) {
          if (cls.startsWith('color-')) toRemove.push(cls);
        }
        for (const cls of toRemove) el.classList.remove(cls);
      }
    }
  }

  // Hover preview on .highlightable rows.
  document.querySelectorAll('.highlightable').forEach(item => {
    const ids = parseIds(item);
    const colorCls = colorClassFor(item);

    item.addEventListener('mouseenter', () => {
      // Don't preview while step controls are visible — the animation owns the diagram state then.
      if (stepControls && !stepControls.hidden) return;
      applyClass(ids, 'preview', true);
      if (colorCls) applyClass(ids, colorCls, true);
      item.classList.add('preview');
      const headline = item.querySelector('summary')?.textContent?.trim() || item.textContent.trim();
      setStatus('Preview: ' + headline);
    });
    item.addEventListener('mouseleave', () => {
      if (stepControls && !stepControls.hidden) return;
      applyClass(ids, 'preview', false);
      // Don't clear the color class if a sticky highlight is also present on this element —
      // the .highlight class still uses the same color. We achieve this by only clearing color
      // when the element no longer carries .preview AND .highlight. The straightforward way:
      // re-apply for any sticky-highlighted rows after clearing.
      if (colorCls) clearColorClasses(ids);
      reapplyStickyColors();
      item.classList.remove('preview');
      if (!document.querySelector('.theorem-item.highlight, .config-line.highlight, .config-token.highlight'))
        setStatus('Hover or click a theorem to highlight its objects.');
    });

    // Click toggles the sticky highlight.
    const trigger = item.querySelector('summary') || item;
    trigger.addEventListener('click', e => {
      // Don't intercept the play button — its own handler takes over.
      if (e.target.closest('.play-btn')) return;
      // Don't let an inner highlightable (e.g., a config-token inside a config-line) bubble up
      // and trigger the outer line's handler. Stop propagation only when the click target itself
      // is a different .highlightable element.
      const innerHL = e.target.closest('.highlightable');
      if (innerHL && innerHL !== item && item.contains(innerHL)) return;

      const wasHighlighted = item.classList.contains('highlight');
      // Clear all sticky highlights first (single-selection model).
      document.querySelectorAll('.highlightable.highlight').forEach(other => {
        other.classList.remove('highlight');
        const otherIds = parseIds(other);
        applyClass(otherIds, 'highlight', false);
        clearColorClasses(otherIds);
      });
      if (!wasHighlighted) {
        item.classList.add('highlight');
        applyClass(ids, 'highlight', true);
        if (colorCls) applyClass(ids, colorCls, true);
        const headline = (item.querySelector('summary')?.textContent || item.textContent).trim();
        setStatus('Highlighted: ' + headline);
      } else {
        setStatus('Hover or click a theorem to highlight its objects.');
      }
    });
  });

  // Re-apply color classes for any sticky-highlighted row. Used after clearing color from the
  // hovered row so the sticky highlight retains its color.
  function reapplyStickyColors() {
    document.querySelectorAll('.highlightable.highlight').forEach(other => {
      const otherCls = colorClassFor(other);
      if (otherCls) applyClass(parseIds(other), otherCls, true);
    });
  }

  // ---------- Step-through animation ----------
  // Steps are loaded when a theorem's green ▶ is clicked. They stay loaded until another theorem's
  // ▶ is clicked or the page is reloaded — Stop clears the visible state but keeps the loaded
  // sequence so Play can resume from step 0.
  let currentSteps = null;
  let currentIdx = 0;
  let playTimer = null;
  function isPlaying() { return playTimer !== null; }

  function clearStepHighlight() {
    clearClass('step-active');
    // Clear any color classes on diagram elements that were lit by step animation. We only touch
    // elements that ALSO carried a step-active class (which we just cleared), so we don't strip
    // colors from sticky-highlighted elements. Easier: just remove every color-* class — the
    // sticky highlight handler will re-apply them via reapplyStickyColors.
    document.querySelectorAll('.diagram [class*="color-"]').forEach(el => {
      const toRemove = [];
      for (const cls of el.classList) if (cls.startsWith('color-')) toRemove.push(cls);
      for (const cls of toRemove) el.classList.remove(cls);
    });
    // Re-apply colors for any sticky-highlighted row so its color survives the step-clear.
    reapplyStickyColors();
  }

  function showStep(idx) {
    clearStepHighlight();
    if (!currentSteps || idx < 0 || idx >= currentSteps.length) return;
    const step = currentSteps[idx];
    applyClass(step.ids, 'step-active', true);
    // step-active CSS forces amber regardless of color, but we also set the color class so labels
    // (which aren't overridden by step-active) carry the type color.
    if (step.theoremType) applyClass(step.ids, 'color-' + step.theoremType, true);
    setStatus(`Step ${idx + 1}/${currentSteps.length}: ${step.theorem} — ${step.explanation}`);
    if (stepCounter) stepCounter.textContent = `${idx + 1}/${currentSteps.length}`;
    if (stepPrev) stepPrev.disabled = idx === 0;
    if (stepNext) stepNext.disabled = idx === currentSteps.length - 1;
  }

  function pausePlay() {
    if (playTimer) { clearInterval(playTimer); playTimer = null; }
    if (stepPlay) stepPlay.textContent = '▶ Play';
  }

  function startInterval() {
    if (playTimer) return;
    if (stepPlay) stepPlay.textContent = '⏸ Pause';
    playTimer = setInterval(() => {
      if (!currentSteps) { pausePlay(); return; }
      if (currentIdx >= currentSteps.length - 1) { pausePlay(); return; }
      currentIdx++;
      showStep(currentIdx);
    }, 1100);
  }

  // Load a fresh step sequence, reset position, reveal controls, and show the first step.
  function startSteps(steps) {
    pausePlay();
    // Clear any sticky/preview highlights so the animation reads cleanly. Theorem rows are also
    // cleared so the visual "this row is selected" state doesn't compete with the animation.
    clearClass('preview');
    clearClass('highlight');
    document.querySelectorAll('.highlightable.preview, .highlightable.highlight')
      .forEach(el => { el.classList.remove('preview'); el.classList.remove('highlight'); });
    currentSteps = steps;
    currentIdx = 0;
    if (stepControls) stepControls.hidden = false;
    showStep(currentIdx);
  }

  // Stop = pause + clear visible state + reset position + hide controls. We KEEP currentSteps so
  // re-clicking ▶ Play starts the same sequence over from step 0. To load a different sequence,
  // click another theorem's green ▶.
  function stopAnimation() {
    pausePlay();
    clearStepHighlight();
    currentIdx = 0;
    if (stepControls) stepControls.hidden = true;
    setStatus('Hover or click a theorem to highlight its objects.');
  }

  document.querySelectorAll('.play-btn').forEach(btn => {
    btn.addEventListener('click', e => {
      e.preventDefault();
      e.stopPropagation();
      const item = btn.closest('.theorem-item');
      if (!item) return;
      try {
        const steps = JSON.parse(item.getAttribute('data-steps') || '[]');
        if (steps.length === 0) { setStatus('(no steps to play)'); return; }
        startSteps(steps);
      } catch (err) {
        setStatus('Failed to parse steps: ' + err.message);
      }
    });
  });

  if (stepPrev) stepPrev.addEventListener('click', () => {
    if (!currentSteps) return;
    pausePlay();
    if (currentIdx > 0) { currentIdx--; showStep(currentIdx); }
  });
  if (stepNext) stepNext.addEventListener('click', () => {
    if (!currentSteps) return;
    pausePlay();
    if (currentIdx < currentSteps.length - 1) { currentIdx++; showStep(currentIdx); }
  });
  if (stepStop) stepStop.addEventListener('click', stopAnimation);
  if (stepPlay) stepPlay.addEventListener('click', () => {
    if (!currentSteps) return;
    if (isPlaying()) {
      // Currently animating → user wants to pause.
      pausePlay();
    } else {
      // Not animating. If we're at the end, restart from step 0.
      if (currentIdx >= currentSteps.length - 1) { currentIdx = 0; showStep(currentIdx); }
      startInterval();
    }
  });
})();
""";
    }
}
