#nullable enable
using System.Text.Json.Serialization;

namespace GeoGen.TheoremProver.IntegrationTest.Json
{
    /// <summary>
    /// Versioned JSON schema for the GeoGen prover report. The C# dumper emits these records;
    /// the standalone web viewer in <c>Web/geogen-viewer/</c> consumes them via Zod schemas in
    /// <c>src/schema.ts</c> that mirror this shape exactly. Both sides check
    /// <see cref="SchemaVersion"/> on every read; mismatches cause loud failure.
    /// <para>
    /// When the shape changes:
    /// <list type="bullet">
    /// <item>Adding a new optional field is non-breaking — leave <see cref="SchemaVersion"/> as is.</item>
    /// <item>Removing or renaming a field, or making an optional field required, bumps <see cref="SchemaVersion"/>.</item>
    /// </list>
    /// </para>
    /// </summary>
    public static class ReportSchema
    {
        /// <summary>Schema version. Bump on breaking changes; both dumper and viewer assert equality.</summary>
        public const int SchemaVersion = 1;
    }

    /// <summary>The top-level <c>manifest.json</c> emitted alongside per-scenario files.</summary>
    public sealed record Manifest(
        [property: JsonPropertyName("schema")] int Schema,
        [property: JsonPropertyName("generatedAt")] string GeneratedAt,
        [property: JsonPropertyName("geogenCommit")] string? GeogenCommit,
        [property: JsonPropertyName("scenarios")] IReadOnlyList<ManifestEntry> Scenarios);

    public sealed record ManifestEntry(
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("file")] string File,
        [property: JsonPropertyName("stats")] ManifestStats Stats);

    public sealed record ManifestStats(
        [property: JsonPropertyName("proved")] int Proved,
        [property: JsonPropertyName("unproved")] int Unproved,
        [property: JsonPropertyName("traceEvents")] int TraceEvents,
        [property: JsonPropertyName("elapsedMs")] long ElapsedMs);

    /// <summary>One scenario's full report. Self-contained; the viewer only needs this file plus the manifest.</summary>
    public sealed record ScenarioReport(
        [property: JsonPropertyName("schema")] int Schema,
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("elapsedMs")] long ElapsedMs,
        [property: JsonPropertyName("diagram")] DiagramDto? Diagram,
        [property: JsonPropertyName("configuration")] ConfigurationDto Configuration,
        [property: JsonPropertyName("theorems")] IReadOnlyList<TheoremDto> Theorems,
        [property: JsonPropertyName("proofNodes")] IReadOnlyList<ProofNodeDto> ProofNodes,
        [property: JsonPropertyName("proofs")] IReadOnlyDictionary<string, string> Proofs,
        [property: JsonPropertyName("unprovedTheoremKeys")] IReadOnlyList<string> UnprovedTheoremKeys,
        [property: JsonPropertyName("trace")] IReadOnlyList<TraceEventDto> Trace,
        [property: JsonPropertyName("ruleBreakdown")] IReadOnlyList<RuleBreakdownEntry> RuleBreakdown);

    // ---------- Diagram ----------

    public sealed record DiagramDto(
        [property: JsonPropertyName("bounds")] BoundsDto Bounds,
        [property: JsonPropertyName("points")] IReadOnlyList<DiagramPointDto> Points,
        [property: JsonPropertyName("lines")] IReadOnlyList<DiagramLineDto> Lines,
        [property: JsonPropertyName("segments")] IReadOnlyList<DiagramSegmentDto> Segments,
        [property: JsonPropertyName("circles")] IReadOnlyList<DiagramCircleDto> Circles);

    public sealed record BoundsDto(
        [property: JsonPropertyName("minX")] double MinX,
        [property: JsonPropertyName("minY")] double MinY,
        [property: JsonPropertyName("maxX")] double MaxX,
        [property: JsonPropertyName("maxY")] double MaxY);

    public sealed record DiagramPointDto(
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("label")] string Label,
        [property: JsonPropertyName("x")] double X,
        [property: JsonPropertyName("y")] double Y);

    public sealed record DiagramLineDto(
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("label")] string Label,
        [property: JsonPropertyName("x1")] double X1,
        [property: JsonPropertyName("y1")] double Y1,
        [property: JsonPropertyName("x2")] double X2,
        [property: JsonPropertyName("y2")] double Y2);

    /// <summary>
    /// Synthesized point-to-point segment. Carries multiple ids — the canonical seg-id for
    /// theorem highlights plus aliases for any explicit configuration object naming the same line.
    /// </summary>
    public sealed record DiagramSegmentDto(
        [property: JsonPropertyName("ids")] IReadOnlyList<string> Ids,
        [property: JsonPropertyName("label")] string Label,
        [property: JsonPropertyName("x1")] double X1,
        [property: JsonPropertyName("y1")] double Y1,
        [property: JsonPropertyName("x2")] double X2,
        [property: JsonPropertyName("y2")] double Y2);

    public sealed record DiagramCircleDto(
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("label")] string Label,
        [property: JsonPropertyName("cx")] double Cx,
        [property: JsonPropertyName("cy")] double Cy,
        [property: JsonPropertyName("r")] double R);

    // ---------- Configuration ----------

    public sealed record ConfigurationDto(
        [property: JsonPropertyName("layout")] string Layout,
        [property: JsonPropertyName("looseObjects")] IReadOnlyList<LooseObjectDto> LooseObjects,
        [property: JsonPropertyName("constructed")] IReadOnlyList<ConstructedObjectDto> Constructed);

    public sealed record LooseObjectDto(
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("label")] string Label);

    public sealed record ConstructedObjectDto(
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("label")] string Label,
        [property: JsonPropertyName("constructionName")] string ConstructionName,
        [property: JsonPropertyName("arguments")] IReadOnlyList<ArgumentNodeDto> Arguments,
        [property: JsonPropertyName("referencedObjectIds")] IReadOnlyList<string> ReferencedObjectIds);

    /// <summary>
    /// Discriminated union: <c>kind</c> is "object" or "set". Object args carry <c>objectId</c>;
    /// set args carry nested <c>items</c>. Mirrors GeoGen's <c>ObjectConstructionArgument</c> vs
    /// <c>SetConstructionArgument</c>.
    /// </summary>
    public sealed record ArgumentNodeDto(
        [property: JsonPropertyName("kind")] string Kind,
        [property: JsonPropertyName("objectId")] string? ObjectId,
        [property: JsonPropertyName("items")] IReadOnlyList<ArgumentNodeDto>? Items);

    // ---------- Theorems and proofs ----------

    public sealed record TheoremDto(
        [property: JsonPropertyName("key")] string Key,
        [property: JsonPropertyName("type")] string Type,
        [property: JsonPropertyName("text")] string Text,
        [property: JsonPropertyName("highlightIds")] IReadOnlyList<string> HighlightIds);

    public sealed record ProofNodeDto(
        [property: JsonPropertyName("key")] string Key,
        [property: JsonPropertyName("theoremKey")] string TheoremKey,
        [property: JsonPropertyName("rule")] string Rule,
        [property: JsonPropertyName("explanation")] string Explanation,
        [property: JsonPropertyName("customRuleName")] string? CustomRuleName,
        [property: JsonPropertyName("redundantObjectIds")] IReadOnlyList<string>? RedundantObjectIds,
        [property: JsonPropertyName("assumptionKeys")] IReadOnlyList<string> AssumptionKeys);

    // ---------- Trace ----------

    public sealed record TraceEventDto(
        [property: JsonPropertyName("sequence")] int Sequence,
        [property: JsonPropertyName("rule")] string Rule,
        [property: JsonPropertyName("customRuleName")] string? CustomRuleName,
        [property: JsonPropertyName("theoremKey")] string TheoremKey,
        [property: JsonPropertyName("assumptionTheoremKeys")] IReadOnlyList<string> AssumptionTheoremKeys);

    public sealed record RuleBreakdownEntry(
        [property: JsonPropertyName("rule")] string Rule,
        [property: JsonPropertyName("count")] int Count);
}
