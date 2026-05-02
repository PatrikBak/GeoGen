import type { ProofNodeT, ScenarioReportT, TheoremT } from "../types";

/**
 * One step in a proof animation. Mirrors the shape produced by the original C# `BuildStepsJson`
 * but resolves theorem keys to display strings up front so the StepControls don't need to query
 * the report.
 */
export interface ProofStep {
  theoremText: string;
  theoremType: string;
  rule: string;
  explanation: string;
  ids: string[];
}

/**
 * Walk a proof tree DFS bottom-up starting from `rootKey`, deduplicating by theorem so a theorem
 * cited multiple times only appears once. Order matches the original behavior: leaves are shown
 * first, the conclusion last.
 */
export function buildProofSteps(report: ScenarioReportT, rootKey: string): ProofStep[] {
  const proofNodesByKey = new Map<string, ProofNodeT>();
  for (const node of report.proofNodes) proofNodesByKey.set(node.key, node);

  const theoremsByKey = new Map<string, TheoremT>();
  for (const theorem of report.theorems) theoremsByKey.set(theorem.key, theorem);

  const steps: ProofStep[] = [];
  const seenTheorems = new Set<string>();

  function visit(nodeKey: string) {
    const node = proofNodesByKey.get(nodeKey);
    if (!node) return;
    if (seenTheorems.has(node.theoremKey)) return;

    // Children first so the animation builds bottom-up.
    for (const child of node.assumptionKeys) visit(child);

    seenTheorems.add(node.theoremKey);

    const theorem = theoremsByKey.get(node.theoremKey);
    if (!theorem) return;
    steps.push({
      theoremText: theorem.text,
      theoremType: theorem.type,
      rule: node.rule,
      explanation: node.explanation,
      ids: theorem.highlightIds.slice(),
    });
  }

  visit(rootKey);
  return steps;
}

/**
 * One row in the rendered proof tree. The viewer's `ProofStep` component maps a list of these
 * directly to JSX. Computing them as data first eliminates the strict-mode double-render bug we
 * had when mutating a shared `seen` Set during render, and makes the badge count match the row
 * count by construction (one row per entry).
 */
export interface ProofTreeRow {
  /** Stable id for React keys: nodeKey plus an occurrence suffix when the same node renders twice. */
  rowKey: string;
  nodeKey: string;
  /** Indent level — root is 0. */
  depth: number;
  node: ProofNodeT;
  theorem: TheoremT;
  /**
   * True when this is a repeat citation of a node we already showed. The row is rendered (so the
   * dependency is visible) but children are NOT recursed into a second time.
   */
  alreadyVisited: boolean;
}

/**
 * Pre-compute the list of rows the proof tree should render, applying the same dedup rule as
 * the recursive component used to (by node-key). Pure function — no side effects, no React state.
 *
 * Returns the rows in display order (root first, then DFS preorder of children). The component
 * maps over this directly using `depth` for indentation.
 */
export function buildProofTreeRows(
  report: ScenarioReportT,
  rootKey: string,
): ProofTreeRow[] {
  const nodeByKey = new Map<string, ProofNodeT>();
  for (const n of report.proofNodes) nodeByKey.set(n.key, n);
  const theoremByKey = new Map<string, TheoremT>();
  for (const t of report.theorems) theoremByKey.set(t.key, t);

  const rows: ProofTreeRow[] = [];
  const seen = new Set<string>();

  function visit(nodeKey: string, depth: number) {
    const node = nodeByKey.get(nodeKey);
    if (!node) return;
    const theorem = theoremByKey.get(node.theoremKey);
    if (!theorem) return;

    const alreadyVisited = seen.has(nodeKey);
    rows.push({
      rowKey: alreadyVisited ? `${nodeKey}-repeat-${rows.length}` : nodeKey,
      nodeKey,
      depth,
      node,
      theorem,
      alreadyVisited,
    });
    if (alreadyVisited) return;
    seen.add(nodeKey);
    for (const childKey of node.assumptionKeys) visit(childKey, depth + 1);
  }

  visit(rootKey, 0);
  return rows;
}
