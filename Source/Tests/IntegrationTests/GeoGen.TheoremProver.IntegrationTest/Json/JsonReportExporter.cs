#nullable enable
using System.Globalization;
using System.Text.Json;
using GeoGen.Core;
using GeoGen.TheoremProver.IntegrationTest.Diagram;

namespace GeoGen.TheoremProver.IntegrationTest.Json
{
    /// <summary>
    /// Serializes a single proving session into the v1 JSON schema. One file per scenario plus an
    /// index manifest describing all of them. Replaces the inline HTML rendering — the standalone
    /// web viewer in <c>Web/geogen-viewer/</c> consumes this output.
    /// </summary>
    public static class JsonReportExporter
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            WriteIndented = true,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
        };

        /// <summary>
        /// Write one scenario's report to <paramref name="outputFolder"/>/scenarios/&lt;name&gt;.json,
        /// returning a manifest entry the caller can collect into the index.
        /// </summary>
        public static ManifestEntry WriteScenario(
            string outputFolder,
            string scenarioName,
            Configuration configuration,
            OutputFormatter formatter,
            IReadOnlyDictionary<Theorem, TheoremProof> proofs,
            IReadOnlyCollection<Theorem> unprovedTheorems,
            TracedSession? trace,
            long elapsedMs,
            DiagramModel? diagram)
        {
            var scenariosFolder = Path.Combine(outputFolder, "scenarios");
            Directory.CreateDirectory(scenariosFolder);

            var report = BuildScenarioReport(scenarioName, configuration, formatter, proofs, unprovedTheorems, trace, elapsedMs, diagram);

            var fileName = $"{scenarioName}.json";
            var filePath = Path.Combine(scenariosFolder, fileName);
            File.WriteAllText(filePath, JsonSerializer.Serialize(report, JsonOptions));

            return new ManifestEntry(
                Name: scenarioName,
                File: $"scenarios/{fileName}",
                Stats: new ManifestStats(
                    Proved: proofs.Count,
                    Unproved: unprovedTheorems.Count,
                    TraceEvents: trace?.Events.Count ?? 0,
                    ElapsedMs: elapsedMs));
        }

        /// <summary>
        /// Write the top-level <c>manifest.json</c> describing all scenarios in this run.
        /// </summary>
        public static void WriteManifest(string outputFolder, IReadOnlyList<ManifestEntry> scenarios, string? geogenCommit)
        {
            Directory.CreateDirectory(outputFolder);

            var manifest = new Manifest(
                Schema: ReportSchema.SchemaVersion,
                GeneratedAt: DateTime.UtcNow.ToString("O", CultureInfo.InvariantCulture),
                GeogenCommit: geogenCommit,
                Scenarios: scenarios);

            File.WriteAllText(
                Path.Combine(outputFolder, "manifest.json"),
                JsonSerializer.Serialize(manifest, JsonOptions));
        }

        // ---------- Internal: build the scenario DTO from live GeoGen objects ----------

        private static ScenarioReport BuildScenarioReport(
            string scenarioName,
            Configuration configuration,
            OutputFormatter formatter,
            IReadOnlyDictionary<Theorem, TheoremProof> proofs,
            IReadOnlyCollection<Theorem> unprovedTheorems,
            TracedSession? trace,
            long elapsedMs,
            DiagramModel? diagram)
        {
            // Build the dump-local id table: for every ConfigurationObject we want to reference,
            // assign a stable string id of the form "p<n>" / "l<n>" / "c<n>" / "o<n>".
            var idTable = new ObjectIdTable(configuration);

            // Convert the diagram model. Diagram object ids in the existing model use
            // "obj-<hashcode>" / "seg-..." / "cir-..." formats; we re-key them through ObjectIdTable
            // so the JSON uses the same opaque ids for both diagram and configuration references.
            var diagramDto = diagram is null ? null : BuildDiagram(diagram, idTable);

            // Configuration: walk loose + constructed objects, recursively encoding arguments.
            var configurationDto = BuildConfiguration(configuration, formatter, idTable);

            // Theorem table: every distinct theorem mentioned anywhere (proved, unproved, in proof
            // trees as assumptions, in trace events) gets a key "t<n>". TheoremHighlights is reused.
            // We register lazily on first sight via TheoremDetails.AddIfMissing, which formats the
            // text and computes highlight ids in the same call.
            var theoremTable = new TheoremTable();
            foreach (var theorem in proofs.Keys)
                TheoremDetails.AddIfMissing(theoremTable, idTable, formatter, theorem);
            foreach (var theorem in unprovedTheorems)
                TheoremDetails.AddIfMissing(theoremTable, idTable, formatter, theorem);
            // Walk proof trees to capture all assumption theorems.
            foreach (var proof in proofs.Values)
                RegisterTheoremsInProof(proof, theoremTable, idTable, formatter);
            if (trace is not null)
            {
                foreach (var ev in trace.Events)
                {
                    TheoremDetails.AddIfMissing(theoremTable, idTable, formatter, ev.Theorem);
                    foreach (var assumption in ev.Assumptions)
                        TheoremDetails.AddIfMissing(theoremTable, idTable, formatter, assumption);
                }
            }

            // Proof tree: flatten into a node table keyed by "n<n>". A given TheoremProof instance
            // is its own node — we don't dedupe by theorem because two different paths to the same
            // theorem still produce two different proof nodes in the tree (the renderer's old
            // BuildStepsJson deduplicated by theorem inside one play sequence; the schema preserves
            // the full tree and lets the viewer dedupe at display time).
            var proofNodeTable = new ProofNodeTable();
            var proofRoots = new Dictionary<string, string>();
            foreach (var (theorem, proof) in proofs)
            {
                var rootKey = proofNodeTable.Register(proof, theoremTable, idTable, formatter);
                proofRoots[theoremTable.KeyOf(theorem)] = rootKey;
            }

            // Build the trace DTO if a session was captured.
            var traceDto = trace is null
                ? Array.Empty<TraceEventDto>()
                : trace.Events.Select(ev => new TraceEventDto(
                    Sequence: ev.Sequence,
                    Rule: ev.Rule.ToString(),
                    CustomRuleName: ev.CustomRule?.ToString(),
                    TheoremKey: theoremTable.KeyOf(ev.Theorem),
                    AssumptionTheoremKeys: ev.Assumptions.Select(theoremTable.KeyOf).ToArray())).ToArray();

            // Rule breakdown is computed from trace; defaults to empty when no trace was captured.
            var ruleBreakdown = trace is null
                ? Array.Empty<RuleBreakdownEntry>()
                : trace.Events
                    .GroupBy(ev => ev.Rule)
                    .OrderByDescending(g => g.Count())
                    .Select(g => new RuleBreakdownEntry(g.Key.ToString(), g.Count()))
                    .ToArray();

            return new ScenarioReport(
                Schema: ReportSchema.SchemaVersion,
                Name: scenarioName,
                ElapsedMs: elapsedMs,
                Diagram: diagramDto,
                Configuration: configurationDto,
                Theorems: theoremTable.Build(),
                ProofNodes: proofNodeTable.Build(),
                Proofs: proofRoots,
                UnprovedTheoremKeys: unprovedTheorems.Select(theoremTable.KeyOf).ToArray(),
                Trace: traceDto,
                RuleBreakdown: ruleBreakdown);
        }

        private static void RegisterTheoremsInProof(TheoremProof proof, TheoremTable table, ObjectIdTable idTable, OutputFormatter formatter)
        {
            TheoremDetails.AddIfMissing(table, idTable, formatter, proof.Theorem);
            foreach (var child in proof.ProvedAssumptions)
                RegisterTheoremsInProof(child, table, idTable, formatter);
        }

        // ---------- Diagram conversion ----------

        private static DiagramDto BuildDiagram(DiagramModel diagram, ObjectIdTable idTable)
        {
            var bounds = new BoundsDto(diagram.Bounds.MinX, diagram.Bounds.MinY, diagram.Bounds.MaxX, diagram.Bounds.MaxY);

            var points = diagram.Points
                .Select(p => new DiagramPointDto(
                    Id: idTable.Reuse(p.Id),
                    Label: p.Label,
                    X: p.X,
                    Y: p.Y))
                .ToArray();

            var lines = diagram.Lines
                .Select(l => new DiagramLineDto(
                    Id: idTable.Reuse(l.Id),
                    Label: l.Label,
                    X1: l.X1, Y1: l.Y1, X2: l.X2, Y2: l.Y2))
                .ToArray();

            var segments = diagram.Segments
                .Select(s => new DiagramSegmentDto(
                    Ids: s.Ids.Select(idTable.Reuse).ToArray(),
                    Label: s.Label,
                    X1: s.X1, Y1: s.Y1, X2: s.X2, Y2: s.Y2))
                .ToArray();

            var circles = diagram.Circles
                .Select(c => new DiagramCircleDto(
                    Id: idTable.Reuse(c.Id),
                    Label: c.Label,
                    Cx: c.Cx, Cy: c.Cy, R: c.R))
                .ToArray();

            return new DiagramDto(bounds, points, lines, segments, circles);
        }

        // ---------- Configuration conversion ----------

        private static ConfigurationDto BuildConfiguration(Configuration configuration, OutputFormatter formatter, ObjectIdTable idTable)
        {
            var loose = configuration.LooseObjects
                .Select(o => new LooseObjectDto(
                    Id: idTable.IdFor(o),
                    Label: formatter.GetObjectName(o)))
                .ToArray();

            var constructed = configuration.ConstructedObjects
                .Select(c =>
                {
                    var args = c.PassedArguments.ArgumentsList.Select(a => BuildArgument(a, idTable)).ToArray();
                    var referenced = new List<string>();
                    foreach (var arg in c.PassedArguments.ArgumentsList)
                        CollectReferencedIds(arg, idTable, referenced);
                    return new ConstructedObjectDto(
                        Id: idTable.IdFor(c),
                        Label: formatter.GetObjectName(c),
                        ConstructionName: c.Construction.Name,
                        Arguments: args,
                        ReferencedObjectIds: referenced.Distinct().ToArray());
                })
                .ToArray();

            return new ConfigurationDto(
                Layout: configuration.LooseObjectsHolder.Layout.ToString(),
                LooseObjects: loose,
                Constructed: constructed);
        }

        private static ArgumentNodeDto BuildArgument(ConstructionArgument argument, ObjectIdTable idTable)
        {
            switch (argument)
            {
                case ObjectConstructionArgument objArg:
                    return new ArgumentNodeDto(
                        Kind: "object",
                        ObjectId: idTable.IdFor(objArg.PassedObject),
                        Items: null);
                case SetConstructionArgument setArg:
                    return new ArgumentNodeDto(
                        Kind: "set",
                        ObjectId: null,
                        Items: setArg.PassedArguments.Select(a => BuildArgument(a, idTable)).ToArray());
                default:
                    throw new InvalidOperationException($"Unknown ConstructionArgument type {argument.GetType().Name}");
            }
        }

        private static void CollectReferencedIds(ConstructionArgument argument, ObjectIdTable idTable, List<string> into)
        {
            switch (argument)
            {
                case ObjectConstructionArgument objArg:
                    into.Add(idTable.IdFor(objArg.PassedObject));
                    break;
                case SetConstructionArgument setArg:
                    foreach (var inner in setArg.PassedArguments)
                        CollectReferencedIds(inner, idTable, into);
                    break;
            }
        }

        // ---------- Helper tables ----------

        /// <summary>
        /// Maps every <see cref="ConfigurationObject"/> we encounter (and every diagram-element id)
        /// to a stable opaque dump-local string id. Two paths feed it: the diagram, which produces
        /// "obj-..."/"seg-..."/"cir-..." ids that we rekey into "p"/"s"/"c"-prefixed sequential ids
        /// (preserving the alias property — segments can carry multiple ids); and the configuration
        /// walk, which calls IdFor on each ConfigurationObject reference.
        /// </summary>
        private sealed class ObjectIdTable
        {
            private readonly Dictionary<ConfigurationObject, string> _byObject = new();
            private readonly Dictionary<string, string> _byOriginalId = new();
            private int _nextPoint, _nextLine, _nextCircle, _nextOther, _nextSeg;

            public ObjectIdTable(Configuration configuration)
            {
                // Pre-allocate ids for every object known to the configuration so the schema's
                // configuration section uses canonical ids before any diagram lookup happens.
                foreach (var obj in configuration.AllObjects)
                    _ = IdFor(obj);
            }

            public string IdFor(ConfigurationObject configurationObject)
            {
                if (_byObject.TryGetValue(configurationObject, out var id))
                    return id;
                var prefix = configurationObject.ObjectType switch
                {
                    ConfigurationObjectType.Point => "p",
                    ConfigurationObjectType.Line => "l",
                    ConfigurationObjectType.Circle => "c",
                    _ => "o",
                };
                var fresh = configurationObject.ObjectType switch
                {
                    ConfigurationObjectType.Point => $"{prefix}{_nextPoint++}",
                    ConfigurationObjectType.Line => $"{prefix}{_nextLine++}",
                    ConfigurationObjectType.Circle => $"{prefix}{_nextCircle++}",
                    _ => $"{prefix}{_nextOther++}",
                };
                _byObject[configurationObject] = fresh;
                return fresh;
            }

            /// <summary>
            /// Resolve a diagram-original id (e.g., "obj-12345" or "seg-12345-67890") to its
            /// canonical schema id. ConfigurationObject ids that came from
            /// <see cref="DiagramBuilder.ObjectId(ConfigurationObject)"/> need re-keying via the
            /// configuration's reference-identity hash; segment/circle ids get fresh sequential ids.
            /// </summary>
            public string Reuse(string originalId)
            {
                if (_byOriginalId.TryGetValue(originalId, out var existing))
                    return existing;

                // Object ids are the dominant case — try to find the matching ConfigurationObject
                // by reference-identity hash. The diagram produces "obj-<hash>" via
                // RuntimeHelpers.GetHashCode; we walk our id table looking for a match.
                if (originalId.StartsWith("obj-", StringComparison.Ordinal))
                {
                    var hashStr = originalId.Substring("obj-".Length);
                    if (int.TryParse(hashStr, NumberStyles.Integer, CultureInfo.InvariantCulture, out var hash))
                    {
                        foreach (var (obj, id) in _byObject)
                        {
                            if (System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(obj) == hash)
                            {
                                _byOriginalId[originalId] = id;
                                return id;
                            }
                        }
                    }
                }

                // Segment / circle / unknown ids: assign a fresh sequential schema id. The prefix
                // marks the kind so the viewer can tell them apart at a glance, though it doesn't
                // need to (everything is opaque from the viewer's POV).
                var prefix = originalId switch
                {
                    var s when s.StartsWith("seg-", StringComparison.Ordinal) => "s",
                    var s when s.StartsWith("cir-", StringComparison.Ordinal) => "c",
                    _ => "x",
                };
                var fresh = $"{prefix}{_nextSeg++}";
                _byOriginalId[originalId] = fresh;
                return fresh;
            }
        }

        /// <summary>Builds the flat theorem table referenced from every other section.</summary>
        private sealed class TheoremTable
        {
            private readonly Dictionary<Theorem, string> _byTheorem = new();
            private readonly List<TheoremDto> _entries = new();

            public string KeyOf(Theorem theorem)
            {
                if (_byTheorem.TryGetValue(theorem, out var key))
                    return key;
                throw new InvalidOperationException("Theorem missing from table — call AddWithDetails first.");
            }

            public IReadOnlyList<TheoremDto> Build() => _entries;

            /// <summary>
            /// Insert a theorem with already-computed text and highlight ids. Returns its key.
            /// </summary>
            public string AddWithDetails(Theorem theorem, string text, IReadOnlyList<string> highlightIds)
            {
                if (_byTheorem.TryGetValue(theorem, out var existing))
                    return existing;

                var key = $"t{_byTheorem.Count}";
                _byTheorem[theorem] = key;
                _entries.Add(new TheoremDto(
                    Key: key,
                    Type: theorem.Type.ToString(),
                    Text: text,
                    HighlightIds: highlightIds));
                return key;
            }
        }

        /// <summary>
        /// Flattens proof trees. Each TheoremProof instance becomes one ProofNodeDto; recursive
        /// children populate <c>assumptionKeys</c>. We do NOT dedupe by Theorem at this layer —
        /// the same theorem may appear in two proofs through different rules; both nodes are kept.
        /// </summary>
        private sealed class ProofNodeTable
        {
            private readonly List<ProofNodeDto> _entries = new();

            public string Register(TheoremProof proof, TheoremTable theoremTable, ObjectIdTable idTable, OutputFormatter formatter)
            {
                // Walk children first so node keys reflect post-order (leaves before roots).
                var assumptionKeys = proof.ProvedAssumptions
                    .Select(child => Register(child, theoremTable, idTable, formatter))
                    .ToArray();

                var key = $"n{_entries.Count}";
                var theoremKey = TheoremDetails.AddIfMissing(theoremTable, idTable, formatter, proof.Theorem);

                string? customRuleName = null;
                IReadOnlyList<string>? redundantObjectIds = null;
                switch (proof.Data)
                {
                    case CustomInferenceData custom:
                        customRuleName = custom.Rule.ToString();
                        break;
                    case DefinableSimplerInferenceData defSimpler:
                        redundantObjectIds = defSimpler.RedundantObjects
                            .Select(idTable.IdFor)
                            .ToArray();
                        break;
                }

                _entries.Add(new ProofNodeDto(
                    Key: key,
                    TheoremKey: theoremKey,
                    Rule: proof.Rule.ToString(),
                    Explanation: ExplainRule(proof),
                    CustomRuleName: customRuleName,
                    RedundantObjectIds: redundantObjectIds,
                    AssumptionKeys: assumptionKeys));

                return key;
            }

            public IReadOnlyList<ProofNodeDto> Build() => _entries;

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
        }

        /// <summary>
        /// Convenience: register a theorem (computing text + highlight ids) if it isn't already in
        /// the table. Used by both proof-tree walking and direct insertion paths.
        /// </summary>
        private static class TheoremDetails
        {
            public static string AddIfMissing(TheoremTable table, ObjectIdTable idTable, OutputFormatter formatter, Theorem theorem)
            {
                var text = formatter.FormatTheorem(theorem);
                var highlightIds = TheoremHighlights.ObjectIdsFor(theorem)
                    .Select(originalId => MapHighlightId(originalId, idTable))
                    .ToArray();
                return table.AddWithDetails(theorem, text, highlightIds);
            }

            private static string MapHighlightId(string originalId, ObjectIdTable idTable)
            {
                // TheoremHighlights produces "obj-<hash>" ids and "seg-..." / "cir-..." ids the same
                // way the diagram does. Reuse() handles both, mapping them to the canonical ids.
                return idTable.Reuse(originalId);
            }
        }
    }

}
