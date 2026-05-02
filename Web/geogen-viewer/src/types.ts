import type { z } from "zod";
import type { ScenarioReport, Manifest, ManifestEntry, ManifestStats } from "./schema";

export type ScenarioReportT = z.infer<typeof ScenarioReport>;
export type ManifestT = z.infer<typeof Manifest>;
export type ManifestEntryT = z.infer<typeof ManifestEntry>;
export type ManifestStatsT = z.infer<typeof ManifestStats>;

export type DiagramT = ScenarioReportT["diagram"];
export type DiagramPointT = NonNullable<DiagramT>["points"][number];
export type DiagramLineT = NonNullable<DiagramT>["lines"][number];
export type DiagramSegmentT = NonNullable<DiagramT>["segments"][number];
export type DiagramCircleT = NonNullable<DiagramT>["circles"][number];

export type ConfigurationT = ScenarioReportT["configuration"];
export type LooseObjectT = ConfigurationT["looseObjects"][number];
export type ConstructedObjectT = ConfigurationT["constructed"][number];
export type ArgumentNodeT = ConstructedObjectT["arguments"][number];

export type TheoremT = ScenarioReportT["theorems"][number];
export type ProofNodeT = ScenarioReportT["proofNodes"][number];
export type TraceEventT = ScenarioReportT["trace"][number];
export type RuleBreakdownEntryT = ScenarioReportT["ruleBreakdown"][number];

/**
 * The set of inputs the viewer holds in memory after a successful upload — manifest plus a
 * map from scenario name to its parsed report. Looking up by name (rather than by manifest
 * order) keeps the URL-state contract simple: ?scenario=Foo always means the same thing.
 */
export interface UploadedReports {
  manifest: ManifestT;
  scenarios: Map<string, ScenarioReportT>;
}
