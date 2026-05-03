#nullable enable
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
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
        // CamelCase naming policy makes PascalCase record parameters serialize as camelCase JSON
        // keys, so ReportSchema.cs needs zero per-property attributes.
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
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
        public static void WriteManifest(string outputFolder, IReadOnlyList<ManifestEntry> scenarios)
        {
            Directory.CreateDirectory(outputFolder);

            var manifest = new Manifest(
                Schema: ReportSchema.SchemaVersion,
                GeneratedAt: DateTime.UtcNow.ToString("O", CultureInfo.InvariantCulture),
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
            // trees as assumptions, in trace events) gets a key "t<n>". Lazy registration —
            // first sight wins.
            var theoremTable = new TheoremTable();

            string Register(Theorem t) => theoremTable.AddIfMissing(t, formatter, idTable);

            foreach (var theorem in proofs.Keys) Register(theorem);
            foreach (var theorem in unprovedTheorems) Register(theorem);

            // Walk proof trees to capture all assumption theorems.
            void RegisterTheoremsInProof(TheoremProof proof)
            {
                Register(proof.Theorem);
                foreach (var child in proof.ProvedAssumptions)
                    RegisterTheoremsInProof(child);
            }
            foreach (var proof in proofs.Values) RegisterTheoremsInProof(proof);

            if (trace is not null)
            {
                foreach (var ev in trace.Events)
                {
                    Register(ev.Theorem);
                    foreach (var assumption in ev.Assumptions) Register(assumption);
                }
            }

            // Proof tree: flatten into a node table keyed by "n<n>". A given TheoremProof instance
            // is its own node — we don't dedupe by theorem because two different paths to the same
            // theorem still produce two different proof nodes in the tree (the renderer's old
            // BuildStepsJson deduplicated by theorem inside one play sequence; the schema preserves
            // the full tree and lets the viewer dedupe at display time).
            var proofNodeTable = new ProofNodeTable();
            var proofRoots = proofs.ToDictionary(
                kv => theoremTable.KeyOf(kv.Key),
                kv => proofNodeTable.Register(kv.Value, theoremTable, idTable, formatter));

            // Trace + rule breakdown default to empty arrays when no trace was captured.
            var traceEvents = trace?.Events ?? (IEnumerable<InferenceEvent>)Array.Empty<InferenceEvent>();

            var traceDto = traceEvents
                .Select(ev => new TraceEventDto(
                    Sequence: ev.Sequence,
                    Rule: ev.Rule.ToString(),
                    CustomRuleName: ev.CustomRule?.ToString(),
                    TheoremKey: theoremTable.KeyOf(ev.Theorem),
                    AssumptionTheoremKeys: ev.Assumptions.Select(theoremTable.KeyOf).ToArray()))
                .ToArray();

            var ruleBreakdown = traceEvents
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

        // ---------- Diagram conversion ----------

        private static DiagramDto BuildDiagram(DiagramModel diagram, ObjectIdTable idTable) =>
            new(
                Bounds: new BoundsDto(diagram.Bounds.MinX, diagram.Bounds.MinY, diagram.Bounds.MaxX, diagram.Bounds.MaxY),
                Points: diagram.Points
                    .Select(p => new DiagramPointDto(idTable.Reuse(p.Id), p.Label, p.X, p.Y))
                    .ToArray(),
                Lines: diagram.Lines
                    .Select(l => new DiagramLineDto(idTable.Reuse(l.Id), l.Label, l.X1, l.Y1, l.X2, l.Y2))
                    .ToArray(),
                Segments: diagram.Segments
                    .Select(s => new DiagramSegmentDto(s.Ids.Select(idTable.Reuse).ToArray(), s.Label, s.X1, s.Y1, s.X2, s.Y2))
                    .ToArray(),
                Circles: diagram.Circles
                    .Select(c => new DiagramCircleDto(idTable.Reuse(c.Id), c.Label, c.Cx, c.Cy, c.R))
                    .ToArray());

        // ---------- Configuration conversion ----------

        private static ConfigurationDto BuildConfiguration(Configuration configuration, OutputFormatter formatter, ObjectIdTable idTable)
        {
            var loose = configuration.LooseObjects
                .Select(o => new LooseObjectDto(idTable.IdFor(o), formatter.GetObjectName(o)))
                .ToArray();

            var constructed = configuration.ConstructedObjects
                .Select(c =>
                {
                    // Build the argument tree and collect every referenced object id in a single
                    // pass — the viewer wants the flat list for click-to-highlight without having
                    // to re-walk the tree itself.
                    var referenced = new HashSet<string>();
                    var args = c.PassedArguments.ArgumentsList
                        .Select(a => BuildArgument(a, idTable, referenced))
                        .ToArray();
                    return new ConstructedObjectDto(
                        Id: idTable.IdFor(c),
                        Label: formatter.GetObjectName(c),
                        ConstructionName: c.Construction.Name,
                        Arguments: args,
                        ReferencedObjectIds: referenced.ToArray());
                })
                .ToArray();

            return new ConfigurationDto(
                Layout: configuration.LooseObjectsHolder.Layout.ToString(),
                LooseObjects: loose,
                Constructed: constructed);
        }

        /// <summary>
        /// Recursively builds an argument node and accumulates referenced object ids in
        /// <paramref name="referenced"/> in the same pass.
        /// </summary>
        private static ArgumentNodeDto BuildArgument(ConstructionArgument argument, ObjectIdTable idTable, HashSet<string> referenced) =>
            argument switch
            {
                ObjectConstructionArgument objArg => new ArgumentNodeDto(
                    Kind: "object",
                    ObjectId: AddAndReturn(referenced, idTable.IdFor(objArg.PassedObject)),
                    Items: null),
                SetConstructionArgument setArg => new ArgumentNodeDto(
                    Kind: "set",
                    ObjectId: null,
                    Items: setArg.PassedArguments.Select(a => BuildArgument(a, idTable, referenced)).ToArray()),
                _ => throw new InvalidOperationException($"Unknown ConstructionArgument type {argument.GetType().Name}"),
            };

        private static string AddAndReturn(HashSet<string> set, string value)
        {
            set.Add(value);
            return value;
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
            // Reverse index: reference-identity hashcode → schema id. Built lazily as IdFor runs,
            // so Reuse can resolve "obj-<hash>" diagram ids in O(1) instead of scanning _byObject.
            private readonly Dictionary<int, string> _idByObjectHash = new();
            private readonly Dictionary<string, string> _byOriginalId = new();
            private int _nextPoint, _nextLine, _nextCircle, _nextOther;
            // One counter for every diagram-alias prefix (s/c/x). Different prefixes guarantee
            // string uniqueness, so a shared counter is safe — just produces s0, c1, s2, c3, etc.
            private int _nextDiagramAlias;

            public ObjectIdTable(Configuration configuration)
            {
                // Pre-allocate ids for every object so the configuration section uses canonical
                // sequential ids regardless of the order subsequent diagram lookups happen in.
                foreach (var obj in configuration.AllObjects)
                    _ = IdFor(obj);
            }

            public string IdFor(ConfigurationObject configurationObject)
            {
                if (_byObject.TryGetValue(configurationObject, out var id))
                    return id;

                var fresh = configurationObject.ObjectType switch
                {
                    ConfigurationObjectType.Point => $"p{_nextPoint++}",
                    ConfigurationObjectType.Line => $"l{_nextLine++}",
                    ConfigurationObjectType.Circle => $"c{_nextCircle++}",
                    _ => $"o{_nextOther++}",
                };
                _byObject[configurationObject] = fresh;
                _idByObjectHash[RuntimeHelpers.GetHashCode(configurationObject)] = fresh;
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

                // "obj-<hash>" — find the matching ConfigurationObject by reference-identity hash.
                if (originalId.StartsWith("obj-", StringComparison.Ordinal)
                    && int.TryParse(originalId.AsSpan("obj-".Length), NumberStyles.Integer, CultureInfo.InvariantCulture, out var hash)
                    && _idByObjectHash.TryGetValue(hash, out var matched))
                {
                    _byOriginalId[originalId] = matched;
                    return matched;
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
                var fresh = $"{prefix}{_nextDiagramAlias++}";
                _byOriginalId[originalId] = fresh;
                return fresh;
            }
        }

        /// <summary>Builds the flat theorem table referenced from every other section.</summary>
        private sealed class TheoremTable
        {
            private readonly Dictionary<Theorem, string> _byTheorem = new();
            private readonly List<TheoremDto> _entries = new();

            public string KeyOf(Theorem theorem) =>
                _byTheorem.TryGetValue(theorem, out var key)
                    ? key
                    : throw new InvalidOperationException("Theorem missing from table — call AddIfMissing first.");

            public IReadOnlyList<TheoremDto> Build() => _entries;

            /// <summary>
            /// Register a theorem if it isn't already in the table, computing its display text and
            /// highlight ids on first sight. Returns the theorem's key.
            /// </summary>
            public string AddIfMissing(Theorem theorem, OutputFormatter formatter, ObjectIdTable idTable)
            {
                if (_byTheorem.TryGetValue(theorem, out var existing))
                    return existing;

                var key = $"t{_byTheorem.Count}";
                _byTheorem[theorem] = key;
                _entries.Add(new TheoremDto(
                    Key: key,
                    Type: theorem.Type.ToString(),
                    Text: formatter.FormatTheorem(theorem),
                    HighlightIds: TheoremHighlights.ObjectIdsFor(theorem)
                        .Select(idTable.Reuse)
                        .ToArray()));
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
                var theoremKey = theoremTable.AddIfMissing(proof.Theorem, formatter, idTable);

                var (customRuleName, redundantObjectIds) = proof.Data switch
                {
                    CustomInferenceData custom => (custom.Rule.ToString(), (IReadOnlyList<string>?)null),
                    DefinableSimplerInferenceData defSimpler => ((string?)null, defSimpler.RedundantObjects.Select(idTable.IdFor).ToArray()),
                    _ => (null, null),
                };

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
                InferenceRuleType.LayoutTheorem => "true by definition of the configuration's layout",
                InferenceRuleType.ReformulatedTheorem => "reformulation via equalities",
                InferenceRuleType.CustomRule => $"custom rule: {((CustomInferenceData)proof.Data).Rule}",
                InferenceRuleType.EqualityTransitivity => "transitivity of equality",
                InferenceRuleType.InferableFromSymmetry => "inferable by symmetry",
                InferenceRuleType.DefinableSimpler => $"can be stated without {((DefinableSimplerInferenceData)proof.Data).RedundantObjects.Count} object(s)",
                _ => proof.Rule.ToString(),
            };
        }
    }
}
