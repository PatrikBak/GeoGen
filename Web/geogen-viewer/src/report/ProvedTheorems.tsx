import { memo, useMemo } from "react";
import type { ScenarioReportT } from "../types";
import { useRowHighlight } from "../highlight/useHighlight";
import { buildProofTreeRows, type ProofTreeRow } from "../proof/stepSequence";
import { badgeClassFor } from "./badge";
import { ProofStep } from "./ProofStep";

interface Props {
  report: ScenarioReportT;
}

export function ProvedTheorems({ report }: Props) {
  // Pre-compute the rendered tree for every proved theorem. The badge count and the rendered
  // rows then come from the same list — they cannot drift. Earlier we computed the badge from
  // a separate dedup-by-theorem walk and rendered with a separate dedup-by-node-key walk, which
  // produced occasional "5 steps · only 1 row" mismatches.
  const proofEntries = useMemo(() => {
    return Object.entries(report.proofs)
      .map(([theoremKey, rootNodeKey]) => {
        const rows = buildProofTreeRows(report, rootNodeKey);
        const rootRow = rows[0];
        if (!rootRow) return null;
        return { theorem: rootRow.theorem, rootNode: rootRow.node, theoremKey, rows };
      })
      .filter(
        (x): x is {
          theorem: ScenarioReportT["theorems"][number];
          rootNode: ScenarioReportT["proofNodes"][number];
          theoremKey: string;
          rows: ProofTreeRow[];
        } => x !== null,
      )
      .sort((a, b) => a.theorem.text.localeCompare(b.theorem.text));
  }, [report]);

  return (
    <section>
      <h2>
        Proved theorems <span className="muted">({proofEntries.length})</span>
      </h2>
      {proofEntries.length === 0 ? (
        <p className="muted">No proved theorems.</p>
      ) : (
        <ol className="theorem-list">
          {proofEntries.map(({ theorem, rootNode, rows }) => (
            <ProvedItem
              key={theorem.key}
              theorem={theorem}
              rootNode={rootNode}
              rows={rows}
            />
          ))}
        </ol>
      )}
    </section>
  );
}

interface ItemProps {
  theorem: ScenarioReportT["theorems"][number];
  rootNode: ScenarioReportT["proofNodes"][number];
  rows: ProofTreeRow[];
}

const ProvedItem = memo(function ProvedItem({ theorem, rootNode, rows }: ItemProps) {
  const { onMouseEnter, onMouseLeave, onClick, setRef } = useRowHighlight(
    theorem.highlightIds,
    theorem.type,
    theorem.text,
  );
  const stepCount = rows.length;

  return (
    <li
      ref={setRef as (el: HTMLLIElement | null) => void}
      data-row=""
      data-theorem-type={theorem.type}
      className="theorem-item"
      onMouseEnter={onMouseEnter}
      onMouseLeave={onMouseLeave}
      onClick={onClick}
    >
      <details>
        <summary>
          <span
            className="step-count-badge"
            data-no-row-click=""
            title={`${stepCount} step${stepCount === 1 ? "" : "s"}`}
          >
            {stepCount} {stepCount === 1 ? "step" : "steps"}
          </span>{" "}
          <span className={badgeClassFor(rootNode.rule)}>{rootNode.rule}</span>{" "}
          {theorem.text}
        </summary>
        <div className="proof-tree">
          {rows.map((row) => (
            <ProofStep key={row.rowKey} row={row} />
          ))}
        </div>
      </details>
    </li>
  );
});
