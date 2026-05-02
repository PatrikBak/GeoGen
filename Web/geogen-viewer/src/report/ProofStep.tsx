import { memo } from "react";
import type { ProofTreeRow } from "../proof/stepSequence";
import { useRowHighlight } from "../highlight/useHighlight";
import { badgeClassFor } from "./badge";

interface Props {
  row: ProofTreeRow;
}

/**
 * Render one row of the pre-computed proof tree. The flat-row approach replaces an earlier
 * recursive component that mutated a shared `seen` Set during render — that worked in
 * production but failed under React strict mode, where the second render saw an already-full
 * Set and suppressed every child, so the badge ("5 steps") and the rendered tree (1 row)
 * disagreed.
 *
 * `buildProofTreeRows()` does the dedup once as a pure function, so the badge counts the
 * returned list length and the renderer maps over the same list — guaranteed in sync.
 */
export const ProofStep = memo(function ProofStep({ row }: Props) {
  const { node, theorem, depth, alreadyVisited } = row;
  const { onMouseEnter, onMouseLeave, onClick, setRef } = useRowHighlight(
    theorem.highlightIds,
    theorem.type,
    theorem.text,
  );

  return (
    <div
      ref={setRef}
      data-row=""
      data-theorem-type={theorem.type}
      className="proof-step"
      style={{ marginLeft: `${depth * 1.25}rem`, opacity: alreadyVisited ? 0.7 : 1 }}
      onMouseEnter={onMouseEnter}
      onMouseLeave={onMouseLeave}
      onClick={onClick}
      title={alreadyVisited ? "Repeat citation — proof shown above" : undefined}
    >
      <span className={badgeClassFor(node.rule)}>{node.rule}</span>{" "}
      <span className="theorem">{theorem.text}</span>{" "}
      <span className="muted">— {node.explanation}</span>
    </div>
  );
});
