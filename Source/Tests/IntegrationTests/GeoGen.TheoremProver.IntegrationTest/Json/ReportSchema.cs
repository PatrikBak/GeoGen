#nullable enable
namespace GeoGen.TheoremProver.IntegrationTest.Json
{
    /// <summary>
    /// Versioned JSON schema for the GeoGen prover report. The C# dumper emits these records;
    /// the standalone web viewer in <c>Web/geogen-viewer/</c> consumes them via Zod schemas in
    /// <c>src/schema.ts</c> that mirror this shape exactly. Both sides check
    /// <see cref="SchemaVersion"/> on every read; mismatches cause loud failure.
    /// <para>
    /// Property names serialize as lowerCamelCase via <c>JsonNamingPolicy.CamelCase</c> on the
    /// dumper's <c>JsonSerializerOptions</c> — so PascalCase record parameters here become
    /// camelCase JSON keys automatically; no per-property attributes needed.
    /// </para>
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
        int Schema,
        string GeneratedAt,
        IReadOnlyList<ManifestEntry> Scenarios);

    public sealed record ManifestEntry(
        string Name,
        string File,
        ManifestStats Stats);

    public sealed record ManifestStats(
        int Proved,
        int Unproved,
        int TraceEvents,
        long ElapsedMs);

    /// <summary>One scenario's full report. Self-contained; the viewer only needs this file plus the manifest.</summary>
    public sealed record ScenarioReport(
        int Schema,
        string Name,
        long ElapsedMs,
        DiagramDto? Diagram,
        ConfigurationDto Configuration,
        IReadOnlyList<TheoremDto> Theorems,
        IReadOnlyList<ProofNodeDto> ProofNodes,
        IReadOnlyDictionary<string, string> Proofs,
        IReadOnlyList<string> UnprovedTheoremKeys,
        IReadOnlyList<TraceEventDto> Trace,
        IReadOnlyList<RuleBreakdownEntry> RuleBreakdown);

    // ---------- Diagram ----------

    public sealed record DiagramDto(
        BoundsDto Bounds,
        IReadOnlyList<DiagramPointDto> Points,
        IReadOnlyList<DiagramLineDto> Lines,
        IReadOnlyList<DiagramSegmentDto> Segments,
        IReadOnlyList<DiagramCircleDto> Circles);

    public sealed record BoundsDto(double MinX, double MinY, double MaxX, double MaxY);

    public sealed record DiagramPointDto(string Id, string Label, double X, double Y);

    public sealed record DiagramLineDto(string Id, string Label, double X1, double Y1, double X2, double Y2);

    /// <summary>
    /// Synthesized point-to-point segment. Carries multiple ids — the canonical seg-id for
    /// theorem highlights plus aliases for any explicit configuration object naming the same line.
    /// </summary>
    public sealed record DiagramSegmentDto(
        IReadOnlyList<string> Ids,
        string Label,
        double X1, double Y1, double X2, double Y2);

    public sealed record DiagramCircleDto(string Id, string Label, double Cx, double Cy, double R);

    // ---------- Configuration ----------

    public sealed record ConfigurationDto(
        string Layout,
        IReadOnlyList<LooseObjectDto> LooseObjects,
        IReadOnlyList<ConstructedObjectDto> Constructed);

    public sealed record LooseObjectDto(string Id, string Label);

    public sealed record ConstructedObjectDto(
        string Id,
        string Label,
        string ConstructionName,
        IReadOnlyList<ArgumentNodeDto> Arguments,
        IReadOnlyList<string> ReferencedObjectIds);

    /// <summary>
    /// Discriminated union: <c>kind</c> is "object" or "set". Object args carry <c>objectId</c>;
    /// set args carry nested <c>items</c>. Mirrors GeoGen's <c>ObjectConstructionArgument</c> vs
    /// <c>SetConstructionArgument</c>.
    /// </summary>
    public sealed record ArgumentNodeDto(
        string Kind,
        string? ObjectId,
        IReadOnlyList<ArgumentNodeDto>? Items);

    // ---------- Theorems and proofs ----------

    public sealed record TheoremDto(
        string Key,
        string Type,
        string Text,
        IReadOnlyList<string> HighlightIds);

    public sealed record ProofNodeDto(
        string Key,
        string TheoremKey,
        string Rule,
        string Explanation,
        string? CustomRuleName,
        IReadOnlyList<string>? RedundantObjectIds,
        IReadOnlyList<string> AssumptionKeys);

    // ---------- Trace ----------

    public sealed record TraceEventDto(
        int Sequence,
        string Rule,
        string? CustomRuleName,
        string TheoremKey,
        IReadOnlyList<string> AssumptionTheoremKeys);

    public sealed record RuleBreakdownEntry(string Rule, int Count);
}
