import { memo, useMemo } from "react";
import type { ScenarioReportT, TheoremT } from "../types";
import { useRowHighlight } from "../highlight/useHighlight";

interface Props {
  report: ScenarioReportT;
}

export function UnprovedTheorems({ report }: Props) {
  const items = useMemo(() => {
    const theoremByKey = new Map(report.theorems.map((t) => [t.key, t]));
    return report.unprovedTheoremKeys
      .map((k) => theoremByKey.get(k))
      .filter((t): t is TheoremT => t !== undefined);
  }, [report.theorems, report.unprovedTheoremKeys]);

  return (
    <section>
      <h2>
        Unproved theorems <span className="muted">({items.length})</span>{" "}
        <span className="muted" style={{ fontWeight: "normal" }}>
          — what the prover failed to close
        </span>
      </h2>
      {items.length === 0 ? (
        <p className="muted">All theorems were proved.</p>
      ) : (
        <ol className="unproved-list">
          {items.map((t) => (
            <UnprovedItem key={t.key} theorem={t} />
          ))}
        </ol>
      )}
    </section>
  );
}

const UnprovedItem = memo(function UnprovedItem({ theorem }: { theorem: TheoremT }) {
  const { onMouseEnter, onMouseLeave, onClick, setRef } = useRowHighlight(
    theorem.highlightIds,
    theorem.type,
    theorem.text,
  );
  return (
    <li
      ref={setRef as (el: HTMLLIElement | null) => void}
      data-row=""
      data-theorem-type={theorem.type}
      className="unproved-item"
      onMouseEnter={onMouseEnter}
      onMouseLeave={onMouseLeave}
      onClick={onClick}
    >
      {theorem.text}
    </li>
  );
});
