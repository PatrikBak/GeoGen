import { z } from "zod";

/**
 * Schema version — kept in lockstep with `ReportSchema.SchemaVersion` on the C# side. Both
 * dumper and viewer assert equality, so the moment shapes diverge the failure is loud and
 * pinpoints the exact value mismatch in the upload error message.
 */
export const EXPECTED_SCHEMA = 1;

// ---------- Diagram ----------

const Bounds = z.object({
  minX: z.number(),
  minY: z.number(),
  maxX: z.number(),
  maxY: z.number(),
});

const DiagramPoint = z.object({
  id: z.string(),
  label: z.string(),
  x: z.number(),
  y: z.number(),
});

const DiagramLine = z.object({
  id: z.string(),
  label: z.string(),
  x1: z.number(),
  y1: z.number(),
  x2: z.number(),
  y2: z.number(),
});

const DiagramSegment = z.object({
  // Plural — segments can carry both a canonical "s<n>" id and aliases for explicit
  // configuration objects naming the same line.
  ids: z.array(z.string()),
  label: z.string(),
  x1: z.number(),
  y1: z.number(),
  x2: z.number(),
  y2: z.number(),
});

const DiagramCircle = z.object({
  id: z.string(),
  label: z.string(),
  cx: z.number(),
  cy: z.number(),
  r: z.number(),
});

const Diagram = z.object({
  bounds: Bounds,
  points: z.array(DiagramPoint),
  lines: z.array(DiagramLine),
  segments: z.array(DiagramSegment),
  circles: z.array(DiagramCircle),
});

// ---------- Configuration ----------

// Construction arguments are recursive: an "object" leaf carries an objectId; a "set" node
// carries a list of nested arguments. Zod can't infer recursive types automatically without a
// little ceremony — declare the type, then use z.lazy for the field.
type ArgumentNode = {
  kind: "object" | "set";
  objectId?: string;
  items?: ArgumentNode[];
};
const ArgumentNode: z.ZodType<ArgumentNode> = z.lazy(() =>
  z.object({
    kind: z.union([z.literal("object"), z.literal("set")]),
    objectId: z.string().optional(),
    items: z.array(ArgumentNode).optional(),
  }),
);

const LooseObject = z.object({
  id: z.string(),
  label: z.string(),
});

const ConstructedObject = z.object({
  id: z.string(),
  label: z.string(),
  constructionName: z.string(),
  arguments: z.array(ArgumentNode),
  // Flat list of every objectId mentioned anywhere in the argument tree. Used by click-to-
  // highlight on the configuration line so we don't re-walk the tree at render time.
  referencedObjectIds: z.array(z.string()),
});

const Configuration = z.object({
  layout: z.string(),
  looseObjects: z.array(LooseObject),
  constructed: z.array(ConstructedObject),
});

// ---------- Theorems and proofs ----------

const Theorem = z.object({
  key: z.string(),
  type: z.string(),
  text: z.string(),
  highlightIds: z.array(z.string()),
});

const ProofNode = z.object({
  key: z.string(),
  theoremKey: z.string(),
  rule: z.string(),
  explanation: z.string(),
  customRuleName: z.string().nullable().optional(),
  redundantObjectIds: z.array(z.string()).nullable().optional(),
  assumptionKeys: z.array(z.string()),
});

// ---------- Trace ----------

const TraceEvent = z.object({
  sequence: z.number().int(),
  rule: z.string(),
  customRuleName: z.string().nullable().optional(),
  theoremKey: z.string(),
  assumptionTheoremKeys: z.array(z.string()),
});

const RuleBreakdownEntry = z.object({
  rule: z.string(),
  count: z.number().int(),
});

// ---------- Top-level ----------

export const ScenarioReport = z.object({
  schema: z.number().int(),
  name: z.string(),
  elapsedMs: z.number().int(),
  diagram: Diagram.nullable(),
  configuration: Configuration,
  theorems: z.array(Theorem),
  proofNodes: z.array(ProofNode),
  // Map theoremKey → root proofNode key.
  proofs: z.record(z.string(), z.string()),
  unprovedTheoremKeys: z.array(z.string()),
  trace: z.array(TraceEvent),
  ruleBreakdown: z.array(RuleBreakdownEntry),
});

export const ManifestStats = z.object({
  proved: z.number().int(),
  unproved: z.number().int(),
  traceEvents: z.number().int(),
  elapsedMs: z.number().int(),
});

export const ManifestEntry = z.object({
  name: z.string(),
  file: z.string(),
  stats: ManifestStats,
});

export const Manifest = z.object({
  schema: z.number().int(),
  generatedAt: z.string(),
  scenarios: z.array(ManifestEntry),
});

// ---------- Schema version checks ----------

export class SchemaVersionMismatchError extends Error {
  constructor(public readonly file: string, public readonly actual: number) {
    super(
      `Schema version mismatch in ${file}: expected ${EXPECTED_SCHEMA}, got ${actual}. ` +
        `The dumper and viewer must run with the same schema version. Check the C# ` +
        `ReportSchema.SchemaVersion constant against this viewer's EXPECTED_SCHEMA.`,
    );
    this.name = "SchemaVersionMismatchError";
  }
}

/**
 * Parse + validate a manifest. Throws SchemaVersionMismatchError on version drift, or a
 * ZodError on structural mismatch.
 */
export function parseManifest(raw: unknown, fileName = "manifest.json"): z.infer<typeof Manifest> {
  const parsed = Manifest.parse(raw);
  if (parsed.schema !== EXPECTED_SCHEMA) {
    throw new SchemaVersionMismatchError(fileName, parsed.schema);
  }
  return parsed;
}

/**
 * Parse + validate a scenario report. Throws SchemaVersionMismatchError on version drift.
 */
export function parseScenarioReport(raw: unknown, fileName: string): z.infer<typeof ScenarioReport> {
  const parsed = ScenarioReport.parse(raw);
  if (parsed.schema !== EXPECTED_SCHEMA) {
    throw new SchemaVersionMismatchError(fileName, parsed.schema);
  }
  return parsed;
}
